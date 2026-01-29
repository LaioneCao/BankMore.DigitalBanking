using BankMore.Transfers.Api.Contracts;
using BankMore.Transfers.Application.Commands.CreateTransfer;
using MediatR;
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
        private readonly IMediator _mediator;

        public TransfersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateTransferRequest request)
        {
            try
            {
                var contaId = Guid.Parse(User.FindFirstValue("accountId")!);
                var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                var command = new CreateTransferCommand
                {
                    RequisitionId = request.RequisitionId,
                    NumeroContaDestino = request.NumeroContaDestino,
                    Valor = request.Valor,
                    ContaOrigemId = contaId,
                    BearerToken = token
                };

                await _mediator.Send(command);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { type = ex.Type, message = ex.Message });
            }
        }
    }
}
