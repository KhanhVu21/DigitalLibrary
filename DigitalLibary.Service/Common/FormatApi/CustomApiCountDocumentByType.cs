using DigitalLibary.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiCountDocumentByType
    {
        public CustomApiCountDocumentByType()
        {

        }
        public DocumentType documentType { get; set; }
        public int count { get; set; }
    }
}
