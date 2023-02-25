using DigitalLibary.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Data.SqlClient;

namespace DigitalLibary.Data.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
                
        }
        public IEnumerable<dynamic> GetDynamicResult(string commandText, params SqlParameter[] parameters)
        {
            // Get the connection from DbContext
            var connection = Database.GetDbConnection();

            // Open the connection if isn't open
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Connection = connection;          

                if (parameters?.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                using (var dataReader = command.ExecuteReader())
                {
                    // List for column names
                    var names = new List<string>();

                    if (dataReader.HasRows)
                    {
                        // Add column names to list
                        for (var i = 0; i < dataReader.VisibleFieldCount; i++)
                        {
                            names.Add(dataReader.GetName(i));
                        }

                        while (dataReader.Read())
                        {
                            // Create the dynamic result for each row
                            var result = new ExpandoObject() as IDictionary<string, object>;

                            foreach (var name in names)
                            {
                                // Add key-value pair
                                // key = column name
                                // value = column value
                                result.Add(name, dataReader[name]);
                            }
                            command.Parameters.Clear();
                            yield return result;
                        }
                    }
                }            
            }
        }

        public DbSet<User> User { get; set; }
        public DbSet<User_Delete> User_Delete { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User_Role> User_Role { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DocumentAvatar> DocumentAvatar { get; set; }
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<RestDate> RestDate { get; set; }
        public DbSet<SchoolYear> SchoolYear { get; set; }
        public DbSet<ContactAndIntroduction> ContactAndIntroduction { get; set; }
        public DbSet<DocumentStock> DocumentStock { get; set; }
        public DbSet<IndividualSample> IndividualSample { get; set; }
        public DbSet<IndividualSampleDeleted> IndividualSampleDeleted { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategorySign> CategorySign { get; set; }
        public DbSet<CategoryPublisher> CategoryPublisher { get; set; }
        public DbSet<CategorySupplier> CategorySupplier { get; set; }
        public DbSet<DocumentInvoice> DocumentInvoice { get; set; }
        public DbSet<DocumentInvoiceDetail> DocumentInvoiceDetail { get; set; }
        public DbSet<Slide> Slide { get; set; }
        public DbSet<Receipt> Receipt { get; set; }
        public DbSet<ReceiptDetail> ReceiptDetail { get; set; }
        public DbSet<CategorySign_V1> CategorySign_V1 { get; set; }
        public DbSet<Diary> Diary { get; set; }
        public DbSet<BookNameEncrypt> BookNameEncrypt { get; set; }
        public DbSet<Participants> Participants { get; set; }
        public DbSet<StatusBook> StatusBook { get; set; }
        public DbSet<AuditReceipt> AuditReceipt { get; set; }
        public DbSet<AuditorList> AuditorList { get; set; }
        public DbSet<AuditMethod> AuditMethod { get; set; }
        public DbSet<AuditBookList> AuditBookList { get; set; }

    }
}
