using System;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApplication1.Filters
{
    public class LogActionAttribute : ActionFilterAttribute
    {
        private const string StopwatchKey = "ActionStopwatch";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var stopwatch = Stopwatch.StartNew();
            actionContext.Request.Properties[StopwatchKey] = stopwatch;

            Debug.WriteLine($"[REQUEST] {actionContext.Request.Method} {actionContext.Request.RequestUri}");
            
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Properties.ContainsKey(StopwatchKey))
            {
                var stopwatch = (Stopwatch)actionExecutedContext.Request.Properties[StopwatchKey];
                stopwatch.Stop();

                var statusCode = actionExecutedContext.Response?.StatusCode ?? System.Net.HttpStatusCode.InternalServerError;
                
                Debug.WriteLine($"[RESPONSE] {actionExecutedContext.Request.Method} {actionExecutedContext.Request.RequestUri} - Status: {(int)statusCode} - Duration: {stopwatch.ElapsedMilliseconds}ms");
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
