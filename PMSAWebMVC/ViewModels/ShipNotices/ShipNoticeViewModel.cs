using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMSAWebMVC.Models;


namespace PMSAWebMVC.ViewModels.ShipNotices
{
    public class UnshipOrderDtlViewModel
    {
        //這行不知道是甚麼??
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UnshipOrderDtlViewModel()
        {
           
        }
        public string PurchaseOrderID { get; set; }
        
        //此集合是用來存放訂單出貨明細檢視時，判斷有無被選取使用
        public IList<OrderDtlItemChecked> orderDtlItemCheckeds { get; set; }

    }

    //此類別是用來存放訂單出貨明細檢視時，判斷有無被選取使用
    public class OrderDtlItemChecked
    {
        public int PurchaseOrderDtlOID { get; set; }
        public string PurchaseOrderDtlCode { get; set; }
        public bool Checked { get; set; }
    }
}