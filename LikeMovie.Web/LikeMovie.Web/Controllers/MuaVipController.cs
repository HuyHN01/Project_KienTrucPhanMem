using LikeMovies.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;





namespace LikeMovies.Controllers
{
    public class MuaVipController : Controller
    {
        private readonly MovieEntities db = new MovieEntities();


        public ActionResult MuaVip()
        {
            if (Session["TaiKhoan"] == null || string.IsNullOrEmpty(Session["TaiKhoan"].ToString()))
            {
                return RedirectToAction("DangNhap", "LikeMovie");
            }
            var subscriptionPlans = db.SubscriptionPlans
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

        private decimal totalAmount(int planID, int subscriptionDuration)
        {
            var plan = db.SubscriptionPlans.Find(planID);
            if (plan == null)
            {
                throw new Exception("Plan not found");
            }
            return plan.Price * subscriptionDuration;
        }

        public ActionResult ThanhToan(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var plan = db.SubscriptionPlans.Find(id);
            if (plan == null)
            {
                return HttpNotFound();
            }
            ViewBag.SelectedPlan = plan;
            ViewBag.PlanID = plan.PlanID;

            return View();
        }

        [HttpPost]
        public ActionResult ThanhToan(int PlanID, int SubscriptionDuration, FormCollection f)
        {
            var plan = db.SubscriptionPlans.Find(PlanID);
            if (plan == null)
            {
                return HttpNotFound();
            }

            // Calculate total amount
            decimal totalAmount = this.totalAmount(PlanID, SubscriptionDuration);
            Session["TotalAmount"] = totalAmount; // Store in session
            Session["PlanID"] = PlanID; // Store PlanID in session
            Session["SubscriptionDuration"] = SubscriptionDuration; // Store SubscriptionDuration in session

            int paymentMethod = Convert.ToInt32(f["thanhtoan"]);
            switch (paymentMethod)
            {
                case 1:
                    return RedirectToAction("PaymentVNPAY");

                case 2:
                    // Handle MoMo payment process
                    return RedirectToAction("CreatePayment");

                default:
                    return View("Error"); // Handle invalid payment method
            }
        }
        public ActionResult PaymentVNPAY()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();
            decimal amount = (decimal)Session["TotalAmount"];
            int amountInCents = (int)(amount * 100); // Convert to cents

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", amountInCents.ToString());
            pay.AddRequestData("vnp_BankCode", "NCB");
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress());
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan gói VIP ");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        // Lưu thông tin giao dịch thành công vào cơ sở dữ liệu
                        luuGiaoDich("Success");

                        // Cập nhật LevelVip và ThoiHanVIP của người dùng
                        capNhatLevelVip();

                        ViewBag.Message = $"Thanh toán thành công hóa đơn {orderId} | Mã giao dịch: {vnpayTranId}";
                    }
                    else
                    {
                        ViewBag.Message = $"Có lỗi xảy ra trong quá trình xử lý hóa đơn {orderId} | Mã giao dịch: {vnpayTranId} | Mã lỗi: {vnp_ResponseCode}";
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
            return View();
        }


        private void luuGiaoDich(string transactionstatus)
        {
            Users kh = (Users)Session["TaiKhoan"];
            if (kh == null)
            {
                throw new Exception("User session is null");
            }

            Payments pay = new Payments
            {
                UserID = kh.UserID,
                PlanID = (int)Session["PlanID"],
                SubscriptionDuration = (int)Session["SubscriptionDuration"],
                Amount = (decimal)Session["TotalAmount"],
                TransactionDate = DateTime.Now,
                TransactionStatus = transactionstatus,
                CreatedAt = DateTime.Now,
                PaymentMethod = 1 // Hoặc giá trị thích hợp dựa trên phương thức thanh toán
            };
            db.Payments.Add(pay);
            db.SaveChanges();
        }
        [HttpPost]
        public ActionResult GiaHanVip(int PlanID, int SubscriptionDuration, FormCollection f)
        {
            if (Session["TaiKhoan"] == null || string.IsNullOrEmpty(Session["TaiKhoan"].ToString()))
            {
                return RedirectToAction("DangNhap", "LikeMovie");
            }

            Users kh = (Users)Session["TaiKhoan"];
            var plan = db.SubscriptionPlans.Find(PlanID);
            if (plan == null)
            {
                return HttpNotFound();
            }

            decimal totalAmount = this.totalAmount(PlanID, SubscriptionDuration);
            Session["TotalAmount"] = totalAmount; 
            Session["PlanID"] = PlanID; 
            Session["SubscriptionDuration"] = SubscriptionDuration; 

            int paymentMethod = Convert.ToInt32(f["thanhtoan"]);
            switch (paymentMethod)
            {
                case 1:
                    return RedirectToAction("PaymentVNPAY");

                case 2:
                    return RedirectToAction("PaymentMomo");

                default:
                    return View("Error"); 
            }
        }

