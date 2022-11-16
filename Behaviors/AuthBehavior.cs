using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace mediatr_todos_mini;

public class AuthBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<AuthBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _req;

    public AuthBehavior(ILogger<AuthBehavior<TRequest, TResponse>> logger, IHttpContextAccessor req)
    {
        _logger = logger;
        _req = req;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var authHeader = _req.HttpContext.Request.Headers["Authorization"];
        if (  request is BaseRequest br) //&&
        {
            //TODO: get the ID from a service
            br.UserId = 100;
        }

        _logger.LogInformation($"AuthBehavior: handling {typeof(TRequest).Name}");
        var response = await next();
        _logger.LogInformation($"AuthBehavior: handled {typeof(TResponse).Name}");

        return response;
    }
}