using System.IO;
using youtube_bot_lib.api;

namespace youtube_bot_test.mock
{
    public class FileAdapter:IAdapter
    {
                private string url;

        public FileAdapter(string url)
        {
            this.url = url;
        }

        public string readPageSource()
        {
            TextReader reader = new StreamReader(url);
            return reader.ReadToEnd();
        }

        public void setUrl(string searchString, long pageNumber)
        {
            throw new System.NotImplementedException();
        }

        public void setUrl(string url)
        {
            this.url = url;
        }
        public string getUrl()
        {
            return url;
        }

        public string getSearchString()
        {
            throw new System.NotImplementedException();
        }
    }
}