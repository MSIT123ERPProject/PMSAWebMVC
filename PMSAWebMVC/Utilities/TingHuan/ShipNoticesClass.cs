using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Utilities.TingHuan
{
    public class ShipNoticesUtilities
    {
        private PMSAEntities db = new PMSAEntities();
        public int FindPOChangedOID(string requesterRole, string purchaseOrderID)
        {
            var poc = db.POChanged.Where(x => (x.RequesterRole == requesterRole) && (x.PurchaseOrderID == purchaseOrderID));
            DateTime dt = poc.FirstOrDefault().RequestDate;
            int pOChangedOID = poc.FirstOrDefault().POChangedOID;
            foreach (var pocD in poc)
            {
                if (pocD.RequestDate > dt)
                {
                    dt = pocD.RequestDate;
                    pOChangedOID = pocD.POChangedOID;
                }
            }
            return pOChangedOID;
        }
        public int FindPOChangedOIDByDtlCode(string requesterRole,string purchaseOrderDtlCode)
        {
            var poc = db.POChanged.Where(x => (x.RequesterRole == requesterRole) && (x.PurchaseOrderDtlCode == purchaseOrderDtlCode));
            DateTime dt = poc.FirstOrDefault().RequestDate;
            int pOChangedOID = poc.FirstOrDefault().POChangedOID;
            foreach (var pocD in poc)
            {
                if (pocD.RequestDate > dt)
                {
                    dt = pocD.RequestDate;
                    pOChangedOID = pocD.POChangedOID;
                }
            }
            return pOChangedOID;
        }
        //把訂單狀態換成文字敘述的方法
        public string GetStatus(string purchaseOrderStatus)
        {
            //N = 新增,P = 送出,C = 異動中,E = 答交,D = 整筆訂單取消,S = 出貨,R = 點交,O = 逾期,Z = 結案
            switch (purchaseOrderStatus)
            {
                case "N":
                    return "新增";
                case "P":
                    return "送出";
                case "C":
                    return "異動中";
                case "E":
                    return "答交";
                case "D":
                    return "取消";
                case "S":
                    return "出貨";
                case "R":
                    return "點交";
                case "O":
                    return "逾期";
                case "Z":
                    return "結案";
                default:
                    return "";
            }

        }
    }
}