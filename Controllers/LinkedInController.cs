using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Social_app;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SocialMediaMiner.Controllers
{
    public class LinkedInController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: LinkedIn
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Posts()
        {
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());
            var accesstoken = currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:linkedin");

            if (accesstoken == null)
            {
                return (new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound, "Problem with Access token"));
            }

            string url = String.Format("https://api.linkedin.com/v2/companies/65691523/updates?format=json&oauth2_access_token={0}", accesstoken.Value);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string result = await reader.ReadToEndAsync();
                updateFacebookData(result);
                ViewBag.result = result;
            }
            return View();
        }

        private void updateFacebookData(string result)
        {

        }
    }
}