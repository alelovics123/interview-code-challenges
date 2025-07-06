using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneBeyondApi.Configuartion;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;
using System.Collections;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogueController : ControllerBase
    {
        private readonly ILogger<CatalogueController> _logger;
        private readonly ICatalogueRepository _catalogueRepository;
        private readonly IOptions<ConfigurationOptions> _options;

        public CatalogueController(ILogger<CatalogueController> logger, ICatalogueRepository catalogueRepository, IOptions<ConfigurationOptions> options)
        {
            _logger = logger;
            _catalogueRepository = catalogueRepository;
            _options = options;
        }

        [HttpGet]
        [Route("GetCatalogue")]
        public IList<BookStock> Get()
        {
            return _catalogueRepository.GetCatalogue();
        }

        [HttpPost]
        [Route("SearchCatalogue")]
        public IList<BookStock> Post(CatalogueSearch search)
        {
            return _catalogueRepository.SearchCatalogue(search);
        }

        [HttpPost]
        [Route("Reserve")]
        public void Reserve(Guid bookId, Guid borrowerId)
        {
            try
            {
                _catalogueRepository.Reserve(bookId, borrowerId, _options.Value.ReservationTimeInDays);
            }
            catch (Exception ex) { _logger.Log(LogLevel.Error,ex,ex.Message);throw; }
        }
    }
}