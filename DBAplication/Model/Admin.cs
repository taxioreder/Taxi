using System;
using System.Collections.Generic;
using System.Text;

namespace DBAplication.Model
{
    public class Admin
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string KeyAuthorized { get; set; }
    }
}
