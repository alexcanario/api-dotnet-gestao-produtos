using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Linq;

namespace DevIO.Api.Controllers {

    [ApiController]
    public abstract class MainController : ControllerBase {
        private readonly INotificador _notificador;

        protected MainController(INotificador notificador) {
            _notificador = notificador;
        }

        protected void NotificateErrorInvalidModelState(ModelStateDictionary modelState) {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in errors) {
                var errorMessage = erro.Exception is null ? erro.ErrorMessage : erro.Exception.Message;
                NotifyError(errorMessage);
            }
        }

        protected void NotifyError(string errorMessage) {
            _notificador.Handle(new Notificacao(errorMessage));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState) {
            if(!modelState.IsValid) NotificateErrorInvalidModelState(modelState);

            return CustomResponse();
        }

        protected ActionResult CustomResponse(object result = null) {
            if (ValidOperation()) {
                return Ok(new {
                    Success = true,
                    Data = result
                });
            }

            return BadRequest(new {
                Sucess = false,
                Errors = _notificador.ObterNotificacoes().Select(n => n.Mensagem)
            });
        }

        protected bool ValidOperation() {
            return (!_notificador.TemNotificacao());
        }

    }
}