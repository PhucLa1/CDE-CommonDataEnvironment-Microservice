namespace Project.Application.Features.Storage.RestoreStorage
{
    public class RestoreStorageRequest : ICommand<RestoreStorageResponse>
    {
        public int ProjectId { get; set; }
        public int Id { get; set; }
        public bool IsFile { get; set; }
    }
}