        private void capNhatLevelVip()
        {
            Users kh = (Users)Session["TaiKhoan"];
            if (kh == null)
            {
                throw new Exception("User session is null");
            }

            // Tìm lại thực thể từ ngữ cảnh hiện tại để tránh lỗi
            Users khFromDb = db.Users.SingleOrDefault(u => u.UserID == kh.UserID);
            if (khFromDb == null)
            {
                throw new Exception("User not found in database");
            }
            int planID = (int)Session["PlanID"];
            int subscriptionDuration = (int)Session["SubscriptionDuration"];

            if (planID == 1)
            {
                khFromDb.levelVIP = 1;
            }
            else if (planID == 2)
            {
                khFromDb.levelVIP = 2;
            }
            else if (planID == 3)
            {
                khFromDb.levelVIP = 3;
            }

            // Cập nhật ThoiHanVIP dựa trên SubscriptionDuration
            if (khFromDb.TimeVIP == null || khFromDb.TimeVIP < DateTime.Now)
            {
                khFromDb.TimeVIP = DateTime.Now.AddMonths(subscriptionDuration); // Nếu thời hạn VIP đã hết hạn hoặc null, tính từ thời điểm hiện tại
            }
            else
            {
                khFromDb.TimeVIP = khFromDb.TimeVIP.Value.AddMonths(subscriptionDuration); // Nếu thời hạn VIP vẫn còn hiệu lực, gia hạn thêm
            }

            db.Entry(khFromDb).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }



        //Các phương thức xử lý thanh toán cho momo
        public async Task<ActionResult> CreatePayment()
        {
            //Lấy thông tin tiền trên web xuống
            decimal Amount = (decimal)Session["TotalAmount"];
            int amountInCents = (int)(Amount); 

            string requestId = MomoConfig.PartnerCode + DateTime.Now.Ticks;
            string orderId = requestId;
            string amount = amountInCents.ToString(); // Số tiền thanh toán
            string orderInfo = "Thanh toán thử nghiệm MoMo";
            string extraData = ""; // Không có dữ liệu thêm
            string requestType = "payWithMethod"; // Loại thanh toán
            // Tạo chuỗi raw signature
            string rawSignature = $"accessKey={MomoConfig.AccessKey}&amount={amount}&extraData={extraData}&ipnUrl={MomoConfig.NotifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={MomoConfig.PartnerCode}&redirectUrl={MomoConfig.ReturnUrl}&requestId={requestId}&requestType={requestType}";
            string signature = GenerateSignature(rawSignature, MomoConfig.SecretKey);

            // Tạo dữ liệu gửi đi
            var jsonData = new
            {
                partnerCode = MomoConfig.PartnerCode,
                accessKey = MomoConfig.AccessKey,
                requestId = requestId,
                amount = amount,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = MomoConfig.ReturnUrl,
                ipnUrl = MomoConfig.NotifyUrl,
                extraData = extraData,
                requestType = requestType,
                signature = signature,
                lang = "en"
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Gửi request POST đến MoMo
                    var content = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(MomoConfig.Endpoint, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Log phản hồi để kiểm tra
                    Console.WriteLine("Phản hồi từ MoMo: " + responseContent);

                    // Phân tích phản hồi JSON thành object
                    var momoResponse = JsonConvert.DeserializeObject<MomoResponse>(responseContent);

                    if (momoResponse != null)
                    {
                        if (momoResponse.errorCode == 0)
                        {
                            // Thành công, chuyển hướng đến URL thanh toán
                            if (!string.IsNullOrEmpty(momoResponse.payUrl))
                            {
                                return Redirect(momoResponse.payUrl);
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Không tìm thấy payUrl trong phản hồi từ MoMo.";
                                return View("Error");
                            }
                        }
                        else
                        {
                            // Hiển thị lỗi từ MoMo
                            ViewBag.ErrorMessage = $"Lỗi từ MoMo: Mã lỗi {momoResponse.errorCode}, Thông báo: {momoResponse.message}";
                            return View("Error");
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Không nhận được phản hồi từ API MoMo.";
                        return View("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi: {ex.Message}";
                return View("Error");
            }
        }


        // Phương thức tạo chữ ký HMAC SHA256
        private string GenerateSignature(string data, string key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        // Phương thức xử lý kết quả thanh toán trả về từ MoMo
        public ActionResult ReturnUrl()
        {
            string resultCode = Request.QueryString["resultCode"];
            if (resultCode == "0")
            {
                ViewBag.Message = "Thanh toán thành công!";
            }
            else
            {
                ViewBag.Message = "Thanh toán thất bại!";
            }
            return View();
        }

        // Phương thức xử lý NotifyUrl từ MoMo
        public ActionResult NotifyUrl()
        {
            // Xử lý callback từ MoMo nếu cần
            return new HttpStatusCodeResult(200);
        }

    }
}

