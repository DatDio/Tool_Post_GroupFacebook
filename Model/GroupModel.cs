using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_Facebook.Model
{
    public class GroupModel
    {
        public string C_IDGroup { get; set; }
        public string C_UIDGroup { get; set; }
        public string C_NameGroup { get; set; }
        public string C_UIDVia { get; set; }
        public string C_PostID { get; set; }
        public string C_StatusGroup { get; set; }
        public string C_CreatedPost { get; set; }
        public string C_TimeEditPost { get; set; }
        public string C_MemberGroup { get; set; }
        public string C_TypeGroup { get; set; }
        public string C_Censorship { get; set; }
        public string C_FolderGroup { get; set; } 
        public DataGridViewRow C_Row { get; set; }
    }
}
