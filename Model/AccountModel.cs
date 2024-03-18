using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_Facebook.Model
{
    public class AccountModel
    {
        [JsonIgnore]
        public ChromeDriver driver { get; set; } = null;
        public string C_UID { get; set; }
        public string C_Password { get; set; }
        public string C_Email { get; set; }
        public string C_PassEmail { get; set; }
        public string C_2FA { get; set; }
        public string C_Status { get; set; }
        public string C_Folder { get; set; }
        public string C_Cookie { get; set; }
        public string C_Token { get; set; }
        public string C_Proxy { get; set; }
        public string C_UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0";
		public string C_GPMID { get; set; }
		[JsonIgnore]
        public DataGridViewRow C_Row { get; set; }
    }
}
