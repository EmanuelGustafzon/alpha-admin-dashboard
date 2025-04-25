using Business.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Presentation.Controllers;

public class CookiesController : Controller
{

    [HttpPost]
    public IActionResult SetCookieConsent(CookieConsent consent)
    {
        Response.Cookies.Append("cookieConsent", JsonSerializer.Serialize(consent), new CookieOptions
        {
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            SameSite = SameSiteMode.None,
            Path = "/"
        });

        return Ok();
    }

    [HttpPost]
    public IActionResult SetFunctionalCookie([FromQuery] string name, [FromQuery] string value, [FromQuery] int days)
    {
        if (String.IsNullOrEmpty(name))
            return BadRequest("Invalid name");

        if (String.IsNullOrEmpty(value))
            return BadRequest("Invalid value");

        var consent = Request.Cookies["cookieConsent"];
        if (string.IsNullOrEmpty(consent) || !JsonSerializer.Deserialize<CookieConsent>(consent)?.Functional == true)
        {
            Response.Cookies.Delete(name);
            return BadRequest("Consent not given");
        }

        Response.Cookies.Append(name, value, new CookieOptions
        {
            IsEssential = false,
            Expires = DateTimeOffset.UtcNow.AddDays(days),
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });

        return Ok();
    }

    [HttpPost]
    public IActionResult DeleteCookie([FromQuery] string name)
    {
        Response.Cookies.Delete(name);

        return Ok();
    }
}
