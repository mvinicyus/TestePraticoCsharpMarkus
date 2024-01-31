using Infrastructure.Message;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Filter
{
    public class AuthenticationFilter : IAsyncActionFilter
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly ILogger<AuthenticationFilter> _logger;

        public AuthenticationFilter(
            INotificationHandler<DomainNotification> notifications,
            ILogger<AuthenticationFilter> logger)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _logger = logger;
        }

        private ICollection<string> GetErrorMessages()
        {
            return _notifications.GetErrorMessages().Select(notification => notification.Value).ToList();
        }

        private bool IsValidOperation()
        {
            return _notifications.IsValidOperation();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            try
            {
                if (context.HttpContext.Response.StatusCode != StatusCodes.Status200OK)
                {
                    return;
                }
                var result = await next();

                if (result?.ExceptionDispatchInfo?.SourceException != null)
                {
                    result.ExceptionHandled = true;
                    var exception = result.Exception;
                    var message = new List<string>()
                    {
                        exception.Message
                    };

                    _logger.LogError(exception, message.FirstOrDefault(), result);
                    await WriteResponse(result, context.HttpContext, message, HttpStatusCode.InternalServerError, _logger).ConfigureAwait(false);
                    return;
                }

                if (!IsValidOperation())
                {
                    var messages = GetErrorMessages();
                    await WriteResponse(result, context.HttpContext, messages, HttpStatusCode.BadRequest, _logger).ConfigureAwait(false);
                    return;
                }
            }
            catch (Exception exception)
            {
                var message = $"{HttpStatusCode.InternalServerError} - Unexpected error: {exception.Message}";
                var serializedObject = ToJson(exception);
                _logger.LogError(exception, message, serializedObject);

                var error = new List<string>()
                {
                    exception?.Message ?? "Erro desconhecido"
                };
            }
        }

        private static string? ToJson(object obj, bool ident = false)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                var jso = new JsonSerializerOptions();
                jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                return JsonSerializer.Serialize(obj, jso);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task WriteResponse(ActionExecutedContext result,
                                                HttpContext context,
                                                object response,
                                                HttpStatusCode httpStatusCode,
                                                ILogger<AuthenticationFilter> logger)
        {
            try
            {
                var json = ToJson(response);
                if (!(context?.Response?.HasStarted ?? false))
                {
                    if (result == null)
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await context.Response.WriteAsync(json).ConfigureAwait(false);
                        return;
                    }

                    result.Result = new ContentResult()
                    {
                        Content = json,
                        ContentType = "application/json",
                        StatusCode = (int)httpStatusCode
                    };
                    return;
                }
                else
                {
                    var bytes = Encoding.Unicode.GetBytes(json);
                    await context.Response.Body.WriteAsync(bytes, 0, bytes?.Length ?? 0);
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message, context);
            }
        }
    }
}
