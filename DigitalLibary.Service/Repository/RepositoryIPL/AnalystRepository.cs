using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class AnalystRepository: IAnalystRepository             
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public AnalystRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region Method
        public List<CustomApiCountUserByUserType> CountUserByType()
        {
            try
            {
                List<CustomApiCountUserByUserType> countUserList = new List<CustomApiCountUserByUserType>();
                User user = new User();

                List<UserType> userType = new List<UserType>();
                userType = _DbContext.UserType.Where(e => e.Id != Guid.Empty).ToList();

                for(int i = 0; i < userType.Count; i++)
                {
                    CustomApiCountUserByUserType countUser = new CustomApiCountUserByUserType();
                    int count = _DbContext.User.Where(e => e.UserTypeId == userType[i].Id && e.IsDeleted == false).Count();
                    countUser.UserType = userType[i];
                    countUser.NumberUser = count;

                    countUserList.Add(countUser);
                }

                return countUserList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiCountDocumentByType> CountDocumentByType()
        {
            try
            {
                List<CustomApiCountDocumentByType> countDocList = new List<CustomApiCountDocumentByType>();
                List<DocumentType> documentType = new List<DocumentType>();
                documentType = _DbContext.DocumentType.Where(e => e.IsDeleted == false).ToList();

               
                for (int i = 0; i < documentType.Count; i++)
                {
                    CustomApiCountDocumentByType countDoc = new CustomApiCountDocumentByType();
                    int count = _DbContext.Document.Where(e => e.DocumentTypeId == documentType[i].Id && e.IsDeleted == false).Count();
                    countDoc.documentType = documentType[i];
                    countDoc.count = count;

                    countDocList.Add(countDoc);
                }

                return countDocList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public CustomApiAnalystUserAndBook AnalystUserAndBook()
        {
            try
            {
                var customApiAnalystUserAndBook = new CustomApiAnalystUserAndBook();
                int month = DateTime.Now.Month;

                var totalserAnalyst = new TotalserAnalyst();
                int totalUser = _DbContext.User.Where(e => e.IsDeleted == false).Count();
                totalserAnalyst.TotalUser = totalUser;
                customApiAnalystUserAndBook.totalser = totalserAnalyst;

                // count list user by current month and last month
                int countUserCurrentMont = _DbContext.User.Where(e => e.CreatedDate.Value.Month == month
                && e.IsDeleted == false).Count();
                int countUserLastMonth = _DbContext.User.Where(e => e.CreatedDate.Value.Month == month - 1
                && e.IsDeleted == false).Count();

                if (countUserLastMonth == 0)
                {
                    countUserLastMonth = 1;
                }
                double Result = countUserCurrentMont - countUserLastMonth / (double)(countUserLastMonth * 100);

                UserAnalyst userAnalyst = new UserAnalyst();
                userAnalyst.NumberUserCurrentMonth = countUserCurrentMont;
                userAnalyst.NumberUserLastMonth = countUserLastMonth;
                userAnalyst.CurrentMonth = month;
                userAnalyst.LastMonth = month - 1;
                userAnalyst.percentDifference = Result;
                customApiAnalystUserAndBook.userAnalyst = userAnalyst;

                // count number book borrow current month and last month
                int numberBookBorrowCurrentMonth = _DbContext.DocumentInvoice
                .Where(e => e.CreateDate.Value.Month == month && e.Status == 0 || e.Status == 2).Count();
                int numberBookBorrowLastMonth = _DbContext.DocumentInvoice
                .Where(e => e.CreateDate.Value.Month == month - 1 && e.Status == 0 || e.Status == 2).Count();

                if(numberBookBorrowLastMonth == 0)
                {
                    numberBookBorrowLastMonth = 1;
                }
                Result = numberBookBorrowCurrentMonth - numberBookBorrowLastMonth / (double)(numberBookBorrowLastMonth * 100);

                BookBorrowAnalyst bookBorrowAnalyst = new BookBorrowAnalyst();
                bookBorrowAnalyst.TotalBorrowBookCurrentMonth = numberBookBorrowCurrentMonth;
                bookBorrowAnalyst.TotalBorrowBookLastMonth = numberBookBorrowLastMonth;
                bookBorrowAnalyst.CurrentMonth = month;
                bookBorrowAnalyst.LastMonth = month - 1;
                bookBorrowAnalyst.percentDifference = Result;
                customApiAnalystUserAndBook.bookBorrowAnalyst = bookBorrowAnalyst;

                // count number book back current month and last month
                int numberBookBackCurrentMonth = _DbContext.DocumentInvoice
                .Where(e => e.CreateDate.Value.Month == month && e.Status == 1).Count();
                int numberBookBackLastMonth = _DbContext.DocumentInvoice
                .Where(e => e.CreateDate.Value.Month == month - 1 && e.Status == 1).Count();

                if (numberBookBackLastMonth == 0)
                {
                    numberBookBackLastMonth = 1;
                }
                Result = numberBookBackCurrentMonth - numberBookBackLastMonth / (double)(numberBookBackLastMonth * 100);

                BookBackAnalyst bookBackAnalyst = new BookBackAnalyst();
                bookBackAnalyst.TotalBookBackCurrentMonth = numberBookBackCurrentMonth;
                bookBackAnalyst.TotalBookBackLastMonth = numberBookBackLastMonth;
                bookBackAnalyst.CurrentMonth = month;
                bookBackAnalyst.LastMonth = month - 1;
                bookBackAnalyst.percentDifference = Result;
                customApiAnalystUserAndBook.bookBackAnalyst = bookBackAnalyst;


                return customApiAnalystUserAndBook;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiBorrowByUserType> customApiBorrowByUserTypes(string status, string fromDate, string toDate)
        {
            try
            {
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();

                List<CustomApiBorrowByUserType> customApiBorrowByUserTypes = new List<CustomApiBorrowByUserType>();
                List<UserType> userTypes = _DbContext.UserType.Where(e => e.Id != Guid.Empty).ToList();

                for(int i = 0; i < userTypes.Count; i++)
                {
                    List<User> users = _DbContext.User.Where(e => e.UserTypeId == userTypes[i].Id
                    && e.IsDeleted == false && e.IsActive == true && e.IsLocked == false).ToList();

                    int countUserBorrow = 0;
                    CustomApiBorrowByUserType customApiBorrowByUserType = new CustomApiBorrowByUserType();
                    for (int j = 0; j < users.Count; j++)
                    {
                        if(status == "NotLate")
                        {
                            // if empty todate and fromdate return full data
                            // if param exit return data by todate and fromdate
                            if (fromDate == null && toDate == null)
                            {
                                int count = _DbContext.DocumentInvoice.Where(e => e.UserId == users[j].Id).Count();
                                countUserBorrow += count;
                            }
                            else
                            {
                                FromDate = DateTime.Parse(fromDate, new CultureInfo("en-CA"));
                                ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                                int count = _DbContext.DocumentInvoice.Where(e => e.UserId == users[j].Id
                                && e.CreateDate >= FromDate && e.CreateDate <= ToDate).Count();
                                countUserBorrow += count;
                            }
                        }
                        else
                        {
                            // if empty todate and fromdate return full data
                            // if param exit return data by todate and fromdate
                            if (fromDate == null && toDate == null)
                            {
                                int count = _DbContext.DocumentInvoice.Where(e => e.UserId == users[j].Id
                                && e.DateInReality > e.DateIn && e.Status == 2).Count();
                                countUserBorrow += count;
                            }
                            else
                            {
                                FromDate = DateTime.Parse(fromDate, new CultureInfo("en-CA"));
                                ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                                int count = _DbContext.DocumentInvoice.Where(e => e.UserId == users[j].Id
                                && e.DateInReality > e.DateIn && e.Status == 2 && e.CreateDate >= FromDate
                                && e.CreateDate <= ToDate).Count();
                                countUserBorrow += count;
                            }
                        }
                    }
                    
                    customApiBorrowByUserType.UserType = userTypes[i].TypeName;
                    customApiBorrowByUserType.NumberUserType = countUserBorrow;
                    customApiBorrowByUserTypes.Add(customApiBorrowByUserType);
                }
                int sum = 0;
                for (int i = 0; i < customApiBorrowByUserTypes.Count; i++)
                {
                    sum += customApiBorrowByUserTypes[i].NumberUserType;
                }
                if (sum == 0)
                {
                    sum += 1;
                }
                for (int i = 0; i < customApiBorrowByUserTypes.Count; i++)
                {
                    customApiBorrowByUserTypes[i].percent = (customApiBorrowByUserTypes[i].NumberUserType / (double)sum) * 100;
                }

                return customApiBorrowByUserTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiAnalystBookByType> customApiAnalystBookByTypes(Guid IdDocumentType)
        {
            var result = _DbContext.GetDynamicResult("exec [dbo].[SP_AnalystBookByTypes] @IdDocumentType", new SqlParameter("@IdDocumentType", IdDocumentType));

            var listDataBook = result
            .Select(str1 => (AnalystBookAndType)JsonConvert
            .DeserializeObject<AnalystBookAndType>(JsonConvert
            .SerializeObject(str1))).ToList();

            var datas = listDataBook
                .Select(s => new CustomApiAnalystBookByType
                {
                    TotalDocument = s.TotalDocument,
                    RemainDocument = s.RemainDocument,
                    LostDocument = s.LostDocument,
                    BorrowedDocument = s.BorrowedDocument,
                    NameDocmentType = s.NameDocmentType,
                    document = new Document
                    {
                        ID = s.ID,
                        DocName = s.DocName,
                        CreatedDate = s.CreatedDate,
                        IsHavePhysicalVersion = s.IsHavePhysicalVersion,
                        Language = s.Language,
                        Publisher = s.Publisher,
                        PublishYear = s.PublishYear,
                        NumberLike = s.NumberLike,
                        NumberUnlike = s.NumberUnlike,
                        NumberView = s.NumberView,
                        ModifiedDate = s.ModifiedDate,
                        Author = s.Author,
                        Description = s.Description,
                        Price = s.Price,
                        DocumentTypeId = s.DocumentTypeId,
                        OriginalFileName = s.OriginalFileName,
                        FileName = s.FileName,
                        FileNameExtention = s.FileNameExtention,
                        FilePath = s.FilePath
                    }
                }).ToList();

            return new List<CustomApiAnalystBookByType>(datas);
        }
        public List<AnalystBookByGroupType> AnalystBookByGroupTypes(Guid IdDocumentType)
        {
            var datas = customApiAnalystBookByTypes(IdDocumentType);

            var temp = datas.GroupBy(e => e.document.DocumentTypeId)
                .Select(e => new AnalystBookByGroupType
                {
                    IdDocumentType = e.Key,
                    NameDocmentType = e.FirstOrDefault().NameDocmentType,
                    DataAnalystBooks = e.Select(x => new CustomApiAnalystBookByType
                    {
                        document = x.document,
                        TotalDocument = x.TotalDocument,
                        RemainDocument = x.RemainDocument,
                        BorrowedDocument = x.BorrowedDocument,
                        LostDocument = x.LostDocument
                    }).ToList()
                }).ToList();
            return new List<AnalystBookByGroupType>(temp);
        }
        public List<ListBookNew> ListDocumentByIdStock(Guid IdStock)
        {
            try
            {
                List<ListBookNew> lstBookview = new List<ListBookNew>();

                List<DocumentStock> documentStocks = _DbContext.DocumentStock.Where(e => e.Id == IdStock && e.IsDeleted == false
                || e.StockParentId == IdStock && e.IsDeleted == false).ToList();

                HashSet<Guid?> lstIdDocument = new HashSet<Guid?>();
                for(int i = 0; i < documentStocks.Count; i++)
                {
                    List<IndividualSample> individualSamples = _DbContext.IndividualSample.AsNoTracking().Where(e =>
                    e.StockId == documentStocks[i].Id && e.IsDeleted == false && e.IsLostedPhysicalVersion == false
                    ).ToList();
                    for(int j = 0; j < individualSamples.Count; j++)
                    {
                        lstIdDocument.Add(individualSamples[j].IdDocument);
                    }
                }

                foreach(var unique in lstIdDocument)
                {
                    Document document = _DbContext.Document.AsNoTracking().Where(e =>
                    e.ID == unique.Value && e.IsDeleted == false).FirstOrDefault();

                    List<DocumentAvatar> documentAvatars = new List<DocumentAvatar>();
                    DocumentType documentType = new DocumentType();
                    if (document != null)
                    {
                        documentAvatars = _DbContext.DocumentAvatar.Where(e => e.IdDocument == document.ID).ToList();
                        documentType = _DbContext.DocumentType.AsNoTracking().Where(e =>
                        e.Id == document.DocumentTypeId).FirstOrDefault();

                        ListBookNew lstBook = new ListBookNew();
                        lstBook.Document = document;
                        lstBook.listAvatar = documentAvatars;
                        lstBook.IdCategory = documentType.Id;
                        lstBook.NameCategory = documentType.DocTypeName;

                        lstBookview.Add(lstBook);
                    }

                }
                
                return lstBookview;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int CountDocumentByIdStock(Guid IdStock)
        {
            try
            {
                int count = 0;

                List<DocumentStock> documentStocks = _DbContext.DocumentStock.Where(e => e.Id == IdStock || e.StockParentId == IdStock).ToList();
                List<Guid?> lstIdDocument = new List<Guid?>();
                for (int i = 0; i < documentStocks.Count; i++)
                {
                    List<IndividualSample> individualSamples = _DbContext.IndividualSample.AsNoTracking().Where(e =>
                    e.StockId == documentStocks[i].Id && e.IsDeleted == false && e.IsLostedPhysicalVersion == false).ToList();

                    count += individualSamples.Count;
                }

                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiBorrowByUserType> customApiBorrowByUserTypes(string fromDate, string toDate)
        {
            try
            {
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();

                List<CustomApiBorrowByUserType> customApiBorrowByUserTypes = new List<CustomApiBorrowByUserType>();
                List<UserType> userTypes = _DbContext.UserType.Where(e => e.Id != Guid.Empty).ToList();

                for (int i = 0; i < userTypes.Count; i++)
                {
                    List<User> users = _DbContext.User.Where(e => e.UserTypeId == userTypes[i].Id
                    && e.IsDeleted == false && e.IsActive == true && e.IsLocked == false).ToList();

                    int countUserBorrow = 0;
                    CustomApiBorrowByUserType customApiBorrowByUserType = new CustomApiBorrowByUserType();
                    for (int j = 0; j < users.Count; j++)
                    {
                        int count = 0;
                        if(fromDate == null && toDate == null)
                        {
                            count = _DbContext.DocumentInvoice.Where(e => e.UserId == users[j].Id).Count();
                        }
                        else
                        {
                            FromDate = DateTime.Parse(fromDate, new CultureInfo("en-CA"));
                            ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                            count = _DbContext.DocumentInvoice.Where(e => e.UserId == users[j].Id
                            && e.CreateDate >= FromDate && e.CreateDate <= ToDate).Count();
                        }
                        countUserBorrow += count;
                    }

                    customApiBorrowByUserType.UserType = userTypes[i].TypeName;
                    customApiBorrowByUserType.NumberUserType = countUserBorrow;
                    customApiBorrowByUserTypes.Add(customApiBorrowByUserType);
                }

                int sum = 0;
                for(int i = 0; i < customApiBorrowByUserTypes.Count;i++)
                {
                    sum += customApiBorrowByUserTypes[i].NumberUserType;
                }
                if(sum == 0)
                {
                    sum += 1;
                }
                for(int i = 0; i < customApiBorrowByUserTypes.Count;i++)
                {
                    customApiBorrowByUserTypes[i].percent = (customApiBorrowByUserTypes[i].NumberUserType / (double)sum) * 100;
                }
                return customApiBorrowByUserTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiListBorrowByUserType> customApiListBorrowByUserTypes(Guid IdUnit, Guid IdUserType, string fromDate, string toDate)
        {
            try
            {
                List<CustomApiListBorrowByUserType> customApiListBorrowByUserTypes = new List<CustomApiListBorrowByUserType>();
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();

                List<User> users = _DbContext.User.Where(e =>
                e.UserTypeId == IdUserType && e.IsDeleted == false
                && e.UnitId == IdUnit).ToList();

                for(int i = 0; i < users.Count; i++)
                {

                    DocumentInvoice documentInvoice = null;
                    if(fromDate != null && toDate != null)
                    {
                        FromDate = DateTime.Parse(fromDate, new CultureInfo("en-CA"));
                        ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                        documentInvoice = _DbContext.DocumentInvoice.Where(e =>
                        e.UserId == users[i].Id && e.Status == 0
                        && e.CreateDate >= FromDate && e.CreateDate <= ToDate).FirstOrDefault();
                    }
                    else
                    {
                        documentInvoice = _DbContext.DocumentInvoice.Where(e =>
                        e.UserId == users[i].Id && e.Status == 0
                        ).FirstOrDefault();
                    }

                    if(documentInvoice != null)
                    {
                        CustomApiListBorrowByUserType customApiListBorrowByUserType = new CustomApiListBorrowByUserType();

                        customApiListBorrowByUserType.IdUser = users[i].Id;
                        customApiListBorrowByUserType.NameUser = users[i].Fullname;
                        customApiListBorrowByUserType.fromDate = documentInvoice.DateOut;
                        customApiListBorrowByUserType.toDate = documentInvoice.DateIn;
                        customApiListBorrowByUserType.UserCode = users[i].UserCode;
                        customApiListBorrowByUserType.Email = users[i].Email;
                        customApiListBorrowByUserType.Address = users[i].Address;
                        customApiListBorrowByUserType.IdUnit = users[i].UnitId;
                        customApiListBorrowByUserType.IdUserType = users[i].UserTypeId;

                        customApiListBorrowByUserTypes.Add(customApiListBorrowByUserType);
                    }
                }

                return customApiListBorrowByUserTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public CustomApiListBorrowByUserTypeDetail customApiListBorrowByUserTypesDetail(Guid IdUnit, Guid IdUser, string fromDate, string toDate)
        {
            try
            {

                CustomApiListBorrowByUserTypeDetail customApiListBorrowByUserTypeDetail = new CustomApiListBorrowByUserTypeDetail();
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();

                User user = _DbContext.User.Where(e => 
                e.Id == IdUser && e.IsDeleted == false && e.IsActive == true && e.IsLocked == false).FirstOrDefault();

                if(user != null)
                {

                    List<DocumentInvoice> documentInvoice = null;
                    if (fromDate != null && toDate != null)
                    {
                        FromDate = DateTime.Parse(fromDate, new CultureInfo("en-CA"));
                        ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                        documentInvoice = _DbContext.DocumentInvoice.Where(e =>
                        e.UserId == user.Id && e.Status == 0
                        && e.CreateDate >= FromDate && e.CreateDate <= ToDate).ToList();
                    }
                    else
                    {
                        documentInvoice = _DbContext.DocumentInvoice.Where(e =>
                        e.UserId == user.Id && e.Status == 0).ToList();
                    }

                    Unit unit = _DbContext.Unit.Where(e =>
                    e.Id == user.UnitId).FirstOrDefault();

                    customApiListBorrowByUserTypeDetail.IdUser = user.Id;
                    customApiListBorrowByUserTypeDetail.NameUser = user.Fullname;
                    customApiListBorrowByUserTypeDetail.UserCode = user.UserCode;
                    customApiListBorrowByUserTypeDetail.Email = user.Email;
                    customApiListBorrowByUserTypeDetail.Address = user.Address;
                    customApiListBorrowByUserTypeDetail.IdUnit = user.UnitId;
                    customApiListBorrowByUserTypeDetail.NameUnit = unit.UnitName;
                    customApiListBorrowByUserTypeDetail.IdUserType = user.UserTypeId;

                    List<ListBorrowByUserId> listBorrowByUserIds = new List<ListBorrowByUserId>();

                    for (int i = 0; i < documentInvoice.Count; i++)
                    {
                        List<DocumentInvoiceDetail> documentInvoiceDetails = _DbContext.DocumentInvoiceDetail.Where(e =>
                        e.IdDocumentInvoice == documentInvoice[i].Id).ToList();

                        for(int j = 0; j < documentInvoiceDetails.Count; j++)
                        {
                            Document document = _DbContext.Document.Where(e =>
                            e.ID == documentInvoiceDetails[j].IdDocument
                            && e.IsDeleted == false).FirstOrDefault();

                            IndividualSample individualSample = _DbContext.IndividualSample.AsNoTracking().Where(e =>
                            e.Id == documentInvoiceDetails[j].IdIndividual).FirstOrDefault();

                            if(document != null && individualSample != null)
                            {
                                ListBorrowByUserId listBorrowByUserId = new ListBorrowByUserId();

                                listBorrowByUserId.fromDate = documentInvoice[i].DateOut;
                                listBorrowByUserId.toDate = documentInvoice[i].DateIn;
                                listBorrowByUserId.dateReality = documentInvoice[i].DateInReality;
                                listBorrowByUserId.IdDocumnent = document.ID;
                                listBorrowByUserId.NameDocument = document.DocName;
                                listBorrowByUserId.IdIndividual = individualSample.Id;
                                listBorrowByUserId.NumIndividual = individualSample.NumIndividual;
                                listBorrowByUserId.Note = documentInvoice[i].Note;

                                listBorrowByUserIds.Add(listBorrowByUserId);
                            }

                        }
                    }

                    customApiListBorrowByUserTypeDetail.listBorrowByUserIds = listBorrowByUserIds;
                }
                
                return customApiListBorrowByUserTypeDetail;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiListUserByUnit> CustomApiListUserByUnit(Guid IdUnit, Guid IdUserType, string fromDate, string toDate)
        {
            try
            {
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();

                List<CustomApiListUserByUnit> customApiListUserByUnits = new List<CustomApiListUserByUnit>();

                List<User> user = _DbContext.User.Where(e =>
                e.UnitId == IdUnit && e.IsDeleted == false
                && e.UserTypeId == IdUserType).ToList();

                for(int i = 0; i < user.Count; i++)
                {
                    List<DocumentInvoice> documentInvoice = null;                
                    if(fromDate == null && toDate == null)
                    {
                        documentInvoice = _DbContext.DocumentInvoice.Where(e =>
                        e.UserId == user[i].Id && e.Status == 0).ToList();
                    }
                    else
                    {
                        FromDate = DateTime.Parse(fromDate, new CultureInfo("en-CA"));
                        ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                        documentInvoice = _DbContext.DocumentInvoice.Where(e =>
                        e.UserId == user[i].Id && e.Status == 0 && e.CreateDate >= FromDate && e.CreateDate <= ToDate).ToList();
                    }

                    for(int j = 0; j < documentInvoice.Count; j++)
                    {
                        List<DocumentInvoiceDetail> documentInvoiceDetails = _DbContext.DocumentInvoiceDetail.Where(e =>
                        e.IdDocumentInvoice == documentInvoice[j].Id).ToList();
                        for(int k = 0; k < documentInvoiceDetails.Count; k++)
                        {
                            Document document = _DbContext.Document.Where(e =>
                            e.ID == documentInvoiceDetails[k].IdDocument
                            && e.IsDeleted == false).FirstOrDefault();

                            IndividualSample individualSample = _DbContext.IndividualSample.AsNoTracking().Where(e =>
                            e.Id == documentInvoiceDetails[k].IdIndividual).FirstOrDefault();

                            if(document != null && individualSample != null)
                            {
                                CustomApiListUserByUnit customApiListUserByUnit = new CustomApiListUserByUnit();

                                customApiListUserByUnit.NameUser = user[i].Fullname;
                                customApiListUserByUnit.fromDate = documentInvoice[j].DateOut;
                                customApiListUserByUnit.toDate = documentInvoice[j].DateIn;
                                customApiListUserByUnit.Note = documentInvoice[j].Note;
                                customApiListUserByUnit.NameDocument = document.DocName;
                                customApiListUserByUnit.NumIndividual = individualSample.NumIndividual;

                                customApiListUserByUnits.Add(customApiListUserByUnit);
                            }

                        }
                    }
                }
                customApiListUserByUnits = customApiListUserByUnits.OrderBy(e => e.NameDocument).ToList();
                return customApiListUserByUnits;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiListBorrowLateByUserType> customApiListBorrowLateByUserTypes(Guid IdUserType, string toDate)
        {
            try
            {
                DateTime ToDate = new DateTime();
                List<CustomApiListBorrowLateByUserType> customApiListBorrowLateByUserTypes = new List<CustomApiListBorrowLateByUserType>();

                List<User> user = _DbContext.User.Where(e =>
                e.UserTypeId == IdUserType && e.IsDeleted == false).ToList();


                for (int i = 0; i < user.Count; i++)
                {

                    List<DocumentInvoice> documentInvoice = null;
                    if (toDate != null)
                    {
                        ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                        documentInvoice = _DbContext.DocumentInvoice.Where(e =>
                        e.UserId == user[i].Id && e.Status == 2 && e.CreateDate <= ToDate).ToList();
                    }
                    else
                    {
                        documentInvoice = _DbContext.DocumentInvoice.Where(e =>
                        e.UserId == user[i].Id && e.Status == 2).ToList();
                    }

                    Unit unit = _DbContext.Unit.Where(e =>
                    e.Id == user[i].UnitId).FirstOrDefault();

                    for(int j = 0; j < documentInvoice.Count; j++)
                    {
                        List<DocumentInvoiceDetail> documentInvoiceDetails = _DbContext.DocumentInvoiceDetail.Where(e =>
                        e.IdDocumentInvoice == documentInvoice[j].Id).ToList();

                        for(int k = 0; k < documentInvoiceDetails.Count; k++)
                        {
                            Document document = _DbContext.Document.Where(e =>
                            e.ID == documentInvoiceDetails[k].IdDocument
                            && e.IsDeleted == false).FirstOrDefault();

                            IndividualSample individualSample = _DbContext.IndividualSample.AsNoTracking().Where(e =>
                            e.Id == documentInvoiceDetails[k].IdIndividual).FirstOrDefault();

                            if(individualSample != null && document != null)
                            {
                                CustomApiListBorrowLateByUserType customApiListBorrowLateByUserType = new CustomApiListBorrowLateByUserType();

                                customApiListBorrowLateByUserType.IdUser = user[i].Id;
                                customApiListBorrowLateByUserType.NameUser = user[i].Fullname;
                                customApiListBorrowLateByUserType.fromDate = documentInvoice[j].DateOut;
                                customApiListBorrowLateByUserType.toDate = documentInvoice[j].DateIn;
                                customApiListBorrowLateByUserType.IdUnit = user[i].UnitId;
                                customApiListBorrowLateByUserType.NameUnit = unit.UnitName;
                                customApiListBorrowLateByUserType.IdIndividual = individualSample.Id;
                                customApiListBorrowLateByUserType.NumIndividual = individualSample.NumIndividual;
                                customApiListBorrowLateByUserType.IdDocument = document.ID;
                                customApiListBorrowLateByUserType.NameDocument = document.DocName;
                                customApiListBorrowLateByUserType.Author = document.Author;
                                customApiListBorrowLateByUserType.InvoiceCode = documentInvoice[j].InvoiceCode;
                                //caculate time late
                                var time = documentInvoice[j].DateInReality - documentInvoice[j].DateIn;
                                int result = (int)Math.Ceiling(time.Value.TotalDays);
                                if (result < 0) result = 0;
                                customApiListBorrowLateByUserType.NumberDayLate = result;

                                customApiListBorrowLateByUserTypes.Add(customApiListBorrowLateByUserType);
                            }

                        }
                    }
                }

                return customApiListBorrowLateByUserTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiNumIndividualLedger> customApiNumIndividualLedgers(string fromDate, string toDate, Guid DocumentType)
        {
            try
            {
                var FromDate = new DateTime();
                var ToDate = new DateTime();

                var customApiNumIndividualLedgers = new List<CustomApiNumIndividualLedger>();

                var document = _DbContext.Document.Where(e => 
                e.IsDeleted == false && e.DocumentTypeId == DocumentType).ToList();

                for(int i = 0; i < document.Count; i++)
                {
                    List<IndividualSample> individualSamples = null;

                    if(fromDate != null && toDate != null)
                    {
                        FromDate = DateTime.Parse(fromDate, new CultureInfo("en-CA"));
                        ToDate = DateTime.Parse(toDate, new CultureInfo("en-CA"));

                        individualSamples = _DbContext.IndividualSample.Where(e =>
                        e.IsDeleted == false && e.IsLostedPhysicalVersion == false && e.IdDocument == document[i].ID
                        && e.CreatedDate >= FromDate && e.CreatedDate <= ToDate).ToList();
                    }
                    else
                    {
                        individualSamples = _DbContext.IndividualSample.Where(e =>
                        e.IsDeleted == false && e.IsLostedPhysicalVersion == false && e.IdDocument == document[i].ID
                        ).ToList();
                    }

                    for(int j = 0; j < individualSamples.Count; j++)
                    {
                        CustomApiNumIndividualLedger customApiNumIndividualLedger = new CustomApiNumIndividualLedger();

                        customApiNumIndividualLedger.IdIndividual = individualSamples[j].Id;
                        customApiNumIndividualLedger.NameIndividual = individualSamples[j].NumIndividual;
                        customApiNumIndividualLedger.DocumentName = document[i].DocName;
                        customApiNumIndividualLedger.Author = document[i].Author;
                        customApiNumIndividualLedger.DateIn = individualSamples[j].CreatedDate;

                        customApiNumIndividualLedgers.Add(customApiNumIndividualLedger);
                    }
                }
                return customApiNumIndividualLedgers;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }        
}
