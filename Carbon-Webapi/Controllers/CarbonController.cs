using Carbon_Webapi.Model;
using Carbon_Webapi.Util;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace Carbon_Webapi.Controllers
{
    [ApiController]
    [Route("")]
    public class CarbonController : ControllerBase
    {
        private readonly ILogger<CarbonController> _logger;

        private DownloadService _downloadService;
        public CarbonController(ILogger<CarbonController> logger,IConfiguration configuration)
        {
            _logger = logger;
            _downloadService = new DownloadService(configuration["firefoxPath"], int.Parse(configuration["MaxRetry"]));
        }

        [HttpPost]
        [Route("CarbonPng")]
        public IActionResult GetCarbonPng([FromBody] PngRequest request)
        {
            var path = _downloadService.GetPngPath(request);
            if (path == "")
            {
                return new JsonResult(new
                {
                    Status = "Error",
                    Message = "Max retries limits, generating process failed."
                });
            }
            _logger.LogInformation("[PngDownload]PngDownloadPath:{path}", path);
            return PhysicalFile(path, "image/jpg");
        }
    }
}
