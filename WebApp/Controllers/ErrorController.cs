using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("404")]
        public IActionResult PageNotFound()
        {
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }
            ViewBag.OriginalPath = originalPath;
            return View();
        }
    }
}