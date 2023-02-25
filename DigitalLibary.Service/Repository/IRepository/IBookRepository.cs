using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IBookRepository
    {
        #region CRUD TABLE DOCUMENTTYPE
        List<DocumentType> GetDocumentTypes();
        #endregion

        #region CRUD TABLE BOOK
        CustomApiListDocumentByStock ListDocumentByBarcode(string barcode);   
        List<ListBookNew> getBookNews(int pageNumber, int pageSize);
        List<ListBookNew> getBookByNumberView(int pageNumber, int pageSize);
        List<ListBookNew> getBookByCategory(int pageNumber, int pageSize, Guid IdDocumentType);
        ListBookNew getBookById(Guid Id);
        List<ListBookNew> SearchBook(string values, int pageNumber, int pageSize);
        List<SuggestSearch> SuggestSearchBook(string values);
        int GetAllNumberBook();
        CustomApiDocumentAndIndividual GetBookAndIndividualManyParam(IndividualByDocumentDto individualByDocumentDto);
        #endregion

        #region ADMIN SITE
        List<ListBookNew> getBookAdminSite(int pageNumber, int pageSize, int DocumentType);
        List<ListBookNew> getBookByCategoryAdminSite(int pageNumber, int pageSize, Guid IdDocumentType);
        List<ListBookNew> getBookAdminSite(SortAndSearchListDocument sortAndSearchListDocument);
        ListBookNew getBookByIdAdminSite(Guid Id);
        Response DeleteDocument(Guid Id);
        Response InsertDocument(DocumentDto documentDto);
        Response InsertDocumentAvatar(DocumentAvatarDto documentAvatarDto);
        Response UpdateInsertDocument(DocumentDto documentDto);
        Response UpdateDocumentAvartar(DocumentAvatarDto documentAvatarDto);
        Response Approved(Guid Id, bool IsApproved, Guid ApprovedBy);
        CustomApiDocumentAndIndividual customApiDocumentAndIndividuals(Guid Id);
        CustomApiDocumentAndIndividual customApiDocumentAndIndividualsNotBorrow(Guid Id);
        #endregion
    }
}
