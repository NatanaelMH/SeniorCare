using Microsoft.AspNetCore.Mvc;

namespace SeniorCare.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
