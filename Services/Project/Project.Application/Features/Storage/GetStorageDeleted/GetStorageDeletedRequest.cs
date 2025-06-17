namespace Project.Application.Features.Storage.GetStorageDeleted
{
    public class GetStorageDeletedRequest : IQuery<ApiResponse<List<GetStorageDeletedResponse>>>
    {
        public int ProjectId { get; set; }
    }
}
