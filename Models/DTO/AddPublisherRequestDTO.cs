using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.DTO
{
	public class AddPublisherRequestDTO
	{
		[Required]
		[MinLength(3, ErrorMessage = "Tên nhà xuất bản phải có tối thiểu 3 ký tự.")]
		public string Name { get; set; }
	}
}
