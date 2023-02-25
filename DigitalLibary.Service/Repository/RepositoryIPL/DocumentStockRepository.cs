using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class DocumentStockRepository: IDocumentStockRepository
    {
        #region Variables

        private readonly IMapper _mapper;
        public DataContext _DbContext;

        #endregion

        #region Constructors
        public DocumentStockRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region METHOD
        public List<DocumentStock> ListDcumentStocks(Guid IdStock)
        {
            List<DocumentStock> documentStocks = new List<DocumentStock>();
            var result = _DbContext.GetDynamicResult("exec [dbo].[DELETEALLCHILDSTOCK] @ID", new SqlParameter("@ID", IdStock));

            for (int i = 0; i < result.Count(); i++)
            {
                var str1 = result.ElementAt(i);
                var jsonString = JsonConvert.SerializeObject(str1);
                var resultObject = JsonConvert.DeserializeObject<DocumentStock>(jsonString);

                documentStocks.Add(resultObject);
            }
            return documentStocks;
        }
        #endregion

        #region CRUD TABLE DOCUMENTSTOCK
        public Response DeleteDocumentStock(Guid Id)
        {
            Response response = new Response();
            try
            {
                List<DocumentStock> documentStocks = ListDcumentStocks(Id);

                for (int i = 0; i < documentStocks.Count(); i++)
                {
                    documentStocks[i].IsDeleted = true;
                    _DbContext.DocumentStock.Update(documentStocks[i]);
                }
                _DbContext.SaveChanges();

                response = new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Xóa thành công !"
                };
                return response;           
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Xóa không thành công !"
                };
                return response;
            }
        }
        public List<DocumentStockDto> GetAllDocumentStocks()
        {
            try
            {
                List<DocumentStock> result = new List<DocumentStock>();
                result = _DbContext.DocumentStock.Where(e => e.IsDeleted == false)
                .OrderByDescending(e => e.CreatedDate).ToList();

                List<DocumentStockDto> documentStockDtos = new List<DocumentStockDto>();
                documentStockDtos = _mapper.Map<List<DocumentStockDto>>(result);
                return documentStockDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentStockDto> GetAllDocumentStocks(int pageNumber, int pageSize)
        {
            try
            {
                List<DocumentStock> result = new List<DocumentStock>();
                if(pageNumber == 0 && pageSize == 0)
                {
                    result = _DbContext.DocumentStock.Where(e => e.IsDeleted == false && e.StockParentId == null)
                    .OrderByDescending(e => e.CreatedDate).ToList();
                }
                else
                {
                    result = _DbContext.DocumentStock.Where(e => e.IsDeleted == false && e.StockParentId == null)
                    .OrderByDescending(e => e.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                List<DocumentStockDto> documentStockDtos = new List<DocumentStockDto>();
                documentStockDtos = _mapper.Map<List<DocumentStockDto>>(result);
                return documentStockDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentStockDto> GetAllDocumentStocksByParentId(Guid ParentId)
        {
            try
            {
                List<DocumentStock> result = new List<DocumentStock>();
                result = _DbContext.DocumentStock.Where(e => e.StockParentId == ParentId && e.IsDeleted == false).ToList();

                List<DocumentStockDto> documentStockDtos = new List<DocumentStockDto>();
                documentStockDtos = _mapper.Map<List<DocumentStockDto>>(result);
                return documentStockDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response InsertDocumentStock(DocumentStockDto documentStockDto)
        {
            Response response = new Response();
            try
            {
                DocumentStock documentStock = new DocumentStock();
                documentStock = _mapper.Map<DocumentStock>(documentStockDto);

                _DbContext.DocumentStock.Add(documentStock);
                _DbContext.SaveChanges();

                response = new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Thêm mới thành công !"
                };
                return response;
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Thêm mới không thành công !"
                };
                return response;
            }
        }
        public Response UpdateDocumentStock(DocumentStockDto documentStockDto)
        {
            Response response = new Response();
            try
            {
                DocumentStock documentStock = new DocumentStock();
                documentStock = _DbContext.DocumentStock.Where(e => e.Id == documentStockDto.Id).FirstOrDefault();
                if (documentStock != null)
                {
                    // define some col with data concrete
                    documentStock.StockName = String.IsNullOrEmpty(documentStockDto.StockName) ? documentStock.StockName : documentStockDto.StockName;
                    documentStock.StockCode = String.IsNullOrEmpty(documentStockDto.StockCode) ? documentStock.StockCode : documentStockDto.StockCode;
                    documentStock.Description = String.IsNullOrEmpty(documentStockDto.Description) ? documentStock.Description : documentStockDto.Description;

                    documentStock.StockParentId = documentStockDto.StockParentId.HasValue ? documentStockDto.StockParentId : documentStock.StockParentId;
                    documentStock.Status = documentStockDto.Status.HasValue ? documentStockDto.Status : documentStock.Status;
                    documentStock.CreatedBy = documentStockDto.CreatedBy.HasValue ? documentStockDto.CreatedBy : documentStock.CreatedBy;
                    documentStock.CreatedDate = documentStockDto.CreatedDate.HasValue ? documentStockDto.CreatedDate : documentStock.CreatedDate;

                    _DbContext.DocumentStock.Update(documentStock);
                    _DbContext.SaveChanges();

                    response = new Response()
                    {
                        Success = true,
                        Fail = false,
                        Message = "Cập nhật thành công !"
                    };
                    return response;
                }
                else
                {
                    response = new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Cập nhật không thành công !"
                    };
                    return response;
                }
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Cập nhật không thành công !"
                };
                return response;
            }
        }
        public DocumentStockDto GetAllDocumentStocksById(Guid Id)
        {
            try
            {
                DocumentStock result = new DocumentStock();
                result = _DbContext.DocumentStock.Where(e => e.Id == Id && e.IsDeleted == false).FirstOrDefault();

                DocumentStockDto documentStockDtos = new DocumentStockDto();
                if(result != null)
                {
                    documentStockDtos = _mapper.Map<DocumentStockDto>(result);
                }

                return documentStockDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckBookInStock(Guid IdStock)
        {
            try
            {
                List<DocumentStock> result = ListDcumentStocks(IdStock);
                for (int i = 0; i < result.Count; i++)
                {
                    IndividualSample individualSample = _DbContext.IndividualSample.AsNoTracking().Where(e =>
                    e.IsDeleted == false && e.IsLostedPhysicalVersion == false && e.StockId == result[i].Id).FirstOrDefault();

                    if (individualSample != null)
                    {
                        return true;
                    }
                }
                return false;            
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Tuple<Guid, string>> GetIdAndNameDocumentStock()
        {
            try
            {
                var result = _DbContext.DocumentStock.Select(e => new Tuple<Guid, string>(e.Id, e.StockName)).ToList();
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
