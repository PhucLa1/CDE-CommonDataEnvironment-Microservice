
using BuildingBlocks.Enums;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Project.Application.Features.Storage.DeleteStorages
{
    public class DeleteStoragesHandler
        (IBaseRepository<File> fileRepository,
        IBaseRepository<Folder> folderRepository,
        IBaseRepository<UserProject> userProjectRepository,
        IPublishEndpoint publishEndpoint)
        : ICommandHandler<DeleteStoragesRequest, DeleteStoragesResponse>
    {
        public async Task<DeleteStoragesResponse> Handle(DeleteStoragesRequest request, CancellationToken cancellationToken)
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

                

                fileRepository.Remove(file);
                await fileRepository.SaveChangeAsync(cancellationToken);

                // Gửi message activity
                var eventMessage = new CreateActivityEvent
                {
                    Action = "DELETE",
                    ResourceId = file.Id,
                    Content = $"Đã xóa tệp \"{file.Name}\" vĩnh viễn",
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


                var removedFolders = new List<Folder>() { folder };
                var removedFiles = new List<File>();

                //Lấy tất các folder, file con
                var childFolders = await folderRepository.GetAllQueryAble()
                    .Where(e => e.FullPath.StartsWith(folder.FullPath) && e.FullPathName.StartsWith(folder.FullPathName))
                    .ToListAsync(cancellationToken);
                var childFiles = await fileRepository.GetAllQueryAble()
                    .Where(e => e.FullPath.StartsWith(folder.FullPath))
                    .ToListAsync(cancellationToken);
                // Thêm vào danh sách để xử lý
                removedFolders.AddRange(childFolders);
                removedFiles.AddRange(childFiles);


                // Update vào database
                folderRepository.RemoveRange(removedFolders);
                fileRepository.RemoveRange(removedFiles);

                await folderRepository.SaveChangeAsync(cancellationToken);

                // Gửi message activity
                var eventMessage = new CreateActivityEvent
                {
                    Action = "DELETE",
                    ResourceId = folder.Id,
                    Content = $"Đã xóa thư mục \"{folder.Name}\" vĩnh viễn",
                    TypeActivity = TypeActivity.Folder,
                    ProjectId = request.ProjectId
                };
                await publishEndpoint.Publish(eventMessage, cancellationToken);
            }

            return new DeleteStoragesResponse() { Data = true, Message = Message.DELETE_SUCCESSFULLY };
            
        }
    }
}
