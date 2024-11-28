using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace ToDoList.Api.Data
{
	public class DataContext : DbContext
	{
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

		public DbSet<TaskItem> TaskItems { get; set; } = null!;
		public DbSet<User> Users { get; set; } = null!;
		public DbSet<PhoneOtp> PhoneOTPs { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					UserName = "admin",
					Password = PasswordEncryption.GetSHA256Encryption("admin"),
				},
				new User
				{
					Id = 2,
					UserName = "user",
					Password = PasswordEncryption.GetSHA256Encryption("user")
				}
				);
		}
	}
}
