using ISYNC_Contacts.DataLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ISYNC_Contacts.Models;
using ISYNC_Contacts.EntityLogic.Categories.Interface;

namespace ISYNC_Contacts.EntityLogic.Categories
{
    public class CategoryLogic : ICategoryLogic
    {
        private readonly IDapperContext _dapperContext;
        public CategoryLogic(IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<IEnumerable<Models.Categories>> GetCategories()
        {
            return await _dapperContext.ExecuteQuery<Models.Categories>("PS_Categories");
        }

        public async Task<int> UpdateCategory(Models.Categories _params)
        {
            return await _dapperContext.ExecuteNonQuery<Models.Categories>("PU_Category", _params);
        }

        public async Task<int> InsertCategory(Categories_Insert_Params _params)
        {
            return await _dapperContext.ExecuteNonQuery<Categories_Insert_Params>("PI_Category", _params);
        }

        public async Task<int> RemoveCategory(Categories_Remove_Params _params)
        {
            return await _dapperContext.ExecuteNonQuery<Categories_Remove_Params>("PD_Category", _params);
        }

    }
}
