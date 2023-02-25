using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;

        #endregion

        #region Constructors
        public DocumentTypeRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region MeThod
        public Response DeleteDocumentType(Guid Id)
        {
            Response response = new Response();
            try
            {
                Document doc = _DbContext.Document.Where(e => e.DocumentTypeId == Id && e.IsDeleted == false).FirstOrDefault();
                if(doc != null)
                {
                    response = new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Không thể xóa vì trong danh mục có tài liệu !"
                    };
                    return response;
                }

                DocumentType document  = _DbContext.DocumentType.Where(x => x.Id == Id).FirstOrDefault();

                if (document != null)
                {
                    document.IsDeleted = true;
                    _DbContext.DocumentType.Update(document);
                    List<DocumentType> documentTypes = _DbContext.DocumentType.Where(x => x.ParentId == Id).ToList();

                    for (int i = 0; i < documentTypes.Count; i++)
                    {
                        documentTypes[i].IsDeleted = true;
                        _DbContext.DocumentType.Update(documentTypes[i]);
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
                else
                    response = new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Không tìm thấy kết quả !"
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

        public List<DocumentTypeDto> GetAllDocumentType(int status)
        {
            try
            {
                List<DocumentType> result = new List<DocumentType>();
                result = _DbContext.DocumentType.Where(e => e.IsDeleted == false && e.Status == status && e.ParentId == null)
                .OrderByDescending(e => e.CreatedDate).ToList();

                List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
                documentTypeDtos = _mapper.Map<List<DocumentTypeDto>>(result);
                return documentTypeDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DocumentTypeDto> GetAllDocumentType(int pageNumber, int pageSize)
        {
            try
            {
                List<DocumentType> result = new List<DocumentType>();
                if (pageNumber == 0 && pageSize == 0)
                {
                    result = _DbContext.DocumentType.Where(e => e.IsDeleted == false && e.ParentId == null).ToList();
                }
                else
                {
                    result = _DbContext.DocumentType.Where(e => e.IsDeleted == false && e.ParentId == null)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
                documentTypeDtos = _mapper.Map<List<DocumentTypeDto>>(result);
                return documentTypeDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentTypeDto> GetAllDocumentType()
        {
            try
            {
                List<DocumentType> result = new List<DocumentType>();
                result = _DbContext.DocumentType.Where(e => e.IsDeleted == false)
                    .OrderByDescending(e =>e.CreatedDate).ToList();

                List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
                documentTypeDtos = _mapper.Map<List<DocumentTypeDto>>(result);
                return documentTypeDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DocumentTypeDto GetAllDocumentTypeById(Guid Id)
        {
            try
            {
                DocumentType result = new DocumentType();
                result = _DbContext.DocumentType.Where(e => e.Id == Id && e.IsDeleted == false).FirstOrDefault();

                DocumentTypeDto documentTypeDto = new DocumentTypeDto();
                if (result != null)
                {
                    documentTypeDto = _mapper.Map<DocumentTypeDto>(result);
                }

                return documentTypeDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DocumentTypeDto> GetAllDocumentTypeByParentId(Guid Id)
        {
            try
            {
                List<DocumentType> result = new List<DocumentType>();
                result = _DbContext.DocumentType.Where(e => e.ParentId == Id && e.IsDeleted == false).ToList();

                List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
                documentTypeDtos = _mapper.Map<List<DocumentTypeDto>>(result);
                return documentTypeDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetNameDocumentType(Guid IdDocumentType)
        {
            return _DbContext.DocumentType.Where(e => e.Id == IdDocumentType).Select(e => e.DocTypeName).FirstOrDefault();
        }

        public Response InsertDocumentType(DocumentTypeDto documentTypeDto)
        {
            Response response = new Response();
            try
            {
                DocumentType documentType = new DocumentType();
                documentType = _mapper.Map<DocumentType>(documentTypeDto);

                _DbContext.DocumentType.Add(documentType);
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

        public Response UpdateDocumentType(DocumentTypeDto documentTypeDto)
        {
            Response response = new Response();
            try
            {
                DocumentType documentType = new DocumentType();
                documentType = _DbContext.DocumentType.Where(e => e.Id == documentTypeDto.Id).FirstOrDefault();
                if (documentType != null)
                {
                    // define some col with data concrete
                    documentType.DocTypeName = String.IsNullOrEmpty(documentTypeDto.DocTypeName) ? documentType.DocTypeName : documentTypeDto.DocTypeName;
                    documentType.Description = String.IsNullOrEmpty(documentTypeDto.Description) ? documentType.Description : documentTypeDto.Description;

                    documentType.ParentId = documentTypeDto.ParentId.HasValue ? documentTypeDto.ParentId : documentType.ParentId;
                    documentType.Status = documentTypeDto.Status.HasValue ? documentTypeDto.Status : documentType.Status;

                    _DbContext.DocumentType.Update(documentType);
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
        #endregion
    }
}
