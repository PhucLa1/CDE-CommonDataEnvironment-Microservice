﻿using Project.Application.Extensions;
using Project.Application.Grpc;
using Project.Application.Grpc.GrpcRequest;

namespace Project.Application.Features.Storage.GetAllStorages
{
    public class GetAllStoragesHandler
        (IBaseRepository<Folder> folderRepository,
        IBaseRepository<File> fileRepository,
        IBaseRepository<UserProject> userProjectRepository,
        IUserGrpc userGrpc)
        : IQueryHandler<GetAllStoragesRequest, ApiResponse<List<GetAllStoragesResponse>>>
    {
        public async Task<ApiResponse<List<GetAllStoragesResponse>>> Handle(GetAllStoragesRequest request, CancellationToken cancellationToken)
        {
            var IMAGE_EXTENSION = new List<string>() { ".png", ".jpg", ".jpeg" };
            //lấy định dạng ngày tháng
            var currentDateDisplay = folderRepository.GetCurrentDateDisplay();
            var currenTimeDisplay = folderRepository.GetCurrentTimeDisplay();
            var userId = folderRepository.GetCurrentId();
            var user = await userProjectRepository.GetAllQueryAble()
                .Where(e => e.UserProjectStatus == UserProjectStatus.Active && e.ProjectId == request.ProjectId && e.UserId == userId && e.Role == Role.Admin)
                .FirstOrDefaultAsync(cancellationToken);




            var folderQuery = folderRepository.GetAllQueryAble()
                .Include(e => e.FolderPermissions)
                .Include(e => e.FolderTags)
                .ThenInclude(e => e.Tag)
                .Where(e => e.ProjectId == request.ProjectId && e.ParentId == request.ParentId && e.IsDeleted == false);
             

            if (user == null)
            {
                // Không phải admin
                folderQuery = folderQuery.Where(e =>
                    // (1) Không có phân quyền riêng → dùng quyền chung
                    (!e.FolderPermissions.Any(p => p.TargetId == userId) && e.Access != Access.NoAccess)
                    ||
                    // (2) Có phân quyền riêng, nhưng quyền đó ≠ NoAccess
                    (e.FolderPermissions.Any(p => p.TargetId == userId && p.Access != Access.NoAccess)));
            }
            var folders = await folderQuery.Select(e => new GetAllStoragesResponse()
            {
                Id = e.Id,
                IsFile = false,
                Name = e.Name,
                UrlImage = "",
                CreatedAt = e.CreatedAt.ConvertToFormat(currentDateDisplay, currenTimeDisplay),
                CreatedBy = e.CreatedBy,
                TagNames = e.FolderTags.Where(f => f.Tag != null && f.Tag.Name != null).Select(e => e.Tag.Name).ToList()
            })
            .ToListAsync(cancellationToken);

            var fileQuery = fileRepository.GetAllQueryAble()
                .Include(e => e.FileTags)
                .ThenInclude(e => e.Tag)
                .Where(e => e.ProjectId == request.ProjectId && e.FolderId == request.ParentId && e.IsDeleted == false);


            if (user == null)
            {
                // Không phải admin
                fileQuery = fileQuery.Where(e =>
                    // (1) Không có phân quyền riêng → dùng quyền chung
                    (!e.FilePermissions.Any(p => p.TargetId == userId) && e.Access != Access.NoAccess)
                    ||
                    // (2) Có phân quyền riêng, nhưng quyền đó ≠ NoAccess
                    (e.FilePermissions.Any(p => p.TargetId == userId && p.Access != Access.NoAccess)));
            }

            var files = await fileQuery.Select(e => new GetAllStoragesResponse()
            {
                Id = e.Id,
                IsFile = true,
                Name = e.Name + e.Extension,
                UrlImage = IMAGE_EXTENSION.Contains(e.Extension)
                ? e.Url
                : Setting.PROJECT_HOST + "/Extension/" + e.Extension.ConvertToUrl(),
                CreatedAt = e.CreatedAt.ConvertToFormat(currentDateDisplay, currenTimeDisplay),
                CreatedBy = e.CreatedBy,
                TagNames = e.FileTags.Where(f => f.Tag != null && f.Tag.Name != null).Select(e => e.Tag.Name).ToList()
            })
            .ToListAsync(cancellationToken);

            var folderCreatedByList = folders.Select(e => e.CreatedBy).ToList();
            var fileCreatedByList = files.Select(e => e.CreatedBy).ToList();
            var mergeList = folderCreatedByList.Concat(fileCreatedByList).Distinct().ToList();

            var users = await userGrpc
                .GetUsersByIds(new GetUserRequestGrpc { Ids = mergeList });

            var storage = files.Concat(folders).ToList();

            var storages = (from s in storage
                            join u in users on s.CreatedBy equals u.Id
                            select new GetAllStoragesResponse()
                            {
                                Id = s.Id,
                                IsFile = s.IsFile,
                                Name = s.Name,
                                UrlImage = s.UrlImage,
                                CreatedAt = s.CreatedAt,
                                CreatedBy = s.CreatedBy,
                                NameCreatedBy = u.FullName,
                                TagNames = ConvertTagsToView(s.TagNames ?? new List<string>())
                            }).ToList();

            return new ApiResponse<List<GetAllStoragesResponse>> { Data = storages, Message = Message.GET_SUCCESSFULLY };
        }

        private List<string> ConvertTagsToView(List<string> tagNames)
        {
            var MAX_COUNT = 25;
            List<string> result = new List<string>();
            var countChars = 0;
            if (tagNames != null && tagNames.Count > 0)
            {
                for (int i = 0; i < tagNames.Count; i++)
                {
                    countChars += tagNames[i].Count();
                    if (countChars <= MAX_COUNT)
                    {
                        result.Add(tagNames[i]);
                    }
                    else
                    {
                        result.Add("+ " + (tagNames.Count() - i).ToString());
                        break;
                    }
                }
            }

            return result;
        }
    }
}
