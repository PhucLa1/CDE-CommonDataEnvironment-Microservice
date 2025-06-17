namespace Project.Application.Features.Storage.DeleteStorages
{
    public class DeleteStoragesRequest : ICommand<DeleteStoragesResponse>
    {
        public int ProjectId { get; set; }
        public int Id { get; set; }
        public bool IsFile { get; set; }
    }

}
