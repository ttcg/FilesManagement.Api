using Microsoft.AspNetCore.Mvc;

namespace FilesManagement.Api.Controllers
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return Redirect("~/swagger/index.html");
        }
    }
}
