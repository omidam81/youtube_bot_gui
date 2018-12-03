using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace youtube_bot_lib.api
{
    public class CommentRepository : ICommentRepository
    {
        private static Semaphore  fileSemaphore = new Semaphore(1,1);
        public CommentRepository()
        {
            fileSemaphore.WaitOne();
            wordList = new List<string>();
            TextReader reader = new StreamReader("messages.txt");
            string line = reader.ReadLine();
            while (line != null && !line.Equals(""))
            {
                line = line.Replace("\r", "").Replace("\n", "");
                wordList.Add(line);
                line = reader.ReadLine();
            }
            reader.Close();
            fileSemaphore.Release();
        }

        private IList<string> wordList;


        public string getComment()
        {
            Random random = new Random();
            int r = (int)Math.Round(random.NextDouble()*wordList.Count);
            r = r == wordList.Count ? (r - 1) : r;
            return wordList[r];
        }

        public string getRating()
        {
            return 5.ToString();
        }
    }
}