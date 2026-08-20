using EbayClone.MVC.Models;
using EbayClone.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EbayClone.MVC.Controllers;

public class AccountController(AdminApiClient apiClient) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("AdminToken")))
            return RedirectToAction("Index", "Dashboard");
        return View(new LoginInputModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel input, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(input);
        try
        {
            var response = await apiClient.LoginAsync(input, cancellationToken);
            if (response is null || !string.Equals(response.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "Tài khoản không có quyền Admin.");
                return View(input);
            }

            HttpContext.Session.SetString("AdminToken", response.Token);
            HttpContext.Session.SetString("AdminEmail", response.Email);
            return RedirectToAction("Index", "Dashboard");
        }
        catch (AdminApiException)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(input);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(string.Empty, "Không kết nối được Admin API.");
            return View(input);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}
