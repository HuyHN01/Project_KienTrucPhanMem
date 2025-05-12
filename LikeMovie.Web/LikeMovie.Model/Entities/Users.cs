using System;
using System.Collections.Generic;

namespace LikeMovie.Model.Entities
{
    public class Users
    {
        public Users()
        {
            this.Comments = new HashSet<Comments>();
            this.Favorites = new HashSet<Favorites>();
            this.Ratings = new HashSet<Ratings>();
            this.WatchHistory = new HashSet<WatchHistory>();
            this.Payments = new HashSet<Payments>();
        }

        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public int Role { get; set; } // Cân nhắc dùng Enum cho Role sau này
        public DateTime? DateCreated { get; set; } // Đã đổi Nullable<System.DateTime>
        public bool? IsActive { get; set; }      // Đã đổi Nullable<bool>
        public string AvatarURL { get; set; }
        public int? levelVIP { get; set; }      // Cân nhắc đổi tên thành LevelVip hoặc LevelVIP
        public DateTime? TimeVIP { get; set; }     // Cân nhắc đổi tên thành TimeVip hoặc TimeVIP

        public virtual ICollection<Comments> Comments { get; set; }
        public virtual ICollection<Favorites> Favorites { get; set; }
        public virtual ICollection<Ratings> Ratings { get; set; }
        public virtual ICollection<WatchHistory> WatchHistory { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
    }
}