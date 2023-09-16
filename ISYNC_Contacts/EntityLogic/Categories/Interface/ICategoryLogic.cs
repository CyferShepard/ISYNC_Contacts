using ISYNC_Contacts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISYNC_Contacts.EntityLogic.Categories.Interface
{
    public interface ICategoryLogic
    {
        Task<IEnumerable<Models.Categories>> GetCategories();
        Task<int> UpdateCategory(Models.Categories _params);
        Task<int> InsertCategory(Categories_Insert_Params _params);
        Task<int> RemoveCategory(Categories_Remove_Params _params);
    }
}
