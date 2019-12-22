using FiboForm.Web.Models;
using FiboForm.Web.Repositories.Definition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FiboForm.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFiboFormRepository _repo;

        public HomeController(ILogger<HomeController> logger, IFiboFormRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                FiboModel model = await _repo.GetPayload();
                return View(model);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Index()");
                return View("Index", new FiboModel { ErrorMessage = $"Error! : {ex.Message}" });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index(FiboInputModel model)
        {
            try
            {
                if (!model.Index.HasValue)
                    return View("Index", new FiboModel { ErrorMessage = "Enter a valid Index." });

                if (model.Index.Value > 40)
                    return View("Index", new FiboModel { ErrorMessage = "Index must be less than 40." });

                var result = await _repo.SearchIndex(model.Index.Value);

                if (result != null)
                {
                    result.Index = model.Index;
                    return View("Index", result);
                }
                else
                    return View("Index", new FiboModel { ErrorMessage = $"Unable to search Index" });

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "SearchFiboNumber({index})", model.Index);
                return View("Index", new FiboModel { ErrorMessage = $"Error! : {ex.Message}" });
            }
        }

        private FiboModel GetTestModel()
        {
            return new FiboModel()
            {
                VisitedIndexes = new System.Collections.Generic.List<int> { 4, 3, 4, 5 },
                VisitedValues = new System.Collections.Generic.List<VisitedValue>
                   {
                       new VisitedValue { Index = 1, Value = 3 },
                       new VisitedValue { Index = 3, Value = 7 },
                       new VisitedValue { Index = 5, Value = 8 }
                   }
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
