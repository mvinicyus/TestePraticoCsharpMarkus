namespace Application.Boudary.Post
{
    public class GetPostsInput
    {
        public int? StartIndex { get; set; }
        public int? PageLength { get; set; }
        public int? Draw { get; set; }
    }
}
