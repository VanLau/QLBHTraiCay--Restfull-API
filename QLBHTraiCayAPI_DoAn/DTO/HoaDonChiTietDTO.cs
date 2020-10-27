using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBHTraiCayAPI_DoAn.DTO
{
    public class HoaDonChiTietDTO
    {
        public int HoaDonID { get; set; }
        public int HangHoaID { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public int ThanhTien { get; set; }
    }

    public class HoaDonChiTietBSDTO
    {
        public int ID { get; set; }
        public DateTime NgayDatHang { get; set; }
        public string HoTenKhach { get; set; }
        public string TenHang { get; set; }
        public int SoLuong { get; set; }
        public int DonGia { get; set; }
        public int ThanhTien { get; set; }
    }
}