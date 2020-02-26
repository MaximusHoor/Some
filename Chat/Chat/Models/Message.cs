using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_SERVER.Models
{
    public class Message
    {
        public string Header { get; set; }

        public User ToUser { get; set; }

        public User FromUser { get; set; }

        public string Message_ { get; set; }

        public string JsonData { get; set; }

    }
}
