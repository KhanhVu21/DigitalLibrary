using DigitalLibary.Data.Entity;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class ListBookByCategory
    {
        public Document Document { get; set; }
        public List<DocumentAvatar> listAvatar { get; set; }
        public string NameCategory { get; set; }
        public Guid IdCategory { get; set; }
    }
    public class DocumentAndAvatar
    {
        public Document Document { get; set; }
        public List<DocumentAvatar> listDocumentAndAvatar { get; set; }
    }
}
