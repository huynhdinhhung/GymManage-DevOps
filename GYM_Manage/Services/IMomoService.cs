using GYM_Manage.Models.Momo;

namespace GYM_Manage.Services
{
    public interface IMomoService
    {
        // Controller sẽ gọi hàm này: chỉ cần mã gói, Service tự tra giá từ DB
        Task<MomoPaymentResponse> CreatePaymentAsync(int maGoiTap);

        // Overload dùng khi đã có sẵn số tiền (có thể dùng nội bộ Service)
        Task<MomoPaymentResponse> CreatePaymentAsync(string maGoiTap, decimal tongTien);
    }
}
