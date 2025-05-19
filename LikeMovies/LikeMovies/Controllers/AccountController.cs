using Facebook;
using LikeMovies.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LikeMovies.Services.Interfaces;
using System.Configuration;
using LikeMovies.Services;

namespace LikeMovies.Controllers
{
    public class AccountController : Controller
    {
        MovieEntities db = new MovieEntities();
        private readonly IGoogleOAuthService _googleOAuthService;
        private readonly IFacebookOAuthService _facebookOAuthService;
        public AccountController()
        {
            _googleOAuthService = new GoogleOAuthService();
            _facebookOAuthService = new FacebookOAuthService();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        public ActionResult DangNhap(string returnUrl)
        {
            string googleRedirectUri = ConfigurationManager.AppSettings["GoogleRedirectUri"];
            if (string.IsNullOrEmpty(googleRedirectUri)) googleRedirectUri = Url.Action("LoginGoogleCallback", "Account", null, Request.Url.Scheme);
            ViewBag.GoogleLoginUrl = _googleOAuthService.GenerateAuthorizationUrl(googleRedirectUri);

            string facebookRedirectUri = ConfigurationManager.AppSettings["FacebookRedirectUri"];
            if (string.IsNullOrEmpty(facebookRedirectUri)) facebookRedirectUri = Url.Action("LoginFacebookCallback", "Account", null, Request.Url.Scheme);
            ViewBag.FacebookLoginUrl = _facebookOAuthService.GenerateAuthorizationUrl(facebookRedirectUri);

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection f, string returnUrl)
        {
            var sTenDN = f["TenDN"];
            var sMatKhau = f["MatKhau"];
            if (string.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (string.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {
                string matkhau = HashPassword(sMatKhau);
                Users kh = db.Users.SingleOrDefault(n => n.UserName == sTenDN && n.PasswordHash == matkhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoan"] = kh;

                    if (f["remember"] != null && f["remember"].Contains("true"))
                    {
                        Response.Cookies["TenDN"].Value = sTenDN;
                        Response.Cookies["MatKhau"].Value = sMatKhau;
                        Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(1);
                        Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(1);
                    }
                    else
                    {
                        Response.Cookies["TenDN"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["MatKhau"].Expires = DateTime.Now.AddDays(-1);
                    }

                    // Điều hướng đến returnUrl nếu có, nếu không trở về trang chủ
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "LikeMovie");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            // Nếu đăng nhập thất bại, trả lại view đăng nhập
            ViewBag.ReturnUrl = returnUrl; // Truyền lại returnUrl để thử lại
            return View();
        }


        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "LikeMovie");

        }
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(Users kh, HttpPostedFileBase Avatar)
        {
            if (ModelState.IsValid)
            {
                if (Avatar != null && Avatar.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(Avatar.FileName);
                    var directoryPath = Server.MapPath("~/Images/Avatar");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    var path = Path.Combine(directoryPath, fileName);
                    Avatar.SaveAs(path);
                    kh.AvatarURL = "~/Images/Avatar/" + fileName;
                }
                else
                {
                    // Gán ảnh đại diện mặc định nếu không có ảnh được tải lên
                    kh.AvatarURL = "~/Images/Avatar/default_user.png";
                }

                kh.PasswordHash = HashPassword(kh.PasswordHash);
                kh.DateCreated = DateTime.Now;  // Gán giá trị cho thuộc tính NgayDangKy
                db.Users.Add(kh);
                db.SaveChanges();
                SendMail(kh.Email, kh.UserName);
                return RedirectToAction("DangNhap");
            }
            return View(kh);
        }
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Chuyển byte thành chuỗi hex
                }
                return builder.ToString();
            }
        }
        [HttpGet]
        public ActionResult DangKy_KiemLoi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy_KiemLoi(FormCollection f, Users kh)
        {
            string taikhoan = f["UserName"];
            string email = f["Email"];
            if (db.Users.SingleOrDefault(n => n.UserName == taikhoan) != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            }
            else if (db.Users.SingleOrDefault(n => n.Email == email) != null)
            {
                ViewBag.ThongBao = "Email đã được sử dụng";
            }
            else if (ModelState.IsValid)
            {
                db.Users.Add(kh);
                db.SaveChanges();
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        private void SendMail(string email, string hoTen)
        {
            string username = "likemovieshcl@gmail.com";
            string password = "fqnb rebj dbwg tyzt";
            try
            {
                // Cấu hình thông tin máy chủ SMTP
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                // Tạo đối tượng email
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(username),
                    Subject = "Xác nhận đăng ký thành công",
                    Body = $@" <html> <head> <style> body {{ font-family: Arial, sans-serif; color: #333; }} .container {{ padding: 20px; border: 1px solid #ddd; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }} .header {{ background-color: #FFA500; color: #fff; padding: 10px; text-align: center; border-radius: 5px 5px 0 0; }} .content {{ margin-top: 20px; }} .footer {{ margin-top: 20px; text-align: center; font-size: 0.9em; color: #888; }} </style> </head> <body> <div class='container'> <div class='header'> <h2>Xác nhận đăng ký thành công</h2> </div> <div class='content'> <p>Xin chào {hoTen},</p> <p>Cảm ơn bạn đã đăng ký tài khoản tại trang web của chúng tôi.</p> </div> <div class='footer'> <p>Trân trọng,</p> <p>Đội ngũ hỗ trợ</p> </div> </div> </body> </html>",
                    IsBodyHtml = true // Nếu email có định dạng HTML thì đổi thành true
                };

                // Thêm địa chỉ người nhận
                mailMessage.To.Add(email);

                // Gửi email
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra
                Console.WriteLine("Gửi email thất bại: " + ex.Message);
            }
        }
        public async Task<ActionResult> LoginGoogleCallback(string code, string error, string state)
        {
            if (!string.IsNullOrEmpty(error))
            {
                TempData["ErrorMessageOauth"] = $"Lỗi đăng nhập Google: {error}";
                return RedirectToAction("DangNhap");
            }
            if (string.IsNullOrEmpty(code))
            {
                TempData["ErrorMessageOauth"] = "Không nhận được mã xác thực từ Google.";
                return RedirectToAction("DangNhap");
            }

            try
            {
                string redirectUri = ConfigurationManager.AppSettings["GoogleRedirectUri"];
                if (string.IsNullOrEmpty(redirectUri)) redirectUri = Url.Action("LoginGoogleCallback", "Account", null, Request.Url.Scheme);

                var accessToken = await _googleOAuthService.ExchangeCodeForAccessTokenAsync(code, redirectUri);
                dynamic googleUserInfo = await _googleOAuthService.GetUserInfoAsync(accessToken);

                string email = googleUserInfo.email?.ToString();
                string name = googleUserInfo.name?.ToString();
                string avatarUrl = googleUserInfo.picture?.ToString();

                if (string.IsNullOrEmpty(email))
                {
                    TempData["ErrorMessageOauth"] = "Không thể lấy email từ Google.";
                    return RedirectToAction("DangNhap");
                }

                Users kh = db.Users.SingleOrDefault(u => u.Email == email);
                if (kh == null)
                {
                    string baseUsername = email.Split('@')[0].Replace(".", "").Replace("_", "");
                    baseUsername = baseUsername.Length > 15 ? baseUsername.Substring(0, 15) : baseUsername;
                    string finalUsername = baseUsername;
                    int suffix = 1;
                    while (db.Users.Any(u => u.UserName == finalUsername))
                    {
                        finalUsername = baseUsername + suffix;
                        if (finalUsername.Length > 20) finalUsername = finalUsername.Substring(0, 20);
                        suffix++;
                    }

                    kh = new Users
                    {
                        Email = email,
                        UserName = finalUsername,
                        PasswordHash = HashPassword(Guid.NewGuid().ToString("N").Substring(0, 12)), // Mật khẩu ngẫu nhiên mạnh
                        Role = 0,
                        DateCreated = DateTime.Now,
                        AvatarURL = avatarUrl ?? "~/Images/Avatar/default_user.png"
                    };
                    db.Users.Add(kh);
                    await db.SaveChangesAsync(); // Sử dụng SaveChangesAsync
                }
                Session["TaiKhoan"] = kh;
                return RedirectToAction("Index", "LikeMovie");
            }
            catch (DbEntityValidationException dbEx)
            {
                var errorMessages = dbEx.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => $"Property: {x.PropertyName} Error: {x.ErrorMessage}");
                TempData["ErrorMessageOauth"] = "Lỗi xác thực dữ liệu khi lưu người dùng: " + string.Join("; ", errorMessages);
                return RedirectToAction("DangNhap");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessageOauth"] = $"Đã có lỗi xảy ra khi đăng nhập bằng Google: {ex.Message}";
                return RedirectToAction("DangNhap");
            }
        }
        [HttpPost]
        public ActionResult SaveUserData(Users user)
        {
            using (var context = new MovieEntities())
            {
                var existingUser = context.Users.SingleOrDefault(u => u.Email == user.Email);
                if (existingUser == null)
                {
                    user.DateCreated = DateTime.Now;
                    user.IsActive = true;
                    context.Users.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    existingUser.UserName = user.UserName;
                    existingUser.AvatarURL = user.AvatarURL;
                    context.SaveChanges();
                }

                // Lưu thông tin người dùng vào session
                Session["TaiKhoan"] = user;
            }

            return Json(new { success = true });
        }
        public ActionResult LoginWithFacebook()
        {
            var appId = System.Configuration.ConfigurationManager.AppSettings["FacebookAppId"];
            var redirectUri = Url.Action("LoginFacebookCallback", "LikeMovie", null, protocol: Request.Url.Scheme);
            var facebookLoginUrl = $"https://www.facebook.com/v21.0/dialog/oauth?client_id={appId}&redirect_uri={redirectUri}&scope=email,public_profile";

            return Redirect(facebookLoginUrl);
        }
        public ActionResult LoginFacebookCallback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("DangNhap", "LikeMovie");
            }

