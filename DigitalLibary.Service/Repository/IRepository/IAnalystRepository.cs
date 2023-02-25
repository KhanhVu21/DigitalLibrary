using DigitalLibary.Service.Common.FormatApi;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IAnalystRepository
    {
        #region CUSTOM API Analyst
        public List<CustomApiCountUserByUserType> CountUserByType();
        public List<CustomApiCountDocumentByType> CountDocumentByType();
        public CustomApiAnalystUserAndBook AnalystUserAndBook();
        public List<CustomApiBorrowByUserType> customApiBorrowByUserTypes(string status, string fromdate, string todate);
        public List<CustomApiAnalystBookByType> customApiAnalystBookByTypes(Guid IdDocumentType);
        public List<AnalystBookByGroupType> AnalystBookByGroupTypes(Guid IdDocumentType);
        public List<ListBookNew> ListDocumentByIdStock (Guid IdStock);
        public int CountDocumentByIdStock(Guid IdStock);
        public List<CustomApiBorrowByUserType> customApiBorrowByUserTypes(string fromdate, string todate);
        public List<CustomApiListUserByUnit> CustomApiListUserByUnit(Guid IdUnit, Guid IdUserType, string fromDate, string toDate);
        public List<CustomApiListBorrowByUserType> customApiListBorrowByUserTypes(Guid IdUnit, Guid IdUserType, string fromDate, string toDate);
        public CustomApiListBorrowByUserTypeDetail customApiListBorrowByUserTypesDetail(Guid IdUnit, Guid IdUser, string fromDate, string toDate);
        public List<CustomApiListBorrowLateByUserType> customApiListBorrowLateByUserTypes(Guid IdUserType, string toDate);
        public List<CustomApiNumIndividualLedger> customApiNumIndividualLedgers(string fromDate, string toDate, Guid DocumentType);
        #endregion
    }
}
