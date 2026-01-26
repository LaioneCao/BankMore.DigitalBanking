using BankMore.Accounts.Api.Application.Balance;
using BankMore.Accounts.Api.Application.CloseAccount;
using BankMore.Accounts.Api.Application.ListMovements;
using BankMore.Accounts.Api.Application.Login;
using BankMore.Accounts.Api.Application.Movements;
using BankMore.Accounts.Api.Application.OpenAccount;
using BankMore.Accounts.Api.Domain.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankMore.Accounts.Api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly OpenAccountCommandHandler _openAccountHandler;
        private readonly IContaCorrenteRepository _repository;


        public AccountsController(OpenAccountCommandHandler openAccountHandler, IContaCorrenteRepository repository)
        {
            _openAccountHandler = openAccountHandler;
            _repository = repository;
        }

        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var contas = await _repository.ListAsync();
            return Ok(contas);
        }

        [Authorize]
        [HttpGet("movements")]
        [ProducesResponseType(typeof(IEnumerable<MovementListItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListMovements(
    [FromQuery] int? numeroConta,
    [FromServices] IContaCorrenteRepository contaRepo,
    [FromServices] IMovimentoRepository movQueryRepo)
        {
            var contaIdLogada = GetContaIdFromToken(User);
            var numeroLogado = GetContaNumeroFromToken(User);

            // Se não enviar numeroConta -> usa conta logada
            if (!numeroConta.HasValue)
            {
                var movimentos = await movQueryRepo.ListByContaIdAsync(contaIdLogada);
                return Ok(movimentos);
            }

            if (numeroConta.Value != numeroLogado)
                return Forbid();

            var conta = await contaRepo.GetByNumeroAsync(numeroConta.Value);
            if (conta is null)
                return NotFound(new ApiErrorResponse { Type = "INVALID_ACCOUNT", Message = "Conta corrente inválida." });

            var list = await movQueryRepo.ListByContaIdAsync(conta.Id);
            return Ok(list);
        }

        
        [HttpPost("open")]
        [ProducesResponseType(typeof(OpenAccountResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OpenAccount([FromBody] OpenAccountCommand command)
        {
            try
            {
                var result = await _openAccountHandler.HandleAsync(command);

                return Created(string.Empty, result);
            }
            catch (InvalidOperationException ex)
            {
                var errorType = ex.Message.Contains("CPF", StringComparison.OrdinalIgnoreCase)
                    ? "INVALID_DOCUMENT"
                    : "INVALID_REQUEST";

                return BadRequest(new ApiErrorResponse
                {
                    Type = errorType,
                    Message = ex.Message
                });
            }
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, [FromServices] LoginCommandHandler handler)
        {
            try
            {
                var result = await handler.HandleAsync(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiErrorResponse
                {
                    Type = "USER_UNAUTHORIZED",
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpDelete("disable")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CloseAccount(
        [FromBody] CloseAccountCommand command,
        [FromServices] CloseAccountCommandHandler handler)
        {
            try
            {
                var contaId = GetContaIdFromToken(User);
                await handler.HandleAsync(contaId, command);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ApiErrorResponse { Type = ex.Type, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiErrorResponse { Type = "USER_UNAUTHORIZED", Message = ex.Message });
            }
        }


        [Authorize]
        [HttpPost("movements")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMovement(
        [FromBody] CreateMovementCommand command,
        [FromServices] CreateMovementCommandHandler handler)
        {
            try
            {
                var contaId = GetContaIdFromToken(User);
                var contaNumero = GetContaNumeroFromToken(User);

                await handler.HandleAsync(contaId, contaNumero, command);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ApiErrorResponse { Type = ex.Type, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("balance")]
        [ProducesResponseType(typeof(GetBalanceResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBalance([FromServices] GetBalanceQueryHandler handler)
        {
            try
            {
                var contaId = GetContaIdFromToken(User);
                var result = await handler.HandleAsync(contaId);
                return Ok(result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ApiErrorResponse { Type = ex.Type, Message = ex.Message });
            }
        }


        private static Guid GetContaIdFromToken(ClaimsPrincipal user)
        {
            var accountId = user.FindFirstValue("accountId") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(accountId) || !Guid.TryParse(accountId, out var id))
                throw new UnauthorizedAccessException("Token inválido.");

            return id;
        }

        private static int GetContaNumeroFromToken(ClaimsPrincipal user)
        {
            var num = user.FindFirstValue("accountNumber");
            if (string.IsNullOrWhiteSpace(num) || !int.TryParse(num, out var n))
                throw new UnauthorizedAccessException("Token inválido.");
            return n;
        }


    }

    public sealed class ApiErrorResponse
    {
        public string Type { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}
