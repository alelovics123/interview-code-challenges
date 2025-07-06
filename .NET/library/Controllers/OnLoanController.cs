using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneBeyondApi.Configuartion;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;

namespace OneBeyondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnLoanController : ControllerBase
    {
        private readonly ILogger<BorrowerController> _logger;
        private readonly IBorrowerRepository _borrowerRepository;
        private readonly ICatalogueRepository _catalogueRepository;
        private readonly IOptions<ConfigurationOptions> _options;

        public OnLoanController(ILogger<BorrowerController> logger, IBorrowerRepository borrowerRepository, ICatalogueRepository catalogueRepository, IOptions<ConfigurationOptions> options)
        {
            _logger = logger;
            _borrowerRepository = borrowerRepository;
            _catalogueRepository = catalogueRepository;
            _options = options;
        }
        [HttpGet]
        public IList<Borrower> GetOnLoan()
        {
            return _borrowerRepository.GetOnLoan();
        }

        [HttpGet]
        [Route("GetLoanableDate")]
        public string GetLoanableDate(Guid bookId)
        {
            return _catalogueRepository.GetLoanableDate(bookId, _options.Value.ReservationTimeInDays);
        }

        [HttpPost]
        public void ReturnLoaned(Guid bookStockId)
        {
            try
            {
                _catalogueRepository.ReturnLoan(bookStockId, _options.Value.FineAmount);
            }
            catch (Exception ex) { _logger.Log(LogLevel.Error, ex, ex.Message);throw; }
        }

    }
}
