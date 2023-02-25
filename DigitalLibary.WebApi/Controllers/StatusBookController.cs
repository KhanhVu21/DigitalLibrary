using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusBookController: Controller
    {
        #region Variables
        private readonly IStatusBookRepository _statusBookRepository;
        private readonly AppSettingModel _appSettingModel;
        #endregion

        #region Contructor
        public StatusBookController(IStatusBookRepository statusBookRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _statusBookRepository = statusBookRepository;
        }
        #endregion

        #region METHOD
        // GET: api/StatusBook/GetAllStatusBook
        [HttpGet]
        [Route("GetAllStatusBook")]
        public IEnumerable<StatusBookDto> GetAllStatusBook(int pageNumber, int pageSize, int Type)
        {
            var result = _statusBookRepository.GetAllStatusBook(pageNumber, pageSize, Type);
            return result;
        }
        #endregion
    }
}
