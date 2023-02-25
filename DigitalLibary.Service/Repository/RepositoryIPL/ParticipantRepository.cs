using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class ParticipantRepository: IParticipantsRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public ParticipantRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region Method
        public Response DeleteParticipants(Guid IdParticipant)
        {
            try
            {
                Response response = new Response();
                Participants participants = _DbContext.Participants.Where(x => x.Id == IdParticipant).FirstOrDefault();

                if (participants != null)
                {
                    _DbContext.Participants.Remove(participants);
                    _DbContext.SaveChanges();

                    response = new Response()
                    {
                        Success = true,
                        Fail = false,
                        Message = "Xóa thành công !"
                    };
                    return response;
                }
                else
                {
                    response = new Response()
                    {
                        Success = false,
                        Fail = true,
                        Message = "Không tìm thấy kết quả !"
                    };
                    return response;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetlistMission(int pageNumber, int pageSize)
        {
            try
            {
                List<string> Mission = _DbContext.Participants.Where(e => e.Mission != null).Select(e => e.Mission).ToList();
                Mission = Mission.Distinct().ToList();

                if (pageNumber != 0 && pageSize != 0)
                {
                    if (pageNumber < 0) { pageNumber = 1; }
                    Mission = Mission.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                return Mission;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetlistName(int pageNumber, int pageSize)
        {
            try
            {
                List<string> Name = _DbContext.Participants.Where(e => e.Name != null).Select(e => e.Name).ToList();

                Name = Name.Distinct().ToList();

                if (pageNumber != 0 && pageSize != 0)
                {
                    if (pageNumber < 0) { pageNumber = 1; }
                    Name = Name.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                return Name;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetlistPosition(int pageNumber, int pageSize)
        {
            try
            {
                List<string> Position = _DbContext.Participants.Where(e => e.Position != null).Select(e => e.Position).ToList();

                Position = Position.Distinct().ToList();

                if (pageNumber != 0 && pageSize != 0)
                {
                    if (pageNumber < 0) { pageNumber = 1; }
                    Position = Position.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                return Position;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParticipantsDto> GetListParticipantByIdReceipt(int pageNumber, int pageSize, Guid IdReceipt)
        {
            try
            {
                List<Participants> participants = _DbContext.Participants.Where(e => e.IdReceipt == IdReceipt).ToList();
                if (pageNumber != 0 && pageSize != 0)
                {
                    if (pageNumber < 0) { pageNumber = 1; }
                    participants = participants.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                List<ParticipantsDto> participantsDtos = _mapper.Map<List<ParticipantsDto>>(participants);
                return participantsDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParticipantsDto> GetListParticipants(int pageNumber, int pageSize)
        {
            try
            {
                List<Participants> participants = _DbContext.Participants.ToList();
                if (pageNumber != 0 && pageSize != 0)
                {
                    if (pageNumber < 0) { pageNumber = 1; }
                    participants = participants.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                List<ParticipantsDto> participantsDtos = _mapper.Map<List<ParticipantsDto>>(participants);
                return participantsDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ParticipantsDto GetParticipantsById(Guid IdParticipant)
        {
            try
            {
                Participants participants = _DbContext.Participants.Where(e => e.Id == IdParticipant).FirstOrDefault();

                ParticipantsDto participantsDto = _mapper.Map<ParticipantsDto>(participants);
                return participantsDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response InsertParticipants(ParticipantsDto participantsDto)
        {
            try
            {
                Response response = new Response();

                Participants participants = new Participants();
                participants = _mapper.Map<Participants>(participantsDto);

                _DbContext.Participants.Add(participants);
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
                throw;
            }
        }
        public Response UpdateParticipants(ParticipantsDto participantsDto)
        {
            try
            {
                Response response = new Response();
                Participants participants = new Participants();

                participants = _DbContext.Participants.Where(e => e.Id == participantsDto.Id).FirstOrDefault();
                if (participants != null)
                {
                    participants.IdReceipt = participantsDto.IdReceipt;
                    participants.Name = participantsDto.Name;
                    participants.Position = participantsDto.Position;
                    participants.Mission = participantsDto.Mission;
                    participants.Note = participantsDto.Note;

                    _DbContext.Participants.Update(participants);
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
                throw;
            }
        }
        #endregion
    }
}
