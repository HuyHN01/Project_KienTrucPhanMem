using LikeMovies.Models;
using System;
using Facebook;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity.Validation;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace LikeMovies.Controllers
{
    public class LikeMovieController : Controller
    {
        MovieEntities db = new MovieEntities();
        // GET: LikeMovies
        public ActionResult Index()
        {
            var sliders = db.PosterMovie.Include("Movies").ToList();
            return View(sliders);
        }
        [ChildActionOnly]

        public ActionResult PartialNav()
        {
            List<MENUs> lst = db.MENUs.Where(m => m.ParentId == null).OrderBy(m => m.OrderNumber).ToList();

            int[] a = new int[lst.Count()];
            for (int i = 0; i < lst.Count; i++)
            {
                int id = lst[i].Id;
                List<MENUs> l = db.MENUs.Where(m => m.ParentId == id).ToList();
                int k = l.Count();
                a[i] = k;
            }
            ViewBag.lst = a;
            return PartialView("_PartialNav", lst);
        }
        [ChildActionOnly]
        public ActionResult LoadChildMenu(int parentId)
        {
            List<MENUs> lst = db.MENUs
                                  .Where(m => m.ParentId == parentId)
                                  .OrderBy(m => m.OrderNumber)
                                  .ToList();

            // Đếm số lượng menu con
            ViewBag.Count = lst.Count;
            int[] a = new int[lst.Count];
            for (int i = 0; i < lst.Count; i++)
            {
                int id = lst[i].Id;
                List<MENUs> l = db.MENUs.Where(m => m.ParentId == id).ToList();
                int k = l.Count();
                a[i] = k;
            }

            ViewBag.lst = a;
            return PartialView("LoadChildMenu", lst); // Trả về PartialView với danh sách menu con
        }
        public ActionResult PartialSlider()
        {
            return PartialView("_PartialSlider");
        }
        public ActionResult PartialFooter()
        {
            return PartialView("_PartialFooter");
        }
        [HttpGet]
        public ActionResult DangNhap(string returnUrl)
        {
            var clientId = "1007314360605-k8ccl0lu9vbtm2bd47udloijjtkhvl13.apps.googleusercontent.com";
            var url = "https://localhost:44310/LikeMovie/LoginGoogle";
            string response = GenerateGoogleOAuthUrl(clientId, url);
            ViewBag.response = response;
            ViewBag.ReturnUrl = returnUrl; // Truyền returnUrl vào View để giữ lại URL gốc
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
        private string GenerateGoogleOAuthUrl(string clientId, string redirectUri)
        {
            // Base URL of Google OAuth
            string googleOAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth";
            // Create query string
            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("response_type", "code"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("scope", "openid email profile"),
                new KeyValuePair<string, string>("access_type", "online")
            };

            // Create URL by concatenating parameters
            string queryString = string.Join("&", queryParams.Select(q => $"{q.Key}={Uri.EscapeDataString(q.Value)}"));
            return $"{googleOAuthUrl}?{queryString}";
        }
        private async Task<string> ExchangeCodeForTokenAsync(string code, string clientId, string redirectUri, string clientSecret)
        {
            string tokenEndpoint = "https://oauth2.googleapis.com/token";
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        });

                var response = await client.PostAsync(tokenEndpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);

                if (jsonResponse.error != null)
                {
                    throw new Exception($"Error exchanging code: {jsonResponse.error_description}");
                }

                return jsonResponse.access_token;
            }

        }
        private async Task<dynamic> GetGoogleUserInfoAsync(string accessToken)
        {
            string userInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync(userInfoEndpoint);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);

                if (userInfo.error != null)
                {
                    throw new Exception($"Error fetching user info: {userInfo.error.message}");
                }

                return userInfo;
            }
        }
        public async Task<ActionResult> LoginGoogle(string code, string scope, string authuser, string prompt)
        {
            string redirectUri = "https://localhost:44310/LikeMovie/LoginGoogle";
            var clientId = "1007314360605-k8ccl0lu9vbtm2bd47udloijjtkhvl13.apps.googleusercontent.com";
            var clientSecret = "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";

            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("DangNhap", "LikeMovie");
            }

            try
            {
                var accessToken = await ExchangeCodeForTokenAsync(code, clientId, redirectUri, clientSecret);
                var userInfo = await GetGoogleUserInfoAsync(accessToken);

                string name = userInfo.name?.ToString();
                string email = userInfo.email?.ToString();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
                {
                    ViewBag.Error = "Incomplete user information returned by Google.";
                    return RedirectToAction("DangNhap");
                }

                Users kh = db.Users.SingleOrDefault(u => u.Email == email);

                if (kh == null)
                {
                    string pass = Guid.NewGuid().ToString("N").Substring(0, 10);
                    string truncatedTaiKhoan = email.Length > 15 ? email.Substring(0, 15) : email;


                    Users khNew = new Users
                    {
                        Email = email,
                        UserName = truncatedTaiKhoan,
                        PasswordHash = pass,
                        Role = 0,

                        DateCreated = DateTime.Now,
                        AvatarURL = "~/Images/Avatar/default_user.png"
                    };

                    db.Users.Add(khNew);
                    db.SaveChanges();
                    Session["TaiKhoan"] = khNew;
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                }
                else
                {
                    Session["TaiKhoan"] = kh;
                }

                return RedirectToAction("Index", "LikeMovie");
            }
            catch (DbEntityValidationException dbEx)
            {
                var errorMessages = dbEx.EntityValidationErrors
                    .SelectMany(validationResult => validationResult.ValidationErrors)
                    .Select(error => $"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                var fullErrorMessage = string.Join("; ", errorMessages);

                return Content($"Validation Errors: {fullErrorMessage}");
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