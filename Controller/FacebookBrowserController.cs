using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool_Facebook.Model;

namespace Tool_Facebook.Controller
{
    public class FacebookBrowserController
    {
        AccountModel account;
        Random random;
        public FacebookBrowserController(AccountModel account)
        {
            this.account = account;
            random = new Random();
        }
        public ResultModel LoginCookie()
        {
            return ResultModel.Success;
        }
        public ResultModel LoginPass2FA()
        {
            return ResultModel.Success;
        }
    }
}
