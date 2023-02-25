using DigitalLibary.Data.Data;
using DigitalLibary.Data.Entity;
using DigitalLibary.Service.Common.FormatApi;
using DigitalLibary.Service.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLibary.Service.Repository.RepositoryIPL
{
    public class CalculateFolder: ICalculateFolder
    {
        #region Variables
        public DataContext _DbContext;

        #endregion

        #region Constructors
        public CalculateFolder(DataContext DbContext)
        {
            _DbContext = DbContext;
        }
        #endregion

        public List<CalculateDatabase> calculateDatabases()
        {
            List<CalculateDatabase> calculateDatabases = new List<CalculateDatabase>();
            var result = _DbContext.GetDynamicResult("exec [dbo].[Sp_CountCapacityDatabase]");

            for (int i = 0; i < result.Count(); i++)
            {
                var str1 = result.ElementAt(i);
                var jsonString = JsonConvert.SerializeObject(str1);
                var resultObject = JsonConvert.DeserializeObject<CalculateDatabase>(jsonString);

                calculateDatabases.Add(resultObject);
            }
            return calculateDatabases;
        }
    }
}
