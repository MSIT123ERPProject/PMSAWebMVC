using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
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
    public class PartsController : BaseController
    {
        public PMSAEntities db = new PMSAEntities();

        // GET: Parts
        public ActionResult Index()
        {
            var part = db.Part.Include(p => p.PartUnit);

            //var x = (from f in db.PartCategoryDtl select f);
            //var y = (from h in db.PartCategory select h);
            //foreach (var item in x)
            //{
            //    var q = (from n in db.Part select n).Where(n => n.PartNumber == item.PartNumber);
            //    foreach (var item2 in y)
            //    {        
            //    }
            //} HIIII


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
            return View(part);
        }

        // GET: Parts/Create
        public ActionResult Create()
        {
            ViewBag.PartUnitOID = new SelectList(db.PartUnit, "PartUnitOID", "PartUnitName");
            return View();
        }

        // POST: Parts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartOID,PartNumber,PartName,PartSpec,PartUnitOID,CreatedDate,PictureAdress,PictureDescription")] Part part)
        {

            if (ModelState.IsValid)
            {
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
                        string pathqqq = Path.Combine(Server.MapPath("~/imgs"), path);
                        oBitmap.Save(pathqqq);



                    }
                }
                part.PictureAdress = "~/imgs/" + part.PartNumber + "-" + part.PartName + ".jpg";
                part.CreatedDate = DateTime.Now;

                db.Part.Add(part);
                db.SaveChanges();
                string path2 = Server.MapPath("~/imgs");
                return RedirectToAction("Index");
            }

            ViewBag.PartUnitOID = new SelectList(db.PartUnit, "PartUnitOID", "PartUnitName", part.PartUnitOID);
            return View(part);
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
                    FileInfo f = new FileInfo($@"C:\CCLASS\MVC\layoutTest_1018\layoutTest\Imgs\{part.PartNumber}-{part.PartName}.jpg");
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
                        string pathqqq = Path.Combine(Server.MapPath("~/imgs"), path);
                        oBitmap.Save(pathqqq);




                    }
                }
                part.PictureAdress = "~/imgs/" + part.PartNumber + "-" + part.PartName + ".jpg";
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

            FileInfo f = new FileInfo($@"C:\CCLASS\MVC\layoutTest_1018\layoutTest\Imgs\{part.PartNumber}-{part.PartName}.jpg");
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
