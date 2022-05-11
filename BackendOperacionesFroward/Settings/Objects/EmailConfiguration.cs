using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Settings.Objects
{
    public class EmailConfiguration
    {
        public string USERNAME { get; set; }

        public string PASSWORD { get; set; }

        public string SMTP_HOST { get; set; }

        public int PORT { get; set; }
    }
}
