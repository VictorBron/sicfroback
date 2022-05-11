using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Settings.Objects
{
    public class ExpirationTimesConfiguration
    {
        public int CLIENT { get; set; }

        public int FROWARD_ADMIN { get; set; }

        public int FROWARD_READER { get; set; }

        public int FROWARD_SUPERVISOR { get; set; }

    }
}
