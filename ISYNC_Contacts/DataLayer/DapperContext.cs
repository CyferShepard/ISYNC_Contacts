using Dapper;
using ISYNC_Contacts.DataLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISYNC_Contacts.DBLayer
{
    public class DapperContext : IDapperContext
    {
        private readonly IDbConnection _dbConnection;

        public DapperContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }



        public async Task<IEnumerable<T>> ExecuteQuery<T>(string proc)
        {
            return await _dbConnection.QueryAsync<T>(proc, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<T>> ExecuteQuery<T, P>(string proc, P Parameters)
        {
            return await _dbConnection.QueryAsync<T>(proc, Parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ExecuteNonQuery(string proc)
        {
            return await _dbConnection.ExecuteAsync(proc, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ExecuteNonQuery<P>(string proc, P Parameters)
        {
            return await _dbConnection.ExecuteAsync(proc, Parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
