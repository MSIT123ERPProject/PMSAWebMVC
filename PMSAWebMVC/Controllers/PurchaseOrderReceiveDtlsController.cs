using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PMSAWebMVC.Models;

namespace PMSAWebMVC.Controllers
{
    public class PurchaseOrderReceiveDtlsController : Controller
    {
        private PMSAEntities db = new PMSAEntities();

        // GET: PurchaseOrderReceiveDtls
        public ActionResult Index()
        {
            var purchaseOrderReceiveDtl = db.PurchaseOrderReceiveDtl.Include(p => p.PurchaseOrderDtl).Include(p => p.PurchaseOrderReceive);
            return View(purchaseOrderReceiveDtl.ToList());
        }

        // GET: PurchaseOrderReceiveDtls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderReceiveDtl purchaseOrderReceiveDtl = db.PurchaseOrderReceiveDtl.Find(id);
            if (purchaseOrderReceiveDtl == null)
            {
                return HttpNotFound();
            }
            
            var datas = db.PurchaseOrderReceiveDtl.AsEnumerable().Where(w => w.PurchaseOrderReceiveDtlCode == id).
                        Select(s => new
                        {
                            s.PurchaseOrderReceiveDtlCode,
                            s.PurchaseOrderReceiveID,
                            s.PurchaseOrderDtlCode,
                            s.PurchaseQty,
                            s.PurchaseAmount,
                            s.RejectQty,
                            s.AcceptQty,
                            s.RejectReason,
                            s.Remark
                        });

            return Json(datas, JsonRequestBehavior.AllowGet);
        }



        //Create
        public void Create(string suppid, string puchrid)
        {
            //PurchaseOrderDtlCode
            //PurchaseQty
            //PurchaseAmount
            var podtlcode = db.ShipNoticeDtl.Where(w => w.ShipNoticeID == suppid).Select(s => new { s.PurchaseOrderDtlCode, s.ShipQty, s.ShipAmount }).ToList();
            foreach (var item in podtlcode)
            {
                //var q = db.PurchaseOrderReceiveDtl.Select(s => s.PurchaseOrderDtlCode);
                //var q1 = db.ShipNoticeDtl.Select(s => s.PurchaseOrderDtlCode);
                //var data1 = q.Except(q1);
                //var data2 = data1.ToList();
                //for(int i = 0; i < data2.Count(); i++)
                //{
                //    if(item.PurchaseOrderDtlCode == data2[i])
                //    {

                //    }
                //}
                string pocode = $"{puchrid}-";
                PurchaseOrderReceiveDtl purchaseOrderReceiveDtl = new PurchaseOrderReceiveDtl();

                //PurchaseOrderReceiveDtlCode
                int count = db.PurchaseOrderReceiveDtl.Where(x => x.PurchaseOrderReceiveDtlCode.StartsWith(pocode)).Count();
                count++;
                pocode = $"{pocode}{count:000}";
                purchaseOrderReceiveDtl.PurchaseOrderReceiveDtlCode = pocode;
                //PurchaseOrderDtlCode
                purchaseOrderReceiveDtl.PurchaseOrderDtlCode = item.PurchaseOrderDtlCode;
                //PurchaseQty
                purchaseOrderReceiveDtl.PurchaseQty = item.ShipQty;
                //PurchaseAmount
                purchaseOrderReceiveDtl.PurchaseAmount = item.ShipAmount;
                //PurchaseOrderReceiveID
                purchaseOrderReceiveDtl.PurchaseOrderReceiveID = puchrid;
                //AcceptQty 可入庫數量
                purchaseOrderReceiveDtl.AcceptQty = 0;
                //RejectQty 驗退數量
                purchaseOrderReceiveDtl.RejectQty = 0;
                if (ModelState.IsValid)
                {
                    db.PurchaseOrderReceiveDtl.Add(purchaseOrderReceiveDtl);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        System.Data.Entity.Validation.DbEntityValidationException DbEntityValidationException = (System.Data.Entity.Validation.DbEntityValidationException)ex;
                        throw DbEntityValidationException;
                    }
                }
            }
        }
        

        // POST: PurchaseOrderReceiveDtls/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Edit(PurchaseOrderReceiveDtl purchaseOrderReceiveDtl)
        {
            string message = "修改成功!!";
            bool status = true;
            if (ModelState.IsValid)
            {
                int rqty = purchaseOrderReceiveDtl.RejectQty;
                int aqty = purchaseOrderReceiveDtl.AcceptQty;
                db.Entry(purchaseOrderReceiveDtl).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = status, message = message, id = db.PurchaseOrderReceiveDtl.Max(x => x.PurchaseOrderReceiveDtlOID), rqty, aqty }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                message = "修改失敗!!";
                status = false;
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }
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
