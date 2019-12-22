using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace PMSAWebMVC.Controllers
{
    public class PartsController : Controller
    {
        public PMSAEntities db = new PMSAEntities();

        // GET: Parts
        public ActionResult Index()
        {
            var part = db.Part.Include(p => p.PartUnit);
            return View(part);
        }

        // GET: Parts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Part part = db.Part.Find(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            var datas = from p in db.Part.AsEnumerable()
                       join f in db.PartCategoryDtl
                       on p.PartNumber equals f.PartNumber
                       join g in db.PartCategory
                       on f.PartCategoryOID equals g.PartCategoryOID
                       where p.PartNumber == id
                       select new
                       {
                           PartOID = p.PartOID,
                           PartNumber = p.PartNumber,
                           PartName = p.PartName,
                           PartSpec = p.PartSpec,
                           PictureAdress = p.PictureAdress,
                           PartUnitName = p.PartUnit.PartUnitName,
                           QtyPerUnit = p.QtyPerUnit,
                           CreatedDate = p.CreatedDate.ToString("yyyy/MM/dd"),
                           CategoryName = g.CategoryName,
                           PartCategoryOID=g.PartCategoryOID,
                           PartUnitOID = p.PartUnitOID
                       };
            
            
            return Json(datas, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Create(PartView part)
        {
            //抓虛擬圖檔路徑，將檔案移動到正確資料夾並改名;
                string VirtualFileName = "test1.jpg";
                string VirtualPosition = Path.Combine(Server.MapPath("~/images/"), VirtualFileName);
                string partname = $"{ part.PartNumber }-{ part.PartName}.jpg";
                string RightPosition = Path.Combine(Server.MapPath("~/assets/parts/"), partname);
                System.IO.File.Move(VirtualPosition, RightPosition);
            //將Model帶進來的值存入資料庫
                PartCategoryDtl PartCategoryDtl = new PartCategoryDtl();
                Part part1 = new Part();
                part.PictureAdress = "~/assets/parts/" + part.PartNumber + "-" + part.PartName + ".jpg";
                part.CreatedDate = DateTime.Now;
                PartCategoryDtl.PartNumber = part.PartNumber;
                PartCategoryDtl.PartCategoryOID = part.PartCategoryOID;
                part1.PictureAdress = part.PictureAdress;
                part1.CreatedDate = part.CreatedDate;
                part1.PartNumber = part.PartNumber;
                part1.PartName = part.PartName;
                part1.PartSpec = part.PartSpec;
                part1.PartUnitOID = part.PartUnitOID;
                part1.QtyPerUnit = part.QtyPerUnit;
                db.PartCategoryDtl.Add(PartCategoryDtl);
                db.Part.Add(part1);
                db.SaveChanges();
               
                return Json("Index");
        }
        //料件的ViewModel
        public class PartView
        {
            [Display(Name = "料件識別碼")]
            public int PartOID { get; set; }
            [Display(Name = "料件編號")]
            public string PartNumber { get; set; }
            [Display(Name = "料件名稱")]
            public string PartName { get; set; }
            [Display(Name = "料件規格")]
            public string PartSpec { get; set; }
            [Display(Name = "料件單位識別碼")]
            public int PartUnitOID { get; set; }
            [Display(Name = "料件新增時間")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public System.DateTime CreatedDate { get; set; }
            [Display(Name = "料件圖片位置")]
            public string PictureAdress { get; set; }
            [Display(Name = "料件圖片說明")]
            public string PictureDescription { get; set; }
            [Display(Name = "料件批量")]
            public int QtyPerUnit { get; set; }
            public int PartCategoryOID { get; set; }
            public string PartUnitName { get; set; }
           
        }
        [HttpPost]
        public ActionResult Up(HttpPostedFileBase PtImgFile)
        {
            if (PtImgFile.ContentLength > 0)
            {
                var fileName = "test1.jpg";
                var path = Path.Combine(Server.MapPath("~/images"), fileName);
                PtImgFile.SaveAs(path);
            }
            return RedirectToAction("index");
        }
        [HttpPost]
        public JsonResult UploadByAjax()
        {
                //取得目前 HTTP 要求的 HttpRequestBase 物件
                foreach (string file in Request.Files)
            {
                var fileContent = Request.Files[file];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    // 取得的檔案是stream
                    var stream = fileContent.InputStream;
                    var fileName ="test1.jpg";
                    var path = Path.Combine(Server.MapPath("~/images/"), fileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }

            return Json("Successed");
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(PartView part)
        {
            string VirtualFileName = "test1.jpg";
            string partname = $"{ part.PartNumber }-{ part.PartName}.jpg";
            string path = Path.Combine(Server.MapPath("~/assets/parts/"), partname);
            string VirtualPosition = Path.Combine(Server.MapPath("~/images/"), VirtualFileName);
            string RightPosition = Path.Combine(Server.MapPath("~/assets/parts/"), partname);

            FileInfo f = new FileInfo(VirtualPosition);
            if (f.Exists)
            {
                //先刪除原圖檔案
                System.IO.File.Delete(path);
                //抓虛擬圖檔路徑，將檔案移動到正確資料夾並改名;
                System.IO.File.Move(VirtualPosition, RightPosition);
            }
               
            
            //將Model帶進來的值存入資料庫
            PartCategoryDtl PartCategoryDtl = new PartCategoryDtl();
            Part part1 = new Part();
            part.PictureAdress = "~/assets/parts/" + part.PartNumber + "-" + part.PartName + ".jpg";
            part.CreatedDate = DateTime.Now;
            PartCategoryDtl.PartNumber = part.PartNumber;
            PartCategoryDtl.PartCategoryOID = part.PartCategoryOID;
            part1.PictureAdress = part.PictureAdress;
            part1.CreatedDate = part.CreatedDate;
            part1.PartNumber = part.PartNumber;
            part1.PartName = part.PartName;
            part1.PartSpec = part.PartSpec;
            part1.PartUnitOID = part.PartUnitOID;
            part1.QtyPerUnit = part.QtyPerUnit;
            db.Entry(part1).State = EntityState.Modified;
            db.SaveChanges();
            //var PartCategoryDtlOID = db.PartCategoryDtl.Where(x => x.PartNumber == PartCategoryDtl.PartNumber).SingleOrDefault();
            //PartCategoryDtlOID.PartCategoryOID = PartCategoryDtl.PartCategoryOID;
            //db.Entry(PartCategoryDtlOID).State = EntityState.Modified;
            //db.SaveChanges();
            return Json("Index");
        }


      

        // GET: Parts/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Part part = db.Part.Find(id);
            
            if (part == null)
            {
                return HttpNotFound();
            }
            db.Part.Remove(part);
            var PartCategoryDtlOID = db.PartCategoryDtl.Where(x=>x.PartNumber==id).SingleOrDefault();
            db.PartCategoryDtl.Remove(PartCategoryDtlOID);
            db.SaveChanges();
            string partname = $"{ part.PartNumber }-{ part.PartName}.jpg";
            string path = Path.Combine(Server.MapPath("~/assets/parts/"), partname);
            FileInfo f = new FileInfo(path);
            if (f.Exists)
            {
                f.Delete();

            }
            return RedirectToAction("Index");






        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            if (files.First() != null)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    string SourceFilename = Path.GetFileName(file.FileName);
                    string TargetFilename = Path.Combine(Server.MapPath("~/Uploads"), SourceFilename);
                    file.SaveAs(TargetFilename);
                }

            }
            return RedirectToAction("Index");
        }
        // POST: Parts/Delete/5
        //[HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Part part = db.Part.Find(id);
        //    db.Part.Remove(part);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
