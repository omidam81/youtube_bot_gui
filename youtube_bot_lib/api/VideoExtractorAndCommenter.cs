using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using HtmlAgilityPack;
using ScrapySharp.Core;
using ScrapySharp.Extensions;

namespace youtube_bot_lib.api
{
    public class VideoExtractorAndCommenter
    {
        private IAdapter Adapter;
        private IVideoCommenter videoCommenter;
        private string baseWatchUrl = "http://www.youtube.com";
        private bool finished = false;
        private long threshold;
        private static IList<string> totalAddresses = new List<string>();

        private static Hashtable totalVideoAddressesHashtable = new Hashtable();
        private IList<string> searchStringPhrases = new List<string>();
        private int threadNumber;
        private static int threadCount = 0;
        #region constructors

        public VideoExtractorAndCommenter(IAdapter adapter)
        {
            Adapter = adapter;
            threshold = 10000;
            searchStringPhrases.Add(adapter.getSearchString());
            threadCount++;
            threadNumber = threadCount;
        }

        public VideoExtractorAndCommenter(IAdapter adapter, long threshold)
        {
            Adapter = adapter;
            this.threshold = threshold;
            searchStringPhrases.Add(adapter.getSearchString());
            threadCount++;
            threadNumber = threadCount;
        }

        public VideoExtractorAndCommenter(IAdapter adapter, IVideoCommenter videoCommenter, long threshold)
        {
            Adapter = adapter;

            this.videoCommenter = videoCommenter;
            this.threshold = threshold;
            searchStringPhrases.Add(adapter.getSearchString());
            threadCount++;
            threadNumber = threadCount;

        }

        public VideoExtractorAndCommenter(IAdapter adapter, IVideoCommenter videoCommenter, IList<string> searchPhrases,
                                          long threshold)
        {
            Adapter = adapter;

            this.videoCommenter = videoCommenter;
            this.threshold = threshold;
            foreach (var phrase in searchPhrases)
            {
                searchStringPhrases.Add(phrase);
            }
            threadCount++;
            threadNumber = threadCount;

        }

        #endregion

        public void addSearchStringPhrase(string phrase)
        {
            searchStringPhrases.Add(phrase);
        }

