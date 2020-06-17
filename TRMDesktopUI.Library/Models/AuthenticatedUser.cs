using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Models
{
    //the following class will enable us to get token from the api
    public class AuthenticatedUser
    {
        public string Access_Token { get; set; }

        public string UserName { get; set; }
    }
}
