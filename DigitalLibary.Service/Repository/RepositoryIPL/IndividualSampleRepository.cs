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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class IndividualSampleRepository : IIndividualSampleRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public IndividualSampleRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region METHOD
        //Remove tone mark
        private static readonly string[] VietnameseSigns = new string[]
        {

            "aâăAÂĂeêEÊoôơOÔƠuưUƯiIyY",

            "áàạảã",
            "ấầậẩẫ",
            "ắằặẳẵ",

            "ÁÀẠẢÃ",
            "ẤẦẬẨẪ",
            "ẮẰẶẲẴ",

            "éèẹẻẽ",
            "ếềệểễ",

            "ÉÈẸẺẼ",
            "ẾỀỆỂỄ",

            "óòọỏõ",
            "ốồộổỗ",
            "ớờợởỡ",

            "ÓÒỌỎÕ",
            "ỐỒỘỔỖ",
            "ỚỜỢỞỠ",

            "úùụủũ",
            "ứừựửữ",

            "ÚÙỤỦŨ",
            "ỨỪỰỬỮ",

            "íìịỉĩ",
            "ÍÌỊỈĨ",

            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        private static readonly string[] Consonant = new string[]
        {
                "ch", "gh", "gi", "kh", "ng", "ngh", "nh", "th", "tr", "qu", "b",
                "c", "d", "đ", "g", "h", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "x"
        };
        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }
        public static string CheckTwoConsonantOfSecondLetter(string str)
        {
            String EncryptCode = "";
            // get two char of second letter
            String subStringTwoCharOfSencondLetter = str.Length >= 2 ? str.Substring(0, 2) : "";
            // check two char exits in listString
            Boolean checkSecondLetter = false;
            for (int i = 0; i < Consonant.Length; i++)
            {
                if (subStringTwoCharOfSencondLetter.ToLower() == Consonant[i])
                {
                    EncryptCode += subStringTwoCharOfSencondLetter.ToLower();
                    checkSecondLetter = true;
                    break;
                }
            }
            if (!checkSecondLetter)
            {
                //if not exits in listString auto get index 0
                EncryptCode += char.ToLower(str[0]);
            }
            return EncryptCode;
        }
        public string GetCodeBookNameEncrypt(string bookName)
        {
            try
            {
                //remove tone mark of book name
                bookName = Regex.Replace(bookName, @"\s+", " ");
                string[] bookList = bookName.Trim().Split(' ');
                String text = RemoveSign4VietnameseString(bookList[0]);
                String EncryptCode = "";

                //special case
                var checkBookNameIsSpecial = _DbContext.BookNameEncrypt
                .Where(e => e.SignCode.ToLower() == bookList[0].ToLower()).FirstOrDefault();

                if (checkBookNameIsSpecial != null)
                {
                    EncryptCode += bookName[0].ToString().ToUpper() + checkBookNameIsSpecial.SignNum;
                    if (bookList.Length >= 2) EncryptCode += CheckTwoConsonantOfSecondLetter(bookList[1]);
                    return EncryptCode;
                }

                //get three first of bookname
                String subStringThreeChar = text.Length >= 3 ? text.Substring(0, 3) : "";
                if (subStringThreeChar.ToLower() == "ngh")
                {
                    EncryptCode += subStringThreeChar.ToUpper();

                    //get remain of string book name              
                    String remainOfBookName = text.Substring(3);

                    //search in database
                    var signNum = _DbContext.BookNameEncrypt
                    .Where(e => e.SignCode.ToLower() == remainOfBookName.ToLower()).FirstOrDefault();
                    if (signNum != null)
                    EncryptCode += signNum.SignNum;

                    //check second letter in book name
                    EncryptCode += CheckTwoConsonantOfSecondLetter(bookList[1]);
                }
                else
                {
                    //get two first of bookname
                    String subStringTwoChar = text.Length >= 2 ? text.Substring(0, 2) : "";
                    Boolean check = false;
                    for (int i = 0; i < Consonant.Length; i++)
                    {
                        if (subStringTwoChar.ToLower() == Consonant[i])
                        {
                            check = true;
                            break;
                        }
                    }
                    //if exist with two char
                    if (check)
                    {
                        EncryptCode += subStringTwoChar.ToUpper();

                        //get remain of string book name              
                        String remainOfBookName = text.Substring(2);

                        //search in database
                        var signNum = _DbContext.BookNameEncrypt
                        .Where(e => e.SignCode.ToLower() == remainOfBookName.ToLower()).FirstOrDefault();

                        if (signNum != null)
                            EncryptCode += signNum.SignNum;

                        //check second letter in book name
                        if (bookList.Length >= 2) EncryptCode += CheckTwoConsonantOfSecondLetter(bookList[1]);
                    }
                    else
                    {
                        //if exist with onw char
                        String subStringOneChar = text.Substring(0, 1);
                        for (int i = 0; i < Consonant.Length; i++)
                        {
                            if (subStringOneChar.ToLower() == Consonant[i])
                            {
                                EncryptCode += subStringOneChar.ToUpper();

                                //get remain of string book name              
                                String remainOfBookName = text.Substring(1);

                                //search in database
                                var signNum = _DbContext.BookNameEncrypt
                                .Where(e => e.SignCode.ToLower() == remainOfBookName.ToLower()).FirstOrDefault();

                                if (signNum != null)
                                    EncryptCode += signNum.SignNum;

                                //check second letter in book name
                                if(bookList.Length >= 2) EncryptCode += CheckTwoConsonantOfSecondLetter(bookList[1]);
                                break;
                            }
                        }
                    }
                }

                //if book name is english and special case
                if (EncryptCode == "")
                {
                    String subStringOneChar = bookName.Length >= 1 ? bookName.Substring(0, 1) : "";
                    EncryptCode += subStringOneChar.ToUpper();

                    //search in database with one char
                    var signNum = _DbContext.BookNameEncrypt
                    .Where(e => e.SignCode.ToLower() == subStringOneChar.ToLower()).FirstOrDefault();
                    if (signNum != null)
                    {
                        EncryptCode += signNum.SignNum + bookName[1];  
                    }
                    else
                    {
                        String subStringTwoChar = bookName.Length >= 2 ? bookName[1].ToString() : "";

                        signNum = _DbContext.BookNameEncrypt
                        .Where(e => e.SignCode.ToLower() == subStringTwoChar.ToLower()).FirstOrDefault();

                        if (signNum != null)
                        {
                            EncryptCode += signNum.SignNum + bookName[2];
                        }
                        else
                        {
                            signNum = _DbContext.BookNameEncrypt
                            .Where(e => e.SignCode.ToLower() == subStringThreeChar.ToLower()).FirstOrDefault();

                            if (signNum != null)
                            {
                                EncryptCode += signNum.SignNum + bookName[4];
                            }
                            else
                            {
                                String subStringFourChar = bookName.Length >= 4 ? bookName[4].ToString() : "";

                                signNum = _DbContext.BookNameEncrypt
                                .Where(e => e.SignCode.ToLower() == subStringFourChar.ToLower()).FirstOrDefault();

                                if (signNum != null)
                                {
                                    EncryptCode += signNum.SignNum + bookName[5];
                                }
                            }
                        }
                    }
                }

                return EncryptCode;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region FUNCTION COMMON
        public CustomApiSpineBook SpineBookByBarcode(string barcode)
        {
            try
            {
                CustomApiSpineBook customApiSpineBook = new CustomApiSpineBook();
                IndividualSample individualSample = _DbContext.IndividualSample.AsNoTracking().Where(e =>
                e.Barcode == barcode && e.IsDeleted == false && e.IsLostedPhysicalVersion == false
                && e.Status == 1).FirstOrDefault();

                if (individualSample != null)
                {
                    Document document = _DbContext.Document.AsNoTracking().Where(e =>
                    e.ID == individualSample.IdDocument && e.IsDeleted == false).FirstOrDefault();
                    if (document != null)
                    {
                        DocumentType documentType = _DbContext.DocumentType.Where(e =>
                        e.Id == document.DocumentTypeId).FirstOrDefault();

                        customApiSpineBook.IdDocument = document.ID;
                        customApiSpineBook.IdIndividual = individualSample.Id;
                        customApiSpineBook.NumIndividual = individualSample.NumIndividual;
                        customApiSpineBook.NameCategorySign = documentType.DocTypeName;
                        customApiSpineBook.Barcode = individualSample.Barcode;

                    }
                }
                return customApiSpineBook;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Boolean CheckExitNumberIndividualByNumIndividual(string NumIndividual, Guid IdIndividual)
        {
            try
            {
                var individualSample = new IndividualSample();
                individualSample = _DbContext.IndividualSample.AsNoTracking()
                .Where(e => e.NumIndividual == NumIndividual && e.Id != IdIndividual && e.IsDeleted == false).FirstOrDefault();

                if (individualSample == null)
                {
                    return true;
                }
                else return false;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public Boolean CheckExitNumberIndividual(string numberCode, Guid IdDocument, Guid IdDocumentType)
        {
            try
            {
                IndividualSample individualSample = new IndividualSample();
                individualSample = _DbContext.IndividualSample.AsNoTracking()
                .Where(e => e.NumIndividual == numberCode
                && e.IdDocument == IdDocument
                && e.DocumentTypeId == IdDocumentType && e.IsDeleted == false).FirstOrDefault();

                if (individualSample == null)
                {
                    return true;
                }
                else return false;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public Boolean CheckExitDocumnentAndDocumentType(Guid IdDocument, Guid IdDocumentType)
        {
            try
            {
                List<IndividualSample> individualSamples = _DbContext.IndividualSample.AsNoTracking()
                .Where(e => e.DocumentTypeId == IdDocumentType
                 && e.IsDeleted == false && e.IsLostedPhysicalVersion == false).ToList();

                for(int i = 0; i < individualSamples.Count; i++)
                {
                    if(individualSamples[i].IdDocument == IdDocument)
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
        public List<CustomApiSpineBookByGroup> customApiSpineBook(String ListIdIndividual)
        {
            try
            {
                var customApiSpineBookByGroups = new List<CustomApiSpineBookByGroup>();
                if (ListIdIndividual != "null")
                {
                    var listIndividualByString = ListIdIndividual.Split(',');
                    var customApiSpineBooks = new List<CustomApiSpineBook>();
                    for (int i = 0; i < listIndividualByString.Length; i++)
                    {
                        Guid IdIndividual = new Guid(listIndividualByString[i]);
                        var result = _DbContext.GetDynamicResult("exec [dbo].[Sp_GetSpineBooks] @IdIndividualSample", new SqlParameter("@IdIndividualSample", IdIndividual));

                        for (int j = 0; j < result.Count(); j++)
                        {
                            var str1 = result.ElementAt(j);
                            var jsonString = JsonConvert.SerializeObject(str1);
                            var resultObject = JsonConvert.DeserializeObject<CustomApiSpineBook>(jsonString);

                            customApiSpineBooks.Add(resultObject);
                        }
                    }

                    //group by idDocument
                    List<CustomApiSpineBookByGroup> groupedList = customApiSpineBooks.GroupBy(i => i.IdDocument)
                    .Select(j => new CustomApiSpineBookByGroup()
                    {
                        Title = j.First().DocName,

                        ListSpine = j.Select(f => new CustomApiSpineBook()
                        {
                            IdDocument = f.IdDocument,
                            IdIndividual = f.IdIndividual,
                            NametypeBook = f.NametypeBook,
                            NameCategorySign = f.NameCategorySign,
                            Barcode = f.Barcode,
                            NumIndividual = f.NumIndividual,
                            EncryptCode = f.EncryptCode,
                            DocName = f.DocName,

                        }).ToList()
                    })
                    .ToList();

                    return groupedList;
                }
                return customApiSpineBookByGroups;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<CustomApiSpineBook> customApiSpineBook(Guid IdDocument, Guid IdIndividual, String ListIdIndividual)
        {
            try
            {
                var customApiSpineBooks = new List<CustomApiSpineBook>();
                if(IdDocument != Guid.Empty && ListIdIndividual != "null" && IdIndividual == Guid.Empty)
                {
                    var individualSamples = ListIdIndividual.Split(',')
                    .Select(id => new Guid(id))
                    .Select(id => _DbContext.IndividualSample.AsNoTracking().FirstOrDefault(e => e.Id == id && e.IsDeleted == false))
                    .ToList();

                    var datas = (from i in individualSamples
                                 join d in _DbContext.Document on i.IdDocument equals d.ID
                                 join dt in _DbContext.DocumentType on d.DocumentTypeId equals dt.Id
                                 join ct in _DbContext.CategorySign_V1 on d.IdCategorySign_V1 equals ct.Id
                                 where d.IsDeleted == false
                                 select new CustomApiSpineBook
                                 {
                                     IdDocument = d.ID,
                                     IdIndividual = i.Id,
                                     NumIndividual = i.NumIndividual,
                                     NametypeBook = dt.DocTypeName,
                                     NameCategorySign = ct.SignCode,
                                     Barcode = i.Barcode,
                                     EncryptCode = GetCodeBookNameEncrypt(d.DocName)
                                 }).ToList();

                    return new List<CustomApiSpineBook>(datas);
                }
                if (IdDocument == Guid.Empty && IdIndividual != Guid.Empty && ListIdIndividual == "null")
                {
                    var datas = (from i in _DbContext.IndividualSample
                                 join d in _DbContext.Document on i.IdDocument equals d.ID
                                 join dt in _DbContext.DocumentType on d.DocumentTypeId equals dt.Id
                                 join ct in _DbContext.CategorySign_V1 on d.IdCategorySign_V1 equals ct.Id
                                 where d.IsDeleted == false && i.Id == IdIndividual
                                 select new CustomApiSpineBook
                                 {
                                     IdDocument = d.ID,
                                     IdIndividual = i.Id,
                                     NumIndividual = i.NumIndividual,
                                     NametypeBook = dt.DocTypeName,
                                     NameCategorySign = ct.SignCode,
                                     Barcode = i.Barcode,
                                     EncryptCode = GetCodeBookNameEncrypt(d.DocName)
                                 }).ToList();

                    return new List<CustomApiSpineBook>(datas);
                }
                if (IdDocument == Guid.Empty && ListIdIndividual != "null" && IdIndividual == Guid.Empty)
                {
                    var individualSamples = ListIdIndividual.Split(',')
                    .Select(id => new Guid(id))
                    .Select(id => _DbContext.IndividualSample.AsNoTracking().FirstOrDefault(e => e.Id == id && e.IsDeleted == false))
                    .OrderBy(e => e.NumIndividual)
                    .ToList();

                    var datas = (from i in _DbContext.IndividualSample
                                 join d in _DbContext.Document on i.IdDocument equals d.ID
                                 join dt in _DbContext.DocumentType on d.DocumentTypeId equals dt.Id
                                 join ct in _DbContext.CategorySign_V1 on d.IdCategorySign_V1 equals ct.Id
                                 where d.IsDeleted == false && i.Id == IdIndividual
                                 select new CustomApiSpineBook
                                 {
                                     IdDocument = d.ID,
                                     IdIndividual = i.Id,
                                     NumIndividual = i.NumIndividual,
                                     NametypeBook = dt.DocTypeName,
                                     NameCategorySign = ct.SignCode,
                                     Barcode = i.Barcode,
                                     EncryptCode = GetCodeBookNameEncrypt(d.DocName)
                                 }).ToList();

                    return new List<CustomApiSpineBook>(datas);
                }
                return customApiSpineBooks;
            }
            catch(Exception)
            {
                throw;
            }
        }
        public CustomApiIndividual CustomApiIndividual(Guid IdDocument)
        {
            try
            {
                CustomApiIndividual customApiIndividuals = new CustomApiIndividual();
                List<IndividualSample> sample = _DbContext.IndividualSample.AsNoTracking()
                .Where(x => x.StockId != Guid.Empty && x.IsDeleted == false && x.IsLostedPhysicalVersion == false
                && x.IdDocument == IdDocument)
                .ToList();
                
                List<stock> stockList = new List<stock>();
                HashSet<Guid?> set = new HashSet<Guid?>();
                for (int i = 0; i < sample.Count; i++)
                {
                    set.Add(sample[i].StockId);
                }

                foreach (var element in set)
                {
                    //get entity  document stock
                    DocumentStock documentStock = _DbContext.DocumentStock.AsNoTracking()
                    .Where(e => e.Id == element.Value && e.IsDeleted == false
                    ).FirstOrDefault();

                    stock stock = new stock();
                    if(documentStock != null)
                    {
                        //new class stock 
                        stock.idStock = element.Value;
                        stock.individualName = documentStock.StockName;
                    }

                    stockList.Add(stock);
                    customApiIndividuals.stocks = stockList;
                }

                return customApiIndividuals;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public HashSet<string> GetIndividualByDate(Guid IdDocument)
        {
            try
            {
                List<IndividualSample> individualSample = _DbContext.IndividualSample.Where(e =>
                e.IsDeleted == false && e.IsLostedPhysicalVersion == false && e.IdDocument == IdDocument
                ).ToList();

                HashSet<string> set = new HashSet<string>();
                for (int i = 0; i < individualSample.Count; i++)
                {
                    string date = individualSample[i].CreatedDate?.ToString("d");
                    set.Add(date);
                }
                return set;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int getNumIndividualMaxByIdCategorySign(Guid IdCategorySign)
        {
            try
            {
                int max = 0;

                CategorySign categorySigns = _DbContext.CategorySign.AsNoTracking().Where(e =>
                e.Id == IdCategorySign).FirstOrDefault();

                List<IndividualSample> sampleList = _DbContext.IndividualSample.
                Where(x => x.NumIndividual.Substring(0, categorySigns.SignCode.Length) == categorySigns.SignCode && x.IsDeleted == false).ToList();

                for (int i = 0; i < sampleList.Count; i++)
                {
                    if(new Guid(sampleList[i].NumIndividual.Split('/')[1]) == categorySigns.Id)
                    {
                        string numberIndividual = sampleList[i].NumIndividual.Split('/')[0];
                        int lgtOfSignCode = categorySigns.SignCode.Length;
                        int indexOfSign = int.Parse(numberIndividual.Substring(lgtOfSignCode));

                        if (max < indexOfSign)
                        {
                            max = indexOfSign;
                        }
                    }
                }
                return max;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int getNumIndividualMaxByInsertReceipt(Guid IdCategorySign)
        {
            try
            {
                int max = 0;

                CategorySign categorySigns = _DbContext.CategorySign.AsNoTracking().Where(e =>
                e.Id == IdCategorySign).FirstOrDefault();

                List<IndividualSample> sampleList = _DbContext.IndividualSample.
                Where(x => x.NumIndividual.Contains(IdCategorySign.ToString())).ToList();

                for (int i = 0; i < sampleList.Count; i++)
                {
                    string numberIndividual = sampleList[i].NumIndividual.Split('/')[0];
                    int lgtOfSignCode = categorySigns.SignCode.Length;
                    int indexOfSign = int.Parse(numberIndividual.Substring(lgtOfSignCode));

                    if (max < indexOfSign)
                    {
                        max = indexOfSign;
                    }
                }
                return max;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int getNumIndividualMax(Guid IdCategorySign, Guid IdDocument, Guid IdDocumentType)
        {
            try
            {
                int max = 0;
                
                CategorySign categorySigns = _DbContext.CategorySign.AsNoTracking().Where(e =>
                e.Id == IdCategorySign).FirstOrDefault();

                List<IndividualSample> sampleList = _DbContext.IndividualSample.
                Where(x => x.NumIndividual.Contains(IdCategorySign.ToString())
                && x.IdDocument == IdDocument && x.DocumentTypeId == IdDocumentType).ToList();

                for (int i = 0; i < sampleList.Count; i++)
                {
                    string numberIndividual = sampleList[i].NumIndividual.Split('/')[0];
                    int lgtOfSignCode = categorySigns.SignCode.Length;
                    int indexOfSign = int.Parse(numberIndividual.Substring(lgtOfSignCode));

                    if (max < indexOfSign)
                    {
                        max = indexOfSign;
                    }
                }
                return max;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response ChangeStatus(Guid Id, int status)
        {
            Response response = new Response();
            try
            {
                IndividualSample sample = _DbContext.IndividualSample.AsNoTracking()
                    .Where(x => x.Id == Id).FirstOrDefault();

                if (sample != null)
                {
                    sample.Status = status;
                    _DbContext.IndividualSample.Update(sample);
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
        public Response ReturnBook(Guid IdDocument,Guid IdIndividual, bool isLost)
        {
            Response response = new Response();
            try
            {
                IndividualSample sample = _DbContext.IndividualSample.AsNoTracking()
                .Where(x => x.Id == IdIndividual).FirstOrDefault();

                if (sample != null)
                {
                    if(isLost)
                    {
                        sample.Status = 3;
                    }
                    else
                    {
                        sample.Status = 1;
                    }
                    sample.IsLostedPhysicalVersion = isLost;
                    _DbContext.IndividualSample.Update(sample);


                    if(!isLost)
                    {
                        // change status invoice to 2
                        List<DocumentInvoice> documentInvoice = _DbContext.DocumentInvoice.Where(x => x.Status == 4).ToList();

                        for (int i = 0; i < documentInvoice.Count; i++)
                        {
                            List<DocumentInvoiceDetail> documentInvoiceDetails = _DbContext.DocumentInvoiceDetail
                            .Where(x => x.IdDocumentInvoice == documentInvoice[i].Id).ToList();

                            bool check = true;
                            for (int j = 0; j < documentInvoiceDetails.Count; j++)
                            {
                                IndividualSample individualSample = _DbContext.IndividualSample
                                .Where(e => e.Id == documentInvoiceDetails[j].IdIndividual).FirstOrDefault();

                                if (individualSample != null && individualSample.Status != 1)
                                {
                                    check = false;
                                    break;
                                }
                            }
                            if (check)
                            {
                                documentInvoice[i].Status = 2;
                                documentInvoice[i].DateInReality = DateTime.Now;
                                _DbContext.DocumentInvoice.Update(documentInvoice[i]);
                            }
                        }
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
        public Response ChangeIndividualToAvailable(Guid IdDocumentInvoice)
        {
            try
            {
                Response response = new Response();

                List<DocumentInvoiceDetail> documentInvoiceDetails = _DbContext.DocumentInvoiceDetail
                .Where(invoice => invoice.IdDocumentInvoice == IdDocumentInvoice).ToList();

                for (int i = 0; i < documentInvoiceDetails.Count; i++)
                {
                    IndividualSample individualSample = _DbContext.IndividualSample
                    .Where(e => e.Id == documentInvoiceDetails[i].IdIndividual).FirstOrDefault();

                    if (individualSample != null && individualSample.IsLostedPhysicalVersion == false)
                    {
                        individualSample.Status = 1;
                        _DbContext.IndividualSample.Update(individualSample);
                    }
                }
                _DbContext.SaveChanges();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region CRUD TABLE INDIVIDUALSAMPLE
        public List<IndividualSampleDto> getIndividualSampleIsLost(int pageNumber, int pageSize)
        {
            try
            {
                List<IndividualSample> individualSample = new List<IndividualSample>();
                if (pageNumber == 0 && pageSize == 0)
                {
                    individualSample = _DbContext.IndividualSample
                    .AsNoTracking()
                    .Where(x => x.IsLostedPhysicalVersion == true && x.IsDeleted == false)
                    .ToList();
                }
                else
                {
                    individualSample = _DbContext.IndividualSample
                    .AsNoTracking()
                    .Where(x => x.IsLostedPhysicalVersion == true && x.IsDeleted == false)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }


                List<IndividualSampleDto> individualSampleDto = new List<IndividualSampleDto>();
                individualSampleDto = _mapper.Map<List<IndividualSampleDto>>(individualSample);
                return individualSampleDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response DeleteIndividualSample(Guid Id)
        {
            using var context = _DbContext;
            using var transaction = context.Database.BeginTransaction();

            try
            {
                 var individualSample = _DbContext.IndividualSample.AsNoTracking()
                .Where(x => x.Id == Id).FirstOrDefault();

                if(individualSample == null)
                {
                    return new Response() { Success = false, Fail = true, Message = "Không tìm thấy mẫu đăng kí !" };
                }

                #region check ID Individual if status in table documentInvoice = 1
                var documentInvoiceDetail = _DbContext.DocumentInvoiceDetail.Where(x => x.IdIndividual == Id).FirstOrDefault();
                if(documentInvoiceDetail != null)
                {
                    var documentInvoice = _DbContext.DocumentInvoice.Where(x => x.Id == documentInvoiceDetail.IdDocumentInvoice).FirstOrDefault();
/*                    if(documentInvoice.Status != 1)
                    {
                        return new Response() { Success = false, Fail = true, Message = "Mã cá biệt hiện chưa được trả không thể xóa !" };
                    }*/

                    _DbContext.DocumentInvoiceDetail.Remove(documentInvoiceDetail);
                    context.SaveChanges();

                    var checkdocumentInvoiceExits = _DbContext.DocumentInvoiceDetail.Where(x => x.IdDocumentInvoice == documentInvoiceDetail.IdDocumentInvoice).FirstOrDefault();
                    if(checkdocumentInvoiceExits == null)
                    {
                        _DbContext.DocumentInvoice.Remove(documentInvoice);
                        context.SaveChanges();
                    }
                }
                #endregion

                //save record to individualsampledeleted
                InsertIndividualSampleDelete(individualSample);
                context.SaveChanges();

                _DbContext.IndividualSample.Remove(individualSample);
                context.SaveChanges();

                transaction.Commit();
                return new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Xóa thành công !"
                };

            }
            catch (Exception)
            {
                transaction.Rollback();
                return new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Xóa không thành công !"
                };
            }
        }
        public Response DeleteIndividualSampleByList(List<Guid> IdIdIndividual)
        {
            using var context = _DbContext;
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var individualById = _DbContext.IndividualSample.Where(ar => IdIdIndividual.Contains(ar.Id)).ToList();
                if (!individualById.Any())
                {
                    return new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Đã có Id mã cá biệt không tồn tại !"
                    };
                }
                if (individualById.Any(x => x.Status == 0))
                {
                    return new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Đã có mã cá biệt đang được mượn không thể xóa !"
                    };
                }
                if (individualById.Any(x => x.Status == 3))
                {
                    return new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Đã có mã cá biệt đã mất không thể xóa !"
                    };
                }


                var documentInvoiceDetails = _DbContext.DocumentInvoiceDetail.Where(x => IdIdIndividual.Contains(x.IdIndividual));
                var idsToRemove = documentInvoiceDetails.Select(x => x.IdDocumentInvoice).ToList();

                _DbContext.DocumentInvoiceDetail.RemoveRange(documentInvoiceDetails);
                context.SaveChanges();

                _DbContext.DocumentInvoice.RemoveRange(_DbContext.DocumentInvoice.Where(x => idsToRemove.Contains(x.Id) && !_DbContext.DocumentInvoiceDetail.Any(y => y.IdDocumentInvoice == x.Id)));
                context.SaveChanges();

                var listOfIds = String.Join(',', IdIdIndividual.Select(id => $"'{id}'").ToList());
                var sql = $@"DELETE IndividualSample WHERE Id in ({listOfIds})";

                _DbContext.Database.ExecuteSqlRaw(sql);
                context.SaveChanges();
                
                transaction.Commit();
                return new Response() { Success = true, Fail = false, Message = "Xóa thành công !" };

            }
            catch
            {
                transaction.Rollback();
                return new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Xóa không thành công !"
                };
            }
        }
        public Response InsertIndividualSampleDelete(IndividualSample individualSample)
        {
            Response response = new Response();
            try
            {
                IndividualSampleDeleted individualSampleDeleted = new IndividualSampleDeleted()
                {
                    Id = individualSample.Id,
                    IdDocument = individualSample.IdDocument,
                    NumIndividual = individualSample.NumIndividual,
                    Barcode = individualSample.Barcode,
                    StockId = individualSample.StockId,
                    IsLostedPhysicalVersion = individualSample.IsLostedPhysicalVersion,
                    IsDeleted = individualSample.IsDeleted,
                    Status = individualSample.Status,
                    CreatedBy = individualSample.CreatedBy,
                    CreatedDate = individualSample.CreatedDate,
                };
                _DbContext.IndividualSampleDeleted.Add(individualSampleDeleted);
                _DbContext.SaveChanges();

                response = new Response()
                {
                    Success = true,
                    Fail = false,
                    Message = "Thêm thành công !"
                };
                return response;
            }
            catch (Exception)
            {
                response = new Response()
                {
                    Success = false,
                    Fail = true,
                    Message = "Thêm không thành công !"
                };
                return response;
            }
        }
        public List<IndividualSampleDto> getIndividualSample(int pageNumber, int pageSize)
        {
            try
            {
                List<IndividualSample> individualSample = new List<IndividualSample>();
                if (pageNumber == 0 && pageSize == 0)
                {
                    individualSample = _DbContext.IndividualSample
                    .AsNoTracking()
                    .Where(x => x.Id != Guid.Empty && x.IsDeleted == false)
                    .ToList();
                }
                else
                {
                    individualSample = _DbContext.IndividualSample
                    .AsNoTracking()
                    .Where(x => x.Id != Guid.Empty && x.IsDeleted == false)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }


                List<IndividualSampleDto> individualSampleDto = new List<IndividualSampleDto>();
                individualSampleDto = _mapper.Map<List<IndividualSampleDto>>(individualSample);
                return individualSampleDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IndividualSampleDto getIndividualSampleById(Guid Id)
        {
            try
            {
                IndividualSample individualSample = new IndividualSample();
                individualSample = _DbContext.IndividualSample.AsNoTracking()
                    .Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefault();

                IndividualSampleDto individualSampleDto = new IndividualSampleDto();
                individualSampleDto = _mapper.Map<IndividualSampleDto>(individualSample);
                return individualSampleDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IndividualSampleDto getIndividualSampleLostById(Guid Id)
        {
            try
            {
                IndividualSample individualSample = new IndividualSample();
                individualSample = _DbContext.IndividualSample.AsNoTracking()
                    .Where(x => x.Id == Id && x.IsDeleted == false &&
                x.IsLostedPhysicalVersion == true).FirstOrDefault();

                IndividualSampleDto individualSampleDto = new IndividualSampleDto();
                individualSampleDto = _mapper.Map<IndividualSampleDto>(individualSample);
                return individualSampleDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response InsertIndividualSample(IndividualSampleDto individualSampleDto)
        {
            Response response = new Response();
            try
            {
                IndividualSample individualSample = new IndividualSample();
                individualSample = _mapper.Map<IndividualSample>(individualSampleDto);

                Document document = _DbContext.Document.Where(e => e.ID == individualSampleDto.IdDocument).FirstOrDefault();

                if(document != null)
                {
                    document.IsHavePhysicalVersion = true;
                    _DbContext.Document.Update(document);
                }

                _DbContext.IndividualSample.Add(individualSample);
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
        public Response UpdateIndividualSample(IndividualSampleDto individualSampleDto)
        {
            var response = new Response();
            try
            {
                var individualSample = new IndividualSample();
                individualSample = _DbContext.IndividualSample.AsNoTracking()
                .Where(e => e.Id == individualSampleDto.Id).FirstOrDefault();

                if (individualSample != null)
                {
                    individualSample.IdDocument = individualSampleDto.IdDocument;
                    individualSample.StockId = individualSampleDto.StockId;
                    individualSample.IsLostedPhysicalVersion = individualSampleDto.IsLostedPhysicalVersion;
                    individualSample.NumIndividual = String.IsNullOrEmpty(individualSampleDto.NumIndividual) ? individualSample.NumIndividual : individualSampleDto.NumIndividual;
                    individualSample.Barcode = String.IsNullOrEmpty(individualSampleDto.Barcode) ? individualSample.Barcode : individualSampleDto.Barcode;
                    individualSample.CreatedBy = individualSampleDto.CreatedBy.HasValue ? individualSampleDto.CreatedBy : individualSample.CreatedBy;
                    individualSample.CreatedDate = individualSampleDto.CreatedDate.HasValue ? individualSampleDto.CreatedDate : individualSample.CreatedDate;

                    _DbContext.IndividualSample.Update(individualSample);
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
        public Tuple<bool, string> CheckIdIndividualExitsInDocumentInvoice(List<Guid> IdIdIndividual)
        {
            var documentInvoiceDetails = _DbContext.DocumentInvoiceDetail.Where(x => IdIdIndividual.Contains(x.IdIndividual)).ToList();

            if (documentInvoiceDetails.Any())
            {
                var numIndividual = (from a in _DbContext.IndividualSample
                                     join b in _DbContext.DocumentInvoiceDetail on a.Id equals b.IdIndividual
                                     where IdIdIndividual.Contains(a.Id)
                                     select a.NumIndividual).Distinct().ToList();

                List<string> subStrings = numIndividual.Select(x => x.Substring(0, x.IndexOf('/'))).ToList();

                string joinedString = string.Join("/", subStrings);
                return new Tuple<bool, string>(true, joinedString);
            }
            return new Tuple<bool, string>(false, "");
        }
        #endregion
    }
}
