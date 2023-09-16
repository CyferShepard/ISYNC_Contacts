using ISYNC_Contacts.DataLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ISYNC_Contacts.Models;
using ISYNC_Contacts.EntityLogic.Contacts.Interface;

namespace ISYNC_Contacts.EntityLogic.Contacts
{
    public class ContactsLogic : IContactsLogic
    {
        private readonly IDapperContext _dapperContext;
        public ContactsLogic(IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<IEnumerable<Models.Contacts>> GetContacts()
        {
            return await _dapperContext.ExecuteQuery<Models.Contacts>("PS_Contacts");
        }

        public async Task<IEnumerable<Models.Contacts>> GetContacts(Contacts_Search_Params _params)
        {
            return await _dapperContext.ExecuteQuery<Models.Contacts, Contacts_Search_Params>("PS_Contacts", _params);
        }

        public async Task<int> UpdateContact(Models.Contacts_Update_Params contact)
        {
            return await _dapperContext.ExecuteNonQuery<Models.Contacts_Update_Params>("PU_Contact", contact);
        }

        public async Task<int> InsertContact(Contacts_Insert_Params _params)
        {
            return await _dapperContext.ExecuteNonQuery<Contacts_Insert_Params>("PI_Contact", _params);
        }

        public async Task<int> RemoveContact(Contacts_Remove_Params _params)
        {
            return await _dapperContext.ExecuteNonQuery<Contacts_Remove_Params>("PD_Contact", _params);
        }


    }
}
