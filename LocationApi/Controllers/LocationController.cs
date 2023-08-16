using LocationApi.Models;
using LocationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;
        private readonly FileService _fileService;

        public LocationController(ILogger<LocationController> logger, FileService fileService)
        {
            _fileService = fileService;
            _logger = logger;
        }
// test
        [HttpPost]
        public async Task< ActionResult<Location>> WriteLocation(Location location){

            _logger.LogInformation("LOCATION RECEIVED: "+ location.Latitude.ToString()
            +" - "+location.Longitude.ToString()
            +" - "+location.Accuracy.ToString());

            var locationString = DateTime.Now.ToString()+": "
            +location.Accuracy.ToString()
            +location.Latitude.ToString()+", "
            +location.Longitude.ToString();

            var success = await _fileService.AppendToFileAsync(locationString);

            if (success){
                 return Ok(location);
            } else return BadRequest("Error writing location!");
            
        }
    }
}