using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
  public class PersonAlwaysRunResultFIlter : IAlwaysRunResultFilter
  {
    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
    }
  }
}
