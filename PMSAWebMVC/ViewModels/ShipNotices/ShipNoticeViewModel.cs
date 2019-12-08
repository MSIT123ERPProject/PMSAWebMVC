using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace PMSAWebMVC.ViewModels.ShipNotices
{
    /// <summary>
    /// 未出貨採購單ViewModel
    /// </summary>
    public class shipOrderViewModel
    {
        //這行不知道是甚麼??
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public shipOrderViewModel()
        {
        }
        public string PurchaseOrderID { get; set; }
        public string PurchaseOrderStatus { get; set; }
        //此屬性為給TABLE顯示狀態用
        public string PurchaseOrderStatusDisplay { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverTel { get; set; }
        public string ReceiverMobile { get; set; }
        public string ReceiptAddress { get; set; }
        public IEnumerable<OrderDtlItem> orderDtlItems { get; set; }
        //要傳回controller的話要用list
        //此集合是用來存放訂單出貨明細檢視時，判斷有無被選取使用
        public IList<OrderDtlItemChecked> orderDtlItemCheckeds { get; set; }
        //此屬性是要給訂單管理頁面查詢未答交訂單使用，
        [Display(Name ="採購單總金額")]
        public decimal PurchaseOrderTotalAmount { get; set; }
    }
    public class OrderDtlItem
    {
        [Display(Name = "訂單明細流水號")]
        public int PurchaseOrderDtlOID { get; set; }
        [Display(Name = "訂單明細代碼")]
        public string PurchaseOrderDtlCode { get; set; }
        [Display(Name = "料件名稱")]
        public string PartName { get; set; }
        [Display(Name = "料件編號")]
        public string PartNumber { get; set; }
        [Display(Name = "批量")]
        public int QtyPerUnit { get; set; }
        [Display(Name = "總量")]
        public int TotalPartQty { get; set; }

        //此欄位對應到的是採購單的QTY欄位
        [Display(Name = "採購數量")]
        public int PurchaseQty { get; set; }

        //此欄位是要供應商調整出貨數量用
        [Display(Name = "出貨數量")]
        public int Qty { get; set; }

        //出貨明細的SHIPQTY，在此MODEL用來顯示已出貨數量
        [Display(Name = "已出貨數量")]
        public int ShipQty { get; set; }

        [Display(Name = "貨源清單編號")]
        public string SourceListID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "承諾交貨日期")]
        public Nullable<System.DateTime> CommittedArrivalDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "實際出貨日期")]
        public Nullable<System.DateTime> ShipDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        [Display(Name = "需求日期")]
        public Nullable<System.DateTime> DateRequired { get; set; }
        [Display(Name = "庫存")]
        public int UnitsInStock { get; set; }
        //此布林值是用來判斷是否已經出貨，false為已出貨，true為未出貨
        public bool Unship { get; set; }
    }

    //此類別是用來存放訂單出貨明細檢視時，判斷有無被選取使用
    public class OrderDtlItemChecked
    {
        public int PurchaseOrderDtlOID { get; set; }
        public string PurchaseOrderDtlCode { get; set; }
        public bool Checked { get; set; }

        public bool IsEnough { get; set; }

        //此欄位是要供應商調整出貨數量用
        [Display(Name = "出貨數量")]
        public int Qty { get; set; }
    }
}