using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IDocumentTypeRepository
    {
        List<DocumentTypeDto> GetAllDocumentType(int status);
        List<DocumentTypeDto> GetAllDocumentType(int pageNumber, int pageSize);
        List<DocumentTypeDto> GetAllDocumentType();
        List<DocumentTypeDto> GetAllDocumentTypeByParentId(Guid Id);
        DocumentTypeDto GetAllDocumentTypeById(Guid Id);
        Response InsertDocumentType(DocumentTypeDto documentTypeDto);
        Response UpdateDocumentType(DocumentTypeDto documentTypeDto);
        Response DeleteDocumentType(Guid Id);
        String GetNameDocumentType(Guid IdDocumentType);
    }
}
