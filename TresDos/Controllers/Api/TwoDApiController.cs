using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using TresDos.Application.DTOs.BetDto;
using TresDos.Application.Feature.Products.Commands;
using TresDos.Application.Feature.Products.Queries;
using TresDos.Application.Feature.TwoD.Commands;
using TresDos.Application.Feature.TwoD.Queries;
using TresDos.Core.Entities;
[ApiController]
[Route("api/[controller]")]
public class TwoDApiController : ControllerBase
{
    private readonly IMediator _mediator;
    public TwoDApiController(IMediator mediator) => _mediator = mediator;

    [HttpPost("BulkInsertTwoD")]
    public async Task<IActionResult> BulkInsertTwoD([FromBody] BulkInsertTwoDCommand command)
    {
        var result = await _mediator.Send(command);

        var response = new BulkInsertTwoDEntriesResponseDto
        {
            EntriesInserted = result.Item1.ToList(),
            EntriesResults = result.Item2.ToList()
        };

        return Ok(response);
    }
    [HttpPost("BulkValidateTwoD")]
    public async Task<IActionResult> BulkValidateTwoD([FromBody] BulkValidateTwoDCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }
    [HttpGet("GetBetsByUserIdDrawTypeDrawDate")]
    public async Task<IActionResult> GetBetsByUserIdDrawTypeDrawDate(
         int userId,
         string drawType,
         string drawDate)
    {
        var query = new GetBetsByUserIdDrawTypeDrawDateQuery(userId, drawType, Convert.ToDateTime(drawDate));
        var twoBets = await _mediator.Send(query);

        if (twoBets == null)
            return NotFound();

        return Ok(twoBets);
    }
    [HttpPost("BulkDeleteTwoD")]
    public async Task<IActionResult> BulkDeleteTwoD([FromBody] BulkDeleteTwoDCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }
}