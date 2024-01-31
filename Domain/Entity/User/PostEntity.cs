using Domain.Entity.Base;

namespace Domain.Entity.User
{
    public class PostEntity : AggregateRoot<int>
    {
        public int IdUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string Description { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool Active { get; set; }
        public UserEntity User { get; set; }
    }
}
