using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Utilities.YaChen
{
    public class RepositoryUtils
    {
        /// <summary>
        /// 取得採購單狀態中文說明
        /// </summary>
        /// <param name="purchaseOrderStatus"></param>
        /// <returns></returns>
        public static string GetPurchaseOrderStatusCH(string purchaseOrderStatus)
        {
            switch (purchaseOrderStatus)
            {
                case "N":
                    return "新增";
                case "P":
                    return "送出";
                case "C":
                    return "異動中";
                case "A":
                    return "同意修改";
                case "V":
                    return "否決修改";
                case "E":
                    return "答交";
                case "W":
                    return "雙方答交";
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

        /// <summary>
        /// 取得簽核狀態中文說明
        /// </summary>
        /// <param name="signStatusCode"></param>
        /// <returns></returns>
        public static string GetSignStatusCH(string signStatusCode)
        {
            switch (signStatusCode)
            {
                case "Y":
                    return "同意";
                case "N":
                    return "拒絕";
                case "S":
                    return "等待簽核中";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 依角色別取得帳號姓名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static string GetAccountName(string id, string role)
        {
            //提出角色 RequesterRole  P：採購人員、S：供應商、A：系統
            using (PMSAEntities db = new PMSAEntities())
            {
                switch (role)
                {
                    case "A":
                        return "";
                    case "S":
                        return db.SupplierAccount.Find(id).ContactName;
                    case "P":
                        return db.Employee.Find(id).Name;
                    default:
                        return "";
                }
            }

        }

    }
}