using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBHTraiCayAPI_DoAn.DTO
{
    public class InputHDDTO
    {
        public HoaDonDTO HDItem { get; set; }
        public List<HoaDonChiTietDTO> HDCTItems { get; set; }
    }

}