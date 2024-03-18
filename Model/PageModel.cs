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
	public class PageModel
	{
		[JsonIgnore]
		public ChromeDriver driver { get; set; } = null;
		public string C_UIDVia { get; set; }
		public string C_IDPage { get; set; }
		public string C_NamePage { get; set; }
		public string C_Follower { get; set; }
		public string C_StatusPage { get; set; }
		public string C_CookieVia { get; set; }
		public string C_FolderPage { get; set; }
		public string C_ProxyPage { get; set; }
		[JsonIgnore]
		public DataGridViewRow C_Row { get; set; }
	}
}
