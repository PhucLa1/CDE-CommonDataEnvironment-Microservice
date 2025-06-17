using Project.Application.Extensions;
using Project.Application.Grpc;
using Project.Application.Grpc.GrpcRequest;

namespace Project.Application.Features.Storage.GetStorageDeleted
{
    public class GetStorageDeletedHandler
        (IBaseRepository<File> fileRepository,
        IBaseRepository<Folder> folderRepository,
        IUserGrpc userGrpc)
        : IQueryHandler<GetStorageDeletedRequest, ApiResponse<List<GetStorageDeletedResponse>>>
    {
        public async Task<ApiResponse<List<GetStorageDeletedResponse>>> Handle(GetStorageDeletedRequest request, CancellationToken cancellationToken)
        {
            var IMAGE_EXTENSION = new List<string>() { ".png", ".jpg", ".jpeg" };
            var currentDateDisplay = folderRepository.GetCurrentDateDisplay();
            var currenTimeDisplay = folderRepository.GetCurrentTimeDisplay();
            var folders = await folderRepository.GetAllQueryAble()
                .Where(e => e.IsDeleted == true && e.ProjectId == request.ProjectId && e.IsShow == true)
                .OrderByDescending(e => e.Id)
                .ToListAsync();

            var files = await fileRepository.GetAllQueryAble()
                .Where(e => e.IsDeleted == true && e.ProjectId == request.ProjectId && e.IsShow == true)
                .OrderByDescending(e => e.Id)
                .ToListAsync();



            var userIds = files.Select(e => e.UpdatedBy).ToList().Concat(folders.Select(e => e.UpdatedBy)).Distinct().ToList();

            var users = await userGrpc.GetUsersByIds(new GetUserRequestGrpc() { Ids = userIds });
            var folderStorages = (from f in folders
                                  join u in users on f.UpdatedBy equals u.Id
                                  select new GetStorageDeletedResponse()
                                  {
                                      Id = f.Id,
                                      IsFile = false,
                                      Url = "",
                                      DeletedAt = f.UpdatedAt.ConvertToFormat(currentDateDisplay, currenTimeDisplay),
                                      DeletedBy = u.Email,
                                      Name = f.Name,
                                  }).ToList();

            var fileStorages = (from f in files
                                join u in users on f.UpdatedBy equals u.Id
                                select new GetStorageDeletedResponse()
                                {
                                    Id = f.Id,
                                    IsFile = true,
                                    Url = IMAGE_EXTENSION.Contains(f.Extension)
                                      ? f.Url
                                      : Setting.PROJECT_HOST + "/Extension/" + f.Extension.ConvertToUrl(),
                                    DeletedAt = f.UpdatedAt.ConvertToFormat(currentDateDisplay, currenTimeDisplay),
                                    DeletedBy = u.Email,
                                    Name = f.Name,
                                }).ToList();

            var deletedStorages = folderStorages.Concat(fileStorages).ToList();

            return new ApiResponse<List<GetStorageDeletedResponse>>() { Data = deletedStorages, Message = Message.GET_SUCCESSFULLY };
        }
    }
}