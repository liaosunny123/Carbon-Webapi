using Carbon_Webapi.Model;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using System.Text;
using System.Web;

namespace Carbon_Webapi.Util
{
    public class DownloadService
    {
        private string _firefoxPath;

        private int _maxRetry;

        public DownloadService(string FirefoxPath,int MaxRetry)
        {
            _firefoxPath = FirefoxPath;
            _maxRetry = MaxRetry;
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Cache")))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Cache"));
        }

        public string GetPngPath(PngRequest model)
        {
            string defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "Cache");
            string randomPath = Path.Combine(defaultPath, GetTimeStamp().ToString());
            FirefoxOptions tempOptions = new FirefoxOptions();
            tempOptions.BrowserExecutableLocation = _firefoxPath;
            tempOptions.AddArgument("--headless");
            tempOptions.SetPreference("browser.download.folderList", 2);
            tempOptions.SetPreference("browser.download.manager.showWhenStarting", false);
            tempOptions.SetPreference("browser.helperApps.alwaysAsk.force", false);
            tempOptions.SetPreference("browser.download.dir", randomPath);
            tempOptions.SetPreference("browser.download.useDownloadDir", true);
            tempOptions.SetPreference("permissions.default.stylesheet", 2);
            tempOptions.AcceptInsecureCertificates = true;
            using (IWebDriver driver = new FirefoxDriver(tempOptions))
            {
                driver.Navigate().GoToUrl($"{ResovlePngQuery(model)}");
                driver.FindElement(By.ClassName("jsx-2184717013")).Click();
                int trys = 0;
                randomPath = randomPath + "\\carbon.png";
                while (!File.Exists(randomPath))
                {
                    if (trys > _maxRetry)
                    {
                        randomPath = ""; // 表示最大重试次数
                        break;
                    }
                    Thread.Sleep(100);
                    trys++;
                }
            }
            return randomPath;
        }

        private long GetTimeStamp()
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new System.DateTime(1970, 1, 1));
            long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds;
            return timeStamp;
        }

        private string ResovlePngQuery(PngRequest model)
        {
            StringBuilder stringBuilder = new StringBuilder("https://carbon.now.sh/?");
            stringBuilder.Append($"t={model.Theme}");
            stringBuilder.Append($"&l={MatchCodeType(model.CodeType)}");
            stringBuilder.Append($"&code={PrepareCode(model.Code)}");
            return stringBuilder.ToString();
        }

        private string MatchCodeType(string codeType)
            => HttpUtility.UrlEncode(codeType switch
            {
                "csharp" => "text/x-csharp",
                "c++" => "text/x-c++src",
                "auto" => "auto",
                "apache" => "text/apache",
                "c" => "text/x-csrc",

                _ => codeType
            });

        private string PrepareCode(string code)
            => HttpUtility.UrlEncode(code.Replace("\n", "%0A"));
    }
}
