using Google.YouTube;

namespace youtube_bot_lib.api
{
    public interface IYouTubeAccountPool
    {
        YouTubeRequest getYouTubeRequest();
        void resetAllConnectionAndAccount();
    }
}