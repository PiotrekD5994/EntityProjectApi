using Entity.Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Infrastructure.Helper
{
    public class DbGet
    {
        private readonly SqlContext _context;

        public DbGet(SqlContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetDataFromQuery<T>(string query)
        {
            var result = new List<T>();
            using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Dynamically create an instance of the specified type (T).
                            var obj = Activator.CreateInstance<T>();
                            // Iterate over all properties of the specified type (T).
                            foreach (var prop in typeof(T).GetProperties())
                            {
                                // Check if the current SQL column's value is not DBNull.
                                if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                                {
                                    // Set the property value of the created object based on the data from the SQL result.
                                    prop.SetValue(obj, reader[prop.Name]);
                                }
                            }
                            result.Add(obj);
                        }
                    }
                }
            }
            return result;
        }
    }
}
