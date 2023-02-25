using AutoMapper;
using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common;
using DigitalLibary.Service.Dto;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class ReceiptRepository : IReceiptRepository
    {
        #region Variables
        private readonly IMapper _mapper;
        public DataContext _DbContext;
        #endregion

        #region Constructors
        public ReceiptRepository(DataContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }
        #endregion

        #region Method
        public Response DeleteReceipt(Guid Id)
        {
            Response response = new Response();
            try
            {
                Receipt receipt = _DbContext.Receipt.Where(x => x.IdReceipt == Id).FirstOrDefault();

                if (receipt != null)
                {
                    receipt.IsDeleted = true;
                    _DbContext.Receipt.Update(receipt);

                    List<ReceiptDetail > list = _DbContext.ReceiptDetail.
                        Where(x => x.IdReceipt == receipt.IdReceipt).ToList();

                    for(int i = 0; i < list.Count; i++)
                    {
                        list[i].IsDeleted = true;
                        _DbContext.ReceiptDetail.Update(list[i]);
                    }
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
                    Message = "Xóa không thành công !"
                };
                return response;
            }
        }
        public List<ReceiptDto> getAllReceipt(int pageNumber, int pageSize)
        {
            try
            {
                List<Receipt> receipts = new List<Receipt>();
                if (pageNumber == 0 && pageSize == 0)
                {
                    receipts = _DbContext.Receipt.
                    Where(e => e.IsDeleted == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToList();
                }
                else
                {
                    receipts = _DbContext.Receipt.
                    Where(e => e.IsDeleted == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }

                List<ReceiptDto> receiptDtos = new List<ReceiptDto>();

                for(int i = 0; i < receipts.Count; i++)
                {
                    List<ReceiptDetail> receiptDetails = _DbContext.ReceiptDetail.Where(e =>
                    e.IdReceipt == receipts[i].IdReceipt).ToList();
                    ReceiptDto receiptDto = new ReceiptDto();

                    receiptDto = _mapper.Map<ReceiptDto>(receipts[i]);
                    receiptDto.ReceiptDetail = receiptDetails;

                    receiptDtos.Add(receiptDto);
;                }
                return receiptDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DocumentDto GetDocumentType(Guid Id)
        {
            try
            {
                Document document = _DbContext.Document.AsNoTracking()
                .Where(e => e.ID == Id).FirstOrDefault();

                DocumentDto documentDto = new DocumentDto();
                documentDto = _mapper.Map<DocumentDto>(document);

                return documentDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ReceiptDto getReceipt(Guid Id)
        {
            try
            {
                Receipt receipts = new Receipt();
                ReceiptDto receiptDtos = new ReceiptDto();

                receipts = _DbContext.Receipt.
                Where(e => e.IsDeleted == false && e.IdReceipt == Id)
                .FirstOrDefault();

                if(receipts != null)
                {
                    List<ReceiptDetail> receiptDetails = _DbContext.ReceiptDetail.Where(e =>
                    e.IdReceipt == receipts.IdReceipt).ToList();

                    List<Participants> participants = _DbContext.Participants.Where(e => e.IdReceipt == Id).ToList();

                    receiptDtos = _mapper.Map<ReceiptDto>(receipts);
                    receiptDtos.ReceiptDetail = receiptDetails;
                    receiptDtos.participants = participants;
                }

                return receiptDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response InsertReceipt(ReceiptDto receiptDto)
        {
            Response response = new Response();
            try
            {
                Receipt receipt = new Receipt();
                receipt = _mapper.Map<Receipt>(receiptDto);
                _DbContext.Receipt.Add(receipt);

                //save date to table receipt detail
                for (int i = 0; i < receiptDto.DocumentListId.Count; i++)
                {
                    Document document = _DbContext.Document.Where(e =>
                    e.ID == receiptDto.DocumentListId[i].idDocument).FirstOrDefault();

                    if (document == null)
                    {
                        continue;
                    }

                    ReceiptDetail receiptDetail = new ReceiptDetail()
                    {
                        IdReceiptDetail = Guid.NewGuid(),
                        IdDocument = receiptDto.DocumentListId[i].idDocument,
                        DocumentName = document.DocName,
                        Quantity = receiptDto.DocumentListId[i].Quantity,
                        Price = receiptDto.DocumentListId[i].Price,
                        Total = receiptDto.DocumentListId[i].Price * receiptDto.DocumentListId[i].Quantity,
                        IdReceipt = receiptDto.IdReceipt,
                        IdPublisher = receiptDto.DocumentListId[i].IdPublisher,
                        NamePublisher = document.Publisher,
                        Note = receiptDto.DocumentListId[i].Note,
                        Status = 0,
                        CreatedDate = receiptDto.CreatedDate
                    };
                    _DbContext.ReceiptDetail.Add(receiptDetail);

                }


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
        public ReceiptDto SearchReceipt(string code)
        {
            try
            {
                Receipt receipts = new Receipt();
                ReceiptDto receiptDtos = new ReceiptDto();

                receipts = _DbContext.Receipt.
                Where(e => e.IsDeleted == false && e.ReceiptCode == code)
                .FirstOrDefault();

                if (receipts != null)
                {
                    List<ReceiptDetail> receiptDetails = _DbContext.ReceiptDetail.Where(e =>
                    e.IdReceipt == receipts.IdReceipt).ToList();

                    receiptDtos = _mapper.Map<ReceiptDto>(receipts);
                    receiptDtos.ReceiptDetail = receiptDetails;
                }

                return receiptDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetMaxReceiptCode(string Code)
        {
            try
            {
                int ReceiptCodeMax = 0;

                List<Receipt> receipts = _DbContext.Receipt
                .Where(x => x.ReceiptCode.Substring(0, Code.Length).ToLower() == Code.ToLower() && x.IsDeleted == false).ToList();

                for (int i = 0; i < receipts.Count; i++)
                {
                    string number = receipts[i].ReceiptCode.Substring(Code.Length);
                    int numberInt = int.Parse(number);

                    if (ReceiptCodeMax < numberInt)
                    {
                        ReceiptCodeMax = numberInt;
                    }
                }
                return ReceiptCodeMax;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ReceiptDto> getAllReceipt(SortReceiptAndSearch sortReceiptAndSearch)
        {
            try
            {
                List<Receipt> receipts = new List<Receipt>();
                int countRecord = 0;

                receipts = _DbContext.Receipt.Where(e => e.IsDeleted == false)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToList();

                countRecord = receipts.Count();

                if (sortReceiptAndSearch.ReceiverName != null)
                {
                    receipts = receipts.Where(a => a.ReceiverName.ToLower()
                    .Contains(sortReceiptAndSearch.ReceiverName[0].ToLower())).ToList();
                    countRecord = receipts.Count();
                }

                if (sortReceiptAndSearch.ReceiverPosition != null)
                {
                    receipts = receipts.Where(a => a.ReceiverPosition.ToLower()
                    .Contains(sortReceiptAndSearch.ReceiverPosition[0].ToLower())).ToList();
                    countRecord = receipts.Count();
                }

                if (sortReceiptAndSearch.ReceiverUnitRepresent != null)
                {
                    receipts = receipts.Where(a => a.ReceiverUnitRepresent.ToLower()
                    .Contains(sortReceiptAndSearch.ReceiverUnitRepresent[0].ToLower())).ToList();
                    countRecord = receipts.Count();
                }

                if (sortReceiptAndSearch.DeliverName != null)
                {
                    receipts = receipts.Where(a => a.DeliverName.ToLower()
                    .Contains(sortReceiptAndSearch.DeliverName[0].ToLower())).ToList();
                    countRecord = receipts.Count();
                }

                if (sortReceiptAndSearch.DeliverPosition != null)
                {
                    receipts = receipts.Where(a => a.DeliverPosition.ToLower()
                    .Contains(sortReceiptAndSearch.DeliverPosition[0].ToLower())).ToList();
                    countRecord = receipts.Count();
                }

                if (sortReceiptAndSearch.DeliverUnitRepresent != null)
                {
                    receipts = receipts.Where(a => a.DeliverUnitRepresent.ToLower()
                    .Contains(sortReceiptAndSearch.DeliverUnitRepresent[0].ToLower())).ToList();
                    countRecord = receipts.Count();
                }


                if (sortReceiptAndSearch.sortOrder == "ascend")
                {
                    if (sortReceiptAndSearch.sortField == "receiptCode")
                    {
                        receipts.Sort((x, y) => x.ReceiptCode.Substring(2).CompareTo(y.ReceiptCode.Substring(2)));
                    }
                    if (sortReceiptAndSearch.sortField == "createdDate")
                    {
                        if (receipts.Count == 0)
                        {
                            receipts = receipts
                            .OrderBy(e => e.CreatedDate)
                            .ToList();
                        }
                    }
                }
                else
                {
                    if (sortReceiptAndSearch.sortField == "receiptCode")
                    {
                        receipts.Sort((x, y) => y.ReceiptCode.Substring(2).CompareTo(x.ReceiptCode.Substring(2)));
                    }
                    if (sortReceiptAndSearch.sortField == "createdDate")
                    {
                        if (receipts.Count == 0)
                        {
                             receipts = receipts
                            .OrderByDescending(e => e.CreatedDate)
                            .ToList();
                        }
                    }
                }

                List<ReceiptDto> receiptDtos = new List<ReceiptDto>();

                for (int i = 0; i < receipts.Count; i++)
                {
                    List<ReceiptDetail> receiptDetails = _DbContext.ReceiptDetail.Where(e =>
                    e.IdReceipt == receipts[i].IdReceipt).ToList();
                    ReceiptDto receiptDto = new ReceiptDto();

                    receiptDto = _mapper.Map<ReceiptDto>(receipts[i]);
                    receiptDto.ReceiptDetail = receiptDetails;
                    receiptDto.total = countRecord;

                    receiptDtos.Add(receiptDto);
                }
                return receiptDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetlistOriginal()   
        {
            try
            {
                List<string> Original = _DbContext.Receipt.Where(e => e.Original != null && e.IsDeleted == false).Select(e => e.Original).ToList();
                return Original.Distinct().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetlistBookStatus()
        {
            try
            {
                List<string> BookStatus = _DbContext.Receipt.Where(e => e.BookStatus != null && e.IsDeleted == false).Select(e => e.BookStatus).ToList();
                return BookStatus.Distinct().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Response> UpdateReceipt(ReceiptDto receiptDto)
        {
            using var context = _DbContext;
            using var transaction = context.Database.BeginTransaction();

            Response response = new Response();
            try
            {
                Receipt receipt = await _DbContext.Receipt.Where(e => e.IdReceipt == receiptDto.IdReceipt).FirstOrDefaultAsync();
                if(receipt is not null)
                {
                    receipt.Original = receiptDto.Original;
                    receipt.BookStatus = receiptDto.BookStatus;
                    receipt.Reason = receiptDto.Reason;
                    receipt.ReceiverName = receiptDto.ReceiverName;
                    receipt.ReceiverPosition = receiptDto.ReceiverPosition;
                    receipt.ReceiverUnitRepresent = receiptDto.ReceiverUnitRepresent;
                    receipt.DeliverName = receiptDto.DeliverName;
                    receipt.DeliverPosition = receiptDto.DeliverPosition;
                    receipt.DeliverUnitRepresent = receiptDto.DeliverUnitRepresent;
                    receipt.ReceiverIdUser = receiptDto.ReceiverIdUser;

                    _DbContext.Receipt.Update(receipt);

                    //delete list individual by iddocument and delete list receipt detail
                    var listReceiptDetail = await _DbContext.ReceiptDetail.Where(e => e.IdReceipt == receiptDto.IdReceipt).ToListAsync();
                    for(int i = 0; i < listReceiptDetail.Count; i++)
                    {
                        int quantityIndividual = listReceiptDetail[i].Quantity ?? 1;
                        int countIndividual = await _DbContext.IndividualSample.Where(e => e.IdDocument == listReceiptDetail[i].IdDocument).CountAsync();
                        if(quantityIndividual <= countIndividual)
                        {
                            var individual = await _DbContext.IndividualSample.Where(e => e.IdDocument == listReceiptDetail[i].IdDocument)
                                .OrderByDescending(e => e.CreatedDate)
                                .Take(quantityIndividual)
                                .ToListAsync();

                            _DbContext.IndividualSample.RemoveRange(individual);
                        }

                    }
                    _DbContext.ReceiptDetail.RemoveRange(listReceiptDetail);


                    //save new record to table receipt detail
                    for (int i = 0; i < receiptDto.DocumentListId.Count; i++)
                    {
                        Document document = await _DbContext.Document.Where(e =>
                        e.ID == receiptDto.DocumentListId[i].idDocument).FirstOrDefaultAsync();

                        if (document == null)
                        {
                            continue;
                        }

                        ReceiptDetail receiptDetail = new ReceiptDetail()
                        {
                            IdReceiptDetail = Guid.NewGuid(),
                            IdDocument = receiptDto.DocumentListId[i].idDocument,
                            DocumentName = document.DocName,
                            Quantity = receiptDto.DocumentListId[i].Quantity,
                            Price = receiptDto.DocumentListId[i].Price,
                            Total = receiptDto.DocumentListId[i].Price * receiptDto.DocumentListId[i].Quantity,
                            IdReceipt = receiptDto.IdReceipt,
                            IdPublisher = receiptDto.DocumentListId[i].IdPublisher,
                            NamePublisher = document.Publisher,
                            Note = receiptDto.DocumentListId[i].Note,
                            Status = 0,
                            CreatedDate = receiptDto.CreatedDate
                        };
                        await _DbContext.ReceiptDetail.AddAsync(receiptDetail);
                    }

                    //delete Participants
                    var participants = await _DbContext.Participants.Where(e => e.IdReceipt == receipt.IdReceipt).ToListAsync();
                    _DbContext.Participants.RemoveRange(participants);

                    for (int i = 0; i < receiptDto.participants.Count; i++)
                    {
                        Participants participant = new Participants()
                        {
                            Id = Guid.NewGuid(),
                            IdReceipt = receiptDto.IdReceipt,
                            Name = receiptDto.participants[i].Name,
                            Position = receiptDto.participants[i].Position,
                            Mission = receiptDto.participants[i].Mission,
                            Note = receiptDto.participants[i].Note,
                            CreatedDate = DateTime.Now,
                            Status = 0
                        };
                        await _DbContext.Participants.AddAsync(participant);
                    }

                    await _DbContext.SaveChangesAsync();
                    transaction.Commit();

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
                        Message = "Không tìm thấy ID !"
                    };
                    return response;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        #endregion
    }
}
