using System;
using NUnit.Framework;
using youtube_bot_lib.api;
using youtube_bot_test.mock;
using System.Collections.Generic;
namespace youtube_bot_test
{
    public class VideoAddressExtractorTest:BaseTest
    {
         [Test]
        public void VideoAddressExtractor1Test()
         {
             try
             {
                 
                 FileAdapter adapter = new FileAdapter("resources\\1_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter);
                 IList<string> list = extractorAndCommenter.extractVideoAddresses(adapter.readPageSource());
                 Assert.IsTrue(list.Count == 20);
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);
                 
             }
         }

         [Test]
         public void VideoAddressExtractor2Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\2_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter);
                 IList<string> list = extractorAndCommenter.extractVideoAddresses(adapter.readPageSource());
                 Assert.IsTrue(list.Count == 20);
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

         [Test]
         public void VideoAddressExtractor3Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\simple_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 IList<string> list = extractorAndCommenter.extractVideoAddresses(adapter.readPageSource());
                 Assert.IsTrue(list.Count == 2);
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

         [Test]
         public void VideoAddressExtractor4Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\3_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 IList<string> list = extractorAndCommenter.extractVideoAddresses(adapter.readPageSource());
                 Assert.IsTrue(list.Count == 20);
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }


         [Test]
         public void VideoAddressExtractor5Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\4_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 IList<string> list = extractorAndCommenter.extractVideoAddresses(adapter.readPageSource());
                 Assert.IsTrue(list.Count == 16);
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }
         [Test]
         public void getNextPageNumberzeroTest()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\simple_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 Assert.IsTrue(extractorAndCommenter.getNextPageNumber(adapter.readPageSource()) == 0);
                 
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

         [Test]
         public void getNextPageNumberPage2Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\1_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 Assert.IsTrue(extractorAndCommenter.getNextPageNumber(adapter.readPageSource()) == 2);

             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

         [Test]
         public void getNextPageNumberAnotherPage2Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\2_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 Assert.IsTrue(extractorAndCommenter.getNextPageNumber(adapter.readPageSource()) == 2);

             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

         [Test]
         public void getNextPageNumberAnotherPage3Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\3_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 Assert.IsTrue(extractorAndCommenter.getNextPageNumber(adapter.readPageSource()) == 15);

             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }
         [Test]
         public void getNextPageNumberLastPageTest()
         {
             try
             {
                 FileAdapter adapter = new FileAdapter("resources\\last_index_page.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 Assert.IsTrue(extractorAndCommenter.getNextPageNumber(adapter.readPageSource()) == 0);
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

        

         [Test]
         public void extractVideosAndInsertACommentWithNoCommentTest()
         {
             try
             {
                 IYouTubeAccountPool accountPool = new YouTubeAccountPool(YouTubeAccountPool.getUsersFromXml(), 5);
                 ICommentRepository commentRepository = new CommentRepository();
                 IVideoCommenter videoCommenter = new VideoCommenter(accountPool, commentRepository);
                 IAdapter adapter = new WebAdapter("test", 1);
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, videoCommenter, 10);
                 extractorAndCommenter.extractVideosAndInsertAComment();
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

         [Test]
         public void extractVideosAndInsertACommentWithNoCommentMultipleCriteriaTest()
         {
             try
             {
                 IYouTubeAccountPool accountPool = new YouTubeAccountPool(YouTubeAccountPool.getUsersFromXml(), 5);
                 ICommentRepository commentRepository = new CommentRepository();
                 IVideoCommenter videoCommenter = new VideoCommenter(accountPool, commentRepository);
                 IAdapter adapter = new WebAdapter("test", 1);
                 IList<string> list = new List<string>();
                 list.Add("test");
                 list.Add("baby");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, videoCommenter, list, 10);
                 extractorAndCommenter.extractVideosAndInsertAComment();
             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

         [Test]
         public void getRelatedVideoAddressTest()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\1_video.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 Assert.IsTrue(extractorAndCommenter.getRelatedVideoAddress(adapter.getUrl()).Count == 18);

             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }

         [Test]
         public void getRelatedVideoAddress2Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\2_video.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 Assert.IsTrue(extractorAndCommenter.getRelatedVideoAddress(adapter.getUrl()).Count == 20);

             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }
         [Test]
         public void getRelatedVideoAddress3Test()
         {
             try
             {

                 FileAdapter adapter = new FileAdapter("resources\\3_video.htm");
                 VideoExtractorAndCommenter extractorAndCommenter = new VideoExtractorAndCommenter(adapter, 20);
                 Assert.IsTrue(extractorAndCommenter.getRelatedVideoAddress(adapter.getUrl()).Count == 19);

             }
             catch (Exception e)
             {
                 Assert.IsTrue(false, e.Message);

             }
         }
        
    }
}