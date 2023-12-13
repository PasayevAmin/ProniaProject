namespace FrontToBack.Middlewares
{
    public class GlobalExceptionMiddlewares
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddlewares(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);

            }
            catch (Exception e)
            {

                context.Response.Redirect($"/Home/ErrorPage?error={e.Message}");
            }
        }
    }
}