        public IList<string> extractVideoAddresses(string html)
        {
            try
            {
                IList<string> list = new List<string>();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var videoLiTags = doc.DocumentNode.SelectNodes("//*[@id=\"search-results\"]/li");
                if (videoLiTags == null)
                {
                    finished = true;
                    return list;
                }
                foreach (var videoLiTag in videoLiTags)
                {
                    if (!videoLiTag.InnerText.ToLower().Contains("views"))
                    {
                        continue;
                    }
                    //*[@id="search-results"]/li[1]/div[2]/div/ul/li[3]
                    //*[@id="search-results"]/li[3]/div[2]/div/ul/li[3]
                    //*[@id="search-results"]/li[2]/div[2]/div[1]/ul/li[3]
                    //*[@id="search-results"]/li[1]/div[2]/div/ul/li[3]
                    var viewCountDivTag = videoLiTag.SelectNodes("div[2]/div");
                    HtmlNode viewCountTag = null;
                    if (viewCountDivTag[0].InnerText.ToLower().Contains("ago"))
                    {
                        viewCountTag = viewCountDivTag[0].SelectSingleNode("ul/li[3]");
                    }
                    else
                    {
                        viewCountTag = viewCountDivTag[0].SelectSingleNode("ul/li[2]");
                    }
                    long viewCount = 0;
                    try
                    {
                        viewCount =
                            Convert.ToInt64(
                                viewCountTag.InnerText.Replace("&nbsp;", " ").Replace("\n", "").Replace("\t", "").Trim()
                                    .Split(' ')
                                    [0].Replace(",", ""));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(threadNumber + "# error: " + ex.Message);
                        continue;
                    }

                    if (viewCount > threshold)
                    {
                        var aTag = videoLiTag.SelectSingleNode("div[2]/h3/a");
                        list.Add(baseWatchUrl + aTag.Attributes["href"].Value.ToString());
                    }
                    else
                    {
                        finished = true;
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                throw new Exception("could not extract addresses", e);
            }
        }

        //0 = next page doesn't exist       
        public long getNextPageNumber(string html)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var footer = CssQueryExtensions.CssSelect(doc.DocumentNode, "div.search-footer");
                var enumerator = footer.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    if (enumerator.Current.ChildNodes.Count == 1 &&
                        enumerator.Current.ChildNodes[0].Name.Equals("#text"))
                    {
                        return 0;
                    }
                    if (enumerator.Current.ChildNodes.Count > 0)
                    {
                        var aTags = enumerator.Current.SelectNodes("div/a");
                        for (int i = 0; i < aTags.Count; i++)
                        {
                            if (aTags[i].Attributes["class"].Value.Contains("toggled"))
                            {
                                if (i < aTags.Count - 1)
                                {
                                    return Convert.ToInt64(aTags[i].InnerText) + 1;
                                }
                                else
                                {
                                    //the currect page is last page
                                    return 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                throw new Exception("could not extract addresses", e);
            }
        }


        public IList<string> getRelatedVideoAddress(string videoAddress)
        {
            try
            {
                IList<string> list = new List<string>();
                Adapter.setUrl(videoAddress);
                string html = Adapter.readPageSource();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                //*[@id="watch-related"]/li
                var liTags = doc.DocumentNode.SelectNodes("//*[@id=\"watch-related\"]/li");
                foreach (var liTag in liTags)
                {
                    //view count                     
                    HtmlNode vTag = null;
                    if (liTag.InnerText.ToLower().Contains("views"))
                    {
                        vTag = liTag.SelectSingleNode("a/span[4]");
                    }
                    else
                    {
                        vTag = liTag.SelectSingleNode("a/span[5]");
                    }
                    if (vTag == null)
                    {
                        continue;
                    }
                    long viewCount = 0;
                    try
                    {
                        viewCount =
                            Convert.ToInt64(
                                vTag.InnerText.Replace("&nbsp;", " ").Replace("\n", "").Replace("\t", "").Trim().Split(
                                    ' ')[0].Replace(",", ""));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("error: " + ex.Message);
                        continue;
                    }


                    if (viewCount > threshold)
                    {
                        var aTag = liTag.SelectSingleNode("a");
                        list.Add(baseWatchUrl + aTag.Attributes["href"].Value.ToString());
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                 new Exception("could not extract related video addresses from " + videoAddress, e);
                VideoCommenter.Semaphore.WaitOne();
                StreamWriter writer = new StreamWriter("error.log", true);
                writer.WriteLine("could not extract related video addresses from " + videoAddress + Environment.NewLine + e.Message);
                writer.Close();
                VideoCommenter.Semaphore.Release();                
            }
            return new List<string>();
        }

        private static Semaphore totalAddressesSemaphore = new Semaphore(1, 1);
        private static Semaphore hashtableSemaphore = new Semaphore(1, 1);
        private static Semaphore conterSemaphore = new Semaphore(1, 1);
        private static int lastIndex = 0;

        private void addAddressToTotalAddresses(string s)
        {
            hashtableSemaphore.WaitOne();
            totalAddressesSemaphore.WaitOne();
            if (totalVideoAddressesHashtable[s] == null)
            {
                totalVideoAddressesHashtable.Add(s,s);
                
                
                                
            }
            totalAddresses.Add(s);
            totalAddressesSemaphore.Release();
            hashtableSemaphore.Release();
        }

        //public void extractVideosAndInsertAComment()
        //{
        //    try
        //    {
        //        //extract videos from search page
        //        foreach (string stringPhrase in searchStringPhrases)
        //        {
        //            Adapter.setUrl(stringPhrase, 1);
        //            Console.WriteLine(threadNumber + "# " +stringPhrase);
        //            finished = false;
        //            while (!finished)
        //            {
        //                string html = Adapter.readPageSource();
        //                IList<string> videoAddresses = extractVideoAddresses(html);
        //                foreach (var videoAddress in videoAddresses)
        //                {
        //                    //videoCommenter.commentVideo(videoAddress);
        //                    addAddressToTotalAddresses(videoAddress);
        //                }
        //                long nextPage = getNextPageNumber(html);
        //                if (nextPage == 0)
        //                {
        //                    finished = true;
        //                }
        //                else
        //                {
        //                    Adapter.setUrl(Adapter.getSearchString(), nextPage);
        //                }
        //                Console.WriteLine(threadNumber + "# " + stringPhrase + ": page number : " + nextPage);
        //            }
        //        }
        //        Console.WriteLine(threadNumber + "# " + "extract related videos");
        //        //comment each video and add related videos of each video
        //        int count = 0;

        //        hashtableSemaphore.WaitOne();
        //        totalAddressesSemaphore.WaitOne();
        //        count = totalAddresses.Count;
        //        totalAddressesSemaphore.Release();
        //        hashtableSemaphore.Release();

        //        for (int i = 0; i < count; i++)
        //        {
        //            Console.WriteLine(threadNumber + "# list: " + i);
        //            bool f = false;
        //            hashtableSemaphore.WaitOne();
        //            totalAddressesSemaphore.WaitOne();
        //            f = totalVideoAddressesHashtable[totalAddresses[i]] == null;

        //            if (f)
        //            {
        //                string s = totalAddresses[i].Split('=')[1];
        //                totalAddressesSemaphore.Release();
        //                hashtableSemaphore.Release();
        //                Console.WriteLine(threadNumber + "# Enter to commenting section " + DateTime.Now);
        //                videoCommenter.commentVideo(s);
        //                Console.WriteLine(threadNumber + "# Exit from commenting section " + DateTime.Now);
        //                //System.Threading.Thread.Sleep(300000);
        //                IList<string> relatedVideos = null;
        //                totalAddressesSemaphore.WaitOne();
        //                hashtableSemaphore.WaitOne();

        //                if (totalVideoAddressesHashtable[totalAddresses[i]] != null)
        //                {                            
        //                    hashtableSemaphore.Release();
        //                    totalAddressesSemaphore.Release();
        //                    continue;
        //                }
        //                totalVideoAddressesHashtable.Add(totalAddresses[i], totalAddresses[i]);                        
        //                string rvid = totalAddresses[i];
        //                hashtableSemaphore.Release();
        //                totalAddressesSemaphore.Release();

        //                relatedVideos = getRelatedVideoAddress(rvid);
        //                foreach (var relatedVideo in relatedVideos)
        //                {
        //                    totalAddressesSemaphore.WaitOne();
        //                    hashtableSemaphore.WaitOne();

        //                    if (totalVideoAddressesHashtable[relatedVideo] == null)
        //                    {
        //                        totalVideoAddressesHashtable.Add(relatedVideo, relatedVideo);
        //                        totalAddresses.Add(relatedVideo);
        //                    }
        //                    totalAddressesSemaphore.Release();
        //                    hashtableSemaphore.Release();
        //                }
        //            }
        //            else
        //            {
        //                hashtableSemaphore.Release();
        //                totalAddressesSemaphore.Release();
        //            }
        //            totalAddressesSemaphore.WaitOne();
        //            Console.WriteLine(threadNumber +"# total:" + totalAddresses.Count);

        //            count = totalAddresses.Count;
        //            totalAddressesSemaphore.Release();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        hashtableSemaphore.Release();
        //        totalAddressesSemaphore.Release();
        //        throw new Exception("error", e);
        //    }
        //}



        public void extractVideosAndInsertAComment()
        {
            try
            {
                //extract videos from search page
                foreach (string stringPhrase in searchStringPhrases)
                {
                    Adapter.setUrl(stringPhrase, 1);
                    Console.WriteLine(threadNumber + "# " + stringPhrase);
                    finished = false;
                    while (!finished)
                    {
                        string html = Adapter.readPageSource();
                        IList<string> videoAddresses = extractVideoAddresses(html);
                        foreach (var videoAddress in videoAddresses)
                        {
                            //videoCommenter.commentVideo(videoAddress);
                            addAddressToTotalAddresses(videoAddress);
                        }
                        long nextPage = getNextPageNumber(html);
                        if (nextPage == 0)
                        {
                            finished = true;
                        }
                        else
                        {
                            Adapter.setUrl(Adapter.getSearchString(), nextPage);

                            ////for test
                            //if (nextPage == 5)
                            //{
                            //    finished = true;
                            //}
                        }
                        Console.WriteLine(threadNumber + "# " + stringPhrase + ": page number : " + nextPage);
                    }
                }
                Console.WriteLine(threadNumber + "# " + "extract related videos");
                //comment each video and add related videos of each video
                int count = 0;

                hashtableSemaphore.WaitOne();
                totalAddressesSemaphore.WaitOne();

                count = totalAddresses.Count;

                totalAddressesSemaphore.Release();
                hashtableSemaphore.Release();
                int i = 0;

                //for (int i = 0; i < count; i++)
                while(i < count)
                {
                    
                    bool f = false;
                    conterSemaphore.WaitOne();
                    i = lastIndex;
                    lastIndex++;
                    conterSemaphore.Release();                    

                    hashtableSemaphore.WaitOne();
                    totalAddressesSemaphore.WaitOne();

                    Console.WriteLine(threadNumber + "# current element index: " + i);
                    f = totalVideoAddressesHashtable[totalAddresses[i]] != null;

                    if (f)
                    {
                        string s = totalAddresses[i].Split('=')[1];
                        totalAddressesSemaphore.Release();
                        hashtableSemaphore.Release();
                        Console.WriteLine(threadNumber + "# Enter to commenting section " + DateTime.Now);
                        bool success = videoCommenter.commentVideo(s);
                        if(!success)
                        {
                            totalAddresses.Add(s);                            
                        }
                        Console.WriteLine(threadNumber + "# Exit from commenting section " + DateTime.Now);
                        //System.Threading.Thread.Sleep(300000);
                        IList<string> relatedVideos = null;
                        
                        hashtableSemaphore.WaitOne();
                        totalAddressesSemaphore.WaitOne();
                        
                        //if (totalVideoAddressesHashtable[totalAddresses[i]] != null)
                        //{
                        //    totalAddressesSemaphore.Release();
                        //    hashtableSemaphore.Release();
                            
                        //    continue;
                        //}
                        //totalVideoAddressesHashtable.Add(totalAddresses[i], totalAddresses[i]);

                        string rvid = totalAddresses[i];
                        totalAddressesSemaphore.Release();
                        hashtableSemaphore.Release();
                        
                        relatedVideos = getRelatedVideoAddress(rvid);
                        foreach (var relatedVideo in relatedVideos)
                        {
                            hashtableSemaphore.WaitOne();
                            totalAddressesSemaphore.WaitOne();                           

                            if (totalVideoAddressesHashtable[relatedVideo] == null)
                            {
                                totalVideoAddressesHashtable.Add(relatedVideo, relatedVideo);
                                totalAddresses.Add(relatedVideo);
                            }
                            totalAddressesSemaphore.Release();
                            hashtableSemaphore.Release();
                        }
                    }
                    else
                    {
                        totalAddressesSemaphore.Release();
                        hashtableSemaphore.Release();
                        
                    }
                    hashtableSemaphore.WaitOne();
                    totalAddressesSemaphore.WaitOne(); 
                    Console.WriteLine(threadNumber + "# total count of videos:" + totalAddresses.Count);

                    count = totalAddresses.Count;
                    totalAddressesSemaphore.Release();
                    hashtableSemaphore.Release();
                        
                }
            }
            catch (Exception e)
            {
                try
                {
                    totalAddressesSemaphore.Release();
                    hashtableSemaphore.Release();
                }
                catch (Exception)
                {
                    
                    
                }
                new Exception("error", e);
            }
        }
    }
}