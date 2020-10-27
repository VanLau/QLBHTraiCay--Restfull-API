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
    [RoutePrefix("api/admin-loai")]
    public class AdminLoaiApiController : ApiController
    {
        QLBHTraiCayApiDbContext db = new QLBHTraiCayApiDbContext();
        #region Đọc tất cả loại
        //GET: api/AminLoaiApi/LoaiDocTatCa
        //--->api/admin-loai/doc-tat-ca
        [Route("doc-tat-ca")]
        [HttpGet]
        [ResponseType(typeof(List<LoaiOutput>))]
        public async Task<IHttpActionResult> LoaiDocTatCa()
        {
            try
            {
                var items = await db.Loais
                                    .Include(p => p.ChungLoai)
                                    .Select(p => new LoaiOutput
                                    {
                                        LoaiEntity = p,
                                        ChungLoaiEntity = p.ChungLoai
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

        #region Chi tiết loại
        //GET: api/AdminLoaiApi/DocChiTiet/3
        //--->api/admin-loai/doc-chi-tiet/3
        [Route("doc-chi-tiet/{id}")]
        [HttpGet]
        [ResponseType(typeof(LoaiOutput))]
        public async Task<IHttpActionResult> DocChiTiet(int id)
        {
            try
            {
                var item = await db.Loais
                                    .Where(p => p.ID == id)
                                    .Include(p => p.ChungLoai)
                                    .Select(p => new LoaiOutput
                                    {
                                        LoaiEntity = p,
                                        ChungLoaiEntity = p.ChungLoai
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

        #region Search Loại theo Tên hoặc mã - POST
        //GET: api/AdminHangHoaApi/TimKiemMaten
        //--> api/hang-hoa/tim-kiem-ma-ten
        [Route("tim-kiem-loai/{value}")]
        [HttpGet]
        [ResponseType(typeof(List<LoaiOutput>))]
        public async Task<IHttpActionResult> TimKiemMaTen(string value)
        {
            try
            {
                var loais = await db.Loais
                                    .Where(p => p.TenLoai.Contains(value) || p.MaLoai.Contains(value))
                                    .Include(p => p.ChungLoai)
                                    .Select(p => new LoaiOutput
                                    {
                                        LoaiEntity = p,
                                        ChungLoaiEntity = p.ChungLoai
                                    })
                                    .ToListAsync();
                return Ok(loais);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Thêm Loại - POST
        //POST: api/LoaiApi/Them
        //--->api/loai/them-moi
        [Route("them-moi")]
        [HttpPost]
        [ResponseType(typeof(LoaiInput))]
        public async Task<IHttpActionResult> Them(LoaiInput input)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int d1 = await db.Loais.CountAsync(p => p.MaLoai.StartsWith(input.MaLoai) || input.MaLoai.StartsWith(p.MaLoai));
                if (d1 > 0) return BadRequest($"Mã loại ='{input.MaLoai}' đã có rồi.");
                bool ktFK = await db.ChungLoais.AnyAsync(p => p.ID == input.ChungLoaiID);
                if (!ktFK) return BadRequest($"Chủng loại ID='{input.ChungLoaiID}' không tồn tại.");

                var entity = new Loai();
                entity.MaLoai = input.MaLoai;
                entity.TenLoai = input.TenLoai;
                entity.ChungLoaiID = input.ChungLoaiID;

                db.Loais.Add(entity);
                await db.SaveChangesAsync();

                input.ID = entity.ID;
                return Ok(input);
            }
            catch (Exception ex)
            {
                return BadRequest($"Thêm Không thành công. {ex.Message}");
            }
        }
        #endregion

        #region Sửa loại - POST
        //POST: api/AdminLoaiApi/HieuChinh
        //--->api/admin-loai/sua-loai
        [Route("sua-loai")]
        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> HieuChinh(LoaiInput input)
        {
            try
            {
                Loai loai = await db.Loais.FindAsync(input.ID);
                if (loai == null) return BadRequest($"Loại ID ={input.ID} không tồn tại");
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int d = await db.Loais.CountAsync(p => p.ID != input.ID && (p.MaLoai.StartsWith(input.MaLoai) || input.MaLoai.StartsWith(p.MaLoai)));
                if (d > 0) return BadRequest($"Mã loại ='{input.MaLoai}' đã có hoặc lồng nhau.");

                loai.MaLoai = input.MaLoai;
                loai.TenLoai = input.TenLoai;
                loai.ChungLoaiID = input.ChungLoaiID;

                await db.SaveChangesAsync();
                return Ok("Hiệu chỉnh thành công");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiệu chỉnh không thành công {ex.Message}");
            }

        }
        #endregion

        #region Xóa --POST
        //POST: api/AdminLoaiApi/Xoa/6
        //--->api/admin-loai/xoa-loai/6
        [Route("xoa-loai/{id}")]
        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Xoa(int id)
        {
            try
            {
                Loai entity = await db.Loais.FindAsync(id);
                if (entity == null) return BadRequest($"Loại ID={id} không tồn tại.");

                db.Loais.Remove(entity);  
                await db.SaveChangesAsync();

                return Ok($"Đã xóa thông tin của chủng loại ID={id} thành công.");
            }
            catch (Exception ex)
            {
                int d = await db.HangHoas.CountAsync(p => p.LoaiID == id);
                if (d > 0) return BadRequest($"Không xóa được vì đã có {d} mặt hàng phụ thuộc.");
                return BadRequest($"Hiệu chỉnh Không thành công. {ex.Message}");
            }
        }
        #endregion
    }
}
