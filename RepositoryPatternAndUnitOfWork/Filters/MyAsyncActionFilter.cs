using Microsoft.AspNetCore.Mvc.Filters;

namespace RepositoryPatternAndUnitOfWork.Filters
{
    public class MyAsyncActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _callerName;

        public MyAsyncActionFilterAttribute(string callerName)
        {
            _callerName = callerName;
        }
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, 
            ActionExecutionDelegate next
            )
        {
            await Console.Out.WriteLineAsync($"{_callerName} - Async Filter: Before Executing");
            await next();
            await Console.Out.WriteLineAsync($"{_callerName} - Async Filter: After Executing");
        }
    }
}
