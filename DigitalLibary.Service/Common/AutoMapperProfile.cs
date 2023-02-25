using AutoMapper;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;

namespace DigitalLibary.Service.Common
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // INIT MAP DATA FROM DTO TO ENTITY AND REVERSE
            CreateMap<RestDate, RestDateDto>().ReverseMap();
            CreateMap<SchoolYear, SchoolYearDto>().ReverseMap();
            CreateMap<ContactAndIntroduction, ContactAndIntroductionDto>().ReverseMap();
            CreateMap<DocumentStock, DocumentStockDto>().ReverseMap();
            CreateMap<IndividualSample, IndividualSampleDto>().ReverseMap();
            CreateMap<Document, DocumentDto>().ReverseMap();
            CreateMap<CategorySign, CategorySignDto>().ReverseMap();
            CreateMap<DocumentInvoice, DocumentInvoiceDto>().ReverseMap();
            CreateMap<User_Role, User_RoleDto>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Unit, UnitDto>().ReverseMap();
            CreateMap<DocumentInvoiceDetail, DocumentInvoiceDetailDto>().ReverseMap();
            CreateMap<Slide, SlideDto>().ReverseMap();
            CreateMap<DocumentType, DocumentTypeDto>().ReverseMap();
            CreateMap<DocumentAvatar, DocumentAvatarDto>().ReverseMap();
            CreateMap<Receipt, ReceiptDto>().ReverseMap();
            CreateMap<CategoryPublisher, CategoryPublisherDto>().ReverseMap();
            CreateMap<CategorySign_V1, CategorySign_V1Dto>().ReverseMap();
            CreateMap<User, User_Delete>().ReverseMap();
            CreateMap<Diary, DiaryDto>().ReverseMap();
            CreateMap<BookNameEncrypt, BookNameEncryptDto>().ReverseMap();
            CreateMap<Participants, ParticipantsDto>().ReverseMap();
            CreateMap<AuditMethod, AuditMethodDto>().ReverseMap();
            CreateMap<StatusBook, StatusBookDto>().ReverseMap();
            CreateMap<AuditReceipt, AuditReceiptDto>().ReverseMap();
            CreateMap<AuditorList, AuditorListDto>().ReverseMap();
            CreateMap<AuditBookList, AuditBookListDto>().ReverseMap();
            CreateMap<AuditorList, AuditorPayload>().ReverseMap();
            CreateMap<AuditBookList, AuditBookListPayload>().ReverseMap();
        }
    }
}
