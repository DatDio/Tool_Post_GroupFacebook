using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Facebook.Data
{
	public class tblPage
	{
		[Key]
		public string C_IDPage { get; set; }
		public string C_NamePage { get; set; }
		public string C_Follower { get; set; }
		public string C_StatusPage { get; set; }
		public string C_UID { get; set; }
		public virtual tblVia tblVia { get; set; }
	}
}
