using System.Runtime.CompilerServices;
using LocationApi.DTOs;
using LocationApi.Models;
using LocationApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        public async Task< ActionResult<LocationResponseDto>> WriteLocation(Location location){


            _logger.LogInformation("LOCATION RECEIVED: "+location.DeviceId+" "+location.Latitude.ToString()
            +" - "+location.Longitude.ToString()
            +" - "+location.Accuracy.ToString());

            TrackingProfileDto trackingProfileDto = GetProfileFromString(location.DeviceId!);

            if (!IsDeviceOnList(location.DeviceId!)){
                return StatusCode(403, "Device not on the list!");
            }

            LocationParameters locationParameters = null!;

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

            LocationResponseDto locationResponseDto = new LocationResponseDto
            {
                ParametersResponse = locationParameters,
                Message = ""
            };

            if (trackingProfileDto != null && locationParameters != null){
                locationResponseDto = new LocationResponseDto()
                {
                    ParametersResponse = locationParameters,
                    TrackingProfile = trackingProfileDto.TrackingProfile,
                    WorkDays = trackingProfileDto.WorkDays,
                    WorkTime = trackingProfileDto.WorkTime,
                    Message = trackingProfileDto.Message                
                };
            }
            
            if (success){
                _fileService.FilePath = "params/"+location.DeviceId+".txt";
                _fileService.DeleteFile();
                 return Ok(locationResponseDto);
            } else return BadRequest("Error writing location!");
            
        }

        [HttpPost("SendTrackingProfile")]
        public async Task<ActionResult<TrackingProfileDto>> SendTrackingProfile(TrackingProfileDto trackingProfileDto){
            
            if (!ModelState.IsValid) return BadRequest();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _logger.LogInformation(JsonSerializer.Serialize(trackingProfileDto, options));

            var trackingProfileDtoJson = JsonSerializer.Serialize(trackingProfileDto, options);

            _fileService.FilePath = "params/"+trackingProfileDto.DeviceId+"_profile.txt";

            var success = await _fileService.SaveToFileAsync(trackingProfileDtoJson, "params");

            if (!success){
                _logger.LogError("Failed saving tracking profile for device: "+trackingProfileDto.DeviceId);
                return BadRequest("Error writing tracking profile");
            }
            
            return Ok(trackingProfileDto);

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

                var success = await _fileService.SaveToFileAsync(paramsString, "params");

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
        [HttpGet("{deviceId}/{month}")]
        public ActionResult<string> Report([FromRoute(Name = "deviceId")] string id, [FromRoute(Name = "month")] string month){

            DummyReport dummyReport = new DummyReport();

            dummyReport.device_id = id;
            
            return Ok(dummyReport);
        }

        [NonAction]
        private string GetParamsFromFile(string deviceId){
            _fileService.FilePath = "params/"+deviceId+".txt";
            var paramsString = _fileService.ReadParamsFromFile();
            return paramsString;
        }

        [NonAction]
        private string GetProfileFromFile(string deviceId){
            _fileService.FilePath = "params/"+deviceId+"_profile.txt";
            var profileJson = _fileService.ReadParamsFromFile();
            return profileJson;
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

        [NonAction]
        private TrackingProfileDto GetProfileFromString (string deviceId){

            TrackingProfileDto trackingProfileDto = null!;

            var trackingProfileJson = GetProfileFromFile(deviceId!);

            if (GetProfileFromFile(deviceId!) != null){

               
                _logger.LogInformation("GOT PROFILE FROM FILE: "+trackingProfileJson);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                trackingProfileDto = JsonSerializer.Deserialize<TrackingProfileDto>(
                    trackingProfileJson, options
                    )!;
            }

            return trackingProfileDto;
        }

    }

}