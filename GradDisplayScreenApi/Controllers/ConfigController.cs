using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using GradDisplayScreenApi.Models;
using GradDisplayScreenApi.Models.ViewModels.Teleprompt;

namespace GradDisplayScreenApi.Controllers
{
    [Route("api/[controller]")]
    public class ConfigController : Controller
    {
        private GradConfigDbContext _contextGradConfig;
       

        public ConfigController(GradConfigDbContext contextGradConfig)
        {
            _contextGradConfig = contextGradConfig;
        }

        // GET: api/config
        [HttpGet]
        [Route("/api/config/get/initialscreen")]
        public IActionResult Get()
        {
            /* get the teleprompt */
            var conf = _contextGradConfig.GradConfig.FirstOrDefault(c => c.Name == "ShowInitialScreen" && c.UserId == "Global");

            if (conf != null)
            {
                return Json(conf); 
            }

            conf = new GradConfig();
            conf.Id = 0;
            conf.Name = "ShowInitialScreen";
            conf.UserId = "Global";
            conf.Value = "0";

            return Json(conf);
        }

        [HttpPost]
        [Route("/api/config/set/initialscreen")]
        public string SetShowInitialScreen(int initialscreen = 0)
        {

            var configShowInitialScreen = _contextGradConfig.GradConfig.SingleOrDefault(c => c.UserId == "Global" && c.Name == "ShowInitialScreen");

            if (configShowInitialScreen != null)
            {
                configShowInitialScreen.Value = initialscreen.ToString();
                _contextGradConfig.Update(configShowInitialScreen);

                _contextGradConfig.SaveChanges();

                return "success";
            }

            return "failed";
        }
    }
}
