using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PMSAWebMVC.Models;
using PMSAWebMVC.ViewModels.RolesAdmin;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesAdminController : BaseController
    {
        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Roles/
        [HttpGet]
        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        //========================================================================

        public ActionResult getAllRolesToIndexAjax()
        {
            var roles = RoleManager.Roles.ToList();
            var users = UserManager.Users.ToList();
            List<object> allRoles = new List<object>();
            List<string> UsersFinal = new List<string>();

            foreach (var r in roles)
            {
                foreach (var u in users)
                {
                    //每個 role 的 Id
                    var role = RoleManager.FindById(r.Id);
                    if (UserManager.IsInRole(u.Id, role.Name))
                    {
                        var user = new
                        {
                            RoleId = r.Id,
                            RoleEnName = r.Name,
                            RoleChName = r.Description,
                            usersAccId = u.UserName
                        };
                        allRoles.Add(user);
                    }
                }
            }

            return Json(allRoles, JsonRequestBehavior.AllowGet);
        }

        private class usersArray
        {
            public string RoleId { get; set; }
            public string usersAccId { get; set; }
            public string RoleEnName { get; set; }
            public string RoleChName { get; set; }
        }

        public ActionResult getTwoRolesToIndexAjax()
        {
            var roles = RoleManager.Roles.ToList();
            var users = UserManager.Users.ToList();
            List<usersArray> allUsers = new List<usersArray>();
            List<object> obj = new List<object>();
            var all = users.Join(roles, u => u.Roles.Select(x => x.RoleId), r => r.Users.Select(x => x.RoleId), (u, r) => new { u.UserName, r.Name });
            foreach (var u in users)
            {
                var RoleNameEnarr = roles.Where(y => (u.Roles.Select(x => x.RoleId)).Contains(y.Id)).Select(r => r.Name);
                var RoleNameCharr = roles.Where(y => (u.Roles.Select(x => x.RoleId)).Contains(y.Id)).Select(r => r.Description);
                usersArray userArr = new usersArray
                {
                    RoleId = "-",
                    RoleEnName = string.Join(",", RoleNameEnarr),
                    RoleChName = string.Join(",", RoleNameCharr),
                    usersAccId = u.UserName,
                };
                allUsers.Add(userArr);
            }
            foreach (var x in allUsers)
            {
                var splitArr = x.RoleEnName.Split(',');
                if (splitArr.Count() >= 2)
                {
                    usersArray userArr = new usersArray
                    {
                        RoleId = "-",
                        RoleEnName = x.RoleEnName,
                        RoleChName = x.RoleChName,
                        usersAccId = x.usersAccId
                    };
                    obj.Add(userArr);
                }
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //========================================================================
        //
        // GET: /Roles/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var role = await RoleManager.FindByIdAsync(id);
                // Get the list of Users in this Role
                var users = new List<ApplicationUser>();

                // Get the list of Users in this Role
                foreach (var user in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                    {
                        users.Add(user);
                    }
                }

                ViewBag.Users = users;
                ViewBag.UserCount = users.Count();
                return View(role);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // GET: /Roles/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(roleViewModel.Name);
                // Save the new Description property:
                role.Description = roleViewModel.Description;

                if (!RoleManager.RoleExists(roleViewModel.Name))
                {
                    //角色不存在,建立角色
                    var roleresult = await RoleManager.CreateAsync(role);
                    if (!roleresult.Succeeded)
                    {
                        ModelState.AddModelError("", roleresult.Errors.First());
                        return View();
                    }
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Edit/Admin
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name, Description = role.Description };
                return View(roleModel);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id,Description")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                role.Description = roleModel.Description;
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var role = await RoleManager.FindByIdAsync(id);
                    if (role == null)
                    {
                        return HttpNotFound();
                    }
                    IdentityResult result;
                    if (deleteUser != null)
                    {
                        result = await RoleManager.DeleteAsync(role);
                    }
                    else
                    {
                        result = await RoleManager.DeleteAsync(role);
                    }
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return View();
        }
    }
}