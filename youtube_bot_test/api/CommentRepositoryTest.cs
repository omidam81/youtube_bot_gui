using System;
using System.Diagnostics;
using NUnit.Framework;
using youtube_bot_lib.api;

namespace youtube_bot_test.api
{
    public class CommentRepositoryTest
    {
        [Test] 
        public void getCommentRepositoryTest()
        {
            try
            {
                CommentRepository repository = new CommentRepository();
                string n = repository.getComment();
                Debug.WriteLine(n);

                Assert.IsNotNull(n);
                Assert.IsTrue(!repository.getComment().Equals(""));
            }
            catch (Exception e)
            {
                
                Assert.IsTrue(false, e.Message);
            }
        }
    }
}