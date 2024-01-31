using Domain.Entity.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping
{
    internal partial class UserEntityMap
    {
        public class PostEntityMap : IEntityTypeConfiguration<PostEntity>
        {
            public void Configure(EntityTypeBuilder<PostEntity> builder)
            {
                builder.ToTable("Budgets");

                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                builder.Property(x => x.IdUser).IsRequired();
                builder.Property(x => x.CreateDate).IsRequired();
                builder.Property(x => x.Description);
                builder.Property(x => x.UpdateDate);
                builder.Property(x => x.Active).IsRequired();
            }
        }
    }
}
