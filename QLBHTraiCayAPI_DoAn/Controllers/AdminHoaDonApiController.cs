using QLBHTraiCayAPI_DoAn.DAL;
using QLBHTraiCayAPI_DoAn.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Data.Entity;

namespace QLBHTraiCayAPI_DoAn.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/admin-hoa-don")]
    public class AdminHoaDonApiController : ApiController
    {
        QLBHTraiCayApiDbContext db = new QLBHTraiCayApiDbContext();

        #region Đọc tất cả hóa đơn
        //GET: api/AdminHoaDonApi/DocTatCaHoaDon
        //--->api/admin-hoa-don/hoa-don
        [Route("doc-tat-ca-hoa-don")]
        [HttpGet]
        [ResponseType(typeof(List<HoaDonDTO>))]
        public async Task<IHttpActionResult> DocTatCaHoaDon()
        {
            try
            {
                var items = await db.HoaDons
                                    .Include(p => p.HoaDonChiTiets)
                                    .Select(p => new HoaDonDTO
                                    {
                                        ID = p.ID,
                                        NgayDatHang = p.NgayDatHang,
                                        HoTenKhach = p.HoTenKhach,
                                        DiaChi = p.DiaChi, 
                                        DienThoai = p.DienThoai,
                                        Email = p.Email,
                                        TongTien = p.TongTien
                                    })
                                    .ToListAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Chi tiết hóa đơn BS
        //GET: api/AdminHoaDonApi/DocChiTiet/3
        //--->api/admin-hoa-don/doc-chi-tiet/3
        [Route("doc-chi-tiet/{id}")]
        [HttpGet]
        [ResponseType(typeof(List<HoaDonChiTietBSDTO>))]
        public async Task<IHttpActionResult> DocChiTiet(int id)
        {
            try
            {
                var items = await db.HoaDonChiTiets
                                    .Where(p => p.HoaDonID == id)
                                    .Include(p => p.HoaDon)
                                    .Include(p=>p.HangHoa)
                                    .Select(p => new HoaDonChiTietBSDTO
                                    {
                                        ID = p.HoaDonID,
                                        NgayDatHang = p.HoaDon.NgayDatHang,
                                        HoTenKhach = p.HoaDon.HoTenKhach,
                                        TenHang = p.HangHoa.TenHang,
                                        SoLuong = p.SoLuong,
                                        DonGia = p.DonGia,
                                        ThanhTien = p.ThanhTien
                                    })
                                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Chi tiết hóa đơn
        //GET: api/AdminHoaDonApi/DocChiTietHoaDon/3
        //--->api/admin-hoa-don/doc-chi-tiet-hoa-don/3
        [Route("doc-chi-tiet-hoa-don/{id}")]
        [HttpGet]
        [ResponseType(typeof(HoaDonDTO))]
        public async Task<IHttpActionResult> DocChiTietHoaDon(int id)
        {
            try
            {
                var item = await db.HoaDons
                                    .Where(p => p.ID == id)
                                    .Include(p => p.HoaDonChiTiets)
                                    .Select(p => new HoaDonDTO
                                    {
                                        ID = p.ID,
                                        NgayDatHang = p.NgayDatHang,
                                        HoTenKhach = p.HoTenKhach,
                                        DiaChi = p.DiaChi,
                                        DienThoai = p.DienThoai,
                                        Email = p.Email,
                                        TongTien = p.TongTien
                                    })
                                    .SingleOrDefaultAsync();

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Search hóa đơn theo Tên - POST
        //GET: api/AdminHangHoaApi/TimKiemTen
        //--> api/hang-hoa/tim-kiem-ten
        [Route("tim-kiem-ten/{value}")]
        [HttpGet]
        [ResponseType(typeof(List<HoaDonDTO>))]
        public async Task<IHttpActionResult> TimKiemTen(string value)
        {
            try
            {
                var hoaDons = await db.HoaDons
                                        .Where(p => p.HoTenKhach.Contains(value))
                                        .Select(p => new HoaDonDTO
                                        {
                                            ID = p.ID,
                                            NgayDatHang = p.NgayDatHang,
                                            HoTenKhach = p.HoTenKhach,
                                            DiaChi = p.DiaChi,
                                            DienThoai = p.DienThoai,
                                            Email = p.Email,
                                            TongTien = p.TongTien
                                        })
                                        .ToListAsync();
                return Ok(hoaDons);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Sửa hóa đơn - POST
        //POST: api/AdminHoaDonApi/HieuChinh
        //--->api/admin-hoa-don/sua-hoa-don
        [Route("sua-hoa-don")]
        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> HieuChinh(HoaDonDTO input)
        {
            try
            {
                HoaDon entity = await db.HoaDons.FindAsync(input.ID);
                if (entity == null) return BadRequest($"Hóa đơn ID ={input.ID} không tồn tại");
                if (!ModelState.IsValid) return BadRequest(ModelState);

                entity.HoTenKhach = input.HoTenKhach;
                entity.DiaChi = input.DiaChi;
                entity.DienThoai = input.DienThoai;
                entity.Email = input.Email;

                await db.SaveChangesAsync();
                return Ok($"Sửa hóa đơn thành công");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiệu chỉnh không thành công {ex.Message}");
            }

        }
        #endregion
    }
}
