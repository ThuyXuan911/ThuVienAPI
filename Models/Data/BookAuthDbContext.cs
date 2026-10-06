using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace WebAPI.Models.Data
{
	public class BookAuthDbContext : IdentityDbContext
	{
		public BookAuthDbContext(DbContextOptions<BookAuthDbContext> options) : base(options)
		{
		}
		//tao phan quyen reader va write cho user
		protected override void OnModelCreating(ModelBuilder builder)
		{
			var ReaderRoleId = "004c7e80 - 7dfc - 44be - 8952 - 2c7130898655";
			var WriterRoleId = "71e282d3-76ca-485e-b094-eff019287fa5";
			base.OnModelCreating(builder);
			var role = new List<IdentityRole>
			{
				new IdentityRole
				{
					Id = ReaderRoleId,
					ConcurrencyStamp = ReaderRoleId,
					Name = "Read",
					NormalizedName = "READ".ToUpper()
				},
				new IdentityRole
				{
					Id = WriterRoleId,
					ConcurrencyStamp = WriterRoleId,
					Name = "Write",
					NormalizedName = "WRITE".ToUpper()
				}
			};
			builder.Entity<IdentityRole>().HasData(role);
		}
	}
}
