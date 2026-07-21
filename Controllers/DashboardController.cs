using Microsoft.AspNetCore.Mvc;

namespace StudentSuccessDashboard.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}