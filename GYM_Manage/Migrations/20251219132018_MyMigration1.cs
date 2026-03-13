using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GYM_Manage.Migrations
{
    /// <inheritdoc />
    public partial class MyMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaLamViec",
                columns: table => new
                {
                    MaCa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GioBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeSpan>(type: "time", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaLamViec", x => x.MaCa);
                });

            migrationBuilder.CreateTable(
                name: "GoiTaps",
                columns: table => new
                {
                    MaGoiTap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenGoiTap = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThoiHan = table.Column<int>(type: "int", nullable: false),
                    GiaTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AnhDemo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LoaiGoi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoiTaps", x => x.MaGoiTap);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsVip = table.Column<bool>(type: "bit", nullable: false),
                    TongChiTieu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VipSinceUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.MaNguoiDung);
                });

            migrationBuilder.CreateTable(
                name: "OtpCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Used = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThietBis",
                columns: table => new
                {
                    MaThietBi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThietBi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DanhMuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayMua = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayBaoTriCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThietBis", x => x.MaThietBi);
                });

            migrationBuilder.CreateTable(
                name: "BaiViets",
                columns: table => new
                {
                    MaBaiViet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTaNgan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(555)", maxLength: 555, nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNguoiTao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiViets", x => x.MaBaiViet);
                    table.ForeignKey(
                        name: "FK_BaiViets_NguoiDungs_IDNguoiTao",
                        column: x => x.IDNguoiTao,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HuanLuyenViens",
                columns: table => new
                {
                    MaHuanLuyenVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    NguoiDungMaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChuyenMon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChungChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayTuyenDung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HuanLuyenViens", x => x.MaHuanLuyenVien);
                    table.ForeignKey(
                        name: "FK_HuanLuyenViens_NguoiDungs_NguoiDungMaNguoiDung",
                        column: x => x.NguoiDungMaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "ThanhViens",
                columns: table => new
                {
                    MaThanhVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhViens", x => x.MaThanhVien);
                    table.ForeignKey(
                        name: "FK_ThanhViens_NguoiDungs_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "LichLamViecHLVs",
                columns: table => new
                {
                    MaLich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHuanLuyenVien = table.Column<int>(type: "int", nullable: false),
                    MaCa = table.Column<int>(type: "int", nullable: false),
                    ThuTrongTuan = table.Column<int>(type: "int", nullable: false),
                    NgayLamViec = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsThayThe = table.Column<bool>(type: "bit", nullable: false),
                    MaHuanLuyenVienThayThe = table.Column<int>(type: "int", nullable: true),
                    HuanLuyenVienThayTheMaHuanLuyenVien = table.Column<int>(type: "int", nullable: true),
                    LyDoThayDoi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichLamViecHLVs", x => x.MaLich);
                    table.ForeignKey(
                        name: "FK_LichLamViecHLVs_CaLamViec_MaCa",
                        column: x => x.MaCa,
                        principalTable: "CaLamViec",
                        principalColumn: "MaCa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichLamViecHLVs_HuanLuyenViens_HuanLuyenVienThayTheMaHuanLuyenVien",
                        column: x => x.HuanLuyenVienThayTheMaHuanLuyenVien,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien");
                    table.ForeignKey(
                        name: "FK_LichLamViecHLVs_HuanLuyenViens_MaHuanLuyenVien",
                        column: x => x.MaHuanLuyenVien,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyGoiTaps",
                columns: table => new
                {
                    MaDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhVien = table.Column<int>(type: "int", nullable: false),
                    MaGoiTap = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaHuanLuyenVien = table.Column<int>(type: "int", nullable: true),
                    MaCa = table.Column<int>(type: "int", nullable: true),
                    ThuTrongTuan = table.Column<int>(type: "int", nullable: true),
                    NgayTap = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyGoiTaps", x => x.MaDangKy);
                    table.ForeignKey(
                        name: "FK_DangKyGoiTaps_CaLamViec_MaCa",
                        column: x => x.MaCa,
                        principalTable: "CaLamViec",
                        principalColumn: "MaCa");
                    table.ForeignKey(
                        name: "FK_DangKyGoiTaps_GoiTaps_MaGoiTap",
                        column: x => x.MaGoiTap,
                        principalTable: "GoiTaps",
                        principalColumn: "MaGoiTap");
                    table.ForeignKey(
                        name: "FK_DangKyGoiTaps_HuanLuyenViens_MaHuanLuyenVien",
                        column: x => x.MaHuanLuyenVien,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien");
                    table.ForeignKey(
                        name: "FK_DangKyGoiTaps_ThanhViens_MaThanhVien",
                        column: x => x.MaThanhVien,
                        principalTable: "ThanhViens",
                        principalColumn: "MaThanhVien");
                });

            migrationBuilder.CreateTable(
                name: "YeuCauDoiLichs",
                columns: table => new
                {
                    MaYeuCau = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHlvGui = table.Column<int>(type: "int", nullable: false),
                    MaHlvNhan = table.Column<int>(type: "int", nullable: false),
                    MaLichGui = table.Column<int>(type: "int", nullable: false),
                    MaLichNhan = table.Column<int>(type: "int", nullable: true),
                    LoaiDoi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaThanhVien = table.Column<int>(type: "int", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauDoiLichs", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauDoiLichs_HuanLuyenViens_MaHlvGui",
                        column: x => x.MaHlvGui,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauDoiLichs_HuanLuyenViens_MaHlvNhan",
                        column: x => x.MaHlvNhan,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauDoiLichs_LichLamViecHLVs_MaLichGui",
                        column: x => x.MaLichGui,
                        principalTable: "LichLamViecHLVs",
                        principalColumn: "MaLich",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauDoiLichs_LichLamViecHLVs_MaLichNhan",
                        column: x => x.MaLichNhan,
                        principalTable: "LichLamViecHLVs",
                        principalColumn: "MaLich",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauDoiLichs_ThanhViens_MaThanhVien",
                        column: x => x.MaThanhVien,
                        principalTable: "ThanhViens",
                        principalColumn: "MaThanhVien");
                });

            migrationBuilder.CreateTable(
                name: "LichTapThanhViens",
                columns: table => new
                {
                    MaLichTap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDangKy = table.Column<int>(type: "int", nullable: false),
                    MaThanhVien = table.Column<int>(type: "int", nullable: false),
                    MaHuanLuyenVien = table.Column<int>(type: "int", nullable: false),
                    MaCa = table.Column<int>(type: "int", nullable: false),
                    NgayTap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsHLVThayThe = table.Column<bool>(type: "bit", nullable: false),
                    MaHuanLuyenVienThayThe = table.Column<int>(type: "int", nullable: true),
                    HuanLuyenVienMaHuanLuyenVien = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichTapThanhViens", x => x.MaLichTap);
                    table.ForeignKey(
                        name: "FK_LichTapThanhViens_CaLamViec_MaCa",
                        column: x => x.MaCa,
                        principalTable: "CaLamViec",
                        principalColumn: "MaCa");
                    table.ForeignKey(
                        name: "FK_LichTapThanhViens_DangKyGoiTaps_MaDangKy",
                        column: x => x.MaDangKy,
                        principalTable: "DangKyGoiTaps",
                        principalColumn: "MaDangKy",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichTapThanhViens_HuanLuyenViens_HuanLuyenVienMaHuanLuyenVien",
                        column: x => x.HuanLuyenVienMaHuanLuyenVien,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien");
                    table.ForeignKey(
                        name: "FK_LichTapThanhViens_HuanLuyenViens_MaHuanLuyenVien",
                        column: x => x.MaHuanLuyenVien,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien");
                    table.ForeignKey(
                        name: "FK_LichTapThanhViens_HuanLuyenViens_MaHuanLuyenVienThayThe",
                        column: x => x.MaHuanLuyenVienThayThe,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien");
                    table.ForeignKey(
                        name: "FK_LichTapThanhViens_ThanhViens_MaThanhVien",
                        column: x => x.MaThanhVien,
                        principalTable: "ThanhViens",
                        principalColumn: "MaThanhVien");
                });

            migrationBuilder.CreateTable(
                name: "ThanhToans",
                columns: table => new
                {
                    MaThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhVien = table.Column<int>(type: "int", nullable: false),
                    MaDangKyGoiTap = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToans", x => x.MaThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToans_DangKyGoiTaps_MaDangKyGoiTap",
                        column: x => x.MaDangKyGoiTap,
                        principalTable: "DangKyGoiTaps",
                        principalColumn: "MaDangKy");
                    table.ForeignKey(
                        name: "FK_ThanhToans_ThanhViens_MaThanhVien",
                        column: x => x.MaThanhVien,
                        principalTable: "ThanhViens",
                        principalColumn: "MaThanhVien");
                });

            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    MaThongBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhVien = table.Column<int>(type: "int", nullable: true),
                    MaHuanLuyenVien = table.Column<int>(type: "int", nullable: true),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LoaiThongBao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MaYeuCauDoiLich = table.Column<int>(type: "int", nullable: true),
                    MaHuanLuyenVienThayThe = table.Column<int>(type: "int", nullable: true),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false),
                    IsImportant = table.Column<bool>(type: "bit", nullable: false),
                    DaXem = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.MaThongBao);
                    table.ForeignKey(
                        name: "FK_ThongBaos_HuanLuyenViens_MaHuanLuyenVien",
                        column: x => x.MaHuanLuyenVien,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien");
                    table.ForeignKey(
                        name: "FK_ThongBaos_HuanLuyenViens_MaHuanLuyenVienThayThe",
                        column: x => x.MaHuanLuyenVienThayThe,
                        principalTable: "HuanLuyenViens",
                        principalColumn: "MaHuanLuyenVien");
                    table.ForeignKey(
                        name: "FK_ThongBaos_ThanhViens_MaThanhVien",
                        column: x => x.MaThanhVien,
                        principalTable: "ThanhViens",
                        principalColumn: "MaThanhVien");
                    table.ForeignKey(
                        name: "FK_ThongBaos_YeuCauDoiLichs_MaYeuCauDoiLich",
                        column: x => x.MaYeuCauDoiLich,
                        principalTable: "YeuCauDoiLichs",
                        principalColumn: "MaYeuCau");
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhVien = table.Column<int>(type: "int", nullable: false),
                    MaThanhToan = table.Column<int>(type: "int", nullable: false),
                    NgayHoaDon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongSoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDons_ThanhToans_MaThanhToan",
                        column: x => x.MaThanhToan,
                        principalTable: "ThanhToans",
                        principalColumn: "MaThanhToan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDons_ThanhViens_MaThanhVien",
                        column: x => x.MaThanhVien,
                        principalTable: "ThanhViens",
                        principalColumn: "MaThanhVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiViets_IDNguoiTao",
                table: "BaiViets",
                column: "IDNguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyGoiTaps_MaCa",
                table: "DangKyGoiTaps",
                column: "MaCa");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyGoiTaps_MaGoiTap",
                table: "DangKyGoiTaps",
                column: "MaGoiTap");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyGoiTaps_MaHuanLuyenVien",
                table: "DangKyGoiTaps",
                column: "MaHuanLuyenVien");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyGoiTaps_MaThanhVien",
                table: "DangKyGoiTaps",
                column: "MaThanhVien");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaThanhToan",
                table: "HoaDons",
                column: "MaThanhToan");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaThanhVien",
                table: "HoaDons",
                column: "MaThanhVien");

            migrationBuilder.CreateIndex(
                name: "IX_HuanLuyenViens_NguoiDungMaNguoiDung",
                table: "HuanLuyenViens",
                column: "NguoiDungMaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_LichLamViecHLVs_HuanLuyenVienThayTheMaHuanLuyenVien",
                table: "LichLamViecHLVs",
                column: "HuanLuyenVienThayTheMaHuanLuyenVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichLamViecHLVs_MaCa",
                table: "LichLamViecHLVs",
                column: "MaCa");

            migrationBuilder.CreateIndex(
                name: "IX_LichLamViecHLVs_MaHuanLuyenVien",
                table: "LichLamViecHLVs",
                column: "MaHuanLuyenVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichTapThanhViens_HuanLuyenVienMaHuanLuyenVien",
                table: "LichTapThanhViens",
                column: "HuanLuyenVienMaHuanLuyenVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichTapThanhViens_MaCa",
                table: "LichTapThanhViens",
                column: "MaCa");

            migrationBuilder.CreateIndex(
                name: "IX_LichTapThanhViens_MaDangKy",
                table: "LichTapThanhViens",
                column: "MaDangKy");

            migrationBuilder.CreateIndex(
                name: "IX_LichTapThanhViens_MaHuanLuyenVien",
                table: "LichTapThanhViens",
                column: "MaHuanLuyenVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichTapThanhViens_MaHuanLuyenVienThayThe",
                table: "LichTapThanhViens",
                column: "MaHuanLuyenVienThayThe");

            migrationBuilder.CreateIndex(
                name: "IX_LichTapThanhViens_MaThanhVien",
                table: "LichTapThanhViens",
                column: "MaThanhVien");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_Email",
                table: "NguoiDungs",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_TenDangNhap",
                table: "NguoiDungs",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_MaDangKyGoiTap",
                table: "ThanhToans",
                column: "MaDangKyGoiTap");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_MaThanhVien",
                table: "ThanhToans",
                column: "MaThanhVien");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhViens_MaNguoiDung",
                table: "ThanhViens",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhViens_SoDienThoai",
                table: "ThanhViens",
                column: "SoDienThoai",
                unique: true,
                filter: "[SoDienThoai] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaHuanLuyenVien",
                table: "ThongBaos",
                column: "MaHuanLuyenVien");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaHuanLuyenVienThayThe",
                table: "ThongBaos",
                column: "MaHuanLuyenVienThayThe");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaThanhVien",
                table: "ThongBaos",
                column: "MaThanhVien");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaYeuCauDoiLich",
                table: "ThongBaos",
                column: "MaYeuCauDoiLich");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauDoiLichs_MaHlvGui",
                table: "YeuCauDoiLichs",
                column: "MaHlvGui");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauDoiLichs_MaHlvNhan",
                table: "YeuCauDoiLichs",
                column: "MaHlvNhan");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauDoiLichs_MaLichGui",
                table: "YeuCauDoiLichs",
                column: "MaLichGui");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauDoiLichs_MaLichNhan",
                table: "YeuCauDoiLichs",
                column: "MaLichNhan");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauDoiLichs_MaThanhVien",
                table: "YeuCauDoiLichs",
                column: "MaThanhVien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaiViets");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "LichTapThanhViens");

            migrationBuilder.DropTable(
                name: "OtpCodes");

            migrationBuilder.DropTable(
                name: "ThietBis");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropTable(
                name: "ThanhToans");

            migrationBuilder.DropTable(
                name: "YeuCauDoiLichs");

            migrationBuilder.DropTable(
                name: "DangKyGoiTaps");

            migrationBuilder.DropTable(
                name: "LichLamViecHLVs");

            migrationBuilder.DropTable(
                name: "GoiTaps");

            migrationBuilder.DropTable(
                name: "ThanhViens");

            migrationBuilder.DropTable(
                name: "CaLamViec");

            migrationBuilder.DropTable(
                name: "HuanLuyenViens");

            migrationBuilder.DropTable(
                name: "NguoiDungs");
        }
    }
}
