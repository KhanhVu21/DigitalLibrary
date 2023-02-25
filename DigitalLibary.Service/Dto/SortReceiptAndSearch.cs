using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Dto
{
    public class SortReceiptAndSearch
    {
        public SortReceiptAndSearch()
        {

        }
        public string? sortOrder { get; set; }
        public string? sortField { get; set; }
        public int page { get; set; }
        public int results { get; set; }
        public List<String>? ReceiverName { get; set; }
        public List<String>? ReceiverPosition { get; set; }
        public List<String>? ReceiverUnitRepresent { get; set; }
        public List<String>? DeliverName { get; set; }
        public List<String>? DeliverPosition { get; set; }
        public List<String>? DeliverUnitRepresent { get; set; }
    }
}
