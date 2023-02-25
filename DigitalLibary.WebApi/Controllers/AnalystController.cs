using AutoMapper;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DigitalLibary.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalystController : Controller
    {
        #region Variables
        private readonly IAnalystRepository _analystRepository;
        private readonly AppSettingModel _appSettingModel;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IDocumentInvoiceRepository _documentInvoiceRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly ISchoolYearRepository _schoolYearRepository;
        private readonly IContactAndIntroductionRepository _contactAndIntroductionRepository;
        #endregion

        #region Contructor
        public AnalystController(IAnalystRepository analystRepository,
        IOptionsMonitor<AppSettingModel> optionsMonitor,
        IMapper mapper,
        ISchoolYearRepository schoolYearRepository,
        IDocumentTypeRepository documentTypeRepository,
        IContactAndIntroductionRepository contactAndIntroductionRepository,
        IDocumentInvoiceRepository documentInvoiceRepository,
        JwtService jwtService, IUserRepository userRepository)
        {
            _appSettingModel = optionsMonitor.CurrentValue;
            _mapper = mapper;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _analystRepository = analystRepository;
            _documentInvoiceRepository = documentInvoiceRepository;
            _documentTypeRepository = documentTypeRepository;
            _schoolYearRepository = schoolYearRepository;
            _contactAndIntroductionRepository = contactAndIntroductionRepository;
        }
        #endregion

        #region Method
        // GET: api/Analyst/AnalystBookByGroupDocumentType
        [HttpGet]
        [Route("AnalystBookByGroupDocumentType")]
        public List<AnalystBookByGroupType> AnalystBookByGroupDocumentType(Guid IdDocumentType)
        {
            try
            {
                return _analystRepository.AnalystBookByGroupTypes(IdDocumentType);
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/AnalystListBorrowByUserTypeAndUnit
        [HttpGet]
        [Route("AnalystListBorrowByUserTypeAndUnit")]
        public List<CustomApiListUserByUnit> AnalystListBorrowByUserTypeAndUnit(Guid IdUnit, Guid IdUserType, string fromDate, string toDate)
        {
            try
            {
                List<CustomApiListUserByUnit> countUser = new List<CustomApiListUserByUnit>();
                countUser = _analystRepository.CustomApiListUserByUnit(IdUnit, IdUserType, fromDate, toDate);

                return countUser;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetFileExcelAnalystListBorrowLedgerIndividual
        [HttpGet]
        [Route("GetFileExcelAnalystListBorrowLedgerIndividual")]
        public IActionResult GetFileExcelAnalystListBorrowLedgerIndividual(string fromDate, string toDate, Guid DocumentType)
        {
            try
            {
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "SoDangKiCaBiet.xlsx");
                FileInfo fi = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];

                    namedWorksheet.Cells["A50:U2000"].Clear();

                    List<CustomApiNumIndividualLedger> listLedger = new List<CustomApiNumIndividualLedger>();
                    listLedger = _analystRepository.customApiNumIndividualLedgers(fromDate, toDate, DocumentType);

                    namedWorksheet.Cells[$"G2:O2"].Merge = true;
                    namedWorksheet.Cells[2, 7].Style.Font.Size = 13;
                    namedWorksheet.Cells[2, 7].Style.Font.Name = "Times New Roman";
                    namedWorksheet.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[2, 7].Style.Font.Bold = true;

                    List<ContactAndIntroductionDto> ruleDto = new List<ContactAndIntroductionDto>();
                    ruleDto = _contactAndIntroductionRepository.getAllRule(1, 1, 2);
                    if (ruleDto != null)
                    {
                        namedWorksheet.Cells[2, 7].Value = $"THƯ VIỆN {ruleDto[0].col10.ToUpper()}";
                    }
                    else
                    {
                        namedWorksheet.Cells[2, 7].Value = $"THƯ VIỆN TRƯỜNG ...";
                    }

                    namedWorksheet.Cells[$"E19:Q19"].Merge = true;
                    namedWorksheet.Cells[19, 5].Style.Font.Size = 20;
                    namedWorksheet.Cells[19, 5].Style.Font.Name = "Times New Roman";
                    namedWorksheet.Cells[19, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[19, 5].Style.Font.Bold = true;

                    DocumentTypeDto documentTypeDto = new DocumentTypeDto();
                    documentTypeDto = _documentTypeRepository.GetAllDocumentTypeById(DocumentType);

                    if (documentTypeDto != null)
                    {
                        namedWorksheet.Cells[19, 5].Value = $"Loại sách: {documentTypeDto.DocTypeName}";
                    }
                    else
                    {
                        namedWorksheet.Cells[19, 5].Value = $"Loại sách: ...";
                    }

                    namedWorksheet.Cells[$"E20:Q20"].Merge = true;
                    namedWorksheet.Cells[20, 5].Style.Font.Size = 20;
                    namedWorksheet.Cells[20, 5].Style.Font.Name = "Times New Roman";
                    namedWorksheet.Cells[20, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[20, 5].Style.Font.Bold = true;

                    SchoolYearDto schoolYearDto = _schoolYearRepository.getSchoolYear();
                    if (schoolYearDto != null)
                    {
                        namedWorksheet.Cells[20, 5].Value = $"Năm học: {schoolYearDto.FromYear.Value.ToString("yyyy")} - {schoolYearDto.ToYear.Value.ToString("yyyy")}";
                    }
                    else
                    {
                        namedWorksheet.Cells[20, 5].Value = $"Năm học: {DateTime.Now.Year} - {DateTime.Now.Year + 1}";
                    }

                    int numberRows = 50;
                    for (int i = 0; i < listLedger.Count; i++)
                    {
                        namedWorksheet.Cells[$"A{numberRows}:B{numberRows}"].Merge = true;
                        namedWorksheet.Cells[$"A{numberRows}:B{numberRows}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"A{numberRows}:B{numberRows}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"A{numberRows}:B{numberRows}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"A{numberRows}:B{numberRows}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        namedWorksheet.Cells[$"C{numberRows}:F{numberRows}"].Merge = true;
                        namedWorksheet.Cells[$"C{numberRows}:F{numberRows}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"C{numberRows}:F{numberRows}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"C{numberRows}:F{numberRows}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"C{numberRows}:F{numberRows}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        namedWorksheet.Cells[$"G{numberRows}:U{numberRows}"].Merge = true;
                        namedWorksheet.Cells[$"G{numberRows}:U{numberRows}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"G{numberRows}:U{numberRows}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"G{numberRows}:U{numberRows}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[$"G{numberRows}:U{numberRows}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        namedWorksheet.Row(numberRows).Height = 40;

                        namedWorksheet.Cells[numberRows, 1].Style.Font.Size = 14;
                        namedWorksheet.Cells[numberRows, 1].Style.Font.Name = "Times New Roman";

                        namedWorksheet.Cells[numberRows, 3].Style.Font.Size = 14;
                        namedWorksheet.Cells[numberRows, 3].Style.Font.Name = "Times New Roman";

                        namedWorksheet.Cells[numberRows, 7].Style.Font.Size = 14;
                        namedWorksheet.Cells[numberRows, 7].Style.Font.Name = "Times New Roman";

                        namedWorksheet.Cells[numberRows, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        namedWorksheet.Cells[numberRows, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        namedWorksheet.Cells[numberRows, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                        namedWorksheet.Cells[numberRows, 1].Value = listLedger[i].DateIn?.ToString("dd/MM/yyyy");
                        namedWorksheet.Cells[numberRows, 3].Value = listLedger[i].NameIndividual.Split('/')[0];
                        if (listLedger[i].Author != null)
                        {
                            namedWorksheet.Cells[numberRows, 7].Value = listLedger[i].Author + "-" + listLedger[i].DocumentName;
                        }
                        else
                        {
                            namedWorksheet.Cells[numberRows, 7].Value = listLedger[i].DocumentName;
                        }
                        numberRows++;
                    }

                    namedWorksheet.Cells[$"P{numberRows + 5}:U{numberRows + 5}"].Merge = true;
                    namedWorksheet.Cells[$"P{numberRows + 6}:U{numberRows + 6}"].Merge = true;

                    namedWorksheet.Cells[numberRows + 5, 16].Style.Font.Size = 14;
                    namedWorksheet.Cells[numberRows + 5, 16].Style.Font.Name = "Times New Roman";

                    namedWorksheet.Cells[numberRows + 6, 16].Style.Font.Size = 14;
                    namedWorksheet.Cells[numberRows + 6, 16].Style.Font.Name = "Times New Roman";

                    namedWorksheet.Cells[numberRows + 5, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[numberRows + 6, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    namedWorksheet.Cells[numberRows + 5, 16].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    namedWorksheet.Cells[numberRows + 6, 16].Value = "Cán bộ thư viện";
                    namedWorksheet.Cells[numberRows + 6, 16].Style.Font.Bold = true;

                    //overwrite to file old
                    FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "MauBaoCaoNhieu.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetLedgerIndividual
        [HttpGet]
        [Route("GetLedgerIndividual")]
        public List<CustomApiNumIndividualLedger> GetLedgerIndividual(string fromDate, string toDate, Guid DocumentType)
        {
            try
            {
                var listLedger = new List<CustomApiNumIndividualLedger>();
                listLedger = _analystRepository.customApiNumIndividualLedgers(fromDate, toDate, DocumentType);
                return listLedger;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetFileExcelAnalystListBorrowLateByUserType
        [HttpGet]
        [Route("GetFileExcelAnalystListBorrowLateByUserType")]
        public IActionResult GetFileExcelAnalystListBorrowLateByUserType(Guid IdUserType, string toDate)
        {
            try
            {
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "DanhSachMuonChuaTra.xlsx");
                FileInfo fi = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];
                    namedWorksheet.Cells["A5:J2000"].Clear();

                    List<CustomApiListBorrowLateByUserType> countUser = new List<CustomApiListBorrowLateByUserType>();
                    countUser = _analystRepository.customApiListBorrowLateByUserTypes(IdUserType, toDate);

                    var type = _userRepository.getUserType(IdUserType);

                    namedWorksheet.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[$"C3:E3"].Merge = true;

                    namedWorksheet.Cells["C3:E3"].Value = "Đến ngày " + toDate;

                    namedWorksheet.Cells[$"A5:J5"].Merge = true;
                    namedWorksheet.Cells[5, 1].Style.Font.Size = 14;
                    namedWorksheet.Cells[5, 1].Style.Font.Name = "Times New Roman";
                    namedWorksheet.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[5, 1].Style.Font.Bold = true;

                    if (type.TypeName == "GiaoVien")
                    {
                        namedWorksheet.Cells[5, 1].Value = "BẠN ĐỌC GIÁO VIÊN";
                    }
                    if (type.TypeName == "HocSinh")
                    {
                        namedWorksheet.Cells[5, 1].Value = "BẠN ĐỌC HỌC SINH";
                    }
                    if (type.TypeName == "NhanVien")
                    {
                        namedWorksheet.Cells[5, 1].Value = "BẠN ĐỌC NHÂN VIÊN";
                    }

                    namedWorksheet.Row(5).Height = 35;

                    int numberRows = 6;
                    for (int i = 0; i < countUser.Count; i++)
                    {
                        namedWorksheet.Row(numberRows).Height = 35;

                        for (int j = 1; j <= 10; j++)
                        {
                            namedWorksheet.Cells[numberRows, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            namedWorksheet.Cells[numberRows, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            namedWorksheet.Cells[numberRows, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            namedWorksheet.Cells[numberRows, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                            namedWorksheet.Cells[numberRows, j].Style.Font.Size = 14;
                            namedWorksheet.Cells[numberRows, j].Style.Font.Name = "Times New Roman";
                            namedWorksheet.Cells[numberRows, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        }

                        namedWorksheet.Cells[numberRows, 2].Style.WrapText = true;
                        namedWorksheet.Cells[numberRows, 5].Style.WrapText = true;

                        namedWorksheet.Cells[numberRows, 1].Value = i + 1;
                        namedWorksheet.Cells[numberRows, 2].Value = countUser[i].NameUser;
                        namedWorksheet.Cells[numberRows, 3].Value = countUser[i].NameUnit;
                        namedWorksheet.Cells[numberRows, 4].Value = countUser[i].NumIndividual.Split('/')[0];
                        namedWorksheet.Cells[numberRows, 5].Value = countUser[i].NameDocument;
                        namedWorksheet.Cells[numberRows, 6].Value = countUser[i].Author;
                        namedWorksheet.Cells[numberRows, 7].Value = countUser[i].InvoiceCode;
                        namedWorksheet.Cells[numberRows, 8].Value = countUser[i].fromDate.ToString("dd/MM/yyyy");
                        namedWorksheet.Cells[numberRows, 9].Value = countUser[i].toDate.ToString("dd/MM/yyyy");
                        namedWorksheet.Cells[numberRows, 10].Value = countUser[i].NumberDayLate;

                        if (countUser[i].Author == null)
                        {
                            namedWorksheet.Cells[numberRows, 6].Value = " ";
                        }
                        numberRows++;
                    }

                    namedWorksheet.Cells[$"G{numberRows + 5}:J{numberRows + 5}"].Merge = true;
                    namedWorksheet.Cells[$"G{numberRows + 6}:J{numberRows + 6}"].Merge = true;

                    namedWorksheet.Cells[numberRows + 5, 7].Style.Font.Size = 14;
                    namedWorksheet.Cells[numberRows + 5, 7].Style.Font.Name = "Times New Roman";

                    namedWorksheet.Cells[numberRows + 6, 7].Style.Font.Size = 14;
                    namedWorksheet.Cells[numberRows + 6, 7].Style.Font.Name = "Times New Roman";

                    namedWorksheet.Cells[numberRows + 5, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[numberRows + 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                    namedWorksheet.Cells[numberRows + 5, 7].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                    namedWorksheet.Cells[numberRows + 6, 7].Value = "Cán bộ thư viện";
                    namedWorksheet.Cells[numberRows + 6, 7].Style.Font.Bold = true;

                    //overwrite to file old
                    FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "MauBaoCaoNhieu.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetListBorrowLateByUserType
        [HttpGet]
        [Route("GetListBorrowLateByUserType")]
        public List<CustomApiListBorrowLateByUserType> GetListBorrowLateByUserType(Guid IdUserType, string toDate)
        {
            try
            {
                List<CustomApiListBorrowLateByUserType> countUser = new List<CustomApiListBorrowLateByUserType>();
                countUser = _analystRepository.customApiListBorrowLateByUserTypes(IdUserType, toDate);
                return countUser;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetFileExcelAnalystListBorrowByUserTypeDetail
        [HttpGet]
        [Route("GetFileExcelAnalystListBorrowByUserTypeDetail")]
        public IActionResult GetFileExcelAnalystListBorrowByUserTypeDetail(Guid IdUnit, Guid IdUser, string fromDate, string toDate)
        {
            try
            {
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "DanhSachMuonChiTiet.xlsx");
                FileInfo fi = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {
                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];
                    namedWorksheet.Cells["A51:J2000"].Clear();

                    CustomApiListBorrowByUserTypeDetail listBorrow = new CustomApiListBorrowByUserTypeDetail();
                    listBorrow = _analystRepository.customApiListBorrowByUserTypesDetail(IdUnit, IdUser, fromDate, toDate);

                    namedWorksheet.Cells[$"A2:I2"].Merge = true;
                    namedWorksheet.Cells[2, 1].Style.Font.Size = 13;
                    namedWorksheet.Cells[2, 1].Style.Font.Name = "Calibri";
                    namedWorksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[2, 1].Style.Font.Bold = true;

                    List<ContactAndIntroductionDto> ruleDto = new List<ContactAndIntroductionDto>();
                    ruleDto = _contactAndIntroductionRepository.getAllRule(1, 1, 2);
                    if (ruleDto != null)
                    {
                        namedWorksheet.Cells[2, 1].Value = $"THƯ VIỆN {ruleDto[0].col10.ToUpper()}";
                    }
                    else
                    {
                        namedWorksheet.Cells[2, 1].Value = $"THƯ VIỆN TRƯỜNG ...";
                    }

                    namedWorksheet.Cells[$"B19:H21"].Merge = true;
                    namedWorksheet.Cells[19, 2].Style.Font.Size = 40;
                    namedWorksheet.Cells[19, 2].Style.Font.Name = "Calibri";
                    namedWorksheet.Cells[19, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[19, 2].Style.Font.Bold = true;

                    UserDTO userDTO = _userRepository.getUserByID(IdUser);
                    if (userDTO != null)
                    {
                        UserType userType = _userRepository.getUserType(userDTO.UserTypeId);
                        if (userType.TypeName == "HocSinh" && userType != null)
                        {
                            namedWorksheet.Cells[19, 2].Value = "CỦA HỌC SINH";
                        }
                        else namedWorksheet.Cells[19, 2].Value = "CỦA GIÁO VIÊN";
                    }
                    else
                    {
                        namedWorksheet.Cells[19, 2].Value = "CỦA HỌC SINH";
                    }

                    namedWorksheet.Cells[$"B22:H22"].Merge = true;
                    namedWorksheet.Cells[22, 2].Style.Font.Size = 20;
                    namedWorksheet.Cells[22, 2].Style.Font.Name = "Calibri";
                    namedWorksheet.Cells[22, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[22, 2].Style.Font.Bold = true;

                    SchoolYearDto schoolYearDto = _schoolYearRepository.getSchoolYear();
                    if (schoolYearDto != null)
                    {
                        namedWorksheet.Cells[22, 2].Value = $"NĂM HỌC: {schoolYearDto.FromYear.Value.ToString("yyyy")} - {schoolYearDto.ToYear.Value.ToString("yyyy")}";
                    }
                    else
                    {
                        namedWorksheet.Cells[22, 2].Value = $"NĂM HỌC: {DateTime.Now.Year} - {DateTime.Now.Year + 1}";
                    }


                    int tenp = 46;
                    for (int i = 1; i < 4; i++)
                    {
                        namedWorksheet.Cells[$"A{tenp}:E{tenp}"].Merge = true;
                        namedWorksheet.Cells[tenp, 1].Style.Font.Size = 14;
                        namedWorksheet.Cells[tenp, 1].Style.Font.Name = "Times New Roman";
                        tenp++;
                    }

                    namedWorksheet.Cells[46, 1].Value = "Họ và tên: " + listBorrow.NameUser;
                    namedWorksheet.Cells[47, 1].Value = "Phòng ban: " + listBorrow.NameUnit;
                    namedWorksheet.Cells[48, 1].Value = "Địa chỉ: " + listBorrow.Address;
                    namedWorksheet.Cells[48, 1].Style.WrapText = true;

                    if (listBorrow.listBorrowByUserIds != null)
                    {
                        int numberRows = 51;
                        for (int i = 0; i < listBorrow.listBorrowByUserIds.Count; i++)
                        {
                            namedWorksheet.Row(numberRows).Height = 35;

                            for (int j = 1; j < 10; j++)
                            {

                                namedWorksheet.Cells[numberRows, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                namedWorksheet.Cells[numberRows, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                namedWorksheet.Cells[numberRows, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                namedWorksheet.Cells[numberRows, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                                namedWorksheet.Cells[numberRows, j].Style.Font.Size = 14;
                                namedWorksheet.Cells[numberRows, j].Style.Font.Name = "Times New Roman";
                                namedWorksheet.Cells[numberRows, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            }

                            namedWorksheet.Cells[numberRows, 2].Style.WrapText = true;
                            namedWorksheet.Cells[numberRows, 5].Style.WrapText = true;

                            namedWorksheet.Cells[numberRows, 1].Value = i + 1;
                            namedWorksheet.Cells[numberRows, 2].Value = listBorrow.listBorrowByUserIds[i].NameDocument;
                            namedWorksheet.Cells[numberRows, 4].Value = listBorrow.listBorrowByUserIds[i].NumIndividual.Split('/')[0];
                            namedWorksheet.Cells[numberRows, 5].Value = listBorrow.listBorrowByUserIds[i].fromDate.ToString("dd/MM/yyyy");
                            namedWorksheet.Cells[numberRows, 6].Value = "";
                            namedWorksheet.Cells[numberRows, 7].Value = listBorrow.listBorrowByUserIds[i].toDate.ToString("dd/MM/yyyy");
                            namedWorksheet.Cells[numberRows, 8].Value = "";
                            namedWorksheet.Cells[numberRows, 9].Value = listBorrow.listBorrowByUserIds[i].Note;
                            numberRows++;
                        }
                    }

                    //overwrite to file old
                    FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "MauBaoCaoNhieu.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetFileExcelAnalystListBorrowByUserType
        [HttpGet]
        [Route("GetFileExcelAnalystListBorrowByUserType")]
        public IActionResult GetFileExcelAnalystListBorrowByUserType(Guid IdUnit, Guid IdUserType, string fromDate, string toDate)
        {
            try
            {
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "DanhSachMuonTheoPhongBan.xlsx");
                FileInfo fi = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {

                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];
                    namedWorksheet.Cells["A51:J2000"].Clear();

                    List<CustomApiListUserByUnit> countUser = new List<CustomApiListUserByUnit>();
                    countUser = _analystRepository.CustomApiListUserByUnit(IdUnit, IdUserType, fromDate, toDate);

                    namedWorksheet.Cells[$"A3:I3"].Merge = true;
                    namedWorksheet.Cells[3, 1].Style.Font.Size = 13;
                    namedWorksheet.Cells[3, 1].Style.Font.Name = "Calibri";
                    namedWorksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[3, 1].Style.Font.Bold = true;

                    List<ContactAndIntroductionDto> ruleDto = new List<ContactAndIntroductionDto>();
                    ruleDto = _contactAndIntroductionRepository.getAllRule(1, 1, 2);
                    if (ruleDto != null)
                    {
                        namedWorksheet.Cells[3, 1].Value = $"THƯ VIỆN {ruleDto[0].col10.ToUpper()}";
                    }
                    else
                    {
                        namedWorksheet.Cells[3, 1].Value = $"THƯ VIỆN TRƯỜNG ...";
                    }

                    namedWorksheet.Cells[$"B20:I22"].Merge = true;
                    namedWorksheet.Cells[20, 2].Style.Font.Size = 40;
                    namedWorksheet.Cells[20, 2].Style.Font.Name = "Calibri";
                    namedWorksheet.Cells[20, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[20, 2].Style.Font.Bold = true;

                    UserType userType = _userRepository.getUserType(IdUserType);
                    if (userType.TypeName == "HocSinh" && userType != null)
                    {
                        namedWorksheet.Cells[20, 2].Value = "CỦA HỌC SINH";
                    }
                    else namedWorksheet.Cells[20, 2].Value = "CỦA GIÁO VIÊN";

                    namedWorksheet.Cells[$"B23:I23"].Merge = true;
                    namedWorksheet.Cells[23, 2].Style.Font.Size = 20;
                    namedWorksheet.Cells[23, 2].Style.Font.Name = "Calibri";
                    namedWorksheet.Cells[23, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    namedWorksheet.Cells[23, 2].Style.Font.Bold = true;

                    SchoolYearDto schoolYearDto = _schoolYearRepository.getSchoolYear();
                    if (schoolYearDto != null)
                    {
                        string year = schoolYearDto.FromYear.Value.ToString("yyyy");
                        namedWorksheet.Cells[23, 2].Value = $"NĂM HỌC: {schoolYearDto.FromYear.Value.ToString("yyyy")} - {schoolYearDto.ToYear.Value.ToString("yyyy")}";
                    }
                    else
                    {
                        namedWorksheet.Cells[23, 2].Value = $"NĂM HỌC: {DateTime.Now.Year} - {DateTime.Now.Year + 1}";
                    }

                    var unit = _userRepository.getUnit(IdUnit);
                    if (unit != null)
                    {
                        namedWorksheet.Cells[$"A48:J48"].Merge = true;
                        namedWorksheet.Cells[48, 1].Style.Font.Size = 14;
                        namedWorksheet.Cells[48, 1].Style.Font.Name = "Times New Roman";
                        namedWorksheet.Cells[48, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        namedWorksheet.Cells[48, 1].Style.Font.Bold = true;

                        namedWorksheet.Cells[48, 1].Value = unit.UnitName;
                    }

                    int numberRow = 51;
                    for (int i = 0; i < countUser.Count; i++)
                    {
                        namedWorksheet.Row(numberRow).Height = 35;
                        for (int j = 1; j <= 10; j++)
                        {

                            namedWorksheet.Cells[numberRow, j].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            namedWorksheet.Cells[numberRow, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            namedWorksheet.Cells[numberRow, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            namedWorksheet.Cells[numberRow, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                            namedWorksheet.Cells[numberRow, j].Style.Font.Size = 14;
                            namedWorksheet.Cells[numberRow, j].Style.Font.Name = "Times New Roman";
                            namedWorksheet.Cells[numberRow, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        }

                        namedWorksheet.Cells[numberRow, 2].Style.WrapText = true;
                        namedWorksheet.Cells[numberRow, 3].Style.WrapText = true;

                        namedWorksheet.Cells[numberRow, 1].Value = i + 1;
                        namedWorksheet.Cells[numberRow, 2].Value = countUser[i].NameUser;
                        namedWorksheet.Cells[numberRow, 3].Value = countUser[i].NameDocument;
                        namedWorksheet.Cells[numberRow, 5].Value = countUser[i].NumIndividual.Split('/')[0];
                        namedWorksheet.Cells[numberRow, 6].Value = countUser[i].fromDate.ToString("dd/MM/yyyy");
                        namedWorksheet.Cells[numberRow, 7].Value = "";
                        namedWorksheet.Cells[numberRow, 8].Value = countUser[i].toDate.ToString("dd/MM/yyyy");
                        namedWorksheet.Cells[numberRow, 9].Value = "";
                        numberRow++;
                    }

                    //overwrite to file old
                    FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "MauBaoCaoNhieu.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/AnalystListBorrowByUserTypeDetail
        [HttpGet]
        [Route("AnalystListBorrowByUserTypeDetail")]
        public CustomApiListBorrowByUserTypeDetail AnalystListBorrowByUserTypeDetail(Guid IdUnit, Guid IdUser, string fromDate, string toDate)
        {
            try
            {
                CustomApiListBorrowByUserTypeDetail listBorrow = new CustomApiListBorrowByUserTypeDetail();
                listBorrow = _analystRepository.customApiListBorrowByUserTypesDetail(IdUnit, IdUser, fromDate, toDate);
                return listBorrow;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/AnalystListBorrowByUserType
        [HttpGet]
        [Route("AnalystListBorrowByUserType")]
        public List<CustomApiListBorrowByUserType> AnalystListBorrowByUserType(Guid IdUnit, Guid IdUserType, string fromDate, string toDate)
        {
            try
            {
                List<CustomApiListBorrowByUserType> listBorrow = new List<CustomApiListBorrowByUserType>();
                listBorrow = _analystRepository.customApiListBorrowByUserTypes(IdUnit, IdUserType, fromDate, toDate);
                return listBorrow;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: api/Analyst/GetAllCategorySign
        [HttpGet]
        [Route("GetNumberUserByType")]
        public List<CustomApiCountUserByUserType> GetNumberUserByType()
        {
            try
            {
                List<CustomApiCountUserByUserType> countUser = new List<CustomApiCountUserByUserType>();
                countUser = _analystRepository.CountUserByType();
                return countUser;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetDocumentByType
        [HttpGet]
        [Route("GetDocumentByType")]
        public List<CustomApiCountDocumentByType> GetDocumentByType()
        {
            try
            {
                List<CustomApiCountDocumentByType> countDoc = new List<CustomApiCountDocumentByType>();
                countDoc = _analystRepository.CountDocumentByType();
                return countDoc;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/AnalystUserAndBook
        [HttpGet]
        [Route("AnalystUserAndBook")]
        public CustomApiAnalystUserAndBook AnalystUserAndBook()
        {
            try
            {
                CustomApiAnalystUserAndBook customApiAnalystUserAndBook = new CustomApiAnalystUserAndBook();
                customApiAnalystUserAndBook = _analystRepository.AnalystUserAndBook();
                return customApiAnalystUserAndBook;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/AnalystBorrowBookByType
        [HttpGet]
        [Route("AnalystBorrowBookByType")]
        public List<CustomApiBorrowByUserType> AnalystBorrowBookByType(string fromDate, string toDate)
        {
            try
            {
                List<CustomApiBorrowByUserType> customApiBorrowByUserTypes = new List<CustomApiBorrowByUserType>();
                customApiBorrowByUserTypes = _analystRepository.customApiBorrowByUserTypes(fromDate, toDate);
                return customApiBorrowByUserTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/AnalystBookByDocumentType
        [HttpGet]
        [Route("AnalystBookByDocumentType")]
        public IEnumerable<CustomApiAnalystBookByType> AnalystBookByDocumentType(Guid IdDocument)
        {
            try
            {
                return _analystRepository.customApiAnalystBookByTypes(IdDocument);
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetExceLAnalystBorowByUserType
        [HttpGet]
        [Route("GetExceLAnalystBorowByUserType")]
        public IActionResult GetExceLAnalystBorowByUserType(string fromDate, string toDate)
        {
            try
            {

                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_BaoCaoThongKeSachTheoLoaiNguoiMuon.xlsx");
                FileInfo fi = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {

                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];

                    List<CustomApiBorrowByUserType> customApiBorrowByUserTypes = new List<CustomApiBorrowByUserType>();
                    customApiBorrowByUserTypes = _analystRepository.customApiBorrowByUserTypes("NotLate", fromDate, toDate);

                    for (int i = 0; i < customApiBorrowByUserTypes.Count; i++)
                    {
                        if (i == 0)
                        {
                            namedWorksheet.Cells[2, 2].Value = customApiBorrowByUserTypes[i].NumberUserType;
                            namedWorksheet.Cells[3, 2].Value = string.Format("{0:0.00}", customApiBorrowByUserTypes[i].percent) + "%";
                        }
                        else if (i == 1)
                        {
                            namedWorksheet.Cells[2, 3].Value = customApiBorrowByUserTypes[i].NumberUserType;
                            namedWorksheet.Cells[3, 3].Value = string.Format("{0:0.00}", customApiBorrowByUserTypes[i].percent) + "%";
                        }
                        else
                        {
                            namedWorksheet.Cells[2, 4].Value = customApiBorrowByUserTypes[i].NumberUserType;
                            namedWorksheet.Cells[3, 4].Value = string.Format("{0:0.00}", customApiBorrowByUserTypes[i].percent) + "%";
                        }
                    }
                    //overwrite to file old
                    FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Mau_BaoCaoThongKeSachTheoLoaiNguoiMuon.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetExceLAnalystByDocumentType
        [HttpGet]
        [Route("GetExceLAnalystByDocumentType")]
        public IActionResult GetExceLAnalystByDocumentType(Guid IdDocumentType)
        {
            try
            {
                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_BaoCaoThongKeSachTheoLoai.xlsx");
                var fi = new FileInfo(path);

                using (var excelPackage = new ExcelPackage(fi))
                {

                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];

                    int rowCount = namedWorksheet.Dimension.Rows;
                    namedWorksheet.Cells["A3:Q1000"].Clear();

                    namedWorksheet.Cells[1, 1].Value = "THỐNG KÊ SÁCH THEO LOẠI";

                    var datas = _analystRepository.AnalystBookByGroupTypes(IdDocumentType);

                    int startRow = 3;
                    int numberOfRecords = 0;
                    int remainBooks = 0;
                    int borrowed = 0;
                    int losted = 0;
                    long totalMoneys = 0;
                    for (int i = 0; i < datas.Count; i++)
                    {
                        namedWorksheet.Cells[$"A{startRow}:Q{startRow}"].Merge = true;
                        namedWorksheet.Cells[startRow, 1].Value = datas[i].NameDocmentType;
                        namedWorksheet.Row(startRow).Height = 35;
                        namedWorksheet.Cells[startRow, 1].Style.Font.Bold = true;

                        for (int j = 0; j < datas[i].DataAnalystBooks.Count;j++)
                        {
                            numberOfRecords += datas[i].DataAnalystBooks[j].TotalDocument;
                            remainBooks += datas[i].DataAnalystBooks[j].RemainDocument;
                            borrowed += datas[i].DataAnalystBooks[j].BorrowedDocument;
                            losted += datas[i].DataAnalystBooks[j].LostDocument;
                            totalMoneys += (long)datas[i].DataAnalystBooks[j].document.Price;

                            startRow++;
                            for (int k = 1; k < 18; k++)
                            {
                                namedWorksheet.Cells[startRow, k].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                namedWorksheet.Cells[startRow, k].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                namedWorksheet.Cells[startRow, k].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                namedWorksheet.Cells[startRow, k].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            }

                            namedWorksheet.Cells[startRow, 1].Value = datas[i].DataAnalystBooks[j].document.DocName;
                            namedWorksheet.Cells[startRow, 2].Value = datas[i].DataAnalystBooks[j].document.CreatedDate?.ToString("dd/MM/yyyy");
                            namedWorksheet.Cells[startRow, 3].Value = datas[i].DataAnalystBooks[j].TotalDocument;
                            namedWorksheet.Cells[startRow, 4].Value = datas[i].DataAnalystBooks[j].RemainDocument;
                            namedWorksheet.Cells[startRow, 5].Value = datas[i].DataAnalystBooks[j].BorrowedDocument;
                            namedWorksheet.Cells[startRow, 6].Value = datas[i].DataAnalystBooks[j].LostDocument;
                            var isHavePhy = datas[i].DataAnalystBooks[j].document.IsHavePhysicalVersion;
                            if (isHavePhy == true)
                            {
                                namedWorksheet.Cells[startRow, 7].Value = "Có";
                            }
                            else if (isHavePhy == false || isHavePhy == null)
                            {
                                namedWorksheet.Cells[startRow, 7].Value = "Không";
                            }
                            namedWorksheet.Cells[startRow, 8].Value = datas[i].DataAnalystBooks[j].document.Language ?? "";
                            namedWorksheet.Cells[startRow, 9].Value = datas[i].DataAnalystBooks[j].document.Publisher ?? "";
                            namedWorksheet.Cells[startRow, 10].Value = datas[i].DataAnalystBooks[j].document.PublishYear?.ToString("dd/MM/yyyy");
                            namedWorksheet.Cells[startRow, 11].Value = datas[i].DataAnalystBooks[j].document.NumberView;
                            namedWorksheet.Cells[startRow, 12].Value = datas[i].DataAnalystBooks[j].document.NumberLike;
                            namedWorksheet.Cells[startRow, 13].Value = datas[i].DataAnalystBooks[j].document.NumberUnlike;

                            if (datas[i].DataAnalystBooks[j].document.ModifiedDate != null)
                            {
                                namedWorksheet.Cells[startRow, 14].Value = datas[i].DataAnalystBooks[j].document.ModifiedDate?.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                namedWorksheet.Cells[startRow, 14].Value = " ";
                            }
                            namedWorksheet.Cells[startRow, 15].Value = datas[i].DataAnalystBooks[j].document.Author ?? "";
                            namedWorksheet.Cells[startRow, 16].Value = datas[i].DataAnalystBooks[j].document.Description ?? "";
                            namedWorksheet.Cells[startRow, 17].Value = datas[i].DataAnalystBooks[j].document.Price?.ToString("C0", new CultureInfo("vi-VN"));
                            namedWorksheet.Cells[startRow, 18].Value = "";
                        }
                        startRow++;
                    }

                    for (int k = 1; k < 18; k++)
                    {
                        namedWorksheet.Cells[startRow, k].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[startRow, k].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[startRow, k].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        namedWorksheet.Cells[startRow, k].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    namedWorksheet.Cells[$"A{startRow}:B{startRow}"].Merge = true;
                    namedWorksheet.Cells[startRow, 1].Value = "Tổng ";
                    namedWorksheet.Cells[startRow, 1].Style.Font.Bold = true;
                    namedWorksheet.Cells[startRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    namedWorksheet.Cells[startRow, 3].Value = numberOfRecords;
                    namedWorksheet.Cells[startRow, 4].Value = remainBooks;
                    namedWorksheet.Cells[startRow, 5].Value = borrowed;
                    namedWorksheet.Cells[startRow, 6].Value = losted;
                    namedWorksheet.Cells[$"G{startRow}:P{startRow}"].Merge = true;
                    namedWorksheet.Cells[startRow, 17].Value = totalMoneys.ToString("C0", new CultureInfo("vi-VN"));
                    namedWorksheet.Cells[startRow, 18].Value = "";


                    //namedWorksheet.Row(startRow).Height = 35;

                    //overwrite to file old
                    var fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Mau_BaoCaoThongKeSachTheoLoai.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetExceLAnalystBorowLateByUserType
        [HttpGet]
        [Route("GetExceLAnalystBorowLateByUserType")]
        public IActionResult GetExceLAnalystBorowLateByUserType(string fromDate, string toDate)
        {
            try
            {

                var path = string.Concat(_appSettingModel.ServerFileExcel, "\\", "Mau_BaoCaoThongKeSachTheoLoaiNguoiMuon.xlsx");
                FileInfo fi = new FileInfo(path);

                using (ExcelPackage excelPackage = new ExcelPackage(fi))
                {

                    //Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                    ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets[0];

                    List<CustomApiBorrowByUserType> customApiBorrowByUserTypes = new List<CustomApiBorrowByUserType>();
                    customApiBorrowByUserTypes = _analystRepository.customApiBorrowByUserTypes("Late", fromDate, toDate);

                    for (int i = 0; i < customApiBorrowByUserTypes.Count; i++)
                    {
                        if (i == 0)
                        {
                            namedWorksheet.Cells[2, 2].Value = customApiBorrowByUserTypes[i].NumberUserType;
                            namedWorksheet.Cells[3, 2].Value = string.Format("{0:0.00}", customApiBorrowByUserTypes[i].percent) + "%";
                        }
                        else if (i == 1)
                        {
                            namedWorksheet.Cells[2, 3].Value = customApiBorrowByUserTypes[i].NumberUserType;
                            namedWorksheet.Cells[3, 3].Value = string.Format("{0:0.00}", customApiBorrowByUserTypes[i].percent) + "%";
                        }
                        else
                        {
                            namedWorksheet.Cells[2, 4].Value = customApiBorrowByUserTypes[i].NumberUserType;
                            namedWorksheet.Cells[3, 4].Value = string.Format("{0:0.00}", customApiBorrowByUserTypes[i].percent) + "%";
                        }
                    }
                    //overwrite to file old
                    FileInfo fiToSave = new FileInfo(path);
                    //Save your file
                    excelPackage.SaveAs(fiToSave);
                }
                //download file excel
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Mau_BaoCaoThongKeSachTheoLoaiNguoiMuon.xlsx");
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/GetNumberDocumentByIdStock
        [HttpGet]
        [Route("GetNumberDocumentByIdStock")]
        public List<ListBookNew> GetNumberDocumentByIdStock(Guid id)
        {
            try
            {
                List<ListBookNew> listBook = new List<ListBookNew>();
                listBook = _analystRepository.ListDocumentByIdStock(id);
                return listBook;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // GET: api/Analyst/CountDocumentInStock
        [HttpGet]
        [Route("CountDocumentInStock")]
        public int CountDocumentInStock(Guid id)
        {
            try
            {
                int count = _analystRepository.CountDocumentByIdStock(id);
                return count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
