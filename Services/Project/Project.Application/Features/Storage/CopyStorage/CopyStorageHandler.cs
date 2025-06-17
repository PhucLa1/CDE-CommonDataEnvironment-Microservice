
namespace Project.Application.Features.Storage.CopyStorage
{
    public class CopyStorageHandler
        (IBaseRepository<File> fileRepository,
        IBaseRepository<Folder> folderRepository)
        : ICommandHandler<CopyStorageRequest, CopyStorageResponse>
    {
        public async Task<CopyStorageResponse> Handle(CopyStorageRequest request, CancellationToken cancellationToken)
        {
            var transaction = await fileRepository.BeginTransactionAsync(cancellationToken);
            if (request.IsFile)
            {
                var file = await fileRepository.GetAllQueryAble()
                    .FirstAsync(e => e.Id == request.Id);

                var fileQueryable = fileRepository.GetAllQueryAble()
                    .Where(e => e.ProjectId == file.ProjectId && e.FolderId == file.FolderId);

                var newName = await GenerateUniqueFileNameAsync(file.Name, fileQueryable);

                var fileAdd = new File()
                {
                    Name = newName,
                    Size = file.Size,
                    Url = file.Url,
                    FolderId = file.FolderId,
                    ProjectId = file.ProjectId,
                    FileVersion = 0,
                    FileType = file.FileType,
                    MimeType = file.MimeType,
                    Extension = file.Extension,
                    // Không set FullPath tại đây
                };

                await fileRepository.AddAsync(fileAdd, cancellationToken);
                await fileRepository.SaveChangeAsync(cancellationToken);

                // Cập nhật FullPath sau khi đã có Id
                fileAdd.FullPath = ReplaceLastSegment(file.FullPath, fileAdd.Id.ToString());
                fileRepository.Update(fileAdd); // hoặc SaveChanges tự động track cũng được
                await fileRepository.SaveChangeAsync(cancellationToken);
            }
            else
            {
                var folder = await folderRepository.GetAllQueryAble()
                    .FirstAsync(e => e.Id == request.Id);

                var folderQueryable = folderRepository.GetAllQueryAble()
                    .Where(e => e.ProjectId == folder.ProjectId && e.ParentId == folder.ParentId);

                var newName = await GenerateUniqueFolderNameAsync(folder.Name, folderQueryable);

                var folderAdd = new Folder()
                {
                    Name = newName,
                    ParentId = folder.ParentId,
                    ProjectId = folder.ProjectId,
                    // Chưa gán FullPath / FullPathName
                };

                await folderRepository.AddAsync(folderAdd, cancellationToken);
                await folderRepository.SaveChangeAsync(cancellationToken);

                // Bây giờ đã có folderAdd.Id
                folderAdd.FullPath = ReplaceLastSegment(folder.FullPath, folderAdd.Id.ToString());
                folderAdd.FullPathName = ReplaceLastSegment(folder.FullPathName, folderAdd.Name);

                // Có thể gọi Update hoặc SaveChange sẽ tự detect
                folderRepository.Update(folderAdd);
                await folderRepository.SaveChangeAsync(cancellationToken);
            }
            await fileRepository.CommitTransactionAsync(transaction, cancellationToken);

            return new CopyStorageResponse() { Data = true, Message = Message.CREATE_SUCCESSFULLY }; // bạn có thể trả thêm thông tin nếu cần
        }

        private async Task<string> GenerateUniqueFileNameAsync(string originalName, IQueryable<File> files)
        {
            var baseName = originalName;
            var copyIndex = 0;
            string newName;

            do
            {
                newName = copyIndex == 0 ? $"{baseName} - Copy" : $"{baseName} - Copy ({copyIndex})";
                copyIndex++;
            }
            while (await files.AnyAsync(f => f.Name == newName && f.IsDeleted == false));

            return newName;
        }

        private async Task<string> GenerateUniqueFolderNameAsync(string originalName, IQueryable<Folder> folders)
        {
            var baseName = originalName;
            var copyIndex = 0;
            string newName;

            do
            {
                newName = copyIndex == 0 ? $"{baseName} - Copy" : $"{baseName} - Copy ({copyIndex})";
                copyIndex++;
            }
            while (await folders.AnyAsync(f => f.Name == newName && f.IsDeleted == false));

            return newName;
        }

        private string ReplaceLastSegment(string path, string newSegment)
        {
            if (string.IsNullOrWhiteSpace(path)) return newSegment;

            var parts = path.TrimEnd('/').Split('/');
            parts[^1] = newSegment; // Cập nhật phần cuối
            return string.Join('/', parts);
        }


    }
}
