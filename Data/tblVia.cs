using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Facebook.Data
{
	[Table("tblViaPage")]
	public class tblVia
	{
		[Key]
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
		public string C_UserAgent { get; set; }

		public virtual ObservableCollectionListSource<tblPage> Pages { get; set; }
	}
}
