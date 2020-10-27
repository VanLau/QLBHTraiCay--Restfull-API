using QLBHTraiCayAPI_DoAn.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBHTraiCayAPI_DoAn.DTO
{
    public class HoaDonDTO
    {
        public int ID { get; set; }
        public DateTime NgayDatHang { get; set; }
        public string HoTenKhach { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public int TongTien { get; set; }
       
    }
}