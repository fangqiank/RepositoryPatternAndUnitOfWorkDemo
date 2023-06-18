using Microsoft.AspNetCore.Mvc.Filters;

namespace RepositoryPatternAndUnitOfWork.Filters
{
    public class MyFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This filter Executed on: {nameof(OnActionExecuted)}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"This filter Executed on: {nameof(OnActionExecuting)}");
        }
    }
}
