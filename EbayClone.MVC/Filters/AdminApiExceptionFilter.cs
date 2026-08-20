using EbayClone.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EbayClone.MVC.Filters;

public sealed class AdminApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not AdminApiException exception) return;

        if (exception.StatusCode is 401 or 403)
            context.HttpContext.Session.Clear();

        context.Result = exception.StatusCode is 401 or 403
            ? new RedirectToActionResult("Login", "Account", null)
            : new RedirectToActionResult("Index", "Dashboard", null);
        context.ExceptionHandled = true;
    }
}
