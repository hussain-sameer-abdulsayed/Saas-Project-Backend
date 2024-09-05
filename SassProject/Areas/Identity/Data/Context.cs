using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SassProject.Models;
using System.Security.Cryptography;

namespace SassProject.Data;

public class Context : IdentityDbContext<IdentityUser>
{
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }


    public DbSet<User> Users {  get; set; }
    public DbSet<Product> Products {  get; set; }
    public DbSet<Category> Categories {  get; set; }
    public DbSet<Order> Orders {  get; set; }
    public DbSet<OrderItem> OrderItems {  get; set; }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasData(new User
            {
                Id = "0842a1a0-44d2-4882-8266-12e5a939d452",
                FirstName = "Name",
                LastName = "Name",
                UserName = "Name",
                Email = "Name",
                NormalizedEmail = "Name",
                PasswordHash = HashPassword("BankLogin!3"),
                PhoneNumber = "Name",
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
            });
        builder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole
                {
                    Id = "ae2626ab-cea5-458f-82f5-2dbad5009e29",
                    Name = "SuperAdmin",
                    NormalizedName = "SuperAdmin".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id = "9b3e174e-10e6-446f-86af-483d56fd7210",
                    Name = "Viwer",
                    NormalizedName = "Viwer".ToUpper()
                }
                );
        builder.Entity<IdentityUserRole<string>>()
            .HasData(
                new IdentityUserRole<string>()
                {
                    UserId = "0842a1a0-44d2-4882-8266-12e5a939d452",
                    RoleId = "ae2626ab-cea5-458f-82f5-2dbad5009e29"
                }
                );









        builder.Entity<IdentityUser>(b =>
        {
            b.ToTable("Users");
        });

        builder.Entity<IdentityUserClaim<string>>(b =>
        {
            b.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<string>>(b =>
        {
            b.ToTable("UserTokens");
        });

        builder.Entity<IdentityRole>(b =>
        {
            b.ToTable("Roles");
        });

        builder.Entity<IdentityRoleClaim<string>>(b =>
        {
            b.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserRole<string>>(b =>
        {
            b.ToTable("UserRoles");
        });




    }
    public static string HashPassword(string password)
    {
        byte[] salt;
        byte[] buffer2;

        using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
        {
            salt = bytes.Salt;
            buffer2 = bytes.GetBytes(0x20);
        }
        byte[] dst = new byte[0x31];
        Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
        Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
        return Convert.ToBase64String(dst);
    }
}
