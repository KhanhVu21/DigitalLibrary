using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IContactAndIntroductionRepository
    {
        #region CRUD TABLE CONTACTANDINTRODUCTION WITH TYPE
        ContactAndIntroductionDto getRuleClient(Guid Id);
        List<ContactAndIntroductionDto> getAllRule(int pageNumber, int pageSize, int type);
        Response InsertRule(ContactAndIntroductionDto contactAndIntroductionDto);
        Response UpdateRule(ContactAndIntroductionDto contactAndIntroductionDto);
        Response DeleteRule(Guid Id);
        #endregion
    }
}
