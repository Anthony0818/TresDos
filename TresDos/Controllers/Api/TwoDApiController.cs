using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using TresDos.Application.DTOs.BetDto;
using TresDos.Application.Feature.Products.Commands;
using TresDos.Application.Feature.Products.Queries;
using TresDos.Application.Feature.TwoD.Commands;
[ApiController]
[Route("api/[controller]")]
public class TwoDApiController : ControllerBase
{
    private readonly IMediator _mediator;
    public TwoDApiController(IMediator mediator) => _mediator = mediator;

    //[HttpPost]
    //public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    //{
    //    var created = await _mediator.Send(command);
    //    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    //}
    [HttpPost]
    public async Task<IActionResult> BulkInsert([FromBody] BulkInsertTwoDCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(new
        {
            InsertedEntries = result.Item1,
            ProcessingResults = result.Item2.Select(r => new
            {
                r.id,
                r.Bettor,
                r.FirstDigit,
                r.SecondDigit,
                r.Type,
                r.Amount,
                r.Message,
                r.IsInserted,
                r.AvailableBalance
            })
        });
    }
}