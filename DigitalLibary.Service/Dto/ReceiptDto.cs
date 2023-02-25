using DigitalLibary.Data.Entity;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Dto
{
    public class ReceiptDto
    {
        public ReceiptDto()
        {

        }
        public Guid IdReceipt { get; set; }
        public Guid ReceiverIdUser { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPosition { get; set; }
        public string? ReceiverUnitRepresent { get; set; }
        public string? DeliverName { get; set; }
        public string? DeliverPosition { get; set; }
        public string? DeliverUnitRepresent { get; set; }
        public string? ReceiptCode { get; set; }
        public string? Original { get; set; }
        public string? BookStatus { get; set; }
        public string? Reason { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }
        public bool IsDeleted { get; set; }

        //extention
        public string SignIndividual { get; set; }
        public Guid IdCategory { get; set; }
        public Guid IdStock { get; set; }
        public List<ReceiptDetail> ReceiptDetail { get; set; }
        public List<DocumentListId> DocumentListId { get; set; }
        public List<Participants> participants { get; set; }
        public int total { get; set; }
    }
    public class DocumentListId
    {
        public Guid idDocument { get; set; }
        public string? DocumentName { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float Total { get; set; }
        public Guid IdPublisher { get; set; }
        public string? NamePublisher { get; set; }
        public string? Note { get; set; }
        public string? SignIndividual { get; set; }
        public Guid IdCategory { get; set; }
        public Guid IdStock { get; set; }

        // participants
        public String? Name { get; set; }
        public String? Position { get; set; }
        public String? Mission { get; set; }
        public String? NoteParticipants { get; set; }
    }
}
