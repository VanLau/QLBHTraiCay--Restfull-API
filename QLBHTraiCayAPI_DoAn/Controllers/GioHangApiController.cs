using QLBHTraiCayAPI_DoAn.DAL;
using QLBHTraiCayAPI_DoAn.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace QLBHTraiCayAPI_DoAn.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/gio-hang")]
    public class GioHangApiController : ApiController
    {
        QLBHTraiCayApiDbContext db = new QLBHTraiCayApiDbContext();

        #region Thêm hóa đơn -- POST
        //POST: api/GioHangApi/HoaDon
        //--->api/gio-hang/hoa-don
        [Route("hoa-don")]
        [HttpPost]
        [ResponseType(typeof(List<HoaDonChiTietBSDTO>))]
        public async Task<IHttpActionResult> HoaDon(InputHDDTO input)
        {
            try
            {
                //1.Thêm HoaDon
                var entity = new HoaDon();
                ConvertHoaDonDTOToEntity(input.HDItem, entity, true);
                db.HoaDons.Add(entity);

                //2.Thêm HoaDonChiTiet
                foreach (var item in input.HDCTItems)
                {
                    HoaDonChiTiet hDCT = new HoaDonChiTiet();
                    hDCT.HoaDonID = item.HoaDonID;
                    hDCT.HangHoaID = item.HangHoaID;
                    hDCT.SoLuong = item.SoLuong;
                    hDCT.DonGia = item.DonGia;
                    hDCT.ThanhTien = item.ThanhTien;
                    db.HoaDonChiTiets.Add(hDCT);
                }
                await db.SaveChangesAsync();

                var hoaDonCTs = await db.HoaDonChiTiets
                                        .Where(p => p.HoaDonID == entity.ID)
                                        .Include(p => p.HoaDon)
                                        .Include(p => p.HangHoa) 
                                        .Select(p=>new HoaDonChiTietBSDTO
                                        {
                                            ID = p.HoaDon.ID,
                                            NgayDatHang = p.HoaDon.NgayDatHang,
                                            HoTenKhach = p.HoaDon.HoTenKhach,
                                            TenHang = p.HangHoa.TenHang,
                                            SoLuong = p.SoLuong,
                                            DonGia =p.DonGia,
                                            ThanhTien = p.ThanhTien
                                        })
                                        .ToListAsync();               
                return Ok(hoaDonCTs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Thêm Không thành công. {ex.Message}");
            }
        }
        #endregion

        #region Phương thức sử dụng cục bộ
        private void ConvertHoaDonDTOToEntity(HoaDonDTO input, HoaDon entity, bool HoaDon =true)
        {
            entity.ID = input.ID;            
            entity.HoTenKhach = input.HoTenKhach;
            entity.DiaChi = input.DiaChi;
            entity.DienThoai = input.DienThoai;
            entity.Email = input.Email;
            entity.TongTien = input.TongTien;
            if(HoaDon==true) entity.NgayDatHang = DateTime.Now;
        }
        #endregion
    }
}
