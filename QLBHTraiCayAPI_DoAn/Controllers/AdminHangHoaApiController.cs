using QLBHTraiCayAPI_DoAn.DAL;
using QLBHTraiCayAPI_DoAn.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace QLBHTraiCayAPI_DoAn.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/admin-hang-hoa")]
    public class AdminHangHoaApiController : ApiController
    {
        QLBHTraiCayApiDbContext db = new QLBHTraiCayApiDbContext();
        #region Đọc tất cả Hàng Hóa
        //GET: api/AminHangHoaApi/DocTatCa
        //--->api/admin-hang-hoa/doc-tat-ca
        [Route("doc-tat-ca")]
        [HttpGet]
        [ResponseType(typeof(List<HangHoaOutput>))]
        public async Task<IHttpActionResult> HangHoaDocTatCa()
        {
            try
            {
                var items = await db.HangHoas
                                    .Include(p=> p.Loai)
                                    .Select(p => new HangHoaOutput
                                    {
                                        hangHoaEntity = p,
                                        loaiEntity = p.Loai
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

        #region Chi tiết hàng hóa
        //GET: api/AdminHangHoaApi/DocChiTiet/3
        //--->api/admin-hang-hoa/doc-chi-tiet/3
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

        #region Search hàng hóa theo Tên - POST
        //GET: api/AdminHangHoaApi/TimKiemMaten
        //--> api/hang-hoa/tim-kiem-ma-ten
        [Route("tim-kiem-ma-ten/{value}")]
        [HttpGet]
        [ResponseType(typeof(List<HangHoaOutput>))]
        public async Task<IHttpActionResult> TimKiemMaTen(string value)
        {
            try
            {
                var hangHoaItems = await db.HangHoas
                                           .Where(p => p.TenHang.Contains(value)|| p.MaHang.Contains(value))
                                           .Include(p => p.Loai)
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

        #region Thêm hàng hóa-- POST
        //POST: api/HangHoaApi/Them
        //--->api/hang-hoa/them-moi
        [Route("them-moi-hang-hoa")]
        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ThemHangHoa(HangHoaInput input)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                //Tránh Lồng mã 
                int d1 = await db.HangHoas.CountAsync(p => p.MaHang.StartsWith(input.MaHang)||input.MaHang.StartsWith(p.MaHang));
                if (d1 > 0) return BadRequest($"Mã số ='{input.MaHang}' bị trùng hoặc lồng nhau.");
                bool ktFK = await db.Loais.AnyAsync(p => p.ID == input.LoaiID);
                if (!ktFK) return BadRequest($"Loại ID='{input.LoaiID}' không tồn tại.");
                //Khỏi tạo 1 hanghoa mới (entity type - kiểu dữ liệu giao tiếp với nguồn dữ liệu)
                var entity = new HangHoa();
                //Gán giá trị
                ConvertHangHoaDTOToEntity(input, entity, true);
                //Thêm vao DbSet và lưu vào database 
                db.HangHoas.Add(entity);
                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Thêm Không thành công. {ex.Message}");
            }
        }
        #endregion

        #region Upload hình ảnh - POST
        [Route("upload-hinh-anh/{id}")]
        [HttpPost]
        [ResponseType(typeof(void))]

        ////cách 1
        //public async Task<IHttpActionResult> Upload([FromUri] int id = 0)
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        if (httpRequest.Files.Count > 0)
        //        {
        //            foreach (string fileName in httpRequest.Files.Keys)
        //            {
        //                var file = httpRequest.Files[fileName];
        //                var filePath = HostingEnvironment.MapPath($"~/Photos/{file.FileName}");
        //                //var filePath = HttpContext.Current.Server.MapPath($"~/Images/{file.FileName}");
        //                file.SaveAs(filePath);

        //                HangHoa item = await db.HangHoas.FindAsync(id);
        //                item.TenHinh = file.FileName;
        //                await db.SaveChangesAsync();
        //            }
        //            return Ok();
        //        }
        //        return BadRequest($"Lỗi: Chưa chọn tập tin");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Lỗi. {ex.Message}");
        //    }
        //}

        //cách 2
        public async Task<IHttpActionResult> Upload([FromUri] int id = 0)
        {
            try
            {
                HangHoa item = await db.HangHoas.FindAsync(id);
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    //Xóa file trong Photos
                    if (!string.IsNullOrEmpty(item.TenHinh))
                    {
                        var hh = await db.HangHoas
                                         .Where(p => p.ID == id)
                                         .Select(p => new HinhAnh
                                         {
                                             TenHinh = p.TenHinh
                                         })
                                         .SingleOrDefaultAsync();
                        foreach (var tenHinh in hh.Hinhs)
                        {
                            string pathAndFname = HttpContext.Current.Server.MapPath($"~/Photos/{tenHinh}");
                            if (System.IO.File.Exists(pathAndFname))
                                System.IO.File.Delete(pathAndFname);
                        }
                    }

                    //Thêm Hình vào Photos                   
                    string duongDan = HttpContext.Current.Server.MapPath("~/Photos/");
                    string dsTen = null;
                    for (int i = 0; i < httpRequest.Files.Count; i++)
                    {
                        var f = httpRequest.Files[i];
                        string kieu = Path.GetExtension(f.FileName);
                        string ten = $"{id}-{i + 1}{kieu}";
                        f.SaveAs(duongDan + ten);
                        dsTen += $"{ten},";
                    }

                    item.TenHinh = dsTen.TrimEnd(',');
                    await db.SaveChangesAsync();

                    return Ok();
                }
                return BadRequest($"Lỗi: Chưa chọn tập tin");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi. {ex.Message}");
            }
        }

        #endregion

        #region Sửa hàng hóa - POST
        //POST: api/AdminHangHoaApi/HieuChinh
        //--->api/admin-hang-hoa/sua-hang-hoa
        [Route("sua-hang-hoa")]
        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> HieuChinh(HangHoaInput input)
        {
            try
            {
                HangHoa entity = await db.HangHoas.FindAsync(input.ID);
                if (entity == null) return BadRequest($"Hàng hóa ID ={input.ID} không tồn tại");
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int d = await db.HangHoas.CountAsync(p => p.ID != input.ID && (p.MaHang.StartsWith(input.MaHang)|| input.MaHang.StartsWith(p.MaHang)));
                if (d > 0) return BadRequest($"Mã hàng hóa ='{input.MaHang}' đã có hoặc lồng nhau.");

                ConvertHangHoaDTOToEntity(input, entity, false);
                await db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiệu chỉnh không thành công {ex.Message}");
            }

        }
        #endregion

        #region Xóa hang hóa -POST
        //POST: api/AdminHangHoaApi/Xoa/6
        //--->api/admin-hang-hoa/xoa-hang-hoa/6
        [Route("xoa-hang-hoa/{id}")]
        [HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Xoa(int id)
        {
            try
            {
                //Tham chiếu đến thực thể Thỏa theo ID
                var entity = await db.HangHoas.FindAsync(id);
                //Xử lý kiểm tra dữ liệu
                if (entity == null) return BadRequest($"Mặt hàng có ID={id} không tồn tại.");

                db.HangHoas.Remove(entity);
                await db.SaveChangesAsync();
                return Ok($"Đã xóa thông tin của mặt hàng ID={id} thành công.");
            }
            catch (Exception ex)
            {
                int d = await db.HoaDonChiTiets.CountAsync(p => p.HangHoaID == id);
                if (d > 0) return BadRequest($"Không xóa được vì đã có {d} hóa đơn chi tiết phụ thuộc.");
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        #endregion

        #region Phương thức sử dụng cục bộ
        private void ConvertHangHoaDTOToEntity(HangHoaInput input, HangHoa entity, bool ThemHangHoa=true)
        {
            entity.MaHang = input.MaHang;
            entity.TenHang = input.TenHang;
            entity.DVT = input.DVT;
            entity.QuyCach = input.QuyCach;
            entity.MoTa = input.MoTa;
            entity.GiaBan = input.GiaBan;
            entity.GiaThiTruong = input.GiaThiTruong;
            entity.LoaiID = input.LoaiID;
            entity.XuatXu = input.XuatXu;
            entity.TinhTrang = input.TinhTrang;
            if (ThemHangHoa == true) entity.NgayTao = DateTime.Now;
            entity.NgaySua = DateTime.Now;
        }
        #endregion
    }
}
