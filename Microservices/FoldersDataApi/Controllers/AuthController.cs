using FoldersDataApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoldersDataApi.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public string Index()
    {
        return "Приложение запущено";
    }


    public IActionResult GetConnectionState()
    {
        // Console.WriteLine($"GetConnectionState");
        Dictionary<string, bool> state = new Dictionary<string, bool>()
        {
            { "backend", true },
            { "database", _authService.GetConnectionState().Result }
        };
        return Json(state);
    }

}