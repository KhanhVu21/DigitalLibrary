using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class AuditMethodRepository: IAuditMethodRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public AuditMethodRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        #endregion

        #region METHOD
        public IEnumerable<AuditMethodDto> GetAllAuditMethod(int pageNumber, int pageSize)
        {
            try
            {
                var auditMethod = new List<AuditMethod>();
                auditMethod = _DbContext.AuditMethod.ToList();

                if (pageNumber != 0 && pageSize != 0)
                {
                    if (pageNumber < 0) { pageNumber = 1; }
                    auditMethod = auditMethod.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                var result = new List<AuditMethodDto>();
                result = _mapper.Map<List<AuditMethodDto>>(auditMethod);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
