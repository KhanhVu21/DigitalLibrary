using System;
using System.Collections.Generic;

namespace DigitalLibary.Service.Common.FormatApi
{
    public class CustomApiIndividual
    {
        public CustomApiIndividual()
        {

        }
        public List<stock> stocks { get; set; }
    }
    public class stock
    {
        public stock()
        {

        }
        public Guid idStock { get; set; }
        public string individualName { get; set; }
    }
  
}
