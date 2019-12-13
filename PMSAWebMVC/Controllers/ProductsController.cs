using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PMSAWebMVC.Models;

namespace PMSAWebMVC.Controllers
{
    public class ProductsController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: Products
        public ActionResult Index()
        {
           
            var datas = from p in db.Product.AsEnumerable()
                        join f in db.ProductPart
                        on p.ProductNumber equals f.ProductNumber
                        join g in db.Part
                        on f.PartNumber equals g.PartNumber
                        join h in db.PartCategoryDtl
                        on g.PartNumber equals h.PartNumber
                        join k in db.PartCategory
                        on h.PartCategoryOID equals k.PartCategoryOID
                        select new ProductsView
                        {
                            ProductNumber = p.ProductNumber,
                            ProductName = p.ProductName,
                            ProductPictureAdress = p.PictureAdress,
                            ProductPictureDescription = p.PictureDescription,
                            PartNumber = g.PartNumber,
                            PartName = g.PartName,
                            ProductPartPictureAdress = g.PictureAdress,
                            CategoryName = k.CategoryName,
                        };
            return View(datas);
        }

        // GET: Products/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            var datas = from p in db.Product.AsEnumerable()
                        join f in db.ProductPart
                        on p.ProductNumber equals f.ProductNumber
                        join g in db.Part
                        on f.PartNumber equals g.PartNumber
                        join h in db.PartCategoryDtl
                        on g.PartNumber equals h.PartNumber
                        join k in db.PartCategory
                        on h.PartCategoryOID equals k.PartCategoryOID
                        where p.ProductNumber == id
                        select new
                        {
                            ProductNumber=p.ProductNumber,
                            ProductName =p.ProductName,
                            ProductPictureAdress = p.PictureAdress,
                            ProductPictureDescription=p.PictureDescription,
                            PartNumber = g.PartNumber,
                            PartName = g.PartName,
                            ProductPartPictureAdress = g.PictureAdress,
                            CategoryName = k.CategoryName,
                        };
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductOID,ProductNumber,ProductName,PictureAdress,PictureDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductOID,ProductNumber,ProductName,PictureAdress,PictureDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        public class ProductsView
        {
            [Display(Name = "產品識別碼")]
            public int ProductOID { get; set; }
            [Display(Name = "產品編號")]
            public string ProductNumber { get; set; }
            [Display(Name = "產品名稱")]
            public string ProductName { get; set; }
            [Display(Name = "產品料件識別碼")]
            public int ProductPartOID { get; set; }
            [Display(Name = "產品圖片位置")]
            public string ProductPictureAdress { get; set; }
            [Display(Name = "產品圖片說明")]
            public string ProductPictureDescription { get; set; }
            [Display(Name = "料件編號")]
            public string PartNumber { get; set; }
            [Display(Name = "料件識別碼")]
            public int PartOID { get; set; }
            [Display(Name = "料件名稱")]
            public string PartName { get; set; }
            [Display(Name = "料件規格")]
            public string PartSpec { get; set; }
            [Display(Name = "料件單位識別碼")]
            public int PartUnitOID { get; set; }
            [Display(Name = "料件圖片位置")]
            public string ProductPartPictureAdress { get; set; }
            [Display(Name = "料件圖片說明")]
            public string ProductPartPictureDescription { get; set; }
            [Display(Name = "料件批量")]
            public int QtyPerUnit { get; set; }
            public int PartCategoryOID { get; set; }
            public string PartUnitName { get; set; }
            [Display(Name = "料件分類名稱")]
            public string CategoryName { get; set; }
            
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
