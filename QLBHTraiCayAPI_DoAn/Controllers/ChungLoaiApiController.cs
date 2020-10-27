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
    [RoutePrefix("api/chung-loai")]
    public class ChungLoaiApiController : ApiController
    {
        QLBHTraiCayApiDbContext db = new QLBHTraiCayApiDbContext();

        #region Danh sách Chủng Loại (object)
        //GET: api/ChungLoaiApi/DocTatCa
        //--->api/chung-loai/doc-tat-ca
        [Route("danh-sach-chung-loai")]
        [HttpGet]
        [ResponseType(typeof(object))]
        public async Task<IHttpActionResult> DanhSachChungLoai()
        {
            try
            {
                var items = await db.ChungLoais
                                    .Where(p => p.Loais.Count > 0)
                                    .Include(p => p.Loais)
                                    .Select(p => new 
                                    {
                                        p.ID,
                                        p.MaCL,
                                        p.TenCL,
                                        loais = p.Loais.Select(l=>new
                                        {
                                            l.ID,
                                            l.MaLoai,
                                            l.TenLoai,
                                        })
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

    }
}
