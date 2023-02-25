using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class DocumentInvoiceRepository : IDocumentInvoiceRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public DocumentInvoiceRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region CRUD TABLE DOCUMENTINVOICE
        public Response ChangeStatusDocumentInvoice(Guid Id, int status)
        {
            Response response = new Response();
            try
            {
                DocumentInvoice documentInvoice = _DbContext.DocumentInvoice.Where(x => x.Id == Id).FirstOrDefault();

                if (documentInvoice != null)
                {
                    if(status != 3)
                    {
                        documentInvoice.DateInReality = DateTime.Now;
                    }

                    List<DocumentInvoiceDetail> documentInvoiceDetails = _DbContext.DocumentInvoiceDetail
                    .Where(x => x.IdDocumentInvoice == Id).ToList(); 
                    
                    for(int i = 0; i < documentInvoiceDetails.Count; i++)
                    {
                        IndividualSample sample = _DbContext.IndividualSample.
                        Where(e => e.Id == documentInvoiceDetails[i].IdIndividual).FirstOrDefault();

                        if(sample != null)
                        {
                            if(status == 3)
                            {
                                sample.IsLostedPhysicalVersion = true;
                            }
                            sample.Status = status;
                            _DbContext.IndividualSample.Update(sample);
                        }                 
                        if(sample != null && sample.IsLostedPhysicalVersion)
                        {
                            sample.Status = 3;
                            _DbContext.IndividualSample.Update(sample);
                        }
                    }

                    documentInvoice.Status = status;
                    _DbContext.DocumentInvoice.Update(documentInvoice);

                    bool checkLostBook = false;
                    for (int j = 0; j < documentInvoiceDetails.Count; j++)
                    {
                        IndividualSample individualSample = _DbContext.IndividualSample.Where(e => e.Id == documentInvoiceDetails[j].IdIndividual).FirstOrDefault();

                        if (individualSample.IsLostedPhysicalVersion)
                        {
                            checkLostBook = true;
                            break;
                        }
                    }
                    if (checkLostBook)
                    {
                        documentInvoice.Status = 4;
                        _DbContext.DocumentInvoice.Update(documentInvoice);
                    }

                    _DbContext.SaveChanges();

                    response = new Response()
                    {
                        Success = true,
                        Fail = false,
                        Message = "Thay đổi thành công !"
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
                    Message = "Thay đổi không thành công !"
                };
                return response;
            }
        }
        public List<DocumentInvoiceDto> GetDocumentInvoice(int pageNumber, int pageSize)
        {
            try
            {
                List<DocumentInvoice> documentInvoices = new List<DocumentInvoice>();
                List<DocumentInvoiceDto> documentInvoiceDtos = new List<DocumentInvoiceDto>();

                if (pageNumber == 0 && pageSize == 0)
                {
                    documentInvoices = _DbContext.DocumentInvoice.
                    Where(e => e.Id != Guid.Empty)
                    .OrderByDescending(e => e.CreateDate)
                    .ToList();
                }
                else
                {
                    documentInvoices = _DbContext.DocumentInvoice.
                    Where(e => e.Id != Guid.Empty)
                    .OrderByDescending(e => e.CreateDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                 for(int i = 0; i < documentInvoices.Count; i++)
                 {
                    List<DocumentAndIndividualView> documentAndIndividuals = new List<DocumentAndIndividualView>();
                    DocumentInvoiceDto documentInvoiceDto = new DocumentInvoiceDto();
                    documentInvoiceDto = _mapper.Map<DocumentInvoiceDto>(documentInvoices[i]);

                    List<DocumentInvoiceDetail> documentInvoiceDetail = new List<DocumentInvoiceDetail>();
                    documentInvoiceDetail = _DbContext.DocumentInvoiceDetail
                    .Where(e => e.IdDocumentInvoice == documentInvoices[i].Id).ToList();

                    for (int j = 0; j < documentInvoiceDetail.Count; j++)
                    {
                        DocumentAndIndividualView documentAndIndividual = new DocumentAndIndividualView();
                        documentAndIndividual.idDocument = (Guid)documentInvoiceDetail[j].IdDocument;
                        documentAndIndividual.idIndividual = (Guid)documentInvoiceDetail[j].IdIndividual;
                        documentAndIndividuals.Add(documentAndIndividual);
                    }
                    documentInvoiceDto.DocumentAndIndividualView = documentAndIndividuals;
                    documentInvoiceDtos.Add(documentInvoiceDto);
                }
               
                return documentInvoiceDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DocumentInvoiceDto GetDocumentInvoiceById(Guid Id)
        {
            try
            {
                DocumentInvoice documentInvoice = new DocumentInvoice();
                documentInvoice = _DbContext.DocumentInvoice.
                Where(e => e.Id == Id)
                .FirstOrDefault();

                DocumentInvoiceDto documentInvoiceDto = new DocumentInvoiceDto();
                documentInvoiceDto = _mapper.Map<DocumentInvoiceDto>(documentInvoice);

                if(documentInvoice != null)
                {
                    List<DocumentAndIndividualView> documentAndIndividuals = new List<DocumentAndIndividualView>();

                    List<DocumentInvoiceDetail> documentInvoiceDetail = new List<DocumentInvoiceDetail>();
                    documentInvoiceDetail = _DbContext.DocumentInvoiceDetail
                    .Where(e => e.IdDocumentInvoice == documentInvoice.Id).ToList();

                    for (int j = 0; j < documentInvoiceDetail.Count; j++)
                    {
                        DocumentAndIndividualView documentAndIndividual = new DocumentAndIndividualView();
                        documentAndIndividual.idDocument = (Guid)documentInvoiceDetail[j].IdDocument;
                        documentAndIndividual.idIndividual = (Guid)documentInvoiceDetail[j].IdIndividual;
                        documentAndIndividuals.Add(documentAndIndividual);
                    }
                    documentInvoiceDto.DocumentAndIndividualView = documentAndIndividuals;
                }

                return documentInvoiceDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentInvoiceDetailDto> GetListDocumentInvoiceById(Guid Id)
        {
            try
            {
                List<DocumentInvoiceDetail> documentInvoices = _DbContext.DocumentInvoiceDetail.
                Where(e => e.IdDocumentInvoice == Id)
                .ToList();

                List<DocumentInvoiceDetailDto> documentInvoiceDtos = new List<DocumentInvoiceDetailDto>();
                documentInvoiceDtos = _mapper.Map<List<DocumentInvoiceDetailDto>>(documentInvoices);
                return documentInvoiceDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentInvoiceDto> getListBorrowLate(string fromDate, string toDate)
        {
            try
            {
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();

                List<DocumentInvoice> documentInvoice = null;
                if(fromDate != null && toDate != null)
                {
                    FromDate = DateTime.Parse(fromDate, new CultureInfo("en-CA"));
                    ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                    documentInvoice = _DbContext.DocumentInvoice.
                    Where(e => e.DateInReality > e.DateIn && e.Status == 2
                    && e.CreateDate >= FromDate && e.CreateDate <= ToDate)
                    .ToList();
                }
                else
                {
                    documentInvoice = _DbContext.DocumentInvoice.
                    Where(e => e.DateInReality > e.DateIn && e.Status == 2)
                    .ToList();
                }
                

                List<DocumentInvoiceDto> documentInvoiceDtos = new List<DocumentInvoiceDto>();
                documentInvoiceDtos = _mapper.Map<List<DocumentInvoiceDto>>(documentInvoice);
                return documentInvoiceDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response InsertDocumentInvoice(DocumentInvoiceDto documentInvoiceDto)
        {
            Response response = new Response();
            try
            {
                DocumentInvoice documentInvoice = new DocumentInvoice();
                documentInvoice = _mapper.Map<DocumentInvoice>(documentInvoiceDto);

                _DbContext.DocumentInvoice.Add(documentInvoice);

                for (int i = 0; i < documentInvoiceDto.DocumentAndIndividual.Count; i++)
                {
                    for (int j = 0; j < documentInvoiceDto.DocumentAndIndividual[i].idIndividual.Count; j++)
                    {
                        DocumentInvoiceDetail documentInvoiceDetail = new DocumentInvoiceDetail()
                        {
                            Id = Guid.NewGuid(),
                            IdDocument = documentInvoiceDto.DocumentAndIndividual[i].idDocument,
                            IdIndividual = documentInvoiceDto.DocumentAndIndividual[i].idIndividual[j],
                            Status = 0,
                            CreateBy = documentInvoiceDto.CreateBy,
                            CreateDate = documentInvoiceDto.CreateDate,
                            IdDocumentInvoice = documentInvoiceDto.Id
                        };
                        _DbContext.DocumentInvoiceDetail.Add(documentInvoiceDetail);

                        IndividualSample individualSample = _DbContext.IndividualSample.Where(e =>
                        e.Id == documentInvoiceDto.DocumentAndIndividual[i].idIndividual[j]).FirstOrDefault();

                        if (individualSample != null)
                        {
                            individualSample.Status = 0;
                            _DbContext.IndividualSample.Update(individualSample);
                        }
                    }
                }

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
        public Response UpdateDocumentInvoice(DocumentInvoiceDto documentInvoiceDto)
        {
            Response response = new Response();
            try
            {
                DocumentInvoice documentInvoice = new DocumentInvoice();
                documentInvoice = _DbContext.DocumentInvoice.Where(e => e.Id == documentInvoiceDto.Id).FirstOrDefault();

                if(documentInvoice.DateOut >= documentInvoiceDto.DateIn)
                {
                    response = new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Hạn trả sách không thể bé hơn ngày mượn !"
                    };
                    return response;
                }
                if (documentInvoice != null)
                {
                    // define some col with data concrete
                    documentInvoice.UserId = documentInvoiceDto.UserId;
                    documentInvoice.DateOut = documentInvoiceDto.DateOut;
                    documentInvoice.DateIn = documentInvoiceDto.DateIn;
                    documentInvoice.DateInReality = documentInvoiceDto.DateInReality.HasValue ? documentInvoiceDto.DateInReality : documentInvoice.DateInReality;
                    documentInvoice.Status = documentInvoiceDto.Status.HasValue ? documentInvoiceDto.Status : documentInvoice.Status;
                    documentInvoice.Note = String.IsNullOrEmpty(documentInvoiceDto.Note) ? documentInvoice.Note : documentInvoiceDto.Note; 

                    _DbContext.DocumentInvoice.Update(documentInvoice);
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
        public List<CustomApiSearchDocumentInvoice> ListDocumentInvoice(string name)
        {
            try
            {
                List<User> user = new List<User>();
                user = _DbContext.User.Where(e => e.Id != Guid.Empty
                && e.IsDeleted == false
                && e.IsLocked == false
                && e.IsActive == true
                && e.Fullname.ToLower().Trim().Contains(name.ToLower().Trim())).ToList();

                List<CustomApiSearchDocumentInvoice> documentInvoice = new List<CustomApiSearchDocumentInvoice>();
                for(int i = 0; i < user.Count; i++)
                {
                    CustomApiSearchDocumentInvoice documentInvoiceItem = new CustomApiSearchDocumentInvoice();
                    List<DocumentInvoice> documents = _DbContext.DocumentInvoice.Where(e => e.UserId == user[i].Id).ToList();

                    if(documents != null)
                    {
                        documentInvoiceItem.documentInvoices = documents;
                        documentInvoiceItem.NameUser = user[i].Fullname;
                        documentInvoiceItem.IdUser = user[i].Id.ToString();
                        documentInvoiceItem.Email = user[i].Email;

                        documentInvoice.Add(documentInvoiceItem);
                    }
                }
                return documentInvoice;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentInvoiceDto> GetDocumentInvoiceByStatus(int status)
        {
            try
            {
                List<DocumentInvoice> documentInvoices = new List<DocumentInvoice>();
                documentInvoices = _DbContext.DocumentInvoice.
                Where(e => e.Status == status)
                .ToList();
                List<DocumentInvoiceDto> documentInvoiceDtos = new List<DocumentInvoiceDto>();
                if(documentInvoices != null)
                {
                    documentInvoiceDtos = _mapper.Map<List<DocumentInvoiceDto>>(documentInvoices);
                }

                return documentInvoiceDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DocumentInvoiceDetailDto> GetDocumentInvoiceDetailByIdDocumentIncoice(Guid Id)
        {
            try
            {
                List<DocumentInvoiceDetail> documentInvoiceDetails = new List<DocumentInvoiceDetail>();
                documentInvoiceDetails = _DbContext.DocumentInvoiceDetail.
                Where(e => e.IdDocumentInvoice == Id)
                .ToList();
                List<DocumentInvoiceDetailDto> documentInvoiceDtos = new List<DocumentInvoiceDetailDto>();
                if (documentInvoiceDtos != null)
                {
                    documentInvoiceDtos = _mapper.Map<List<DocumentInvoiceDetailDto>>(documentInvoiceDetails);
                }

                return documentInvoiceDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response ChangeStatusCompleteInvoice(Guid Id)
        {
            try
            {
                Response response = new Response();
                try
                {
                    DocumentInvoice documentInvoice = _DbContext.DocumentInvoice.Where(x => x.Id == Id).FirstOrDefault();

                    if (documentInvoice != null)
                    {
                        documentInvoice.IsCompleted = true;
                        _DbContext.DocumentInvoice.Update(documentInvoice);
                        _DbContext.SaveChanges();

                        response = new Response()
                        {
                            Success = true,
                            Fail = false,
                            Message = "Thay đổi thành công !"
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
                        Message = "Thay đổi không thành công !"
                    };
                    return response;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
