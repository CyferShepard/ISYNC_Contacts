using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISYNC_Contacts.DataLayer.Interface
{
    public interface IDapperContext
    {
        Task<IEnumerable<T>> ExecuteQuery<T>(string proc);
        Task<IEnumerable<T>> ExecuteQuery<T, P>(string proc, P Parameters);
        Task<int> ExecuteNonQuery(string proc);
        Task<int> ExecuteNonQuery<P>(string proc, P Parameters);
    }
}
