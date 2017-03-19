using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using GradDisplayScreenApi.Models;
using GradDisplayScreenApi.Models.ViewModels.Teleprompt;
using System.Collections.Generic;
using System.Collections;

namespace GradDisplayScreenApi.Controllers
{
    [Route("api/[controller]")]
    public class TelepromptController : Controller
    {
        private TelepromptDbContext _contextTeleprompt;
        private GradConfigDbContext _contextGradConfig;
        private GraduateDbContext _contextGraduate;
        private QueueDbContext _contextQueue;
        private readonly IHostingEnvironment _hostEnvironment;

        // mimic the way startup is getting
        // custom configuration variables
        public IConfigurationRoot Configuration { get; set; }

        public TelepromptController(TelepromptDbContext contextTeleprompt, GradConfigDbContext contextGradConfig, GraduateDbContext contextGraduate, QueueDbContext contextQueue, IHostingEnvironment hostEnvironment)
        {
            _contextTeleprompt = contextTeleprompt;
            _contextGradConfig = contextGradConfig;
            _contextGraduate = contextGraduate;
            _contextQueue = contextQueue;
            _hostEnvironment = hostEnvironment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // GET: api/teleprompt
        [HttpGet]
        public IActionResult Get()
        {
            /* get the teleprompt */
            var teleprompt = _contextTeleprompt.Teleprompt.FirstOrDefault();

            IDictionary<string, string> dict = new Dictionary<string, string>();

            dict["message"] = "status normal";
            if (teleprompt != null)
            {

                if (teleprompt.Status == 1)
                {
                    // remove the top of the queue
                    var itemTopQueue = _contextQueue.Queue.OrderBy(m => m.Created).FirstOrDefault();
                    if (itemTopQueue != null)
                    {
                        // clean teleprompt
                        foreach (var item in _contextTeleprompt.Teleprompt)
                        {
                            _contextTeleprompt.Teleprompt.Remove(item);
                        }
                        // save don't wait
                        _contextTeleprompt.SaveChanges();


                        _contextQueue.Remove(itemTopQueue);
                        // save don't wait
                        _contextQueue.SaveChanges();

                        // add to teleprompt
                        var respectedTime = DateTime.Now.ToString();

                        // push to teleprompt
                        var tel = new Teleprompt() { GraduateId = itemTopQueue.GraduateId, Created = itemTopQueue.Created };
                        _contextTeleprompt.Teleprompt.Add(tel);
                        _contextTeleprompt.SaveChanges();

                        // set status
                        var graduate = _contextGraduate.Graduate.FirstOrDefault(m => m.GraduateId == itemTopQueue.GraduateId);
                        if (graduate != null)
                        {
                            graduate.Status = 1;

                            _contextGraduate.Update(graduate);
                            _contextGraduate.SaveChanges();
                        }

                    }

                    dict["message"] = "teleprompt cleaned and popped queue";
                    return Json(dict);
                }

            }

            return Json(dict);
        }

        [HttpPost]
        [Route("/api/teleprompt/set/status")]
        public string SetTelepromptStatus(int status = 0)
        {
            /* get the teleprompt */
            var teleprompt = _contextTeleprompt.Teleprompt.FirstOrDefault();

            if (teleprompt != null)
            {
                teleprompt.Status = 1;
                _contextTeleprompt.Update(teleprompt);
                _contextTeleprompt.SaveChanges();

                return "success";
            }

            return "failed";
        }

        [HttpGet]
        [Route("/api/teleprompt/reset")]
        public string TelepromptOkayToReset()
        {
            /* get the teleprompt */
            var configShowInitialScreen = _contextGradConfig.GradConfig.FirstOrDefault(c => c.Name == "ShowInitialScreen" && c.UserId == "Global");

            if (configShowInitialScreen != null)
            {
                int iConf = Int32.Parse(configShowInitialScreen.Value);

                if (iConf == 1)
                {

                    var teleprompt = _contextTeleprompt.Teleprompt.FirstOrDefault();
                    var queue = _contextQueue.Queue.FirstOrDefault();


                    if (teleprompt == null && queue == null)
                    {
                        return "force_reset_screen";
                    }

                }
            }
            return "cant_force_reset";
        }
    }
}
