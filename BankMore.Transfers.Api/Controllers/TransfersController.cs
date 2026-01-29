using BankMore.Transfers.Application.Commands.CreateTransfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankMore.Transfers.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transfers")]
    public sealed class TransfersController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateTransferCommand command,
            [FromServices] CreateTransferCommandHandler handler)
        {
            try
            {
                var contaId = Guid.Parse(User.FindFirstValue("accountId")!);
                var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                await handler.HandleAsync(contaId, token, command);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { type = ex.Type, message = ex.Message });
            }
        }
    }
}
