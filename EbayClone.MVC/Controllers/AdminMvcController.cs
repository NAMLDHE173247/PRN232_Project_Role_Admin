using EbayClone.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.MVC.Controllers;

public abstract class AdminMvcController : Controller
{
    protected IActionResult HandleApiFailure(AdminApiException exception, string action = "Index")
    {
        if (exception.StatusCode is 401 or 403)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        TempData["Error"] = "API không xử lý được yêu cầu. Vui lòng thử lại.";
        return RedirectToAction(action);
    }
}
