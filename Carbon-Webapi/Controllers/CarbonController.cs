using Carbon_Webapi.Model;
using Carbon_Webapi.Util;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace Carbon_Webapi.Controllers
{
    [ApiController]
    [Route("")]
    public class CarbonController
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
        public HttpResponseMessage GetCarbonPng([FromBody] PngRequest request)
        {
            var path = _downloadService.GetPngPath(request);
            HttpResponseMessage resp;
            if (path == "")
            {
                resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new
                    {
                        Status = "Error",
                        Message = "Max retry limit, generating process failed."
                    })
                };
                return resp;
            }
            _logger.LogInformation("PngDownload", "PngDownloadPath=" + path);
            var imgStream = new MemoryStream(File.ReadAllBytes(path));
            resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(imgStream)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return resp;
        }
    }
}
