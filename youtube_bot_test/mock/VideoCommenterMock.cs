using System;
using youtube_bot_lib.api;

namespace youtube_bot_test.mock
{
    public class VideoCommenterMock:IVideoCommenter
    {
        private IYouTubeAccountPool accountPool;
        private ICommentRepository commentRepository;

        public VideoCommenterMock(IYouTubeAccountPool accountPool, ICommentRepository commentRepository)
        {
            this.accountPool = accountPool;
            this.commentRepository = commentRepository;
        }

        public bool commentVideo(string videoId)
        {
            try
            {
                return true;
            }
            catch (Exception e)
            {
                
                throw new Exception("Error in commenting video: " + videoId, e);
            }
        }
    }
}