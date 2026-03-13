# 🏋️ GYM Manage System - Hệ Thống Quản Lý Phòng Tập Gym

Chào mừng bạn đến với **GYM Manage System**, một giải pháp toàn diện để quản lý phòng tập gym hiện đại, được xây dựng trên nền tảng **ASP.NET Core MVC (.NET 9.0)**. Hệ thống cung cấp các công cụ mạnh mẽ để quản lý thành viên, huấn luyện viên, gói tập, lịch trình và thanh toán trực tuyến.

---

## 🌟 Tính Năng Chính

### 🛡️ Quản Lý Hệ Thống & Phân Quyền
- **Xác thực & Phân quyền**: Hệ thống phân quyền dựa trên Session với 4 vai trò chính: **Admin**, **Staff** (Nhân viên), **Trainer** (Huấn luyện viên), và **User** (Thành viên).
- **Dashboard**: Biểu đồ thống kê doanh thu, số lượng thành viên và các hoạt động của phòng tập.

### 👥 Quản Lý Thành Viên & Nhân Sự
- **Thành viên**: Đăng ký, gia hạn gói tập, theo dõi lịch tập cá nhân.
- **Huấn luyện viên (HLV)**: Quản lý hồ sơ HLV, phân ca làm việc, theo dõi học viên và phản hồi yêu cầu đổi lịch.
- **Nhân viên**: Hỗ trợ quản lý các hoạt động hàng ngày của phòng tập.

### 📅 Lịch Tập & Ca Làm Việc
- **Lịch tập**: Sắp xếp lịch tập cho thành viên (Thường/Vip).
- **Ca làm việc**: Quản lý ca làm việc linh hoạt cho HLV và nhân viên.
- **Yêu cầu đổi lịch**: Hệ thống xử lý yêu cầu đổi lịch tập từ học viên và HLV.

### 💳 Thanh Toán & Tài Chính
- **Tích hợp Momo**: Hỗ trợ thanh toán trực tuyến an toàn qua cổng Momo.
- **Quản lý hóa đơn**: Tự động tạo hóa đơn, hỗ trợ xuất file PDF (iText7) và mã QR (QRCoder).
- **Gói tập**: Quản lý danh mục gói tập đa dạng (theo tháng, năm, gói PT).

### 📢 Truyền Thông & Thông Báo
- **Blog/Tin tức**: Quản lý bài viết về sức khỏe, dinh dưỡng và thông tin phòng tập.
- **Thông báo tự động**: Sử dụng **Hangfire** để gửi thông báo nhắc lịch tập hàng ngày vào lúc 6:00 sáng.
- **Email Service**: Gửi thông báo đăng ký, thanh toán và OTP qua email.

### 🛠️ Quản Lý Thiết Bị
- Theo dõi danh mục thiết bị, tình trạng bảo trì và số lượng trong phòng tập.

---

## 🚀 Công Nghệ Sử Dụng

- **Backend**: ASP.NET Core MVC (.NET 9.0)
- **Database**: SQL Server (Entity Framework Core 9.0)
- **Background Jobs**: Hangfire (Quản lý các tác vụ chạy ngầm)
- **Payment Gateway**: Momo API Integration
- **Libraries**:
  - `iText7`: Xuất hóa đơn PDF chuyên nghiệp.
  - `QRCoder`: Tạo mã QR thanh toán.
  - `RestSharp`: Gọi API bên thứ ba.
  - `MailKit`: Xử lý gửi email thông báo.
- **Frontend**: HTML5, CSS3, JavaScript, Razor Pages, Bootstrap.

---

## 🛠️ Hướng Dẫn Cài Đặt

### 1. Yêu Cầu Hệ Thống
- .NET 9.0 SDK
- SQL Server (2019 hoặc mới hơn)
- Visual Studio 2022 (v17.12+)

### 2. Cấu Hình Cơ Sở Dữ Liệu
Mở file `appsettings.json` và cập nhật chuỗi kết nối:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=GYM_Manage;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### 3. Chạy Migration
Mở **Package Manager Console** trong Visual Studio và chạy:
```powershell
Update-Database
```
*(Hoặc sử dụng file `DB.sql` đính kèm trong thư mục gốc để khởi tạo dữ liệu)*

### 4. Cấu Hình Dịch Vụ (Email & Momo)
Cập nhật các thông số trong `appsettings.json`:
- `MomoOption`: Thông tin API từ Momo Partner.
- `EmailSettings`: Cấu hình SMTP (Gmail/Outlook).

### 5. Khởi Chạy
Nhấn `F5` hoặc chạy lệnh:
```bash
dotnet run --project GYM_Manage
```
- Truy cập Dashboard Admin: `/Admin/Dashboard`
- Truy cập Hangfire: `/hangfire`

---

## 📂 Cấu Trúc Thư Mục

- `Controllers/`: Xử lý logic điều hướng và nghiệp vụ (chia theo Areas: Admin, HLV).
- `Models/`: Định nghĩa các thực thể dữ liệu (ThanhVien, GoiTap, HoaDon,...).
- `Views/`: Giao diện người dùng (Razor Pages).
- `Services/`: Các dịch vụ xử lý (Momo, Email, Hangfire).
- `Data/`: Cấu hình DBContext và Migrations.
- `wwwroot/`: Tài nguyên tĩnh (CSS, JS, Images, Fonts).

---

## 🤝 Đóng Góp
Nếu bạn có bất kỳ ý tưởng hoặc phát hiện lỗi, vui lòng tạo **Issue** hoặc gửi **Pull Request**. Mọi đóng góp đều được trân trọng!

---

## 📄 Giấy Phép
Dự án được phát triển cho mục đích quản lý và học tập.

---
*Phát triển bởi Đội ngũ GYM Manage System*
