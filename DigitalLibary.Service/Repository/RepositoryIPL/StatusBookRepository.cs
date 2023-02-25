using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class StatusBookRepository: IStatusBookRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public StatusBookRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region METHOD
        public IEnumerable<StatusBookDto> GetAllStatusBook(int pageNumber, int pageSize, int Type)
        {
            try
            {
                var statusBooks = new List<StatusBook>();
                statusBooks = _DbContext.StatusBook.Where(e => e.Status == Type).ToList();

                if (pageNumber != 0 && pageSize != 0)
                {
                    if (pageNumber < 0) { pageNumber = 1; }
                    statusBooks = statusBooks.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                var result = new List<StatusBookDto>();
                result = _mapper.Map<List<StatusBookDto>>(statusBooks);

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
