using Microsoft.AspNetCore.Mvc;

namespace PlantBiologyEducation.Controllers
{
    public class AccessLessonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
