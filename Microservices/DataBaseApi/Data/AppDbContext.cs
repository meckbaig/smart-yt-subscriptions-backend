using DataBaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBaseApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Access> Access { get; set; }
    public DbSet<Folder> Folders { get; set; }
}