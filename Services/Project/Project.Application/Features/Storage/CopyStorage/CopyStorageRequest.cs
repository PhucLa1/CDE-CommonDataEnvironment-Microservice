namespace Project.Application.Features.Storage.CopyStorage
{
    public class CopyStorageRequest : ICommand<CopyStorageResponse>
    {
        public int Id { get; set; }
        public bool IsFile { get; set; }
    }
}
