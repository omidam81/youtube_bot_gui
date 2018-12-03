using Google.YouTube;
using youtube_bot_lib.api;

namespace youtube_bot_test.mock
{
    public class YouTubeAccountPoolMock:IYouTubeAccountPool
    {
        public YouTubeRequest getYouTubeRequest()
        {
            throw new System.NotImplementedException();
        }

        public void resetAllConnectionAndAccount()
        {
            throw new System.NotImplementedException();
        }
    }
}