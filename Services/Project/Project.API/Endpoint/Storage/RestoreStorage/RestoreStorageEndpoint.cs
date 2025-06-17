using Project.Application.Features.Storage.RestoreStorage;

namespace Project.API.Endpoint.Storage.RestoreStorage
{
    [ApiController]
    [Route(NameRouter.STORAGE_ROUTER)]
    public class RestoreStorageEndpoint(IMediator mediator) : ControllerBase
    {
        [HttpPut]
        [Route("restore-storage")]
        public async Task<IActionResult> RestoreStorage([FromBody] RestoreStorageRequest restoreStorageRequest)
        {
            return Ok(await mediator.Send(restoreStorageRequest));
        }
    }
}
