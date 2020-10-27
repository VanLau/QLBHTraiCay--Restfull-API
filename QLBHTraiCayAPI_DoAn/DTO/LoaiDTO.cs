using QLBHTraiCayAPI_DoAn.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBHTraiCayAPI_DoAn.DTO
{
    public class LoaiInput
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Mã loại")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        [MaxLength(10, ErrorMessage = "{0} tối đa là {1} ký tự.")]
        public string MaLoai { get; set; }

        [Display(Name = "Tên loại")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [MaxLength(100, ErrorMessage = "{0} phải nhập tối đa là {1} ký tự.")]
        public string TenLoai { get; set; }

        [Display(Name = "Chủng loại")]
        public Nullable<int> ChungLoaiID { get; set; }
    }

    public class LoaiOutput
    {
        internal LoaiOutput()
        {
            LoaiEntity = new Loai();
            ChungLoaiEntity = new ChungLoai();
        }

        internal Loai LoaiEntity { private get; set; }
        internal ChungLoai ChungLoaiEntity { private get; set; }

        public int ID => LoaiEntity.ID;
        public string MaLoai => LoaiEntity.MaLoai;
        public string TenLoai => LoaiEntity.TenLoai;
        public Nullable<int> ChungLoaiID => LoaiEntity.ChungLoaiID;

        public ChungLoaiInput ChungLoai => new ChungLoaiInput
        {
            ID = ChungLoaiEntity.ID,
            MaCL = ChungLoaiEntity.MaCL,
            TenCL = ChungLoaiEntity.TenCL
        };
    }
}