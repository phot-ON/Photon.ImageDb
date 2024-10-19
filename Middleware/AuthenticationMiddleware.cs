using System.Net.Http.Headers;

namespace Photon.ImageDb.Middleware;

public class AuthenticationMiddleware(IConfiguration config) : IMiddleware
{
    private readonly string _authServer = (config["AUTH_SERVER"] 
                                          ?? throw new Exception("AuthServer not found in config")) + "/auth/validate?token=";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault();
        if (token == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("No token provided");
            return;
        }
        var client = new HttpClient();
        var response = await client.GetAsync(_authServer + token);
        if (!response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid token");
            return;
        }
        await next(context);
    }
}