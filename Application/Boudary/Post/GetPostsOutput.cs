
namespace Application.Boudary.Post
{
    public class GetPostsOutput
    {
        public int Draw { get; set; }
        public int TotalItens { get; set; }
        public IEnumerable<PostInfoOutput> Data { get; set; }
    }
}
