using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTime.Foundation.Options
{
    public class SmtpConnectionOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool Ssl { get; set; }

        public bool SupportsAuthentication { get; set; }
        public bool AllowUntrustedSsl { get; set; }
    }
}
