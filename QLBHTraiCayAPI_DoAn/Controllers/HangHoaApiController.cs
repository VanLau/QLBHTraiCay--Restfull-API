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
    [RoutePrefix("api/hang-hoa")]
    public class HangHoaApiController : ApiController
    {
        QLBHTraiCayApiDbContext db = new QLBHTraiCayApiDbContext();

        #region Hàng hóa trong nước (Theo Chủng loại)
        //POST: api/HangHoaApi/DocMotTrangHangHoa
        //--> api/hang-hoa/doc-mot-trang-hang-hoa
        [Route("doc-mot-trang-hang-hoa-theo-chung-loai/{CLID}")]
        [HttpPost]
        [ResponseType(typeof(PagedOutput<HangHoaOutput>))]
        public async Task<IHttpActionResult> DocMotTrangHangHoa([FromBody] PagedInput input, int CLID)
        {
            try
            {
                int n = (input.PageIndex - 1) * input.PageSize;
                int totalItems = await db.HangHoas.CountAsync(p=>p.Loai.ChungLoaiID==CLID);
                var hangHoaItems = await db.HangHoas
                                           .Where(p=>p.Loai.ChungLoaiID==CLID)
                                           .OrderBy(p => p.ID)
                                           .Skip(n)
                                           .Take(input.PageSize)
                                           .Include(p => p.Loai)
                                           .Select(p => new HangHoaOutput
                                           {
                                               hangHoaEntity = p,
                                               loaiEntity = p.Loai
                                           })
                                           .ToListAsync();
                var onePageOfData = new PagedOutput<HangHoaOutput>
                {
                    Items = hangHoaItems,
                    TotalItemCount = totalItems
                };
                return Ok(onePageOfData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Hang hoa Nhập khẩu (Theo Chủng loại)
        //POST: api/HangHoaApi/HangHoaNhapKhau
        //--> api/hang-hoa/-hang-hoa-nhap-khau
        [Route("hang-hoa-nhap-khau/{CLID}")]
        [HttpGet]
        [ResponseType(typeof(List<HangHoaOutput>))]
        public async Task<IHttpActionResult> HangHoaNhapKhau(int CLID)
        {
            try
            {
                var hangHoaItems = await db.HangHoas
                                           .Where(p => p.Loai.ChungLoaiID == CLID)
                                           .Select(p => new HangHoaOutput
                                           {
                                               hangHoaEntity = p,
                                               loaiEntity = p.Loai
                                           })
                                           .ToListAsync();

                return Ok(hangHoaItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Đọc một trang hang hoa (shop)
        //POST: api/HangHoaApi/DocMotTrangHangHoa
        //--> api/hang-hoa/doc-mot-trang-hang-hoa
        [Route("doc-mot-trang-hang-hoa")]
        [HttpPost]
        [ResponseType(typeof(PagedOutput<HangHoaOutput>))]
        public async Task<IHttpActionResult> DocMotTrangHangHoa([FromBody] PagedInput input)
        {
            try
            {
                int n = (input.PageIndex - 1) * input.PageSize;
                int totalItems = await db.HangHoas.CountAsync();
                var hangHoaItems = await db.HangHoas
                                           .OrderBy(p => p.ID)
                                           .Skip(n)
                                           .Take(input.PageSize)
                                           .Include(p => p.Loai)
                                           .Select(p => new HangHoaOutput
                                           {
                                               hangHoaEntity = p,
                                               loaiEntity = p.Loai
                                           })
                                           .ToListAsync();
                var onePageOfData = new PagedOutput<HangHoaOutput>
                {
                    Items = hangHoaItems,
                    TotalItemCount = totalItems
                };
                return Ok(onePageOfData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Chi tiết hàng hóa
        //GET: api/HangHoaApi/DocChiTiet/3
        //--->api/hang-hoa/doc-chi-tiet/3
        [Route("doc-chi-tiet/{id}")]
        [HttpGet]
        [ResponseType(typeof(HangHoaOutput))]
        public async Task<IHttpActionResult> DocChiTiet(int id) 
        {
            try
            {
                var item = await db.HangHoas
                                    .Where(p => p.ID == id)
                                    .Include(p => p.Loai)
                                    .Select(p => new HangHoaOutput
                                    {
                                        hangHoaEntity = p,
                                        loaiEntity = p.Loai
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

        #region Đọc 1 trang theo Loại
        [Route("doc-mot-trang-theo-loai/{id}")]
        [HttpPost]
        [ResponseType(typeof(PagedOutput<HangHoaOutput>))]
        public async Task<IHttpActionResult> DocMotTrangTheoLoai([FromBody] PagedInput input, int id)
        {
            try
            {
                var LoaiItem = await db.Loais
                                       .SingleOrDefaultAsync(p => p.ID == id);
                if (LoaiItem == null) throw new Exception($"Loại ID={id} không tồn tại."); ;
                int n = (input.PageIndex - 1) * input.PageSize;
                int totalItems = await db.HangHoas.CountAsync(p=>p.LoaiID==id);
                var hangHoaItems = await db.HangHoas
                                         .Where(p => p.LoaiID == id)
                                         .OrderBy(p => p.ID)
                                         .Skip(n)
                                         .Take(input.PageSize)
                                         .Select(p => new HangHoaOutput
                                         {
                                             hangHoaEntity = p,
                                             loaiEntity = p.Loai
                                         })
                                         .ToListAsync();
                var onePageOfData = new PagedOutput<HangHoaOutput>
                {
                    Items = hangHoaItems,
                    TotalItemCount = totalItems
                };
                return Ok(onePageOfData);
            }
            catch(Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. lý do: {ex.Message}");
            }
        }
        #endregion

        #region Sản phẩm bán chạy
        [Route("san-pham-ban-chay")]
        [HttpGet]
        [ResponseType(typeof(List<HangHoaOutputBS>))]
        public async Task<IHttpActionResult> HangHoaBanChay()
        {
            try
            {
                var rs = await db.HangHoas
                           .Join(db.HoaDonChiTiets,
                                    emp => emp.ID, per => per.HangHoaID,
                                    (emp, per) => new
                                    {
                                        emp.ID,
                                        emp.MaHang,
                                        emp.TenHang,
                                        emp.DVT,
                                        emp.QuyCach,
                                        emp.MoTa,
                                        emp.TenHinh,
                                        emp.GiaBan,
                                        emp.GiaThiTruong,
                                        emp.XuatXu,
                                        emp.TinhTrang,
                                        per.SoLuong
                                        
                                    })
                           .GroupBy(x => x.ID)
                           .Select(a => new HangHoaOutputBS
                           {
                               ID = a.Max(b => b.ID),
                               MaHang = a.Max(b => b.MaHang),
                               TenHang = a.Max(b => b.TenHang),
                               DVT = a.Max(b => b.DVT),
                               QuyCach = a.Max(b => b.QuyCach),
                               MoTa = a.Max(b => b.MoTa),
                               TenHinh = a.Max(b => b.TenHinh),
                               GiaBan = a.Max(b => b.GiaBan),
                               GiaThiTruong = a.Max(b => b.GiaThiTruong),   
                               XuatXu = a.Max(b=>b.XuatXu),
                               TinhTrang = a.Max(b=>b.TinhTrang),
                               SoLuong = a.Sum(b => b.SoLuong)
                           })
                          .OrderByDescending(a => a.SoLuong)
                          .Take(7)
                          .ToArrayAsync();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Search hàng hóa theo Tên - POST
        //POST: api/HangHoaApi/TimKiem
        //--> api/hang-hoa/tim-kiem
        [Route("tim-kiem/{value}")]
        [HttpPost]
        [ResponseType(typeof(PagedOutput<HangHoaOutput>))]
        public async Task<IHttpActionResult> TimKiem([FromBody] PagedInput input, string value)
        {
            try
            {
                int n = (input.PageIndex - 1) * input.PageSize;
                int totalItems = await db.HangHoas
                                         .Where(p => p.TenHang.Contains(value))
                                         .CountAsync();
                var hangHoaItems = await db.HangHoas
                                           .Where(p => p.TenHang.Contains(value))
                                           .OrderBy(p => p.ID)
                                           .Skip(n)
                                           .Take(input.PageSize)
                                           .Include(p => p.Loai)
                                           .Select(p => new HangHoaOutput
                                           {
                                               hangHoaEntity = p,
                                               loaiEntity = p.Loai
                                           })
                                           .ToListAsync();
                var onePageOfData = new PagedOutput<HangHoaOutput>
                {
                    Items = hangHoaItems,
                    TotalItemCount = totalItems
                };
                return Ok(onePageOfData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion
    }
}
