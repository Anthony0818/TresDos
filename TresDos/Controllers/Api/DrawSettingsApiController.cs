using MediatR;
using Microsoft.AspNetCore.Mvc;
using TresDos.Application.Feature.DrawSettings.Queries;
[ApiController]
[Route("api/[controller]")]
public class DrawSettingsApiController : ControllerBase
{
    private readonly IMediator _mediator;
    public DrawSettingsApiController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetAllDrawSettingsQuery()));
}