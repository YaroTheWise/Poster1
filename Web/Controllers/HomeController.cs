using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InstagramWrapper.Model;
using InstagramWrapper.Service;
using InstaSharp;
using InstaSharp.Models.Responses;
using Newtonsoft.Json;
using Web.Models;
using CommentsResponse = InstagramWrapper.Model.CommentsResponse;

namespace Web.Controllers
{
    public class Result 
    {
        public string username { get; set; }
        public string status { get; set; }
    }

    public class HomeController : Controller
    {
        private static string clientId = "f56b2d2b099a41d19db9955812455f0b";
        private static string clientSecret = "9aa272cbdbc84b73ab951d54e39aa45c";
        private static string redirectUri = "http://localhost:49230/home/oauth";
        private static string realtimeUri = "";

        InstagramConfig config = new InstagramConfig(clientId, clientSecret, redirectUri, realtimeUri);

        public async Task<ActionResult> Index()
        {
            var client = new HttpClient();
            //var uri = new Uri("https://www.instagram.com/query/?q=ig_user(3){id,username,external_url,full_name,profile_pic_url,biography,followed_by{count},follows{count},media{count},is_private,is_verified}");
            var uri = new Uri("https://www.instagram.com/query/?q=ig_user(3){username}");
            var third = await client.GetAsync(uri);

            var t = await third.Content.ReadAsStringAsync() ;
            var result = JsonConvert.DeserializeObject<Result>(t);




            var scopes = new List<OAuth.Scope>();
            scopes.Add(InstaSharp.OAuth.Scope.Likes);
            scopes.Add(InstaSharp.OAuth.Scope.Comments);

            //var link = InstaSharp.OAuth.AuthLink(config.OAuthUri + "authorize", config.ClientId, config.RedirectUri, scopes, InstaSharp.OAuth.ResponseType.Code);
            var link =
                "https://api.instagram.com/oauth/authorize/?client_id=f56b2d2b099a41d19db9955812455f0b&redirect_uri=http://localhost:49230/home/oauth&response_type=code&scope=public_content+follower_list+comments+relationships+likes";
            return Redirect(link);
        }

        public async Task<ActionResult> OAuth(string code)
        {
            var instagramAuth = new InstagramAuth();
            var config = new InstagramWrapper.Model.InstaConfig
            {
                client_id= "f56b2d2b099a41d19db9955812455f0b",
                client_secret= "9aa272cbdbc84b73ab951d54e39aa45c",
                redirect_uri= "http://localhost:49230/home/oauth",
                website_url = "http://localhost:49230",
            };

            TokenClass.OuthUser = instagramAuth.GetAccessToken(code, config);

            var cl = new CommentGetter();

            cl.GetComments();

            //var userMethod = new InstagramWrapper.EndPoints.Users();

            ////var users = userMethod.UserSearch("shnurovs", result.access_token);

            ////var user = users.data.Single(x => x.username == "shnurovs");

            //var users2 = userMethod.UserSearch("remont.lucky.wn", result.access_token);

            //var user2 = users2.data.Single(x => x.username == "remont.lucky.wn");
            

            ////var t = new InstagramWrapper.EndPoints.Relationship();
            ////var follows = t.GetUserFollows(user.username, result.access_token,"20");

            //var ac = new InstagramCall();
            //Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("access_token", result.access_token);
            ////if (next_cursor != "1")
            ////parameters.Add("cursor", "1");
            //var js = ac.ApiCall(string.Format("users/{0}/follows", "shnurovs"), parameters);
            //var js1 = ac.ApiCall(string.Format("users/{0}/follows", "remont.lucky.wn"), parameters);
            ////var response = JsonConvert.DeserializeObject<UserProfiles>(js);
            ////return response;
            //var userMedia = userMethod.GetUserMedia(user2.id.ToString(), result.access_token, "1");
            //var commentMethod = new InstagramWrapper.EndPoints.Comments();

            //var comments = commentMethod.GetMediaComments(userMedia.data.First().id, result.access_token);

            ////var auth = new OAuth(config);

            ////var oauthResponse = await auth.RequestToken(code);
            //Dictionary<string, string> parameters2 = new Dictionary<string, string>();
            //parameters2.Add("cursor", "1");
            //parameters2.Add("access_token", result.access_token);
            //var url2 = String.Format("media/{0}/comments", userMedia.data.First().id);
            //var result2 = ac.ApiCall(url2, parameters2);
            //var response2 = JsonConvert.DeserializeObject<CommentsResponse>(result2);

            //Session.Add("InstaSharp.AuthInfo", result);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> MyFeed()
        {
            var oAuthResponse = Session["InstaSharp.AuthInfo"] as OAuthResponse;

            if (oAuthResponse == null)
            {
                return RedirectToAction("Index");
            }

            var users = new InstaSharp.Endpoints.Users(config, oAuthResponse);

            var feed = await users.Feed(null, null, null);

            //return System.Web.UI.WebControls.View(feed.Data);
            return View();
        }
    }
}