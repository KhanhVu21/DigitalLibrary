    using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class AuditReceiptRepository: IAuditReceiptRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public AuditReceiptRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region METHOD
        public Response DeleteAuditReceipt(Guid IdAuditReceipt)
        {
            using var context = _DbContext;
            using var transaction = context.Database.BeginTransaction();

            try
            {
                #region AuditorList
                var auditorList = _DbContext.AuditorList.Where(e => e.IdAuditReceipt == IdAuditReceipt).ToList();

                _DbContext.AuditorList.RemoveRange(auditorList);
                context.SaveChanges();
                #endregion

                #region AuditBookList
                var auditBookList = _DbContext.AuditBookList.Where(e => e.IdAuditReceipt == IdAuditReceipt).ToList();

                _DbContext.AuditBookList.RemoveRange(auditBookList);
                context.SaveChanges();
                #endregion

                #region AuditReceipt
                var auditReceipt = _DbContext.AuditReceipt.Find(IdAuditReceipt);
                if (auditReceipt == null)
                {
                    return new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Không tìm thấy phiếu kiểm cần xóa !"
                    };
                }

                _DbContext.AuditReceipt.Remove(auditReceipt);
                context.SaveChanges();
                #endregion

                transaction.Commit();
                return new Response() { Success = true, Fail = false, Message = "Xóa thành công !" };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }          
        }
        public Response DeleteAuditReceiptByList(List<Guid> IdAuditReceipt)
        {
            using var context = _DbContext;
            using var transaction = context.Database.BeginTransaction();

            try
            {
                #region AuditorList
                var listOfIds = String.Join(',', IdAuditReceipt.Select(id => $"'{id}'").ToList());
                var sql = $@"DELETE AuditorList WHERE IdAuditReceipt in ({listOfIds})";

                _DbContext.Database.ExecuteSqlRaw(sql);
                context.SaveChanges();
                #endregion

                #region AuditBookList
                sql = $@"DELETE AuditBookList WHERE IdAuditReceipt in ({listOfIds})";

                _DbContext.Database.ExecuteSqlRaw(sql);
                context.SaveChanges();
                #endregion

                #region AuditReceipt
                var auditReceipts = _DbContext.AuditReceipt.Where(ar => IdAuditReceipt.Contains(ar.Id)).ToList();
                if (!auditReceipts.Any())
                {
                    return new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Không tìm thấy phiếu kiểm cần xóa !"
                    };
                }

                _DbContext.AuditReceipt.RemoveRange(auditReceipts);
                context.SaveChanges();
                #endregion

                transaction.Commit();
                return new Response() { Success = true, Fail = false, Message = "Xóa thành công !" };

            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public IEnumerable<AuditReceiptDto> GetAllAuditReceipt(int pageNumber, int pageSize, string ReportCreateDate, string ReportToDate)
        {
            var auditReceipts = _DbContext.AuditReceipt.OrderByDescending(e => e.CreatedDate).ToList();

            if (ReportCreateDate != null || ReportToDate != null)
            {
                // Parse the date strings into DateTime objects using the "dd/MM/yyyy" format
                var reportCreateDate = ReportCreateDate != null ? DateTime.ParseExact(ReportCreateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
                var reportToDate = ReportToDate != null ? DateTime.ParseExact(ReportToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;

                // Use the Where clause to filter the entities by the Date properties
                auditReceipts = _DbContext.AuditReceipt
                    .Where(e => (reportCreateDate == null || e.ReportCreateDate >= reportCreateDate) &&
                                (reportToDate == null || e.ReportToDate <= reportToDate))
                    .OrderByDescending(e => e.CreatedDate)
                    .ToList();
            }

            if (pageNumber != 0 && pageSize != 0)
            {
                if (pageNumber < 0) { pageNumber = 1; }
                auditReceipts = auditReceipts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            var result = new List<AuditReceiptDto>();
            result = _mapper.Map<List<AuditReceiptDto>>(auditReceipts);

            return result;
        }
        public DataOfOneIdAuditReceipt GetAuditReceiptById(Guid IdAuditReceipt)
        {
            var dataOfOneIdAuditReceipt = new DataOfOneIdAuditReceipt();

            var auditBookData = (from ab in _DbContext.AuditBookList
                                join d in _DbContext.Document on ab.IdDocument equals d.ID
                                join i in _DbContext.IndividualSample on ab.IdIndividualSample equals i.Id
                                join dt in _DbContext.DocumentType on d.DocumentTypeId equals dt.Id
                                join sb in _DbContext.StatusBook on ab.IdStatusBook equals sb.Id
                                where ab.IdAuditReceipt == IdAuditReceipt
                                select new DataDocumentAndAuditBookListByIdAuditReceipt()
                                {
                                    IdBook = d.ID,
                                    BookName = d.DocName,
                                    PriceBook = d.Price,
                                    Author = d.Author,
                                    IdTypeBook = d.DocumentTypeId,
                                    TypeBook = dt.DocTypeName,
                                    NumIndividual = i.NumIndividual,
                                    IdIndividual = i.Id,
                                    WasLost = ab.WasLost,
                                    Redundant = ab.Redundant,
                                    IsLiquidation = ab.IsLiquidation,
                                    IdStatusBook = ab.IdStatusBook,
                                    NameStatusBook = sb.NameStatusBook,
                                    Note = ab.Note
                                }).ToList();

            var auditorData = (from a in _DbContext.AuditorList
                              join b in _DbContext.User on a.IdUser equals b.Id
                              join c in _DbContext.Unit on b.UnitId equals c.Id
                              join d in _DbContext.UserType on b.UserTypeId equals d.Id
                              where a.IdAuditReceipt == IdAuditReceipt
                              select new AuditorListByIdAuditReceipt()
                              {
                                  IdUser = a.IdUser,
                                  UnitId = b.UnitId,
                                  UnitName = c.UnitName,
                                  UserTypeId = d.Id,
                                  TypeName = d.TypeName,
                                  DescriptionRole = a.DescriptionRole,
                                  UserName = b.Fullname
                              }).ToList();

            var getAuditReceiptById = _DbContext.AuditReceipt.Find(IdAuditReceipt);
            dataOfOneIdAuditReceipt.IdAuditReceipt = IdAuditReceipt;
            dataOfOneIdAuditReceipt.ReportCreateDate = getAuditReceiptById.ReportCreateDate;
            dataOfOneIdAuditReceipt.ReportToDate = getAuditReceiptById.ReportCreateDate;
            dataOfOneIdAuditReceipt.Note = getAuditReceiptById.Note;
            dataOfOneIdAuditReceipt.IdAuditMethod = getAuditReceiptById.IdAuditMethod;

            dataOfOneIdAuditReceipt.datas = new List<DataDocumentAndAuditBookListByIdAuditReceipt>();
            dataOfOneIdAuditReceipt.dataAuditor = new List<AuditorListByIdAuditReceipt>();
            dataOfOneIdAuditReceipt.datas.AddRange(auditBookData);
            dataOfOneIdAuditReceipt.dataAuditor.AddRange(auditorData);

            return dataOfOneIdAuditReceipt;
        }
        public Response InsertAuditReceipt(AuditReceiptDto auditReceiptDto)
        {
            using var context = _DbContext;
            using var transaction = context.Database.BeginTransaction();

            try
            {
                // Perform database operations here
                #region AuditReceipt
                // Map the auditReceiptDto object to an AuditReceipt object using the mapper
                var auditReceipt = _mapper.Map<AuditReceipt>(auditReceiptDto);

                // Set the CreatedDate and Status properties of the AuditReceipt object to the current date and a default value (0), respectively
                auditReceipt.CreatedDate = DateTime.Now;
                auditReceipt.Status = 0;

                // Generate a new Guid for the Id property of the AuditReceipt object
                auditReceipt.Id = Guid.NewGuid();
                auditReceipt.TotalBook = _DbContext.Document.Where(e => e.IsDeleted == false).Count();

                // Retrieve the maximum AuditNumber value from the AuditReceipt table in the database, sorted in descending order
                var maxAuditNumber = _DbContext.AuditReceipt
                    .OrderByDescending(e => e.CreatedDate)
                    .Select(e => e.AuditNumber)
                    .FirstOrDefault();

                // If a maximum AuditNumber value was found, parse it to extract the numeric part (after the "pkk" prefix) and increment it by 1
                // Otherwise, set the currentNumber to 0
                var currentNumber = maxAuditNumber != null ? int.Parse(maxAuditNumber.Substring(3)) : 0;

                // Set the AuditNumber property of the AuditReceipt object to the incremented value with the "pkk" prefix
                auditReceipt.AuditNumber = $"pkk{currentNumber + 1}";

                _DbContext.AuditReceipt.Add(auditReceipt);
                context.SaveChanges();
                #endregion

                #region AuditorList
                // Use the mapper and LINQ's Select method to map the list of AuditorModels in the auditReceiptDto object to a list of AuditorList objects,
                // setting the CreatedDate, Status, and Id properties of each object to the current date, a default value (0), and a new Guid, respectively
                var auditorList = auditReceiptDto.AuditorModels
                    .Select(a => _mapper.Map<AuditorList>(a))
                    .Select(a =>
                    {
                        a.CreatedDate = DateTime.Now;
                        a.Status = 0;
                        a.Id = Guid.NewGuid();
                        a.IdAuditReceipt = auditReceipt.Id;
                        return a;
                    })
                    .ToList();

                // Add the list of AuditorList objects to the AuditorList table in the database
                _DbContext.AuditorList.AddRange(auditorList);
                context.SaveChanges();
                #endregion

                #region AuditBookList
                // Use LINQ's Select method to map the list of AuditBookListPayloads to a list of AuditBookList objects using the mapper,
                // and set the CreatedDate, Status, and Id properties of each object to the current date, a default value (0), and a new Guid, respectively
                var auditBookList = auditReceiptDto.AuditBookListPayloads
                    .Select(a => _mapper.Map<AuditBookList>(a))
                    .Select(a =>
                    {
                        a.CreatedDate = DateTime.Now;
                        a.Status = 0;
                        a.Id = Guid.NewGuid();
                        a.IdAuditReceipt = auditReceipt.Id;
                        return a;
                    })
                    .ToList();

                // Add the list of AuditBookList objects to the AuditBookList table in the database
                _DbContext.AuditBookList.AddRange(auditBookList);
                context.SaveChanges();

                //Update all Id in table Individual IsLostedPhysicalVersion = 1 if wasLost == true when insert AuditBookList
                #region Individual
                var listIdIndividual = auditBookList.Where(e => e.WasLost == true).Select(e => e.IdIndividualSample).ToList();
                if(listIdIndividual.Any())
                {
                    var listOfIds = String.Join(',', listIdIndividual.Select(id => $"'{id}'").ToList());
                    var sql = $@"UPDATE IndividualSample SET IsLostedPhysicalVersion = 1 WHERE Id in ({listOfIds})";

                    _DbContext.Database.ExecuteSqlRaw(sql);
                    context.SaveChanges();
                }
                #endregion
                #endregion

                transaction.Commit();

                return new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Thêm mới thành công !"
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public Response UpdateAuditReceipt(Guid IdAuditReceipt, AuditReceiptDto auditReceiptDto)
        {
            using var context = _DbContext;
            using var transaction = context.Database.BeginTransaction();

            try
            {
                #region AuditReceipt
                var auditReceipt = _DbContext.AuditReceipt.Find(IdAuditReceipt);
                if (auditReceipt == null)
                {
                    return new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Không tìm thấy phiếu kiểm cần cập nhật !"
                    };
                }

                auditReceipt.ReportToDate = auditReceiptDto.ReportToDate;
                auditReceipt.ReportCreateDate = auditReceiptDto.ReportCreateDate;
                auditReceipt.Note = auditReceiptDto.Note;
                auditReceipt.IdAuditMethod = auditReceiptDto.IdAuditMethod;

                _DbContext.AuditReceipt.Update(auditReceipt);
                context.SaveChanges();
                #endregion

                #region AuditorList
                //delete table AuditorList before insert new row
                var auditorList = _DbContext.AuditorList.Where(e => e.IdAuditReceipt == IdAuditReceipt).ToList();
                _DbContext.AuditorList.RemoveRange(auditorList);
                context.SaveChanges();

                // Use the mapper and LINQ's Select method to map the list of AuditorModels in the auditReceiptDto object to a list of AuditorList objects,
                // setting the CreatedDate, Status, and Id properties of each object to the current date, a default value (0), and a new Guid, respectively
                auditorList = auditReceiptDto.AuditorModels
                    .Select(a => _mapper.Map<AuditorList>(a))
                    .Select(a =>
                    {
                        a.CreatedDate = DateTime.Now;
                        a.Status = 0;
                        a.Id = Guid.NewGuid();
                        a.IdAuditReceipt = auditReceipt.Id;
                        return a;
                    })
                    .ToList();

                // Add the list of AuditorList objects to the AuditorList table in the database
                _DbContext.AuditorList.AddRange(auditorList);
                context.SaveChanges();
                #endregion

                #region AuditBookList
                //delete table AuditBookList before insert new row
                var auditBookList = _DbContext.AuditBookList.Where(e => e.IdAuditReceipt == IdAuditReceipt).ToList();
                _DbContext.AuditBookList.RemoveRange(auditBookList);
                context.SaveChanges();

                // Use LINQ's Select method to map the list of AuditBookListPayloads to a list of AuditBookList objects using the mapper,
                // and set the CreatedDate, Status, and Id properties of each object to the current date, a default value (0), and a new Guid, respectively
                auditBookList = auditReceiptDto.AuditBookListPayloads
                    .Select(a => _mapper.Map<AuditBookList>(a))
                    .Select(a =>
                    {
                        a.CreatedDate = DateTime.Now;
                        a.Status = 0;
                        a.Id = Guid.NewGuid();
                        a.IdAuditReceipt = auditReceipt.Id;
                        return a;
                    })
                    .ToList();

                // Add the list of AuditBookList objects to the AuditBookList table in the database
                _DbContext.AuditBookList.AddRange(auditBookList);
                context.SaveChanges();
                #endregion

                transaction.Commit();
                return new Response() { Success = true, Fail = false, Message = "Cập nhật thành công !" };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }  
        }
        public CustomApiAuditReceipt GetDataBookByBarcode(string barcode)
        {
            var customApiAuditReceipt = new CustomApiAuditReceipt();

            var individualSample = _DbContext.IndividualSample.Where(e => e.Barcode == barcode
            && e.IsDeleted == false && e.IsLostedPhysicalVersion == false
            && e.Status == 1).FirstOrDefault();
            if (individualSample is null) return customApiAuditReceipt;

            var document = _DbContext.Document.Where(e => e.ID == individualSample.IdDocument && e.IsDeleted == false).FirstOrDefault();
            if (document is null) return customApiAuditReceipt;

            var documentType = _DbContext.DocumentType.Where(e => e.IsDeleted == false && e.Id == document.DocumentTypeId).FirstOrDefault();
            if (documentType is null) return customApiAuditReceipt;

            return new CustomApiAuditReceipt
            {
                IdBook = document.ID,
                BookName = document.DocName,
                PriceBook = document.Price,
                IdTypeBook = document.DocumentTypeId,
                TypeBook = documentType.DocTypeName,
                NumIndividual = individualSample.NumIndividual,
                IdIndividual = individualSample.Id
            };
        }
        public Tuple<string, string> GetUnitAndTypeOfUser(Guid IdUser)
        {
            var user = _DbContext.User.Find(IdUser);
            if (user is null) return new Tuple<string, string>("","");

            var unit = _DbContext.Unit.Where(e => e.Id == user.UnitId).Select(e => e.UnitName).FirstOrDefault();
            if (unit is null) return new Tuple<string, string>("", "");

            var type = _DbContext.UserType.Where(e => e.Id == user.UserTypeId).Select(e => e.TypeName).FirstOrDefault();
            if (type is null) return new Tuple<string, string>("", "");

            return new Tuple<string, string>(unit.ToString(), type.ToString());
        }
        public Response LiquidationAuditReceiptByListId(List<Guid> IdAuditReceipt)
        {
            using var context = _DbContext;
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var invalidIds = IdAuditReceipt.Where(id => _DbContext.AuditReceipt.Find(id) == null);
                if (invalidIds.Any())
                {
                    return new Response() { Success = false, Fail = true, Message = "Đã có ID không tồn tại !" };
                }

                var lostBooks = _DbContext.AuditBookList.Where(e => IdAuditReceipt.Contains((Guid)e.IdAuditReceipt) && e.WasLost == true).ToList();
                if (lostBooks.Any())
                {
                    return new Response() { Success = false, Fail = true, Message = "Đã có sách mất không thể thanh lý !" };
                }

                #region AuditBookList
                var listOfIds = String.Join(',', IdAuditReceipt.Select(id => $"'{id}'").ToList());
                var sql = $@"UPDATE AuditBookList SET IsLiquidation = 1 WHERE IdAuditReceipt in ({listOfIds})";

                _DbContext.Database.ExecuteSqlRaw(sql);
                context.SaveChanges();

                var listIdIndividual = _DbContext.AuditBookList
                .Where(e => IdAuditReceipt.Contains((Guid)e.IdAuditReceipt)).Select(e => e.IdIndividualSample).ToList();

                if (listIdIndividual.Any())
                {
                    listOfIds = String.Join(',', listIdIndividual.Select(id => $"'{id}'").ToList());
                    sql = $@"UPDATE IndividualSample SET IsDeleted = 1 WHERE Id in ({listOfIds})";

                    _DbContext.Database.ExecuteSqlRaw(sql);
                    context.SaveChanges();
                }
                #endregion

                transaction.Commit();
                return new Response() { Success = true, Fail = false, Message = "Thanh lý thành công !" };

            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public List<CustomApiAuditReceipt> ConfirmLostBook(int pageNumber, int pageSize, List<Guid> IdIndividual)
        {
            var listIndividualInvoicing = (from a in _DbContext.DocumentInvoice
                                      join b in _DbContext.DocumentInvoiceDetail on a.Id equals b.IdDocumentInvoice
                                      where a.Status == 0
                                      select b.IdIndividual).ToList();

            var bookData = (from d in _DbContext.Document
                                join i in _DbContext.IndividualSample on d.ID equals i.IdDocument
                                join dt in _DbContext.DocumentType on d.DocumentTypeId equals dt.Id
                                where d.IsDeleted == false && i.IsDeleted == false && !IdIndividual.Contains(i.Id)
                                && !listIndividualInvoicing.Contains(i.Id)
                                select new CustomApiAuditReceipt
                                {
                                    IdBook = d.ID,
                                    BookName = d.DocName,
                                    PriceBook = d.Price,
                                    IdTypeBook = d.DocumentTypeId,
                                    TypeBook = dt.DocTypeName,
                                    NumIndividual = i.NumIndividual,
                                    IdIndividual = i.Id,
                                    Author = d.Author          
                                }).ToList();

            if (pageNumber != 0 && pageSize != 0)
            {
                if (pageNumber < 0) { pageNumber = 1; }
                bookData = bookData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            return bookData;
        }
        public ReportAuditReceipt ReportAuditReceipt(Guid IdAuditReceipt)
        {
            var auditReceipt = _DbContext.AuditReceipt.Find(IdAuditReceipt);
            if (auditReceipt is null) return new ReportAuditReceipt();
            var auditBookList = _DbContext.AuditBookList.Where(e => e.IdAuditReceipt == IdAuditReceipt).Select(e => e.Id).ToList();

            var auditorData = (from a in _DbContext.AuditorList
                               join b in _DbContext.User on a.IdUser equals b.Id
                               join c in _DbContext.Unit on b.UnitId equals c.Id
                               join d in _DbContext.UserType on b.UserTypeId equals d.Id
                               where a.IdAuditReceipt == IdAuditReceipt
                               select new AuditorListByIdAuditReceipt()
                               {
                                   IdUser = a.IdUser,
                                   UnitId = b.UnitId,
                                   UnitName = c.UnitName,
                                   UserTypeId = d.Id,
                                   TypeName = d.TypeName,
                                   DescriptionRole = a.DescriptionRole,
                                   UserName = b.Fullname
                               }).ToList();

            var auditBookData = (from ab in _DbContext.AuditBookList
                                 join d in _DbContext.Document on ab.IdDocument equals d.ID
                                 join i in _DbContext.IndividualSample on ab.IdIndividualSample equals i.Id
                                 join dt in _DbContext.DocumentType on d.DocumentTypeId equals dt.Id
                                 join sb in _DbContext.StatusBook on ab.IdStatusBook equals sb.Id
                                 where ab.IdAuditReceipt == IdAuditReceipt
                                 select new DataDocumentAndAuditBookListByIdAuditReceipt()
                                 {
                                     IdBook = d.ID,
                                     BookName = d.DocName,
                                     PriceBook = d.Price,
                                     Author = d.Author,
                                     IdTypeBook = d.DocumentTypeId,
                                     TypeBook = dt.DocTypeName,
                                     NumIndividual = i.NumIndividual,
                                     IdIndividual = i.Id,
                                     WasLost = ab.WasLost,
                                     Redundant = ab.Redundant,
                                     IsLiquidation = ab.IsLiquidation,
                                     IdStatusBook = ab.IdStatusBook,
                                     NameStatusBook = sb.NameStatusBook,
                                     Note = ab.Note
                                 }).ToList();

            var auditBookDatas = auditBookData
            .GroupBy(g => g.IdTypeBook)
            .Select(g => new ReportAuditReceiptDetail()
            {
                DocumentTypeId = g.Key,
                DocumentTypeName = g.FirstOrDefault().TypeBook,
                Datas = g.Select(x => new DataDocumentAndAuditBookListByIdDocumentType
                {
                    IdBook = x.IdBook,
                    BookName = x.BookName,
                    PriceBook = x.PriceBook,
                    Author = x.Author,
                    NumIndividual = x.NumIndividual,
                    IdIndividual = x.IdIndividual,
                    WasLost = x.WasLost,
                    Redundant = x.Redundant,
                    IsLiquidation = x.IsLiquidation,
                    IdStatusBook = x.IdStatusBook,
                    NameStatusBook = x.NameStatusBook,
                    Note = x.Note
                }).ToList()
            })
            .ToList();

            var documentTypeAndQuantities = _DbContext.DocumentType
                .Where(e => e.IsDeleted == false && e.Status == 1 && e.ParentId == null)
                .Select(e => new DocumentTypeAndQuantity
                {
                    Quantity = _DbContext.Document
                        .Where(d => d.DocumentTypeId == e.Id && d.IsDeleted == false && auditBookList.Contains(d.ID))
                        .Count(),
                    DocumentType = e.DocTypeName
                })
                .ToList();

            var statusBookAndQuantities = _DbContext.StatusBook
                .Select(e => new ResultOfStatusBook
                {
                    Quantity = _DbContext.AuditBookList
                        .Where(d => d.IdStatusBook == e.Id)
                        .Count(),
                    StatusBook = e.NameStatusBook
                })
                .ToList();

            return new ReportAuditReceipt()
            {
                IdAuditReceipt = IdAuditReceipt,
                ReportCreateDate = auditReceipt.ReportCreateDate,
                ReportToDate = auditReceipt.ReportCreateDate,
                Note = auditReceipt.Note,
                IdAuditMethod = auditReceipt.IdAuditMethod,
                DataAuditor = auditorData,
                DataQuantityDocument = new ResultOfAuditReceipt()
                {
                    TotalBookInLibrary = auditReceipt.TotalBook,
                    Datas = documentTypeAndQuantities
                },
                ResultOfStatusBooks = statusBookAndQuantities,
                ResultReportAuditReceiptDetail = auditBookDatas
            };
        }
        public AuditTraditionalDocument PrintListDataDocument(Guid IdDocumentType, int sortByCondition)
        {
            var result = _DbContext.GetDynamicResult("exec [dbo].[Sp_GetAllDataOfBookByDocumentType] @IdDocumentType", new SqlParameter("@IdDocumentType", IdDocumentType));

            var listDataBook = result
            .Select(str1 => (DataBook)JsonConvert.DeserializeObject<DataBook>(JsonConvert.SerializeObject(str1))).ToList();

            var temp = listDataBook
            .GroupBy(g => g.Id)
            .Select(g => new DataBookByDocumentType()
            {
                DocumentTypeName = g.FirstOrDefault().DocTypeName,
                DataOfBook = sortByCondition == 0 ? g.Select(x => new DataBook
                {
                    DocName = x.DocName,
                    NumIndividual = x.NumIndividual,
                    Author = x.Author,
                    WasLost = x.WasLost,
                    Redundant = x.Redundant,
                    IsLiquidation = x.IsLiquidation,
                    NameStatusBook = x.NameStatusBook,
                    SignCode = x.SignCode,
                    EncryptDocumentName = x.EncryptDocumentName,
                    Publisher = x.Publisher,
                    PublishYear = x.PublishYear
                }).OrderBy(x => x.NumIndividual).ToList() : sortByCondition == 1 ?
                g.Select(x => new DataBook
                {
                    DocName = x.DocName,
                    NumIndividual = x.NumIndividual,
                    Author = x.Author,
                    WasLost = x.WasLost,
                    Redundant = x.Redundant,
                    IsLiquidation = x.IsLiquidation,
                    NameStatusBook = x.NameStatusBook,
                    SignCode = x.SignCode,
                    EncryptDocumentName = x.EncryptDocumentName,
                    Publisher = x.Publisher,
                    PublishYear = x.PublishYear
                }).OrderBy(e => e.SignCode).ToList() :
                g.Select(x => new DataBook
                {
                    DocName = x.DocName,
                    NumIndividual = x.NumIndividual,
                    Author = x.Author,
                    WasLost = x.WasLost,
                    Redundant = x.Redundant,
                    IsLiquidation = x.IsLiquidation,
                    NameStatusBook = x.NameStatusBook,
                    SignCode = x.SignCode,
                    EncryptDocumentName = x.EncryptDocumentName,
                    Publisher = x.Publisher,
                    PublishYear = x.PublishYear
                }).OrderBy(e => e.EncryptDocumentName).ToList()
            }).ToList();

            return new AuditTraditionalDocument
            {
                dataBookByDocumentTypes = temp
            };
        }
        public long CountAllNumberOfBook()
        {
            var bookData = (from d in _DbContext.Document
                            join i in _DbContext.IndividualSample on d.ID equals i.IdDocument
                            join dt in _DbContext.DocumentType on d.DocumentTypeId equals dt.Id
                            where d.IsDeleted == false && i.IsDeleted == false && i.IsLostedPhysicalVersion == false
                            && dt.IsDeleted == false && dt.Status == 1
                            select i).Count();

            return bookData;
        }
        public List<CustomApiAuditReceipt> GetListBookToAuditReceipt(string filter, Guid IdDocumentType, int pageNumber, int pageSize)
        {
            var bookData = (from d in _DbContext.Document
                            join i in _DbContext.IndividualSample on d.ID equals i.IdDocument
                            join dt in _DbContext.DocumentType on d.DocumentTypeId equals dt.Id
                            where d.IsDeleted == false && i.IsDeleted == false && i.IsLostedPhysicalVersion == false
                            && dt.IsDeleted == false && dt.Status == 1
                            select new CustomApiAuditReceipt
                            {
                                IdBook = d.ID,
                                BookName = d.DocName,
                                PriceBook = d.Price,
                                IdTypeBook = d.DocumentTypeId,
                                TypeBook = dt.DocTypeName,
                                NumIndividual = i.NumIndividual,
                                IdIndividual = i.Id,
                                Author = d.Author,
                                IsLostedPhysicalVersion = i.IsLostedPhysicalVersion
                            }).ToList();

            if(IdDocumentType != Guid.Empty)
            {
                bookData = bookData.Where(e => e.IdTypeBook == IdDocumentType).ToList();
            }

            if (filter is not null)
            {
                var tempFilter = Regex.Replace(filter, @"\s+", " ").Trim().ToLower();       
                bookData = bookData.Where(e => (e.BookName ?? "").ToLower().Contains(tempFilter)
                || (e.Author ?? "").ToLower().Contains(tempFilter)).ToList();
            }

            if (pageNumber != 0 && pageSize != 0)
            {
                if (pageNumber < 0) { pageNumber = 1; }
                bookData = bookData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            return bookData;
        }
        #endregion
    }
}
