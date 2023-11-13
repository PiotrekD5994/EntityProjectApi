﻿using Entity.Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Entity.Infrastructure.Helper
{
    public class DbGet
    {
        private readonly SqlContext _context;

        public DbGet(SqlContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetDataFromQueryWithSqlValue<T>(string query, List<SqlParameter> parameters)
        {
            var result = new List<T>();
            var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);

            await connection.OpenAsync();
            var command = new SqlCommand(query, connection);

            foreach (var param in parameters)
            {
                command.Parameters.Add(param);
            }

            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var obj = Activator.CreateInstance<T>();
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                    {
                        prop.SetValue(obj, reader[prop.Name]);
                    }
                }
                result.Add(obj);
            }
            return result;
        }
        public async Task<List<T>> GetDataFromQuery<T>(string query)
        {
            var result = new List<T>();

            var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);

            await connection.OpenAsync();

            var command = new SqlCommand(query, connection);

            var reader = await command.ExecuteReaderAsync();

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
            return result;
        }
    }
}
