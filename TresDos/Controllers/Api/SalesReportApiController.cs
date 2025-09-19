using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using TresDos.Application.DTOs.BetDto;
using TresDos.Application.Feature.Products.Commands;
using TresDos.Application.Feature.Products.Queries;
using TresDos.Application.Feature.Reports.Queries;
using TresDos.Application.Feature.TwoD.Commands;
using TresDos.Application.Feature.TwoD.Queries;
using TresDos.Core.Entities;
[ApiController]
[Route("api/[controller]")]
public class SalesReportApiController : ControllerBase
{
    private readonly IMediator _mediator;
    public SalesReportApiController(IMediator mediator) => _mediator = mediator;

    [HttpGet("AllUsersByDate")]
    public async Task<IActionResult> AllUsersByDate(
         string UserId, string drawDate)
    {
        var query = new GetAllUserSalesQuery(UserId, Convert.ToDateTime(drawDate));
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}