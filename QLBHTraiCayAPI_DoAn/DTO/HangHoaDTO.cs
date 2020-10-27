using QLBHTraiCayAPI_DoAn.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBHTraiCayAPI_DoAn.DTO
{
    public class HangHoaInput
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Mã hàng")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        [MaxLength(10, ErrorMessage = "{0} tối đa là {1} ký tự")]
        [MinLength(2, ErrorMessage = "{0} tối thiểu là {1} ký tự")]
        public string MaHang { get; set; }

        [Display(Name = "Tên hàng")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        [MaxLength(100, ErrorMessage = "{0} tối đa là {1} ký tự")]
        public string TenHang { get; set; }


        [Display(Name = "Đơn vị tính")]
        [MaxLength(20, ErrorMessage = "{0} tối đa là {1} ký tự")]
        public string DVT { get; set; }

        [Display(Name = "Quy cách")]
        [MaxLength(50, ErrorMessage = "{0} tối đa là {1} ký tự")]
        public string QuyCach { get; set; }

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Display(Name = "Giá bán")]
        [DisplayFormat(DataFormatString = "{0:#,##0VND}")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        [RegularExpression(@"\d*", ErrorMessage = "{0} Phải nhập số nguyên >=0.")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} phải từ {1} đến {2}")]
        public int GiaBan { get; set; }

        [Display(Name = "Giá thị trường")]
        [DisplayFormat(DataFormatString = "{0:#,##0VND}")]
        [RegularExpression(@"\d*", ErrorMessage = "{0} Phải nhập số nguyên >=0.")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} phải từ {1} đến {2}")]
        public Nullable<int> GiaThiTruong { get; set; }

        [Display(Name = "Loại")]
        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn {0} cho mặt hàng.")]
        [RegularExpression(@"\d*", ErrorMessage = "{0} phải là số nguyên")]
        public int LoaiID { get; set; }

        //[Display(Name = "Ngày tạo")]
        //[Required(ErrorMessage = "{0} không được để trống.")]
        //[DisplayFormat(DataFormatString = "0:dd/MM/yyyy")]
        //public DateTime NgayTao { get; set; }

        //[Display(Name = "Ngày sửa")]
        //[DisplayFormat(DataFormatString = "0:dd/MM/yyyy")]
        //[Required(ErrorMessage = "{0} không được để trống.")]
        //public DateTime NgaySua { get; set; }

        [Display(Name = "Xuất xứ")]
        public string XuatXu { get; set; }

        [Display(Name = "Tình Trạng")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        [RegularExpression(@"\d*", ErrorMessage = "{0} phải là số nguyên")]
        public int TinhTrang { get; set; }
    }

    public class HangHoaOutput
    {
        internal HangHoaOutput()
        {
            hangHoaEntity = new HangHoa();
            loaiEntity = new Loai();
        }
        
        internal HangHoa hangHoaEntity { private get; set; }
        internal Loai loaiEntity { private get; set; }

        public int ID => hangHoaEntity.ID;
        public string MaHang => hangHoaEntity.MaHang;
        public string TenHang => hangHoaEntity.TenHang;
        public string DVT => hangHoaEntity.DVT;
        public string QuyCach => hangHoaEntity.QuyCach;
        public string MoTa => hangHoaEntity.MoTa;
        public int GiaBan => hangHoaEntity.GiaBan;
        public Nullable<int> GiaThiTruong => hangHoaEntity.GiaThiTruong;
        public int LoaiID => hangHoaEntity.LoaiID;
        public string NgayTao => hangHoaEntity.NgayTao.ToString("yyyy-MM-dd");
        public string NgaySua => hangHoaEntity.NgayTao.ToString("yyyy-MM-dd");
        public string XuatXu => hangHoaEntity.XuatXu;
        public int TinhTrang => hangHoaEntity.TinhTrang;

        public LoaiInput loai => new LoaiInput
        {
            ID = loaiEntity.ID,
            MaLoai = loaiEntity.MaLoai,
            TenLoai = loaiEntity.TenLoai,
            ChungLoaiID = loaiEntity.ChungLoaiID
        };
        public List<string> HinhURLs
        {
            get
            {
                string Authority = HttpContext.Current.Request.Url.Authority;
                string ApplicationPath = HttpContext.Current.Request.ApplicationPath;
                if (ApplicationPath.Length > 1) ApplicationPath += "/";
                List<string> urls = new List<string>();
                if (!string.IsNullOrEmpty(hangHoaEntity.TenHinh))
                {
                    var arrTenHinh = hangHoaEntity.TenHinh.Split(',');
                    foreach (var tenHinh in arrTenHinh)
                    {
                        urls.Add($"https://{Authority}{ApplicationPath}Photos/{tenHinh}");
                    }
                }
                else
                    urls.Add($"https://{Authority}{ApplicationPath}Photos/noImage.jpg");
                return urls;
            }            
        }       
    }

    public class HangHoaOutputBS
    {
        public int ID { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string DVT { get; set; }
        public string QuyCach { get; set; }
        public string MoTa { get; set; }
        public string TenHinh { get; set; }
        public int GiaBan { get; set; }
        public Nullable<int> GiaThiTruong { get; set; }
        public string XuatXu { get; set; }
        public int TinhTrang { get; set; }

        public List<string> HinhURLs
        {
            get
            {
                string Authority = HttpContext.Current.Request.Url.Authority;
                string ApplicationPath = HttpContext.Current.Request.ApplicationPath;
                if (ApplicationPath.Length > 1) ApplicationPath += "/";
                List<string> urls = new List<string>();
                if (!string.IsNullOrEmpty(TenHinh))
                {
                    var arrTenHinh = TenHinh.Split(',');
                    foreach (var tenHinh in arrTenHinh)
                    {
                        urls.Add($"https://{Authority}{ApplicationPath}Photos/{tenHinh}");
                    }
                }
                else
                    urls.Add($"https://{Authority}{ApplicationPath}Photos/noImage.jpg");
                return urls;
            }
            set { }
        }

        public int SoLuong { get; set; }
    }

    public class HinhAnh
    {
        public string TenHinh { get; set; }

        public List<string> Hinhs
        {
            get
            {
                var _Hinhs = new List<string>();
                if (!string.IsNullOrEmpty(TenHinh))
                    _Hinhs.AddRange(TenHinh.Split(','));
                else
                    _Hinhs.Add("noImage.jpg");
                return _Hinhs;
            }

            set { }
        }
    }
}