using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.DTO
{
	public class AddAuthorRequestDTO
	{
		[Required]
		[MinLength(3, ErrorMessage = "Tên tác giả phải có tối thiểu 3 ký tự.")]
		public string FullName { get; set; }
	}
}
