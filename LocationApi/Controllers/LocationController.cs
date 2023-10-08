using System.Runtime.CompilerServices;
using LocationApi.DTOs;
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
        private List<string>? DevicesList;

        public LocationController(ILogger<LocationController> logger, FileService fileService)
        {
            _fileService = fileService;
            _logger = logger;
            
            _fileService.FilePath = "devices/devicesList.txt";

            DevicesList = _fileService.ReadDevicesList();

            if (!(DevicesList == null)){
                foreach(string device in DevicesList){
                    _logger.LogInformation("DEVICE: "+device);
                }
            } else {
                 _logger.LogInformation("No devices on the list!");
            }
        }

        [HttpPost]
        public async Task< ActionResult<Location>> WriteLocation(Location location){


            _logger.LogInformation("LOCATION RECEIVED: "+location.DeviceId+" "+location.Latitude.ToString()
            +" - "+location.Longitude.ToString()
            +" - "+location.Accuracy.ToString());

            if (!IsDeviceOnList(location.DeviceId!)){
                return StatusCode(403, "Device not on the list!");
            }

            LocationParameters locationParameters = null;
            if (GetParamsFromFile(location.DeviceId!) != null){
                locationParameters = GetParamsFromString(GetParamsFromFile(location.DeviceId!));
            }

            var comma = ", ";

            if (location.Message!.Equals("")){
                comma = "";
            }

            var locationString = DateTime.Now.ToString()+": "
            +location.Time.ToString()+", "
            +location.DeviceId+" - "
            +location.Accuracy.ToString()+", "
            +location.Latitude.ToString()+", "
            +location.Longitude.ToString()+", "
            +location.BatteryLevel+", "
            +location.TimeZone+", "
            +location.TZoneOffset+", "
            +location.Provider+comma
            +location.Message;

            _fileService.FilePath ="log/" 
                +location.DeviceId+"_"
                +DateTime.Now.Year.ToString()
                +DateTime.Now.Month.ToString()
                +DateTime.Now.Day.ToString()+".txt";

            var success = await _fileService.AppendToFileAsync(locationString);
            
            LocationResponseDto locationResponseDto = new LocationResponseDto()
            {
                LocationResponse = location,
                ParametersResponse = locationParameters,
                Command = ""
                
            };

            if (success){
                _fileService.FilePath = "params/"+location.DeviceId+".txt";
                _fileService.DeleteFile();
                 return Ok(locationResponseDto);
            } else return BadRequest("Error writing location!");
            
        }

        [HttpPost("SendParameters")]
        public async Task<ActionResult<LocationParamSetDto>> SendParameters(
            LocationParamSetDto locationParamSetDto){

            if (!ModelState.IsValid) return BadRequest();

            if (locationParamSetDto == null || locationParamSetDto.DevicesList?.Count<1) {
                return BadRequest("Device list can not be empty");
            }

            var paramsString = locationParamSetDto.LocationParameters!.UpdateInterval.ToString()+","
            + locationParamSetDto.LocationParameters.MinUpdateInterval.ToString()+","
            + locationParamSetDto.LocationParameters.UpdateDistance.ToString()+","
            +locationParamSetDto.LocationParameters.StartOnBoot.ToString();

            foreach (string deviceId in locationParamSetDto.DevicesList!){

                _fileService.FilePath = "params/"+deviceId+".txt";

                var success = await _fileService.SaveToFileAsync(paramsString);

                if (!success){
                    _logger.LogError("Failed saving params for device: "+deviceId);
                }

            }

            return Ok(locationParamSetDto);
        }
        [HttpPost("AddDevice")]
        public async Task<ActionResult<string>> AddDevice(Device device){
            if (!ModelState.IsValid) return BadRequest();
            _fileService.FilePath = "devices/devicesList.txt";

            if (await _fileService.AppendToDevicesList(device.DeviceId!)){
                DevicesList ??= new List<string>();
                DevicesList.Add(device.DeviceId!);
                return Ok(device.DeviceId!);
            } else {
                return BadRequest("Error adding device to list!");
            }
        }
        [HttpGet("IsRegistered")]
        public ActionResult<string> IsDeviceRegistered(string deviceId){
            if (IsDeviceOnList(deviceId)){
                return Ok(deviceId);
            } else {
                return StatusCode(403, "Device not on the list!");
            }
            
        }

        [NonAction]
        private string GetParamsFromFile(string deviceId){
            _fileService.FilePath = "params/"+deviceId+".txt";
            var paramsString = _fileService.ReadParamsFromFile();
            return paramsString;
        }

        [NonAction]
        private LocationParameters GetParamsFromString (string paramsString){

            LocationParameters locationParameters = new LocationParameters();

            string[] stringValues = paramsString.Split(',');

            int updateInterval = int.Parse(stringValues[0]);
            int minUpdateInterval = int.Parse(stringValues[1]);
            int updateDistance = int.Parse(stringValues[2]);
            bool startOnBoot = bool.Parse(stringValues[3]);

            locationParameters.UpdateInterval = updateInterval;
            locationParameters.MinUpdateInterval = minUpdateInterval;
            locationParameters.UpdateDistance = updateDistance;
            locationParameters.StartOnBoot = startOnBoot;


            return locationParameters;
        }

        [NonAction]
        private bool IsDeviceOnList(string deviceId){

            return !(DevicesList == null || !DevicesList!.Contains(deviceId));
            
        }

    }
}