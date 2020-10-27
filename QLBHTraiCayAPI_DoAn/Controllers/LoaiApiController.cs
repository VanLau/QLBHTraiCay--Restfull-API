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
    [RoutePrefix("api/loai")]
    public class LoaiApiController : ApiController
    {
        QLBHTraiCayApiDbContext db = new QLBHTraiCayApiDbContext();
        #region Đọc tất cả loại
        //GET: api/LoaiApi/DocTatCa
        //--->api/loai/doc-tat-ca
        [Route("doc-tat-ca")]
        [HttpGet]
        [ResponseType(typeof(List<LoaiOutput>))]
        public async Task<IHttpActionResult> DocTatCa()
        {
            try
            {
                var items = await db.Loais                                  
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

        #region Đọc loại theo id
        //GET: api/LoaiApi/LoaiTheoID
        //--->api/loai/loai-theo-id
        [Route("loai-theo-id/{id}")]
        [HttpGet]
        [ResponseType(typeof(List<object>))]
        public async Task<IHttpActionResult> LoaiTheoID(int id)
        {
            try
            {
                var items = await db.Loais
                                    .Include(p=>p.ChungLoai)
                                    .Where(p=>p.ID==id)
                                    .Select(p => new LoaiOutput
                                    {
                                        LoaiEntity = p,
                                        ChungLoaiEntity = p.ChungLoai
                                    })
                                    .SingleOrDefaultAsync();

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
