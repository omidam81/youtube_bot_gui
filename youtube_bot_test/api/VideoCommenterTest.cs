using System;
using NUnit.Framework;
using youtube_bot_lib.api;

namespace youtube_bot_test.api
{
    public class VideoCommenterTest
    {
        [Test] 
        public void commentVideoTest()
        {
            try
            {
                IVideoCommenter videoCommenter = new VideoCommenter(new YouTubeAccountPool(YouTubeAccountPool.getUsersFromXml(), 0), new CommentRepository());
                string videoId = "VFbYadm_mrw";
                Assert.IsTrue(videoCommenter.commentVideo(videoId));
            }
            catch (Exception e)
            {
                
                Assert.IsTrue(false, e.Message);
            }
        }
    }
}