using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LikeMovies.Models
{
    public static class MomoConfig
    {
        public static string PartnerCode = "MOMO";
        public static string AccessKey = "F8BBA842ECF85";
        public static string SecretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
        public static string Endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        public static string ReturnUrl = "https://localhost:44310/MuaVip/ReturnUrl";
        public static string NotifyUrl = "https://localhost:44310/MuaVip/NotifyUrl"; // URL callback for local development
    }
}