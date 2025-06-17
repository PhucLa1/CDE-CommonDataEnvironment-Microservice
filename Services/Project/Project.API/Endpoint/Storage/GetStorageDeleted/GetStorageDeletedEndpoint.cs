using Project.Application.Features.Statuses.GetStatuses;
using Project.Application.Features.Storage.GetStorageDeleted;

namespace Project.API.Endpoint.Storage.GetStorageDeleted
{
    [ApiController]
    [Route(NameRouter.STORAGE_ROUTER)]
    public class GetStorageDeletedEndpoint(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [Route("{projectId}/deleted-storages")]
        public async Task<IActionResult> GetStorageDeleted(int projectId)
        {
            return Ok(await mediator.Send(new GetStorageDeletedRequest() { ProjectId = projectId }));
        }
    }
}
