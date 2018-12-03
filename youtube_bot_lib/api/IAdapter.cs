namespace youtube_bot_lib.api
{
    public interface IAdapter
    {
        string readPageSource();
        void setUrl(string searchString, long pageNumber);
        void setUrl(string url);
        string getUrl();
        string getSearchString();
    }
}