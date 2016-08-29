using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using InstagramWrapper.Model;
using InstaSharp.Models;


namespace Web.Models
{
    public static class TokenClass
    {
        public static OuthUser OuthUser { get; set; }
    }

    public class CommentGetter
    {
        public void GetComments()
        {
            var userMethod = new InstagramWrapper.EndPoints.Users();

            var userProfile = userMethod.GetUserSelf(TokenClass.OuthUser.access_token);
            var userProfile2 = userMethod.GetUserByUsername("shnurovs", TokenClass.OuthUser.access_token);
            var userContent = userMethod.GetUserMedia(userProfile2.id.ToString(), TokenClass.OuthUser.access_token);

            var comments = new List<comment>();
            var commentMethod = new InstagramWrapper.EndPoints.Comments();
            var likesMethod = new InstagramWrapper.EndPoints.Likes();
            

            
            foreach (var item in userContent.data )
            {

               //var resp =  likesMethod.LikeMedia(item.id, TokenClass.OuthUser.access_token);

               var c = commentMethod.GetMediaComments(item.id, TokenClass.OuthUser.access_token);
                comments.AddRange(c.data);

                //var rep2 = likesMethod.GetMediaLikes(item.id, TokenClass.OuthUser.access_token);


            }

            string pattern = @"@\w+";
            Regex regex = new Regex(pattern);
            var users = new List<string>();

            foreach (var item in comments)
            {


                foreach (Match match in regex.Matches(item.text))
                {
                    users.Add(match.Groups[0].Value);
                }


              

              //  var t = commentMethod.DeleteComment(item.id, "1320333004400398859_442445510", TokenClass.OuthUser.access_token);
            }

            var userProfile3 = userMethod.GetUserByUsername("remont.lucky.wn", TokenClass.OuthUser.access_token);
            var userContent2 = userMethod.GetUserMedia(userProfile3.id.ToString(), TokenClass.OuthUser.access_token);

            for (var i = 0; i<= (users.Count / 5); i++)
            {
                var text = string.Join(", ",users.Skip(i*5).Take(5)); 
                var tt= commentMethod.PostComment(userContent2.data[0].id, text, TokenClass.OuthUser.access_token);
            }
        }
    }
}