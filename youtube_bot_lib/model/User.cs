using System;
using System.Collections.Generic;
using System.Net;

namespace youtube_bot_lib.model
{
    public class User
    {
        public User()
        {
            
           
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime lastActivity { get; set; }
        public List<Cookie> Cookies { get; set; }
        public string Proxy { get; set; }
        public bool Enabled { get; set; }
        public string appName;
        public string apiKey;

        public string ApiKey
        {
            get { return apiKey; }
            set { apiKey = value; }
        }

        public string AppName
        {
            get { return appName; }
            set { appName = value; }
        }


        public bool IsLogin { get; set; }
                       
        private int todayCommentsCount = 0;
       
     
    }
}