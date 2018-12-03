using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Google.YouTube;

namespace youtube_bot_lib.api
{
    public class VideoCommenter:IVideoCommenter
    {
        private IYouTubeAccountPool accountPool;
        private ICommentRepository commentRepository;

        public VideoCommenter(IYouTubeAccountPool accountPool, ICommentRepository commentRepository)
        {
            this.accountPool = accountPool;
            this.commentRepository = commentRepository;
        }

        public bool commentVideo(string videoId)
        {
            try
            {
                Console.WriteLine(videoId);
                Uri Url = new Uri("http://gdata.youtube.com/feeds/api/videos/" + videoId);
                var account = accountPool.getYouTubeRequest();
                Video video = account.Retrieve<Video>(Url);
                //if (video.ComplaintUri.AbsoluteUri.Contains("/comments"))
                //{
                //    video =
                //        accountPool.getYouTubeRequest().Retrieve<Video>(
                //            new Uri(video.ComplaintUri.AbsoluteUri.Replace("/comments", "")));
                //}
                //Video video = new Video();
                //video.VideoId = videoId;
                //System.Threading.Thread.Sleep(50000);
                string rating = commentRepository.getRating();
                video.Rating = int.Parse(rating);
                Comment c = new Comment();
                c.Content = commentRepository.getComment();
                //accountPool.getYouTubeRequest().Insert(video.RatingsUri, video);
                account.AddComment(video, c);
                Console.WriteLine("Comment successfully added to : " + videoId);
                return true;
            }
            catch (Exception e)
            {                
                new Exception("Error in commenting video: " + videoId, e);
                //Console.WriteLine("Error in commenting video: " + videoId + Environment.NewLine + e.Message);
                Semaphore.WaitOne();
                StreamWriter writer = new StreamWriter("error.log", true);
                writer.WriteLine("Error in commenting video: " + videoId + Environment.NewLine + e.Message);
                writer.Close();
                Semaphore.Release();
                accountPool.resetAllConnectionAndAccount();
                return false;
            }
        }
        public static Semaphore Semaphore = new Semaphore(1, 1);
    }
}