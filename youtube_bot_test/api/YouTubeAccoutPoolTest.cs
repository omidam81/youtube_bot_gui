using System;
using System.Collections.Generic;
using NUnit.Framework;
using youtube_bot_lib.api;
using youtube_bot_lib.model;

namespace youtube_bot_test.api
{
    public class YouTubeAccoutPoolTest:BaseTest
    {
         [Test]
        public void getYouTubeRequestTest()
         {
             try
             {
                 IList<User> users = new List<User>();
                 User user = new User();
                 user.UserName = "test";
                 user.Password = "";
                 user.apiKey = "";
                 user.AppName = "TEST";
                 users.Add(user);
                 IYouTubeAccountPool pool = new YouTubeAccountPool(users, 3);
                 Assert.IsNotNull(pool.getYouTubeRequest());
             }
             catch (Exception e)
             {
                 
                 Assert.IsTrue(false, e.Message);
             }
         }
    }
}