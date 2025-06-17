using BuildingBlocks.Enums;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Project.Application.Features.Storage.RestoreStorage
{
    public class RestoreStorageHandler
        (IBaseRepository<File> fileRepository,
        IBaseRepository<Folder> folderRepository,
        IBaseRepository<UserProject> userProjectRepository,
        IPublishEndpoint publishEndpoint)
        : ICommandHandler<RestoreStorageRequest, RestoreStorageResponse>
    {
        public async Task<RestoreStorageResponse> Handle(RestoreStorageRequest request, CancellationToken cancellationToken)
        {
            if (request.IsFile)
            {
                var currentUserId = userProjectRepository.GetCurrentId();

                var userProject = await userProjectRepository.GetAllQueryAble()
                   .FirstOrDefaultAsync(e => e.UserId == currentUserId && e.ProjectId == request.ProjectId);

                var file = await fileRepository.GetAllQueryAble()
                    .FirstOrDefaultAsync(e => e.Id == request.Id);

                if (userProject is null || file is null)
                    throw new NotFoundException(Message.NOT_FOUND);


                //Không phải admin và cũng không phải người tạo thư mục
                if (userProject.Role is not Role.Admin && file.CreatedBy != currentUserId)
                    throw new ForbiddenException(Message.FORBIDDEN_CHANGE);

                file.IsDeleted = false;

                fileRepository.Update(file);
                await fileRepository.SaveChangeAsync(cancellationToken);

                // Gửi message activity
                var eventMessage = new CreateActivityEvent
                {
                    Action = "UPDATE",
                    ResourceId = file.Id,
                    Content = $"Đã khôi phục tệp \"{file.Name}\" trở lại",
                    TypeActivity = TypeActivity.File,
                    ProjectId = request.ProjectId
                };
                await publishEndpoint.Publish(eventMessage, cancellationToken);
            }
            else
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


                var restoreFolders = new List<Folder>() { folder };
                var restoreFiles = new List<File>();

                //Lấy tất các folder, file con
                var childFolders = await folderRepository.GetAllQueryAble()
                    .Where(e => e.FullPath.StartsWith(folder.FullPath) && e.FullPathName.StartsWith(folder.FullPathName) && e.Id != folder.Id)
                    .ToListAsync(cancellationToken);
                var childFiles = await fileRepository.GetAllQueryAble()
                    .Where(e => e.FullPath.StartsWith(folder.FullPath + "/"))
                    .ToListAsync(cancellationToken);
                // Thêm vào danh sách để xử lý
                restoreFolders.AddRange(childFolders);
                restoreFiles.AddRange(childFiles);

                foreach(var f in restoreFolders)
                {
                    f.IsDeleted = false;
                }
                foreach (var f in restoreFiles)
                {
                    f.IsDeleted = false;
                }


                // Update vào database
                folderRepository.UpdateMany(restoreFolders);
                fileRepository.UpdateMany(restoreFiles);

                await folderRepository.SaveChangeAsync(cancellationToken);

                // Gửi message activity
                var eventMessage = new CreateActivityEvent
                {
                    Action = "UPDATE",
                    ResourceId = folder.Id,
                    Content = $"Đã khôi phục thư mục \"{folder.Name}\" trở lại",
                    TypeActivity = TypeActivity.Folder,
                    ProjectId = request.ProjectId
                };
                await publishEndpoint.Publish(eventMessage, cancellationToken);
            }

            return new RestoreStorageResponse() { Data = true, Message = Message.UPDATE_SUCCESSFULLY };
        }
    }
}
