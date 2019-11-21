using Microsoft.AspNet.Identity;
using PMSAWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace PMSAWebMVC
{
    public static class IdentityExtensions
    {
        public static string GetRealName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            ClaimsIdentity ci = identity as ClaimsIdentity;
            return ci.FindFirstValue("RealName");
        }

        public static Employee GetEmployee(this IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }
            string id = identity.GetUserName();
            //員工帳號CE開頭
            if (!id.StartsWith("CE"))
            {
                return null;
            }
            using (PMSAEntities db = new PMSAEntities())
            {
                Employee emp = db.Employee.Find(id);
                return emp;
            }
        }

        public static SupplierAccount GetSupplierAccount(this IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }
            string id = identity.GetUserName();
            //供應商帳號SE開頭
            if (!id.StartsWith("SE"))
            {
                return null;
            }
            using (PMSAEntities db = new PMSAEntities())
            {
                SupplierAccount sa = db.SupplierAccount.Find(id);
                return sa;
            }
        }

        public static string GetTitle(this IIdentity identity)
        {
            if (identity == null)
            {
                return string.Empty;
            }
            string id = identity.GetUserName();
            //供應商帳號SE開頭
            if (id.StartsWith("SE"))
            {
                return "供應商";
            }
            using (PMSAEntities db = new PMSAEntities())
            {
                Employee emp = db.Employee.Find(id);
                return emp.Title;
            }
        }
    }
}