            var appId = System.Configuration.ConfigurationManager.AppSettings["FacebookAppId"];
            var appSecret = System.Configuration.ConfigurationManager.AppSettings["FacebookAppSecret"];
            var redirectUri = Url.Action("LoginFacebookCallback", "LikeMovie", null, protocol: Request.Url.Scheme);

            try
            {
                // Lấy token từ Facebook
                var tokenUrl = $"https://graph.facebook.com/v21.0/oauth/access_token?client_id={appId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&client_secret={appSecret}&code={code}";

                var client = new System.Net.Http.HttpClient();
                var response = client.GetStringAsync(tokenUrl).Result;
                var tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);

                // Kiểm tra nếu tokenData không null và có trường access_token
                if (tokenData == null || tokenData.access_token == null)
                {
                    throw new Exception("Không thể lấy mã truy cập từ Facebook.");
                }

                string accessToken = tokenData.access_token.ToString(); // Chắc chắn là chuỗi

                // Kiểm tra accessToken
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception("Không thể lấy mã truy cập từ Facebook.");
                }

                // Dùng access token để lấy thông tin người dùng
                var fbClient = new FacebookClient(accessToken);
                dynamic userInfo = fbClient.Get("me?fields=id,name,email");

                if (userInfo == null)
                {
                    throw new Exception("Không thể lấy thông tin người dùng từ Facebook.");
                }

                var name = userInfo.name;
                string email = userInfo.email.ToString(); // Ép kiểu rõ ràng sang string

                // Kiểm tra người dùng trong cơ sở dữ liệu
                var user = db.Users.SingleOrDefault(u => u.Email == email);

                if (user == null)
                {
                    // Tạo tài khoản mới nếu người dùng chưa có
                    string password = Guid.NewGuid().ToString("N").Substring(0, 10); // Mật khẩu ngẫu nhiên
                    string username = email.Length > 15 ? email.Substring(0, 15) : email; // Lấy username từ email

                    user = new Users
                    {
                        UserName = username,
                        Email = email,
                        PasswordHash = HashPassword(password), // Lưu mật khẩu đã mã hóa
                        Role = 0,
                        AvatarURL = "~/Images/Avatar/default_user.png", // Lấy avatar từ Facebook
                        DateCreated = DateTime.Now
                    };

                    db.Users.Add(user);
                    db.SaveChanges();
                }

                // Lưu thông tin người dùng vào session
                Session["TaiKhoan"] = user;

                // Điều hướng về trang chính
                return RedirectToAction("Index", "LikeMovie");
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, quay lại trang đăng nhập
                ViewBag.Error = "Đã có lỗi xảy ra khi đăng nhập bằng Facebook: " + ex.Message;
                return RedirectToAction("DangNhap", "LikeMovie");
            }
        }
    }
}