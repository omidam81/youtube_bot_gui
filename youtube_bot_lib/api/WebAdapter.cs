using System;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace youtube_bot_lib.api
{
    public class WebAdapter:IAdapter
    {
        //sample url
        private string baseSearchUrl =
            "http://www.youtube.com/results?search_type=videos&search_sort=video_view_count&search_query=";
        //http://www.youtube.com/results?search_type=videos&search_sort=video_view_count&search_query=
        private string url;

        public WebAdapter(string url)
        {
            this.url = url;
            proxy = getProxyFromXml();
        }

        public WebAdapter(string searchString, long pageNumber)
        {
            Regex rex = new Regex(" +");
            searchString = rex.Replace(searchString, "+");
            this.searchString = searchString;
            this.pageNumber = pageNumber;
            url = baseSearchUrl + this.searchString + "&page=" + this.pageNumber.ToString();
            proxy = getProxyFromXml();
        }

        private string proxy;
        private string searchString;
        private long pageNumber;

        public string getSearchString()
        {
            return searchString;
        }
        public static string getProxyFromXml()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(AppDomain.CurrentDomain.BaseDirectory + "accounts.xml");
                XmlNodeList accountsNodes = document.GetElementsByTagName("main-proxy");
                XmlNodeList proxyNodes = accountsNodes[0].ChildNodes;

                if (proxyNodes.Count == 0)
                {
                    return "";
                }
                    return proxyNodes[0].InnerText.Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", "");
            }
            catch (Exception e)
            {                
                throw new Exception("File not found.", e);
            }          
        }

        public string readPageSource()
        {
            try
            {

                WebClient Wr = new WebClient();
                //W.Proxy = new WebProxy("127.0.0.1", 8580);
                WebProxy W = null;


                var Proxy = proxy;

                var strs = Proxy.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    if (strs.Length == 2)
                    {
                        W = new WebProxy(Proxy);
                       
                    }
                    else if (strs.Length == 3)
                    {
                        W = new WebProxy(string.Join(":", strs[0], strs[1]));
                        W.Credentials = new NetworkCredential() { UserName = strs[2] };
                        
                    }
                    else if (strs.Length == 4)
                    {
                        W = new WebProxy(string.Join(":", strs[0], strs[1]));
                        W.Credentials = new NetworkCredential() { UserName = strs[2], Password = strs[3] };
                        
                    }
                    
                }
                catch (Exception)
                {
                    
                }
                if(!proxy.Equals(""))
                    Wr.Proxy = W;
                try
                {

                try
                {
                    return Wr.DownloadString(url);
                }
                catch (Exception)
                {
                    
                    System.Threading.Thread.Sleep(10000);
                    return Wr.DownloadString(url);
                }

                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(30000);
                    return Wr.DownloadString(url);
                }

            }
            catch (Exception e)
            {

                throw new Exception("could not load webpage", e);

            }
        }

        public void setUrl(string searchString, long pageNumber)
        {
            Regex regex = new Regex(" +");
            searchString = regex.Replace(searchString, "+");
            this.searchString = searchString;
            this.pageNumber = pageNumber;
            this.url = baseSearchUrl+searchString + "&page=" + this.pageNumber.ToString();
        }

        public void setUrl(string url)
        {
            this.url = url;
        }

        public string getUrl()
        {
            return url;
        } 
    }
}