using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using Google.YouTube;
using youtube_bot_lib.model;

namespace youtube_bot_lib.api
{
    public class YouTubeAccountPool : IYouTubeAccountPool
    {
        private IList<YouTubeRequestSettings> settings;
        private IList<YouTubeRequest> requests;
        private IList<DateTime> lastUsed;

        private IList<User> users;

        public static IList<User> getUsersFromXml()
        {
            IList<User> users = new List<User>();
            XmlDocument document = new XmlDocument();
            document.Load("accounts.xml");
            XmlNodeList accountsNodes = document.GetElementsByTagName("accounts");
            XmlNodeList accountNodes = accountsNodes[0].ChildNodes;
            for (int i = 0; i < accountNodes.Count; i++)
            {
                if (!accountNodes[i].Name.ToLower().Equals("account"))
                {
                    continue;
                }
                XmlNodeList list = accountNodes[i].ChildNodes;
                string username = "";
                string password = "";
                string apiKey = "";
                string appName = "";
                string proxy = "";

                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].Name.ToLower().Equals("username"))
                    {
                        username = list[j].InnerText;
                    }
                    else if (list[j].Name.ToLower().Equals("password"))
                    {
                        password = list[j].InnerText;
                    }
                    else if (list[j].Name.ToLower().Equals("app-name"))
                    {
                        appName = list[j].InnerText;
                    }
                    else if (list[j].Name.ToLower().Equals("proxy"))
                    {
                        proxy = list[j].InnerText;
                    }
                    else
                    {
                        apiKey = list[j].InnerText;
                    }
                }
                User user = new User();
                user.UserName = username;
                user.Password = password;
                user.Proxy = proxy;
                user.appName = appName;
                user.apiKey = apiKey;
                users.Add(user);
            }
            return users;
        }
        Random random = new Random();
        private int minDelayMinute;
        public YouTubeAccountPool(IList<User> users, int minDelayMinute)
        {
            settings = new List<YouTubeRequestSettings>();
            requests = new List<YouTubeRequest>();
            this.users = users;
            lastUsed = new List<DateTime>();
            foreach (var user in users)
            {
                YouTubeRequestSettings s = new YouTubeRequestSettings(user.AppName, user.ApiKey, user.UserName,
                                                                      user.Password);
                s.Timeout = 10000000;
                settings.Add(s);
                YouTubeRequest r = new YouTubeRequest(s);
                r.Proxy = GetProxyForUser(user);
                requests.Add(r);
                
                lastUsed.Add(DateTime.Now.Subtract(new TimeSpan(0, 0, (int)(2*(random.NextDouble() + 0.01)*minDelayMinute), 0)));
            }
            this.minDelayMinute = minDelayMinute;
        }

        public void resetAllConnectionAndAccount()
        {
            try
            {
                Console.WriteLine("Resetting accounts.");
                System.Threading.Thread.Sleep(60 * 1000 * minDelayMinute / 2);

                settings = new List<YouTubeRequestSettings>();
                requests = new List<YouTubeRequest>();                
                lastUsed = new List<DateTime>();
                foreach (var user in users)
                {
                    YouTubeRequestSettings s = new YouTubeRequestSettings(user.AppName, user.ApiKey, user.UserName,
                                                                          user.Password);
                    s.Timeout = 10000000;
                    settings.Add(s);
                    YouTubeRequest r = new YouTubeRequest(s);
                    r.Proxy = GetProxyForUser(user);
                    requests.Add(r);

                    lastUsed.Add(DateTime.Now.Subtract(new TimeSpan(0, 0, (int)(2 * (random.NextDouble() + 0.01) * minDelayMinute), 0)));
                }
            }
            catch (Exception)
            {
                
                
            }
        }

        private IWebProxy GetProxyForUser(User User)
        {
            var Proxy = User.Proxy;

            var strs = Proxy.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            try
            {
                if (strs.Length == 2)
                {
                    WebProxy W = new WebProxy(User.Proxy);
                    return W;
                }
                else if (strs.Length == 3)
                {
                    WebProxy W = new WebProxy(string.Join(":", strs[0], strs[1]));
                    W.Credentials = new NetworkCredential() {UserName = strs[2]};

                    return W;
                }
                else if (strs.Length == 4)
                {
                    WebProxy W = new WebProxy(string.Join(":", strs[0], strs[1]));
                    W.Credentials = new NetworkCredential() {UserName = strs[2], Password = strs[3]};
                    return W;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public YouTubeRequest getYouTubeRequest()
        {
            double max = 0;
            int max_id = 0;
            for (int j = 0; j < lastUsed.Count; j++)
            {
                double temp = DateTime.Now.Subtract(lastUsed[j]).TotalMinutes;
                if (temp > max)
                {
                    max_id = j;
                    max = temp;
                }
            }
            if (max >= minDelayMinute)
            {
                lastUsed[max_id] = DateTime.Now;
                return requests[max_id];
            }
            else
            {
                double delay =  minDelayMinute - max >= 0? (minDelayMinute - max):0;
                Console.WriteLine("Wait for " + delay + " minutes");
                System.Threading.Thread.Sleep((int)(60 * 1000 * (delay)));
                
                return getYouTubeRequest();
            }
        }
    }
}