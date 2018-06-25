using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTextSampleApp
{
    class Entity
    {
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public string RecipientsCSV { get; set; }
        public List<string> Recipients { get; set; }
    }
}
