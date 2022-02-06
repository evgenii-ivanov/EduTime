using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTime.ViewModels.Token
{
    public class TokenVm : BaseVm
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
