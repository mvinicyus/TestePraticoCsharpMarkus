using Domain.Entity.Base;

namespace Domain.Entity.User
{
    public class UserEntity : AggregateRoot<int>
    {
        public UserEntity()
        {
            Posts = new HashSet<PostEntity>();
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool Active { get; set; }
        public ICollection<PostEntity> Posts { get; set; }
    }
}
