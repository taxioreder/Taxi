using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMobaileTaxi.Model
{
    public class ResponseAppS
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public object ResponseStr { get; set; }
        public object ResponseStr1 { get; set; }

        public ResponseAppS(string status, string description, object responseStr, object responseStr1 = null)
        {
            Status = status;
            Description = description;
            ResponseStr = responseStr;
            ResponseStr1 = responseStr1;
        }
    }
}
