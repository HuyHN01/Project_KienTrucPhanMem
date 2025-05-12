using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LikeMovie.Model.Entities
{
    public class Comments
    {
        public int CommentID { get; set; }
        public int MovieID { get; set; }
        public int UserID { get; set; }
        public string CommentText { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? Likes { get; set; }
        public int? Rating { get; set; }
        public virtual Users User { get; set; }
        public virtual Movies Movie { get; set; }
    }
}
