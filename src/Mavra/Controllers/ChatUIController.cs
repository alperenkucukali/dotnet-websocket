using Microsoft.AspNetCore.Mvc;

namespace Mavra.Controllers
{
    public class ChatUIController : Controller
    {
        [HttpGet("/chat-ui")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
