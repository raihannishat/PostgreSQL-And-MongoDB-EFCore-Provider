namespace EFCoreForPostgreSQLAndMongoDB.Entities;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Set the table name
        builder.ToTable("users");

        // Configure the primary key
        builder.HasKey(u => u.Id);

        // Configure the Name property
        builder.Property(u => u.Name)
            .HasMaxLength(100);

        // Configure the Email property
        builder.Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired(false);

        // Configure the Address complex type
        builder.OwnsOne(u => u.Address, address =>
        {
            address.ToTable("user_address"); // Set a separate table for contacts
            address.Property(a => a.Street).HasColumnName("street").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("city").HasMaxLength(50);
            address.Property(a => a.State).HasColumnName("state").HasMaxLength(50);
            address.Property(a => a.Zip).HasColumnName("zip").HasMaxLength(20);
        });

        // Configure the Contacts relationship as an owned entity
        builder.OwnsMany(u => u.Contacts, contacts =>
        {
            contacts.ToTable("user_contacts"); // Set a separate table for contacts
            contacts.Property(c => c.Type).HasColumnName("type").HasMaxLength(50);
            contacts.Property(c => c.Number).HasColumnName("number").HasMaxLength(20);
        });
    }
}