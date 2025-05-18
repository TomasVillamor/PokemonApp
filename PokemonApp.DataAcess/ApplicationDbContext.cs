using Microsoft.EntityFrameworkCore;
using PokemonApp.Domain.Models;

namespace PokemonApp.DataAcess;
public class ApplicationDbContext : DbContext
{

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Pokemon> Pokemons { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash)
                .IsRequired();
            entity.Property(u => u.PasswordSalt)
                .IsRequired();
            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);
            entity.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Pokemon>(entity =>
        {
            entity.ToTable("Pokemons");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(p => p.Height).IsRequired();
            entity.Property(p => p.Weight).IsRequired();
            entity.HasIndex(p => p.Name)
                  .IsUnique()
                  .HasDatabaseName("UX_Pokemon_Name");
            entity.HasIndex(p => p.PokeApiId)
                  .IsUnique()
                  .HasDatabaseName("UX_Pokemon_PokeApiId");
        });

    }
}
