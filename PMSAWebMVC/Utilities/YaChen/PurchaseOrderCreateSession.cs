using PMSAWebMVC.Models;
using PMSAWebMVC.ViewModels.PurchaseOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Utilities.YaChen
{
    /// <summary>
    /// 採購單建立時使用Session
    /// </summary>
    public class PurchaseOrderCreateSession
    {
        // private constructor
        private PurchaseOrderCreateSession()
        {
            this.PRItems = new List<PurchaseRequisition>();
            this.PRDItems = new List<PRDtlTableViewModel>();
            this.POItem = null;
            this.PODItems = new List<PurchaseOrderDtlItem>();
            this.Supplier = null;
        }

        // Gets the current session.
        public static PurchaseOrderCreateSession Current
        {
            get
            {
                PurchaseOrderCreateSession session =
                      (PurchaseOrderCreateSession)HttpContext.Current.Session["__PurchaseOrderCreateSession__"];
                if (session == null)
                {
                    session = new PurchaseOrderCreateSession();
                    HttpContext.Current.Session["__PurchaseOrderCreateSession__"] = session;
                }
                return session;
            }
        }

        /// <summary>
        /// 重設所有屬性
        /// </summary>
        public void ResetAllItems()
        {
            this.PRItems?.Clear();
            this.PRDItems = new List<PRDtlTableViewModel>();
            this.POItem = null;
            this.PODItems?.Clear();
            this.Supplier = null;
        }

        /// <summary>
        /// 請購單主表
        /// </summary>
        public IList<PurchaseRequisition> PRItems { get; set; }
        /// <summary>
        /// 請購單明細
        /// </summary>
        public IList<PRDtlTableViewModel> PRDItems { get; set; }
        /// <summary>
        /// 採購單主表
        /// </summary>
        public PurchaseOrder POItem { get; set; }
        /// <summary>
        /// 採購單明細
        /// </summary>
        public IList<PurchaseOrderDtlItem> PODItems { get; set; }
        /// <summary>
        /// 新增編輯中採購單明細
        /// </summary>
        public PurchaseOrderDtlItem PODItemEditting { get; set; }
        /// <summary>
        /// 供應商帳號
        /// </summary>
        public SupplierAccount Supplier { get; set; }

    }
}