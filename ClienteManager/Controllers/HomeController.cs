
using Microsoft.AspNetCore.Mvc;

namespace ClienteManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
