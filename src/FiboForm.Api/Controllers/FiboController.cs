using FiboForm.Api.Components.Definition;
using FiboForm.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FiboForm.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class FiboController : ControllerBase
    {

        private readonly ILogger<FiboController> _logger;
        private readonly IFiboComponent _fiboComponent;

        public FiboController(ILogger<FiboController> logger, IFiboComponent fiboComponent)
        {
            _logger = logger;
            _fiboComponent = fiboComponent;
        }

        [HttpGet("", Name = "Index")]
        [ProducesResponseType(typeof(Payload), 200)]
        public async Task<IActionResult> Index()
        {
            try
            {
                Payload payload = await _fiboComponent.GetFiboPayload();

                if (payload == null)
                    return Ok(new Payload());

                return Ok(payload);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "ERROR Index()");
                return StatusCode(500);
            }
        }


        [HttpPost("{index}", Name = "SearchFiboNumber")]
        [ProducesResponseType(typeof(Payload), 200)]
        public async Task<IActionResult> SearchFiboNumber(int index)
        {
            try
            {
                Payload payload = await _fiboComponent.SearchFiboNumber(index);

                if (payload == null)
                    return Ok(new Payload());

                return Ok(payload);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "ERROR Index()");
                return StatusCode(500);
            }
        }



    }
}
