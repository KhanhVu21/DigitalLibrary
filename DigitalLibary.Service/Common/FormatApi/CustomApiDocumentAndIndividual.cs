using DigitalLibary.Data.Entity;
using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiDocumentAndIndividual
    {
        public CustomApiDocumentAndIndividual()
        {

        }
        public List<IndividualSample> individuals { get; set; }
        public Data.Entity.Document document { get; set; }
        public int totalCount { get; set; }
    }
}
