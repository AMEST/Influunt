using Microsoft.AspNetCore.Http;

public class NoOpHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext { get => null; set => throw new NotImplementedException(); }

}