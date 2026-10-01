using WebAPI.Models.Data;
using WebAPI.Models.Domain;
using WebAPI.Models.DTO;

namespace WebAPI.Repositories
{
	public class SQLPublisherRepository : IPublisherRepository
	{
		private readonly AppDbContext _dbContext;

		public SQLPublisherRepository(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public List<PublisherDTO> GetAllPublishers()
		{
			return _dbContext.Publishers.Select(p => new PublisherDTO
			{
				Id = p.Id,
				Name = p.Name
			}).ToList();
		}

		public PublisherNoIdDTO GetPublisherById(int id)
		{
			var publisherWithIdDomain = _dbContext.Publishers.FirstOrDefault(x => x.Id == id);
			if (publisherWithIdDomain != null)
			{
				var publisherNoIdDTO = new PublisherNoIdDTO
				{
					Name = publisherWithIdDomain.Name,
				};
				return publisherNoIdDTO;
			}
			return null!;
		}

		public AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
		{
			var publisherDomainModel = new Publisher
			{
				Name = addPublisherRequestDTO.Name,
			};

			_dbContext.Publishers.Add(publisherDomainModel);
			_dbContext.SaveChanges();

			return addPublisherRequestDTO;
		}

		public PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO)
		{
			var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
			if (publisherDomain != null)
			{
				publisherDomain.Name = publisherNoIdDTO.Name;
				_dbContext.SaveChanges();
			}
			return null!;
		}

		public Publisher? DeletePublisherById(int id)
		{
			var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.Id == id);
			if (publisherDomain != null)
			{
				_dbContext.Publishers.Remove(publisherDomain);
				_dbContext.SaveChanges();
			}
			return null;
		}
	}
}
