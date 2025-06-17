namespace Project.Application.Features.Storage.GetStorageDeleted
{
    public class GetStorageDeletedResponse
    {
        public int Id { get; set; }
        public bool IsFile { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string DeletedAt { get; set; } = string.Empty;
        public string DeletedBy { get; set; } = string.Empty;
    }
}
