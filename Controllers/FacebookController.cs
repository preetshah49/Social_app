using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Social_app.Models;


namespace Social_app.Controllers
{
    [Authorize]
    public class FacebookController : Controller
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

        // GET: Facebook
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Posts()
        {
            //Access Token
            var currentClaims = await UserManager.GetClaimsAsync(HttpContext.User.Identity.GetUserId());
            var accesstoken = currentClaims.FirstOrDefault(x => x.Type == "urn:tokens:facebook");

            if (accesstoken == null)
            {
                return (new HttpStatusCodeResult(HttpStatusCode.NotFound, "Token not found"));
            }

            //format a url  to retrieve posts or feed data from facebook
            string url = string.Format(
                "https://graph.facebook.com/me?fields=id,name,feed.limit(1000){{attachments,message,story,created_time}}&access_token=EAAYZBjZBHfshEBAKUMJkiZBSeGirVKLlJ8w59N8dbmUHCvd9UXe2B9RGoeDudmrISqQEzAwtvYbRQIrOnmewZAtYXQbvG4qAglPFyylYmZBeSn8lzS8j9z6O7EiUuahbvOhoScs6OEEN1IgZBbFB7xdmvvpdxD83I6GmLoi7WRHuNXeaODZAEb9bEpBsyTRi9vY7AiuJiBwfpnYNfd5ZBFvJLt6cvXIkOWyB5ZCDIly8KOQZDZD", accesstoken.Value);
           
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";

            using(HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string result = await reader.ReadToEndAsync();

                dynamic jsonobj = System.Web.Helpers.Json.Decode(result);

                Models.SocialMedia.Facebook.posts posts = new Models.SocialMedia.Facebook.posts(jsonobj);

                ViewBag.JSON = result;
                return View(posts);
            }
           
        }
    }
}