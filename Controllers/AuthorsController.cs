using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Data;
using WebAPI.Models.DTO;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthorsController : ControllerBase
	{
		private readonly AppDbContext _dbContext;
		private readonly IAuthorRepository _authorRepository;

		public AuthorsController(AppDbContext dbContext, IAuthorRepository authorRepository)
		{
			_dbContext = dbContext;
			_authorRepository = authorRepository;
		}

		[HttpGet("get-all-author")]
		public IActionResult GetAllAuthor()
		{
			var allAuthors = _authorRepository.GetAllAuthors();
			return Ok(allAuthors);
		}

		[HttpGet("get-author-by-id/{id}")]
		public IActionResult GetAuthorById(int id)
		{
			var authorWithId = _authorRepository.GetAuthorById(id);
			return Ok(authorWithId);
		}

		[HttpPost("add-author")]
		public ActionResult AddAuthors([FromBody] AddAuthorRequestDTO addAuthorRequestDTO)
		{
			var isDuplicate = _dbContext.Authors.Any(a => a.FullName == addAuthorRequestDTO.FullName);
			if (isDuplicate)
			{
				return BadRequest("Tên tác giả đã tồn tại trong hệ thống.");
			}

			var authorAdd = _authorRepository.AddAuthor(addAuthorRequestDTO);
			return Ok();
		}

		[HttpPut("update-author-by-id/{id}")]
		public IActionResult UpdateAuthorById(int id, [FromBody] AuthorNoIdDTO authorDTO)
		{
			var authorUpdate = _authorRepository.UpdateAuthorById(id, authorDTO);
			return Ok(authorUpdate);
		}

		[HttpDelete("delete-author-by-id/{id}")]
		public IActionResult DeleteAuthorById(int id)
		{
			var hasBooks = _dbContext.Books_Authors.Any(ba => ba.AuthorId == id);
			if (hasBooks)
			{
				return BadRequest("Hãy gỡ liên kết trong Book_Author trước khi xóa.");
			}

			var authorDelete = _authorRepository.DeleteAuthorById(id);
			return Ok();
		}
	}
}