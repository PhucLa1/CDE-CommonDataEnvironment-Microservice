using Project.Application.Features.Storage.CopyStorage;

namespace Project.API.Endpoint.Storage.CopyStorage
{
    [ApiController]
    [Route(NameRouter.STORAGE_ROUTER)]
    public class CopyStorageEndpoint(IMediator mediator): ControllerBase
    {
        [HttpPost]
        [Route(NameRouter.COPY_STORAGE)]
        public async Task<IActionResult> CopyStorage([FromBody] CopyStorageRequest copyStorageRequest)
        {
            return Ok(await mediator.Send(copyStorageRequest));
        }
    }
}
