using Domain.Entity.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Mapping
{
    internal partial class UserEntityMap : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("User");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.UpdateDate);
            builder.Property(x => x.Active).IsRequired();

            builder
                .HasMany(x => x.Posts)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.IdUser);
        }
    }
}
