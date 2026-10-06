using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Data;
using WebAPI.Models.DTO;
using WebAPI.Models.Domain;
using WebAPI.Repositories;
using WebAPI.CustomActionFilter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class BooksController : ControllerBase
	{
		private readonly AppDbContext _dbContext;
		private readonly IBookRepository _bookRepository;
		private readonly ILogger<BooksController> _logger;

		public BooksController(AppDbContext dbContext, IBookRepository bookRepository, ILogger<BooksController> logger)
		{
			_dbContext = dbContext;
			_bookRepository = bookRepository;
			_logger = logger;
		}

		[HttpGet("get-all-books")]
		[Authorize(Roles = "Read")]
		public IActionResult GetAll(
			[FromQuery] string? filterOn,
			[FromQuery] string? filterQuery,
			[FromQuery] string? sortBy,
			[FromQuery] bool isAscending,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 1000)
		{
			_logger.LogInformation("GetAll Book Action method was invoked");
			_logger.LogWarning("This is a warning log");
			_logger.LogError("This is a error log");

			var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);

			_logger.LogInformation($"Finished GetAllBook request with data {JsonSerializer.Serialize(allBooks)}");

			return Ok(allBooks);
		}

		[HttpGet("get-book-by-id/{id}")]
		[Authorize(Roles = "Read")]
		public IActionResult GetBookById([FromRoute] int id)
		{
			var bookWithIdDTO = _bookRepository.GetBookById(id);
			return Ok(bookWithIdDTO);
		}

		[HttpPost("add-book")]
		[ValidateModel]
		[Authorize(Roles = "Write")]
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
		[Authorize(Roles = "Write")]
		public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
		{
			var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
			return Ok(updateBook);
		}

		[HttpDelete("delete-book-by-id/{id}")]
		[Authorize(Roles = "Write")]
		public IActionResult DeleteBookById(int id)
		{
			var deleteBook = _bookRepository.DeleteBookById(id);
			return Ok(deleteBook);
		}
	}
}