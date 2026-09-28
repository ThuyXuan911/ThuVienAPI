using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Data;
using WebAPI.Models.DTO;
using WebAPI.Models.Domain;
using WebAPI.Repositories;
using WebAPI.CustomActionFilter;
namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class BooksController : ControllerBase
	{
		private readonly AppDbContext _dbContext;
		private readonly IBookRepository _bookRepository;

		public BooksController(AppDbContext dbContext, IBookRepository bookRepository)
		{
			_dbContext = dbContext;
			_bookRepository = bookRepository;
		}

		[HttpGet("get-all-books")]
		public IActionResult GetAll()
		{
			var allBooks = _bookRepository.GetAllBooks();
			return Ok(allBooks);
		}

		[HttpGet("get-book-by-id/{id}")]
		public IActionResult GetBookById([FromRoute] int id)
		{
			var bookWithIdDTO = _bookRepository.GetBookById(id);
			return Ok(bookWithIdDTO);
		}

		[HttpPost("add-book")]
		[ValidateModel]

		public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
		{
			if (ValidateAddBook(addBookRequestDTO))
			{
				var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
				return Ok(bookAdd);
			}
			return BadRequest(ModelState);
		}

		private bool ValidateAddBook(AddBookRequestDTO addBookRequestDTO)
		{
			if (addBookRequestDTO == null)
			{
				ModelState.AddModelError(nameof(addBookRequestDTO), "Please add book data");
				return false;
			}

			if (string.IsNullOrEmpty(addBookRequestDTO.Description))
			{
				ModelState.AddModelError(nameof(addBookRequestDTO.Description), $"{nameof(addBookRequestDTO.Description)} cannot be null");
			}

			if (addBookRequestDTO.Rate < 0 || addBookRequestDTO.Rate > 5)
			{
				ModelState.AddModelError(nameof(addBookRequestDTO.Rate), $"{nameof(addBookRequestDTO.Rate)} cannot be less than 0 and more than 5");
			}

			var publisherExists = _dbContext.Publishers.Any(p => p.Id == addBookRequestDTO.PublisherID);
			if (!publisherExists)
			{
				ModelState.AddModelError(nameof(addBookRequestDTO.PublisherID), "PublisherID không tồn tại trong hệ thống.");
			}

			if (addBookRequestDTO.AuthorIds == null || !addBookRequestDTO.AuthorIds.Any())
			{
				ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), "Danh sách tác giả không được để trống.");
			}
			else
			{
				var distinctAuthors = addBookRequestDTO.AuthorIds.Distinct().ToList();
				if (distinctAuthors.Count != addBookRequestDTO.AuthorIds.Count)
				{
					ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), "Không được gán trùng tác giả cho cùng một sách.");
				}

				var existingAuthors = _dbContext.Authors
					.Where(a => distinctAuthors.Contains(a.Id))
					.Select(a => a.Id)
					.ToList();

				var invalidAuthors = distinctAuthors.Except(existingAuthors).ToList();
				foreach (var authorId in invalidAuthors)
				{
					ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"AuthorID {authorId} không tồn tại.");
				}
			}

			return ModelState.ErrorCount == 0;
		}

		[HttpPut("update-book-by-id/{id}")]
		public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
		{
			var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
			return Ok(updateBook);
		}

		[HttpDelete("delete-book-by-id/{id}")]
		public IActionResult DeleteBookById(int id)
		{
			var deleteBook = _bookRepository.DeleteBookById(id);
			return Ok(deleteBook);
		}
	}
}