


using BuildingBlocks.Enums;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Project.Application.Features.Storage.DeleteFolder
{
    public class DeleteFolderHandler
        (IBaseRepository<Folder> folderRepository,
        IBaseRepository<File> fileRepository,
        IBaseRepository<UserProject> userProjectRepository,
        IPublishEndpoint publishEndpoint)
        : ICommandHandler<DeleteFolderRequest, DeleteFolderResponse>
    {
        public async Task<DeleteFolderResponse> Handle(DeleteFolderRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = userProjectRepository.GetCurrentId();

            var userProject = await userProjectRepository.GetAllQueryAble()
               .FirstOrDefaultAsync(e => e.UserId == currentUserId && e.ProjectId == request.ProjectId);

            var folder = await folderRepository.GetAllQueryAble()
                .FirstOrDefaultAsync(e => e.Id == request.Id);

            if (userProject is null || folder is null)
                throw new NotFoundException(Message.NOT_FOUND);


            //Không phải admin và cũng không phải người tạo thư mục
            if (userProject.Role is not Role.Admin && folder.CreatedBy != currentUserId)
                throw new ForbiddenException(Message.FORBIDDEN_CHANGE);

            folder.IsShow = true;
            folder.IsDeleted = true;
            folderRepository.Update(folder);

            var removedFolders = new List<Folder>();
            var removedFiles = new List<File>();

            //Lấy tất các folder, file con
            var childFolders = await folderRepository.GetAllQueryAble()
                .Where(e => e.FullPath.StartsWith(folder.FullPath) && e.FullPathName.StartsWith(folder.FullPathName) && e.Id != folder.Id)
                .ToListAsync(cancellationToken);
            var childFiles = await fileRepository.GetAllQueryAble()
                .Where(e => e.FullPath.StartsWith(folder.FullPath + "/"))
                .ToListAsync(cancellationToken);
            // Thêm vào danh sách để xử lý
            removedFolders.AddRange(childFolders);
            removedFiles.AddRange(childFiles);

            // Đánh dấu đã xóa (soft delete)
            foreach (var f in removedFolders)
            {
                f.IsShow = false;
                f.IsDeleted = true;
            }

            foreach (var f in removedFiles)
            {
                f.IsShow = false;
                f.IsDeleted = true;
            }

            // Update vào database
            folderRepository.UpdateMany(removedFolders);
            fileRepository.UpdateMany(removedFiles);

            await folderRepository.SaveChangeAsync(cancellationToken);

            // Gửi message activity
            var eventMessage = new CreateActivityEvent
            {
                Action = "DELETE",
                ResourceId = folder.Id,
                Content = $"Đã chuyển thư mục \"{folder.Name}\" vào thùng rác",
                TypeActivity = TypeActivity.Folder,
                ProjectId = request.ProjectId
            };
            await publishEndpoint.Publish(eventMessage, cancellationToken);

            return new DeleteFolderResponse() { Data = true, Message = Message.DELETE_SUCCESSFULLY };
        }
    }
}
