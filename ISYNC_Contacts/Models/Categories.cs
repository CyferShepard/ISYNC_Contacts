using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ISYNC_Contacts.Models
{
    public class Categories
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; }
    }

    public class Categories_Insert_Params
    {
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; }
    }

    public class Categories_Remove_Params
    {
        public int ID { get; set; }
    }
}
