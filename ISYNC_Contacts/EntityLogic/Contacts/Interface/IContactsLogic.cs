using ISYNC_Contacts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISYNC_Contacts.EntityLogic.Contacts.Interface
{
    public interface IContactsLogic
    {
        Task<IEnumerable<Models.Contacts>> GetContacts();
        Task<IEnumerable<Models.Contacts>> GetContacts(Contacts_Search_Params _params);

        Task<int> UpdateContact(Models.Contacts_Update_Params contact);
        Task<int> InsertContact(Contacts_Insert_Params _params);
        Task<int> RemoveContact(Contacts_Remove_Params _params);

    }
}
