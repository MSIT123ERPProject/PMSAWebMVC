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
          
            //var datas = db.Part.Where(P => P.PartNumber == id).
            //Select(g => new { g.PartOID, g.PartNumber, g.PartName, g.PartSpec, g.PictureAdress, g.PartUnit.PartUnitName, g.QtyPerUnit, g.CreatedDate});

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
                           CategoryName = g.CategoryName
                       };


            return Json(datas, JsonRequestBehavior.AllowGet);
        }
    

        // GET: Parts/Create
        //public ActionResult Create()
        //{
        //    ViewBag.PartUnitOID = new SelectList(db.PartUnit, "PartUnitOID", "PartUnitName");
            
        //    return View();
        //}

        // POST: Parts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Create(PartView part)
        {

            /*
                if (Request.Files["File1"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(Request.Files["File1"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["File1"].ContentLength);
                        MemoryStream oMemoryStream = new MemoryStream(data);
                        oMemoryStream.Position = 0;
                        Image a = System.Drawing.Image.FromStream(oMemoryStream);
                        Bitmap oBitmap = new Bitmap(a);
                        string path = part.PartNumber + "-" + part.PartName + ".jpg";
                        string pathqqq = Path.Combine(Server.MapPath("~/assets/parts/"), path);
                        oBitmap.Save(pathqqq);
                    }
                }*/
                part.PictureAdress = "~/assets/parts/" + part.PartNumber + "-" + part.PartName + ".jpg";
                part.CreatedDate = DateTime.Now;
                PartCategoryDtl PartCategoryDtl = new PartCategoryDtl();
                PartCategoryDtl.PartNumber = part.PartNumber;
                PartCategoryDtl.PartCategoryOID = part.PartCategoryOID;
                db.PartCategoryDtl.Add(PartCategoryDtl);
                Part part1 = new Part();
                part1.PictureAdress = part.PictureAdress;
                part1.CreatedDate = part.CreatedDate;
                part1.PartNumber = part.PartNumber;
                part1.PartName = part.PartName;
                //"PartSpec": PartSpec,
                //    "PartUnitName": PartUnitName,
                //    "QtyPerUnit": QtyPerUnit,
                //    "PartCategoryOID": CategoryName
                part1.PartSpec = part.PartSpec;
                part1.PartUnitOID = part.PartUnitOID;
                part1.QtyPerUnit = part.QtyPerUnit;
                db.PartCategoryDtl.Add(PartCategoryDtl);
                db.Part.Add(part1);
                db.SaveChanges();
                string path2 = Server.MapPath("~/assets/parts/");
                return Json("Index");
                    
            ViewBag.PartUnitOID = new SelectList(db.PartUnit, "PartUnitOID", "PartUnitName", part.PartUnitOID);
            return View(part);
        }
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
            public Nullable<int> PartUnitOID { get; set; }
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
        // GET: Parts/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.PartUnitOID = new SelectList(db.PartUnit, "PartUnitOID", "PartUnitName", part.PartUnitOID);
            return View(part);
        }

        // POST: Parts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PartOID,PartNumber,PartName,PartSpec,PartUnitOID,CreatedDate,PictureAdress,PictureDescription")] Part part)
        {

            if (ModelState.IsValid)
            {

                if (Request.Files["File1"].ContentLength != 0)
                {
                    FileInfo f = new FileInfo($@"~/assets/parts{part.PartNumber}-{part.PartName}.jpg");
                    if (f.Exists)
                    {
                        f.Delete();

                    }
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(Request.Files["File1"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["File1"].ContentLength);
                        MemoryStream oMemoryStream = new MemoryStream(data);
                        oMemoryStream.Position = 0;
                        Image a = System.Drawing.Image.FromStream(oMemoryStream);
                        Bitmap oBitmap = new Bitmap(a);
                        string path = part.PartNumber + "-" + part.PartName + ".jpg";
                        string pathqqq = Path.Combine(Server.MapPath("~/assets/parts/"), path);
                        oBitmap.Save(pathqqq);
                    }
                }
                part.PictureAdress = "~/assets/parts/" + part.PartNumber + "-" + part.PartName + ".jpg";
                db.Entry(part).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PartUnitOID = new SelectList(db.PartUnit, "PartUnitOID", "PartUnitName", part.PartUnitOID);
            return View(part);
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
            //return View(part);
            db.Part.Remove(part);
            db.SaveChanges();

            //FileInfo f = new FileInfo($@"C:\PMSAWebMVC\PMSAWebMVC\assets\parts\{part.PartNumber}-{part.PartName}.jpg");
            string partname = $"{ part.PartNumber }-{ part.PartName}.jpg";
            string path = Path.Combine(Server.MapPath("~/assets/parts/"), partname);
            FileInfo f = new FileInfo(path);
           
            //if (f.Exists)
            //{
            f.Delete();

            //}
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
