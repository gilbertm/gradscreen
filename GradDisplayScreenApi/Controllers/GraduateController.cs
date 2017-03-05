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
    public class GraduateController : Controller
    {
        private TelepromptDbContext _contextTeleprompt;
        private GradConfigDbContext _contextGradConfig;
        private GraduateDbContext _contextGraduate;
        private readonly IHostingEnvironment _hostEnvironment;

        // mimic the way startup is getting
        // custom configuration variables
        public IConfigurationRoot Configuration { get; set; }

        public GraduateController(TelepromptDbContext contextTeleprompt, GradConfigDbContext contextGradConfig, GraduateDbContext contextGraduate, IHostingEnvironment hostEnvironment)
        {
            _contextTeleprompt = contextTeleprompt;
            _contextGradConfig = contextGradConfig;
            _contextGraduate = contextGraduate;
            _hostEnvironment = hostEnvironment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            string voicePerson = null;
            string voiceExtra = null;
            bool isFullName = false;
            bool isBackgroundWithFooter = false;

            if (!String.IsNullOrEmpty(Configuration["Custom:Fields:fullname"]))
            {
                isFullName = Configuration["Custom:Fields:fullname"] == "yes" ? true : false;
            }

            if (!String.IsNullOrEmpty(Configuration["Custom:Template:Root:Default:withfooter"]))
            {
                isBackgroundWithFooter = Configuration["Custom:Template:Root:Default:withfooter"] == "yes" ? true : false;
            }

            if (!String.IsNullOrEmpty(Configuration["Custom:Voice:person"]))
            {
                voicePerson = Configuration["Custom:Voice:person"];
            }

            if (!String.IsNullOrEmpty(Configuration["Custom:Voice:extra"]))
            {
                voiceExtra = Configuration["Custom:Voice:extra"];
            }

            /* get the teleprompt */
            var mGraduate = _contextTeleprompt.Teleprompt.FirstOrDefault();

            if (mGraduate != null)
            {
                /* get the complete graduate details */
                var Graduate = _contextGraduate.Graduate.Where(x => x.GraduateId == mGraduate.GraduateId);

                if (Graduate != null)
                {

                    var vGraduate = (from m in Graduate
                                     select new TelepromptViewModel()
                                     {
                                         GraduateId = m.GraduateId,
                                         GraduateScannerId = m.GraduateScannerId,
                                         Status = m.Status,
                                         School = m.School,
                                         Program = m.Program,
                                         Major = m.Major,
                                         Merit = m.Merit,
                                         FirstName = m.FirstName,
                                         LastName = m.LastName,
                                         MiddleName = m.MiddleName,
                                         Fullname = m.Fullname,
                                         ArabicFullname = m.ArabicFullname,
                                         Arabic = m.Arabic
                                     }).FirstOrDefault();

                    if (vGraduate != null)
                    {
                        string dirPath = String.Concat(_hostEnvironment.WebRootPath, "\\", Configuration["Custom:Template:data"], "\\", Configuration["Custom:Template:Root:data"], "\\", Configuration["Custom:Template:Root:Default:data"], "\\");

                        // system template css
                        if (System.IO.Directory.Exists(dirPath))
                        {
                            // css
                            string strGraduateImageSystemTemplateCss = String.Concat("/", Configuration["Custom:Template:data"], "/", Configuration["Custom:Template:Root:data"], "/", Configuration["Custom:Template:Root:Default:data"], "/", "StyleSheet.css");
                            if (System.IO.File.Exists(String.Concat(dirPath, "\\", "StyleSheet.css")))
                            {
                                vGraduate.GraduateImageSystemTemplateCss = strGraduateImageSystemTemplateCss;
                            }

                            // image group
                            string strTemplateImageGroup = null;

                            // school images
                            string strGraduateImageSystemTemplateFlag = String.Concat("/", Configuration["Custom:Template:data"], "/", Configuration["Custom:Template:Root:data"], "/", Configuration["Custom:Template:Root:Default:data"]);
                            string strGraduateImageSystemTemplateMaple = String.Concat("/", Configuration["Custom:Template:data"], "/", Configuration["Custom:Template:Root:data"], "/", Configuration["Custom:Template:Root:Default:data"]);
                            string strGraduateImageSystemTemplateDate = String.Concat("/", Configuration["Custom:Template:data"], "/", Configuration["Custom:Template:Root:data"], "/", Configuration["Custom:Template:Root:Default:data"]);

                            switch (vGraduate.School.ToString().Trim())
                            {
                                case "School of Liberal Arts and Sciences":
                                    strTemplateImageGroup = "liberal";
                                    break;
                                case "School of Business Administration":
                                    strTemplateImageGroup = "business";
                                    break;
                                case "School of Architecture and Interior Design":
                                    strTemplateImageGroup = "architecture";
                                    break;
                                case "School of Environment and Health Sciences":
                                    strTemplateImageGroup = "environment";
                                    break;
                                case "School of Engineering, Applied Science and Technology":
                                    strTemplateImageGroup = "engineering";
                                    break;
                                case "School of Communication & Media Studies":
                                    strTemplateImageGroup = "communication";
                                    break;
                                case "School of Graduate Studies":
                                    strTemplateImageGroup = "graduate";
                                    break;
                                default:
                                    strTemplateImageGroup = "default";
                                    break;

                            }

                            strGraduateImageSystemTemplateFlag = String.Concat(strGraduateImageSystemTemplateFlag, "/", "flag-", strTemplateImageGroup, ".png");
                            strGraduateImageSystemTemplateMaple = String.Concat(strGraduateImageSystemTemplateMaple, "/", "date-", strTemplateImageGroup, ".png");
                            strGraduateImageSystemTemplateDate = String.Concat(strGraduateImageSystemTemplateDate, "/", "maple-", strTemplateImageGroup, ".png");

                            if (System.IO.File.Exists(String.Concat(dirPath, "flag-", strTemplateImageGroup, ".png")))
                            {
                                vGraduate.GraduateImageSystemTemplateFlag = strGraduateImageSystemTemplateFlag;
                            }

                            if (System.IO.File.Exists(String.Concat(dirPath, "date-", strTemplateImageGroup, ".png")))
                            {
                                vGraduate.GraduateImageSystemTemplateDate = strGraduateImageSystemTemplateDate;
                            }

                            if (System.IO.File.Exists(String.Concat(dirPath, "maple-", strTemplateImageGroup, ".png")))
                            {
                                vGraduate.GraduateImageSystemTemplateMaple = strGraduateImageSystemTemplateMaple;
                            }
                        }

                        // voice
                        if (!String.IsNullOrEmpty(voicePerson) && !String.IsNullOrEmpty(voiceExtra))
                        {
                            string filePathSound = String.Concat(_hostEnvironment.WebRootPath, "\\", Configuration["Custom:Resources:data"], "\\", Configuration["Custom:Resources:Sounds:data"], "\\", voicePerson, "\\", voiceExtra, "\\", vGraduate.GraduateId, ".wav");

                            if (System.IO.File.Exists(filePathSound))
                            {
                                vGraduate.GraduateSound = String.Concat("/", Configuration["Custom:Resources:data"], "/", Configuration["Custom:Resources:Sounds:data"], "/", voicePerson, "/", voiceExtra, "/", vGraduate.GraduateId, ".wav");

                            }
                        }
                        else
                        {
                            // no defaults
                            // use the root location 
                            string filePathSound = String.Concat(_hostEnvironment.WebRootPath, "\\", Configuration["Custom:Resources:data"], "\\", Configuration["Custom:Resources:Sounds:data"], "\\", vGraduate.GraduateId, ".wav");

                            if (System.IO.File.Exists(filePathSound))
                            {
                                vGraduate.GraduateSound = String.Concat("/", Configuration["Custom:Resources:data"], "/", Configuration["Custom:Resources:Sounds:data"], "/", vGraduate.GraduateId, ".wav");
                            }
                        }

                        // photo
                        string filePathImage = String.Concat(_hostEnvironment.WebRootPath, "\\", Configuration["Custom:Resources:data"], "\\", Configuration["Custom:Resources:Images:data"], "\\", vGraduate.GraduateId, ".jpg");

                        if (System.IO.File.Exists(filePathImage))
                        {
                            vGraduate.GraduatePicture = String.Concat("/", Configuration["Custom:Resources:data"], "/", Configuration["Custom:Resources:Images:data"], "/", vGraduate.GraduateId, ".jpg");
                        }

                        // is fullname
                        vGraduate.isFullname = "NO";

                        if (isFullName)
                        {
                            vGraduate.isFullname = "YES";
                        }

                        if (isBackgroundWithFooter)
                        {
                            vGraduate.isBackgroundWithFooter = "with-footer";
                        }


                    }

                    return Json(vGraduate);
                }

            }

            return Json(null);
        }
    }
}
