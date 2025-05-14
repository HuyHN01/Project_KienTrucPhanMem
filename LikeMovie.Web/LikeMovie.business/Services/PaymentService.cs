// LikeMovie.Business/Services/PaymentService.cs
using AutoMapper;
using LikeMovie.Business.Dtos.PaymentDtos;
using LikeMovie.Business.Dtos.UserDtos;
using LikeMovie.Business.Interfaces;
using LikeMovie.DataAccess.Interfaces; // Cho IUserRepository, ISubscriptionPlanRepository, IPaymentRepository
using LikeMovie.Model.Entities;
using Microsoft.Extensions.Configuration; // Để đọc appsettings.json
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; // Cho LINQ
using System.Net.Http; // Cho HttpClient
using System.Security.Cryptography; // Cho HMACSHA256
using System.Text;
using System.Threading.Tasks;
// using Newtonsoft.Json; // Nếu cổng thanh toán trả về JSON và bạn muốn parse thủ công

namespace LikeMovie.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISubscriptionService _subscriptionService; // Để cập nhật trạng thái VIP
        private readonly IHttpClientFactory _httpClientFactory; // Cách tốt hơn để dùng HttpClient

        // Cấu hình VNPAY (ví dụ, nên đặt trong một class cấu hình riêng và inject qua IOptions<VnPayConfig>)
        private string VnPayUrl => _configuration["VnPay:Url"] ?? string.Empty;
        private string VnPayReturnUrlBase => _configuration["VnPay:ReturnUrlBase"] ?? string.Empty; // Ví dụ: https://yourdomain.com
        private string VnPayTmnCode => _configuration["VnPay:TmnCode"] ?? string.Empty;
        private string VnPayHashSecret => _configuration["VnPay:HashSecret"] ?? string.Empty;

        // Cấu hình MoMo (ví dụ)
        private string MomoEndpoint => _configuration["Momo:Endpoint"] ?? string.Empty;
        private string MomoPartnerCode => _configuration["Momo:PartnerCode"] ?? string.Empty;
        private string MomoAccessKey => _configuration["Momo:AccessKey"] ?? string.Empty;
        private string MomoSecretKey => _configuration["Momo:SecretKey"] ?? string.Empty;
        private string MomoReturnUrlBase => _configuration["Momo:ReturnUrlBase"] ?? string.Empty;
        private string MomoNotifyUrlBase => _configuration["Momo:NotifyUrlBase"] ?? string.Empty;


        public PaymentService(
            IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PaymentService> logger,
            IConfiguration configuration,
            ISubscriptionService subscriptionService,
            IHttpClientFactory httpClientFactory)
        {
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _subscriptionService = subscriptionService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string?> CreateVnPayPaymentUrlAsync(int userId, int planId, int subscriptionDurationMonths, decimal totalAmount, string ipAddress, string httpContextBaseUrl)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(planId);
                if (plan == null) throw new Exception("Gói VIP không hợp lệ.");

                // Logic tạo URL cho VNPAY (sử dụng PayLib hoặc logic tương tự bạn có)
                // Ví dụ về PayLib (bạn cần có class PayLib này)
                var pay = new PayLib(); // Giả định bạn có class PayLib
                var vnp_ReturnUrl = $"{httpContextBaseUrl.TrimEnd('/')}/MuaVip/PaymentConfirm"; // Điều chỉnh controller/action
                var vnp_OrderInfo = $"Thanh toan goi {plan.PlanName} {subscriptionDurationMonths} thang";
                var vnp_TxnRef = DateTime.Now.Ticks.ToString(); // Mã giao dịch tham chiếu

                pay.AddRequestData("vnp_Version", "2.1.0");
                pay.AddRequestData("vnp_Command", "pay");
                pay.AddRequestData("vnp_TmnCode", VnPayTmnCode);
                pay.AddRequestData("vnp_Amount", ((long)(totalAmount * 100)).ToString()); // VNPAY dùng đơn vị xu
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", "VND");
                pay.AddRequestData("vnp_IpAddr", ipAddress);
                pay.AddRequestData("vnp_Locale", "vn");
                pay.AddRequestData("vnp_OrderInfo", vnp_OrderInfo);
                pay.AddRequestData("vnp_OrderType", "other"); // Hoặc mã loại hàng hóa phù hợp
                pay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
                pay.AddRequestData("vnp_TxnRef", vnp_TxnRef);
                // pay.AddRequestData("vnp_BankCode", "NCB"); // Có thể bỏ qua để người dùng chọn ngân hàng trên cổng VNPAY

                string paymentUrl = pay.CreateRequestUrl(VnPayUrl, VnPayHashSecret);
                _logger.LogInformation("VNPAY Payment URL created for UserID {UserId}, PlanID {PlanId}: {PaymentUrl}", userId, planId, paymentUrl);

                // Lưu trữ thông tin giao dịch tạm thời nếu cần (ví dụ: TxnRef, UserId, PlanId, Duration, Amount)
                // để đối chiếu khi VNPAY callback. Có thể lưu vào Cache hoặc một bảng tạm.
                // Hoặc bạn có thể mã hóa thông tin này vào vnp_OrderInfo hoặc vnp_TxnRef (nếu VNPAY cho phép đủ dài)
                // và giải mã khi callback.

                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPAY payment URL for UserID {UserId}, PlanID {PlanId}.", userId, planId);
                return null;
            }
        }

        public async Task<(bool success, string message, PaymentDto? payment, UserDto? updatedUser)> ProcessVnPayReturnAsync(Dictionary<string, string> queryString)
        {
            _logger.LogInformation("Processing VNPAY return data: {QueryString}", string.Join("&", queryString.Select(kv => $"{kv.Key}={kv.Value}")));
            var pay = new PayLib();
            foreach (var kvp in queryString)
            {
                if (!string.IsNullOrEmpty(kvp.Key) && kvp.Key.StartsWith("vnp_"))
                {
                    pay.AddResponseData(kvp.Key, kvp.Value);
                }
            }

            string vnp_TxnRef = pay.GetResponseData("vnp_TxnRef"); // Mã đơn hàng của bạn
            string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); // Mã phản hồi 00 là thành công
            string vnp_TransactionNo = pay.GetResponseData("vnp_TransactionNo"); // Mã giao dịch của VNPAY
            string vnp_SecureHash = queryString.FirstOrDefault(q => q.Key == "vnp_SecureHash").Value ?? string.Empty;

            bool checkSignature = pay.ValidateSignature(vnp_SecureHash, VnPayHashSecret);

            if (!checkSignature)
            {
                _logger.LogWarning("VNPAY return: Invalid signature for TxnRef {TxnRef}.", vnp_TxnRef);
                return (false, "Chữ ký không hợp lệ.", null, null);
            }

            try
            {
                // TODO: Lấy thông tin UserID, PlanID, Duration, Amount đã lưu tạm thời khi tạo URL
                // Dựa vào vnp_TxnRef hoặc thông tin mã hóa trong vnp_OrderInfo
                // Ví dụ (giả định bạn đã lưu vào Session hoặc Cache, cách này không tốt nếu ứng dụng scale):
                // var userId = (int)Session["VnPay_UserId_" + vnp_TxnRef];
                // var planId = (int)Session["VnPay_PlanId_" + vnp_TxnRef];
                // var duration = (int)Session["VnPay_Duration_" + vnp_TxnRef];
                // var amount = (decimal)Session["VnPay_Amount_" + vnp_TxnRef];
                // --> Cần một cơ chế lưu trữ tốt hơn cho thông tin giao dịch chờ xử lý.

                // *** GIẢ ĐỊNH LẤY ĐƯỢC THÔNG TIN GIAO DỊCH TẠM THỜI ***
                // Đây là phần bạn cần tự implement cơ chế lấy lại thông tin này
                int MOCKED_USER_ID = 1; // THAY THẾ BẰNG LOGIC LẤY USERID THỰC TẾ
                int MOCKED_PLAN_ID = 1; // THAY THẾ BẰNG LOGIC LẤY PLANID THỰC TẾ
                int MOCKED_DURATION = 1; // THAY THẾ
                decimal MOCKED_AMOUNT = Convert.ToDecimal(pay.GetResponseData("vnp_Amount")) / 100;


                if (vnp_ResponseCode == "00")
                {
                    _logger.LogInformation("VNPAY payment successful for TxnRef {TxnRef}, VNPAY TransNo {VnpTransactionNo}.", vnp_TxnRef, vnp_TransactionNo);

                    var paymentEntity = new Payment
                    {
                        UserId = MOCKED_USER_ID, // Lấy từ thông tin đã lưu
                        PlanId = MOCKED_PLAN_ID, // Lấy từ thông tin đã lưu
                        SubscriptionDuration = MOCKED_DURATION, // Lấy từ thông tin đã lưu
                        Amount = MOCKED_AMOUNT,
                        TransactionDate = DateTime.UtcNow,
                        TransactionStatus = "Success",
                        PaymentMethod = 1, // 1 for VNPAY
                        TransactionReference = vnp_TransactionNo, // Mã giao dịch của VNPAY
                        CreatedAt = DateTime.UtcNow
                    };
                    await _paymentRepository.AddAsync(paymentEntity);
                    // Không gọi SaveChanges ở đây, để SubscriptionService xử lý và commit chung

                    var subscriptionResult = await _subscriptionService.ProcessSuccessfulSubscriptionAsync(
                        paymentEntity.UserId,
                        paymentEntity.PlanId,
                        paymentEntity.SubscriptionDuration);

                    await _unitOfWork.SaveChangesAsync(); // Commit sau khi cả payment và subscription đã được xử lý

                    if (subscriptionResult.success)
                    {
                        var paymentDto = _mapper.Map<PaymentDto>(paymentEntity);
                        return (true, $"Thanh toán thành công hóa đơn {vnp_TxnRef} | Mã giao dịch VNPAY: {vnp_TransactionNo}", paymentDto, subscriptionResult.updatedUser);
                    }
                    else
                    {
                        // Thanh toán thành công nhưng cập nhật VIP thất bại (hiếm khi xảy ra nếu logic đúng)
                        // Cần có cơ chế rollback hoặc đánh dấu để xử lý thủ công
                        _logger.LogError("VNPAY payment successful for TxnRef {TxnRef} but failed to update VIP status for UserID {UserId}.", vnp_TxnRef, paymentEntity.UserId);
                        return (false, "Thanh toán thành công nhưng có lỗi khi cập nhật trạng thái VIP.", null, null);
                    }
                }
                else
                {
                    _logger.LogWarning("VNPAY payment failed for TxnRef {TxnRef}. ResponseCode: {ResponseCode}", vnp_TxnRef, vnp_ResponseCode);
                    // Lưu giao dịch thất bại nếu cần
                    var failedPayment = new Payment {/* ... thông tin ... */ TransactionStatus = $"Failed_VNPAY_{vnp_ResponseCode}" };
                    await _paymentRepository.AddAsync(failedPayment);
                    await _unitOfWork.SaveChangesAsync();
                    return (false, $"Thanh toán thất bại. Mã lỗi VNPAY: {vnp_ResponseCode}", _mapper.Map<PaymentDto>(failedPayment), null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPAY return for TxnRef {TxnRef}.", vnp_TxnRef);
                return (false, "Lỗi xử lý kết quả thanh toán từ VNPAY.", null, null);
            }
        }


        // TODO: Implement các phương thức cho MoMo tương tự
        public async Task<(string? payUrl, string? errorMessage)> CreateMomoPaymentRequestAsync(int userId, int planId, int subscriptionDurationMonths, decimal totalAmount, string httpContextBaseUrl)
        {
            _logger.LogInformation("Creating MoMo payment request for UserID {UserId}, PlanID {PlanId}", userId, planId);
            // ... Logic tạo request và signature cho MoMo ...
            // var client = _httpClientFactory.CreateClient("MomoApiClient");
            // var response = await client.PostAsync(...);
            // var momoResponse = JsonConvert.DeserializeObject<MomoResponse>(await response.Content.ReadAsStringAsync());
            // if (momoResponse?.errorCode == 0 && !string.IsNullOrEmpty(momoResponse.payUrl)) return (momoResponse.payUrl, null);
            return (null, "Chức năng thanh toán MoMo chưa được triển khai hoàn chỉnh.");
        }

        public async Task<(bool success, string message, PaymentDto? payment, UserDto? updatedUser)> ProcessMomoReturnAsync(Dictionary<string, string> responseData)
        {
            _logger.LogInformation("Processing MoMo return data.");
            // ... Logic xử lý callback từ MoMo, kiểm tra signature, lưu giao dịch, cập nhật VIP ...
            return (false, "Chức năng xử lý MoMo return chưa được triển khai.", null, null);
        }

        public async Task<(bool success, string message)> ProcessMomoIpnAsync(Dictionary<string, string> ipnData)
        {
            _logger.LogInformation("Processing MoMo IPN data.");
            // ... Logic xử lý IPN từ MoMo, kiểm tra signature, cập nhật trạng thái giao dịch/VIP (nếu chưa được cập nhật ở ReturnUrl) ...
            // Quan trọng: Phải xác thực IPN cẩn thận.
            return (false, "Chức năng xử lý MoMo IPN chưa được triển khai.");
        }


        public async Task<IEnumerable<PaymentDto>> GetPaymentHistoryByUserAsync(int userId)
        {
            // var paymentEntities = await _paymentRepository.GetByUserIdAsync(userId); // Cần thêm phương thức này vào IPaymentRepository
            // return _mapper.Map<IEnumerable<PaymentDto>>(paymentEntities);
            _logger.LogInformation("Fetching payment history for UserID {UserId}.", userId);
            return await Task.FromResult(new List<PaymentDto>()); // Placeholder
        }


        // Helper cho MoMo signature (ví dụ, bạn cần class MomoConfig)
        private string GenerateMomoSignature(string data, string secretKey)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        // Class PayLib giả định (bạn cần có class này từ code cũ hoặc viết lại logic tương tự)
        private class PayLib
        {
            private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
            private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

            public void AddRequestData(string key, string value) => _requestData.Add(key, value);
            public void AddResponseData(string key, string value) => _responseData.Add(key, value);
            public string GetResponseData(string key) => _responseData.TryGetValue(key, out var value) ? value : string.Empty;

            public string CreateRequestUrl(string baseUrl, string hashSecret)
            {
                StringBuilder data = new StringBuilder();
                foreach (var kvp in _requestData)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        data.Append(Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value) + "&");
                    }
                }
                string queryString = data.ToString().TrimEnd('&');
                string vnp_SecureHash = HmacSHA512(hashSecret, queryString); // VNPAY dùng HMACSHA512
                return baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;
            }

            public bool ValidateSignature(string inputHash, string secretKey)
            {
                StringBuilder data = new StringBuilder();
                foreach (var kvp in _responseData)
                {
                    if (!string.IsNullOrEmpty(kvp.Value) && kvp.Key != "vnp_SecureHash" && kvp.Key != "vnp_SecureHashType") // Loại bỏ SecureHash và SecureHashType
                    {
                        data.Append(Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value) + "&");
                    }
                }
                string queryString = data.ToString().TrimEnd('&');
                string calculatedHash = HmacSHA512(secretKey, queryString);
                return calculatedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
            }

            private string HmacSHA512(string key, string inputData)
            {
                var hash = new StringBuilder();
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
                using (var hmac = new HMACSHA512(keyBytes))
                {
                    byte[] hashValue = hmac.ComputeHash(inputBytes);
                    foreach (var b in hashValue) { hash.AppendFormat("{0:x2}", b); }
                }
                return hash.ToString();
            }
        }
        // Helper class cho việc sắp xếp các tham số của VNPAY
        private class VnPayCompare : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return string.CompareOrdinal(x, y);
            }
        }

        // Lớp giả định cho phản hồi từ MoMo (bạn cần định nghĩa dựa trên tài liệu của MoMo)
        private class MomoResponse
        {
            public int errorCode { get; set; }
            public string? message { get; set; }
            public string? payUrl { get; set; }
            // ... các trường khác
        }
    }
}