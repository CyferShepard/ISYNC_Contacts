using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISYNC_Contacts.Models
{
    public class Contacts
    {
        public int ID { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string CellNumber { get; set; } = string.Empty;
        public string EMail { get; set; } = string.Empty;
        public byte[] Image { get; set; } = new byte[0];
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool Active { get; set; }

        public static implicit operator Contacts(int v)
        {
            throw new NotImplementedException();
        }
    }


    public class Contacts_Insert_Params
    {
        public int CategoryId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string CellNumber { get; set; } = string.Empty;
        public string EMail { get; set; } = string.Empty;
        public byte[] Image { get; set; } = new byte[0];

    }

    public class Contacts_Update_Params : Contacts_Insert_Params
    {
        public int ID { get; set; }
        public bool Active { get; set; }
    }

    public class Contacts_Remove_Params
    {
        public int ID { get; set; }
    }



    public class Contacts_Search_Params
    {
        public int ID { get; set; }
        public bool? Active { get; set; }
        public string? FirstName { get; set; }
        public int? CategoryId { get; set; }
        public string? EMail { get; set; }


    }
}
