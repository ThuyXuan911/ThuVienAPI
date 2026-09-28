using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Data;
using WebAPI.Models.DTO;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PublishersController : ControllerBase
	{
		private readonly AppDbContext _dbContext;
		private readonly IPublisherRepository _publisherRepository;

		public PublishersController(AppDbContext dbContext, IPublisherRepository publisherRepository)
		{
			_dbContext = dbContext;
			_publisherRepository = publisherRepository;
		}

		[HttpGet("get-all-publisher")]
		public IActionResult GetAllPublisher()
		{
			var allPublishers = _publisherRepository.GetAllPublishers();
			return Ok(allPublishers);
		}

		[HttpGet("get-publisher-by-id/{id}")]
		public IActionResult GetPublisherById(int id)
		{
			var publisherWithId = _publisherRepository.GetPublisherById(id);
			return Ok(publisherWithId);
		}

		[HttpPost("add-publisher")]
		public IActionResult AddPublisher([FromBody] AddPublisherRequestDTO addPublisherRequestDTO)
		{
			var isDuplicate = _dbContext.Publishers.Any(p => p.Name == addPublisherRequestDTO.Name);
			if (isDuplicate)
			{
				return BadRequest("Tên nhà xuất bản đã tồn tại.");
			}

			var publisherAdd = _publisherRepository.AddPublisher(addPublisherRequestDTO);
			return Ok(publisherAdd);
		}

		[HttpPut("update-publisher-by-id/{id}")]
		public IActionResult UpdatePublisherById(int id, [FromBody] PublisherNoIdDTO publisherDTO)
		{
			var publisherUpdate = _publisherRepository.UpdatePublisherById(id, publisherDTO);
			return Ok(publisherUpdate);
		}

		[HttpDelete("delete-publisher-by-id/{id}")]
		public IActionResult DeletePublisherById(int id)
		{
			var hasBooks = _dbContext.Books.Any(b => b.PublisherID == id);
			if (hasBooks)
			{
				return BadRequest("Không thể xóa Publisher vì vẫn còn sách tham chiếu.");
			}

			var publisherDelete = _publisherRepository.DeletePublisherById(id);
			return Ok();
		}
	}
}