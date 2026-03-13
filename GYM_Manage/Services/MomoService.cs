using GYM_Manage.Data;                 // DbContext
using GYM_Manage.Models.Momo;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace GYM_Manage.Services
{
    public class MomoService : IMomoService
    {
        private readonly IConfiguration _configuration;
        private readonly GYM_DBcontext _db;

        public MomoService(IConfiguration configuration, GYM_DBcontext db)
        {
            _configuration = configuration;
            _db = db;
        }

        // ❶ Khớp interface: Controller chỉ truyền maGoiTap
        public async Task<MomoPaymentResponse> CreatePaymentAsync(int maGoiTap)
        {
            var goiTap = await _db.GoiTaps
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.MaGoiTap == maGoiTap);

            if (goiTap == null)
                throw new KeyNotFoundException($"Không tìm thấy gói tập {maGoiTap}.");

            return await CreatePaymentAsync(maGoiTap.ToString(), goiTap.GiaTien);
        }

        // ❷ Overload: có sẵn số tiền
        public async Task<MomoPaymentResponse> CreatePaymentAsync(string maGoiTap, decimal tongTien)
        {
            // Đọc cấu hình MoMo
            string endpoint = _configuration["MoMo:Endpoint"];
            string partnerCode = _configuration["MoMo:PartnerCode"];
            string accessKey = _configuration["MoMo:AccessKey"];
            string secretKey = _configuration["MoMo:SecretKey"];
            string redirectUrl = _configuration["MoMo:RedirectUrl"];
            string ipnUrl = _configuration["MoMo:IpnUrl"];

            // Khởi tạo tham số giao dịch
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string orderInfo = $"Thanh toán gói tập {maGoiTap}";
            string amount = decimal.Truncate(tongTien).ToString(CultureInfo.InvariantCulture);

            // Tạo rawHash theo đúng thứ tự tham số MoMo yêu cầu
            string rawHash =
                $"accessKey={accessKey}" +
                $"&amount={amount}" +
                $"&extraData=" +
                $"&ipnUrl={ipnUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={partnerCode}" +
                $"&redirectUrl={redirectUrl}" +
                $"&requestId={requestId}" +
                $"&requestType=captureWallet";

            string signature = SignSHA256(rawHash, secretKey);

            var requestData = new
            {
                partnerCode,
                accessKey,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                extraData = "",
                requestType = "captureWallet",
                signature,
                lang = "vi"
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Giả định bạn đã có class MomoPaymentResponse (Models.Momo)
            var momoResp = JsonConvert.DeserializeObject<MomoPaymentResponse>(responseContent);
            if (momoResp == null)
                throw new InvalidOperationException("MoMo trả về dữ liệu không hợp lệ.");

            return momoResp;
        }

        // Helper ký HMAC-SHA256
        private static string SignSHA256(string rawData, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
