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
    [RoutePrefix("api/admin-chung-loai")]
    public class AdminChungLoaiApiController : ApiController
    {
        QLBHTraiCayApiDbContext db = new QLBHTraiCayApiDbContext();
        #region Đọc tất cả chủng loại
        //GET: api/AminChungLoaiApi/ChungLoaiDocTatCa
        //--->api/admin-chung-loai/doc-tat-ca
        [Route("doc-tat-ca")]
        [HttpGet]
        [ResponseType(typeof(List<ChungLoaiOutput>))]
        public async Task<IHttpActionResult> ChungLoaiDocTatCa()
        {
            try
            {
                var items = await db.ChungLoais
                                    .Select(p => new ChungLoaiOutput
                                    {
                                        chungLoaiEntity = p
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

        #region Chi tiết chủng loại
        //GET: api/AdminChungLoaiApi/DocChiTiet/3
        //--->api/admin-chung-loai/doc-chi-tiet/3
        [Route("doc-chi-tiet/{id}")]
        [HttpGet]
        [ResponseType(typeof(LoaiOutput))]
        public async Task<IHttpActionResult> DocChiTiet(int id)
        {
            try
            {
                var item = await db.ChungLoais
                                    .Where(p => p.ID == id)
                                    .Select(p => new ChungLoaiOutput
                                    {
                                        chungLoaiEntity = p
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

        #region Search chủng Loại theo Tên hoặc mã - POST
        //GET: api/AdminHangHoaApi/TimKiemMaten
        //--> api/hang-hoa/tim-kiem-chung-loai/{value}
        [Route("tim-kiem-chung-loai/{value}")]
        [HttpGet]
        [ResponseType(typeof(List<ChungLoaiOutput>))]
        public async Task<IHttpActionResult> TimKiemMaTen(string value)
        {
            try
            {
                var chungLoais = await db.ChungLoais
                                    .Where(p => p.TenCL.Contains(value) || p.MaCL.Contains(value))
                                    .Select(p => new ChungLoaiOutput
                                    {
                                        chungLoaiEntity = p
                                    })
                                    .ToListAsync();
                return Ok(chungLoais);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi không truy cập được dữ liệu. Lý do: {ex.Message}");
            }
        }
        #endregion

        #region Thêm Chủng Loại - POST
        //POST: api/AdminChungLoaiApi/Them
        //--->api/admin-chung-loai/them-moi
        [Route("them-moi")]
        [HttpPost]
        [ResponseType(typeof(ChungLoaiInput))]
        public async Task<IHttpActionResult> Them(ChungLoaiInput input)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int d1 = await db.ChungLoais.CountAsync(p => p.MaCL.StartsWith(input.MaCL) || input.MaCL.StartsWith(p.MaCL));
                if (d1 > 0) return BadRequest($"Mã chủng loại ='{input.MaCL}' đã có rồi.");

                var entity = new ChungLoai();
                entity.MaCL = input.MaCL;
                entity.TenCL = input.TenCL;

                db.ChungLoais.Add(entity);
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

        #region Sửa chủng loại - POST
        //POST: api/AdminChungLoaiApi/HieuChinh
        //--->api/admin-chung-loai/sua-loai
        [Route("sua")]
        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> HieuChinh(ChungLoaiInput input)
        {
            try
            {
                ChungLoai chungLoai = await db.ChungLoais.FindAsync(input.ID);
                if (chungLoai == null) return BadRequest($"Chủng loại ID ={input.ID} không tồn tại");
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int d = await db.ChungLoais.CountAsync(p => p.ID != input.ID && (p.MaCL.StartsWith(input.MaCL) || input.MaCL.StartsWith(p.MaCL)));
                if (d > 0) return BadRequest($"Mã chủng loại ='{input.MaCL}' đã có hoặc lồng nhau.");

                chungLoai.MaCL = input.MaCL;
                chungLoai.TenCL = input.TenCL;

                await db.SaveChangesAsync();
                return Ok("Hiệu chỉnh thành công");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiệu chỉnh không thành công {ex.Message}");
            }
        }
        #endregion

        #region Xóa chủng loại--POST
        //POST: api/AdminChungLoaiApi/Xoa/6
        //--->api/admin-chung-loai/xoa-chung-loai/6
        [Route("xoa-chung-loai/{id}")]
        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Xoa(int id)
        {
            try
            {
                ChungLoai entity = await db.ChungLoais.FindAsync(id);
                if (entity == null) return BadRequest($"Chủng loại ID={id} không tồn tại.");

                db.ChungLoais.Remove(entity);
                await db.SaveChangesAsync();

                return Ok($"Đã xóa thông tin của chủng loại ID={id} thành công.");
            }
            catch (Exception ex)
            {
                int d = await db.Loais.CountAsync(p => p.ChungLoaiID == id);
                if (d > 0) return BadRequest($"Không xóa được vì đã có {d} loại phụ thuộc.");
                return BadRequest($"Hiệu chỉnh Không thành công. {ex.Message}");
            }
        }
        #endregion
    }
}
