using LikeMovies.Models;
using LikeMovies.Controllers; 
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace LikeMovies.Controllers
{
    public class UserController : Controller
    {
        MovieEntities db = new MovieEntities();
        LikeMovieController likeMovieController = new LikeMovieController();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThongTinCaNhan()
        {
            if (Session["TaiKhoan"] != null)
            {
                var userId = ((Users)Session["TaiKhoan"]).UserID;
                var user = db.Users.SingleOrDefault(u => u.UserID == userId);

                if (user != null)
                {
                    return View(user);
                }
            }

            return RedirectToAction("DangNhap", "LikeMovie");
        }

        [HttpGet]
        public ActionResult GetAvatars()
        {
            var avatars = Directory.GetFiles(Server.MapPath("~/Images/Avatar"))
                                   .Select(Path.GetFileName)
                                   .ToList();
            return Json(avatars, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateAvatar(string fileName)
        {
            if (Session["TaiKhoan"] != null)
            {
                var khachHang = (Users)Session["TaiKhoan"];
                khachHang.AvatarURL = "~/Images/Avatar/" + fileName;

                // Cập nhật cơ sở dữ liệu với URL avatar mới
                var user = db.Users.Find(khachHang.UserID);
                if (user != null)
                {
                    user.AvatarURL = khachHang.AvatarURL;
                    db.SaveChanges();
                }

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        public ActionResult ThayDoiMatKhau(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu và xác nhận mật khẩu không khớp.");
                return Json(new { success = false, message = "Mật khẩu và xác nhận mật khẩu không khớp." });
            }

            var user = (Users)Session["TaiKhoan"];
            if (user != null)
            {
                string hashedCurrentPassword = likeMovieController.HashPassword(currentPassword);

                if (user.PasswordHash != hashedCurrentPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu hiện tại không chính xác.");
                    return Json(new { success = false, message = "Mật khẩu hiện tại không chính xác." });
                }

                string otp = GenerateOtp();
                Session["OTP"] = otp;

                try
                {
                    // Fetch the email from the database
                    SendOtpEmail(user.UserID, otp);

                    string hashedNewPassword = likeMovieController.HashPassword(newPassword);
                    Session["NewPassword"] = hashedNewPassword;
                    Session["UserId"] = user.UserID;

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Gửi email thất bại: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Người dùng không được tìm thấy." });
        }


        [HttpGet]
            public ActionResult VerifyOtp()
            {
                return View();
            }

            [HttpPost]
            public ActionResult VerifyOtp(string otp)
            {
                var storedOtp = Session["OTP"]?.ToString();
                if (otp != storedOtp)
                {
                    ModelState.AddModelError("", "Mã OTP không chính xác.");
                    return View();
                }

                var userId = (int?)Session["UserId"];
                var newPassword = Session["NewPassword"]?.ToString();
                if (userId.HasValue && !string.IsNullOrEmpty(newPassword))
                {
                    var user = db.Users.Find(userId.Value);
                    if (user != null)
                    {
                        user.PasswordHash = newPassword;
                        db.SaveChanges();
                        Session.Remove("OTP");
                        Session.Remove("NewPassword");
                        Session.Remove("UserId");
                    }
                }
                return RedirectToAction("Index", "User");
            }

            private string GenerateOtp()
            {
                var random = new Random();
                return random.Next(100000, 999999).ToString();
            }

            private void SendOtpEmail(int userId, string otp)
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    string email = user.Email;
                    string username = "likemovieshcl@gmail.com";
                    string password = "fqnb rebj dbwg tyzt";
                    try
                    {
                        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential(username, password),
                            EnableSsl = true
                        };

                        MailMessage mailMessage = new MailMessage
                        {
                            From = new MailAddress(username),
                            Subject = "Mã xác nhận OTP",
                            Body = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                background-color: #121212;
                                color: #ffffff;
                                padding: 20px;
                                text-align: center;
                            }}
                            .container {{
                                padding: 20px;
                                border: 1px solid #FFA500;
                                border-radius: 10px;
                                box-shadow: 0 0 10px rgba(255, 165, 0, 0.5);
                            }}
                            h2 {{
                                color: #FFA500;
                            }}
                            p {{
                                margin-top: 20px;
                                font-size: 1.1em;
                            }}
                            .otp {{
                                font-size: 1.5em;
                                font-weight: bold;
                                color: #FFA500;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h2>Mã xác nhận OTP</h2>
                            <p>Xin chào,</p>
                            <p>Mã OTP của bạn là: <span class='otp'>{otp}</span></p>
                            <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>
                        </div>
                    </body>
                    </html>",
                            IsBodyHtml = true
                        };

                        mailMessage.To.Add(email);
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Gửi email thất bại: " + ex.Message);
                        throw;
                    }
                }
            }
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var user = db.Users.SingleOrDefault(u => u.Email == email);
            if (user != null)
            {
                string otp = GenerateOtp();
                Session["OTP"] = otp;
                Session["UserId"] = user.UserID;

                try
                {
                    SendOtpEmail(user.UserID, otp);
                    return RedirectToAction("ResetPassword");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Failed to send email: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Email not found.");
            }

            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string otp, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            var storedOtp = Session["OTP"]?.ToString();
            if (otp != storedOtp)
            {
                ModelState.AddModelError("", "Invalid OTP.");
                return View();
            }

            var userId = (int?)Session["UserId"];
            if (userId.HasValue)
            {
                var user = db.Users.Find(userId.Value);
                if (user != null)
                {
                    user.PasswordHash = likeMovieController.HashPassword(newPassword);
                    db.SaveChanges();

                    // Clear the session
                    Session.Remove("OTP");
                    Session.Remove("UserId");

                    return RedirectToAction("DangNhap", "LikeMovie");
                }
            }

            ModelState.AddModelError("", "Failed to reset password.");
            return View();
        }
        public ActionResult GiaHanVip()
        {
            if (Session["TaiKhoan"] == null || string.IsNullOrEmpty(Session["TaiKhoan"].ToString()))
            {
                return RedirectToAction("DangNhap", "LikeMovie");
            }

            var khachHang = (Users)Session["TaiKhoan"];
            var levelVip = khachHang.levelVIP;

            // Lọc các gói VIP dựa trên mức VIP của người dùng
            var subscriptionPlans = db.SubscriptionPlans
                .Where(s => s.PlanID == levelVip) // Lọc theo mức VIP
                .Select(s => new SubscriptionPlanViewModel
                {
                    planID = s.PlanID,
                    PlanName = s.PlanName,
                    Price = s.Price,
                    DevicesLimit = s.DevicesLimit ?? 0,
                    AdFree = s.AdFree ?? false,
                    VipContentAccess = s.VipContentAccess ?? false
                })
                .ToList();

            return View(subscriptionPlans);
        }

        [HttpPost]
        public ActionResult GiaHanVip(int PlanID, int SubscriptionDuration, FormCollection f)
        {
            if (Session["TaiKhoan"] == null || string.IsNullOrEmpty(Session["TaiKhoan"].ToString()))
            {
                return RedirectToAction("DangNhap", "LikeMovie");
            }

            var plan = db.SubscriptionPlans.Find(PlanID);
            if (plan == null)
            {
                return HttpNotFound();
            }

            decimal totalAmount = plan.Price * SubscriptionDuration;
            Session["TotalAmount"] = totalAmount;
            Session["PlanID"] = PlanID;
            Session["SubscriptionDuration"] = SubscriptionDuration;

            int paymentMethod = Convert.ToInt32(f["thanhtoan"]);
            switch (paymentMethod)
            {
                case 1:
                    return RedirectToAction("PaymentVNPAY", "MuaVip");

                case 2:
                    return RedirectToAction("PaymentMomo", "MuaVip");

                default:
                    return View("Error");
            }
        }
    }
}