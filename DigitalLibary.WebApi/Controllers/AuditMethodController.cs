using AutoMapper;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.Service.Repository.RepositoryIPL;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using DigitalLibary.WebApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditMethodController: Controller
    {
        #region Variables
        private readonly IAuditMethodRepository _auditMethodRepository;
        private readonly AppSettingModel _appSettingModel;
        #endregion

        #region Contructor
        public AuditMethodController(IAuditMethodRepository auditMethodRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _auditMethodRepository = auditMethodRepository;
        }
        #endregion

        #region METHOD
        // GET: api/AuditMethod/GetAllAuditMethod
        [HttpGet]
        [Route("GetAllAuditMethod")]
        public IEnumerable<AuditMethodDto> GetAllAuditMethod(int pageNumber, int pageSize)
        {
            var result = _auditMethodRepository.GetAllAuditMethod(pageNumber, pageSize);
            return result;
        }
        #endregion
    }
}
