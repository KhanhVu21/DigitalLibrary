using System;
using System.Collections;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiSpineBook
    {
        public Guid IdDocument { get; set; }
        public Guid IdIndividual { get; set; }
        public string NametypeBook { get; set; }
        public string NameCategorySign { get; set; }
        public string NumIndividual { get; set; }
        public string Barcode { get; set; }
        public String EncryptCode { get; set; }
        public String DocName { get; set; }
    }
    public class CustomApiSpineBookByGroup
    {
        public string? Title { get; set; }
        public IEnumerable<CustomApiSpineBook> ListSpine { get; set; }
    }
}
