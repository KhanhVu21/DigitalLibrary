using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface IParticipantsRepository
    {
        public List<ParticipantsDto> GetListParticipantByIdReceipt(int pageNumber, int pageSize, Guid IdReceipt);
        public List<ParticipantsDto> GetListParticipants(int pageNumber, int pageSize);
        public ParticipantsDto GetParticipantsById(Guid IdParticipant);
        Response InsertParticipants(ParticipantsDto participantsDto);
        Response UpdateParticipants(ParticipantsDto participantsDto);
        Response DeleteParticipants(Guid IdParticipant);
        public List<string> GetlistName(int pageNumber, int pageSize);
        public List<string> GetlistPosition(int pageNumber, int pageSize);
        public List<string> GetlistMission(int pageNumber, int pageSize);
    }
}
