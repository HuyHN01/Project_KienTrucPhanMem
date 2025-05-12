using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LikeMovie.Model.Entities
{
    public class AdminMovie
    {
        public int AdminID { get; set; }
        public string NameAd { get; set; }
        public string Email { get; set; }
        public string UsernameAd { get; set; }
        public string PasswordAd { get; set; }
        public string AvartarURL { get; set; }
    }
}