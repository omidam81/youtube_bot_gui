using youtube_bot_lib.api;

namespace youtube_bot_test.mock
{
    public class CommentRepositoryMock : ICommentRepository
    {
        public string getComment()
        {
            return "test comment";
        }

        public string getRating()
        {
            return 5.ToString();
        }
    }
}