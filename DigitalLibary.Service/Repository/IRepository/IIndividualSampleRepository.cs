using DigitalLibary.Service.Common;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IIndividualSampleRepository
    {
        #region METHOD
        public HashSet<string> GetIndividualByDate(Guid IdDocument);
        public CustomApiIndividual CustomApiIndividual(Guid IdDocument);
        Boolean CheckExitNumberIndividual(string numberCode, Guid IdDocument, Guid IdDocumentType);
        Boolean CheckExitNumberIndividualByNumIndividual(string NumIndividual, Guid IdIndividual);
        Boolean CheckExitDocumnentAndDocumentType(Guid IdDocument, Guid IdDocumentType);
        int getNumIndividualMax(Guid IdCategorySign, Guid IdDocument, Guid IdDocumentType);
        int getNumIndividualMaxByInsertReceipt(Guid IdCategorySign);
        int getNumIndividualMaxByIdCategorySign(Guid IdCategorySign);

        public List<CustomApiSpineBook> customApiSpineBook(Guid IdDocument, Guid IdIndividual,String ListIdIndividual);
        public List<CustomApiSpineBookByGroup> customApiSpineBook(String ListIdIndividual);
        public CustomApiSpineBook SpineBookByBarcode(string barcode);
        string GetCodeBookNameEncrypt(string bookName);
        #endregion

        #region CRUD TABLE INDIVIDUALSAMPLE
        IndividualSampleDto getIndividualSampleById(Guid Id);
        List<IndividualSampleDto> getIndividualSample(int pageNumber, int pageSize);
        Response InsertIndividualSample(IndividualSampleDto individualSampleDto);
        Response UpdateIndividualSample(IndividualSampleDto individualSampleDto);
        Response DeleteIndividualSample(Guid IdIndividual);
        Response DeleteIndividualSampleByList(List<Guid> IdIdIndividual);
        public Tuple<Boolean, String> CheckIdIndividualExitsInDocumentInvoice(List<Guid> IdIdIndividual);
        Response ChangeStatus(Guid Id, int status);
        Response ChangeIndividualToAvailable(Guid IdDocumentInvoice);
        Response ReturnBook(Guid IdDocument, Guid IdIndividual, bool isLost);
        List<IndividualSampleDto> getIndividualSampleIsLost(int pageNumber, int pageSize);
        IndividualSampleDto getIndividualSampleLostById(Guid Id);
        #endregion
    }
}
