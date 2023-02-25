using DigitalLibary.Service.Common.FormatApi;
using System.Collections.Generic;

namespace DigitalLibary.Service.Repository.IRepository
{
    public interface ICalculateFolder
    {
        public List<CalculateDatabase> calculateDatabases();
    }
}
