using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PMSAWebMVC;
using PMSAWebMVC.Models;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PMSAWebMVC.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private PMSAEntities db = new PMSAEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult SetLanguage(string cultureName, string returnUrl)
        {
            HttpCookie c = new HttpCookie(cookieName);
            c.Values.Add(cookieKey, cultureName);
            c.Expires.AddDays(30);
            Response.Cookies.Add(c);
            return Redirect(returnUrl);
        }

        // 採購系統 ====================================================================================================================
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // 這不會計算為帳戶鎖定的登入失敗
            // 若要啟用密碼失敗來觸發帳戶鎖定，請變更為 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", "Account", new { UserName = model.UserName, ReturnUrl = "", RememberMe = model.RememberMe });

                case SignInStatus.Success:
                    var user = UserManager.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();
                    if (user.LastPasswordChangedDate == null)
                    {
                        return RedirectToAction("ResetPassword", "Account");
                    }
                    else
                    {
                        return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "登入嘗試失試。");
                    return View(model);
            }
        }//

        // GET: /Account/VerifyCode
        [Authorize]
        public ActionResult ConfirmEmailError()
        {
            return View();
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string token, string provider, string returnUrl, bool rememberMe)
        {
            // 需要使用者已透過使用者名稱/密碼或外部登入進行登入
            //if (!await SignInManager.HasBeenVerifiedAsync())
            //{
            //    return View("Error");
            //}
            return View(new VerifyCodeViewModel { token = token, Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 下列程式碼保護兩個因素碼不受暴力密碼破解攻擊。
            // 如果使用者輸入不正確的代碼來表示一段指定的時間，則使用者帳戶
            // 會有一段指定的時間遭到鎖定。
            // 您可以在 IdentityConfig 中設定帳戶鎖定設定
            var result = SignInManager.TwoFactorSignIn(model.token, model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "代碼無效。");
                    return View(model);
            }
        }

        //註冊 【Admin】採購人員帳號管理=====================================================================================
        public string generateFirstPwd()
        {
            //salt: 隨機位元組=>演算法=>計算後給值
            RNGCryptoServiceProvider rngbyte = new RNGCryptoServiceProvider();
            byte[] bytesalt = new byte[8];
            rngbyte.GetBytes(bytesalt);
            string salty = Convert.ToBase64String(bytesalt);
            Regex regex = new Regex(@"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])");
            if (regex.IsMatch(salty))
            {
                return salty;
            }
            else
            {
                return generateFirstPwd();
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (!string.IsNullOrWhiteSpace(userId) || code == null)
            {
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                return View(result.Succeeded ? "ConfirmEmail" : "ConfirmEmailError");
            }
            return View("ConfirmEmailError");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // 不顯示使用者不存在或未受確認
                    return View("ForgotPasswordConfirmation");
                }

                // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                // 傳送包含此連結的電子郵件
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "重設密碼", "請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設密碼");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [Authorize]
        public ActionResult ResetPassword()
        {
            return View();
        }

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            //登入可能為供應商或採購系統
            var LoginId = User.Identity.GetUserName();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (LoginId.Contains("C"))
            {
                var emp = db.Employee.Where(x => x.EmployeeID == LoginId).SingleOrDefault();
                var user = await UserManager.FindByNameAsync(LoginId);
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.ConfirmPassword);
                await UserManager.UpdateSecurityStampAsync(user.Id);
                emp.PasswordHash = user.PasswordHash;
                user.LastPasswordChangedDate = DateTime.Now;
                emp.AccountStatus = "E";

                //按下重設確定後要判斷是否成功修改
                var isResetPwd = await updateEmpUserTable(user, emp);
                //成功
                if (isResetPwd > 0)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
            }
            else if (LoginId.Contains("S"))
            {
                var supAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == LoginId).SingleOrDefault();
                var user = await UserManager.FindByNameAsync(LoginId);
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.ConfirmPassword);
                await UserManager.UpdateSecurityStampAsync(user.Id);
                supAcc.PasswordHash = user.PasswordHash;
                user.LastPasswordChangedDate = DateTime.Now;
                supAcc.AccountStatus = "E";

                //按下重設確定後要判斷是否成功修改
                var isResetPwd = await updateTable(user, supAcc);
                //成功
                if (isResetPwd > 0)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
            }
            //失敗
            return RedirectToAction("Error");
        }

        //TODO BuyerSup Admin 學這邊把update結果變回傳0 1 //判斷一起成敗 改成用交易
        //4.存到資料庫
        //更新此 user table
        private async Task<int> updateTable(ApplicationUser user, SupplierAccount sa)
        {
            //更新此 user table
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var u1 = await UserManager.UpdateAsync(user);
            var ctx = store.Context;
            await ctx.SaveChangesAsync();
            //sa table
            db.Entry(sa).State = EntityState.Modified;
            var u2 = await db.SaveChangesAsync();
            //成功
            if (u1.Succeeded && u2 > 0)
            {
                return 1;
            }
            //失敗
            return 0;
        }

        //更新此 user table
        //await updateEmpUserTable(user, emp);
        private async Task<int> updateEmpUserTable(ApplicationUser user, Employee emp)
        {
            //4.存到資料庫
            //更新此 user table
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            var u1 = await UserManager.UpdateAsync(user);
            var ctx = store.Context;
            await ctx.SaveChangesAsync();
            //Emp table
            db.Entry(emp).State = EntityState.Modified;
            var u2 = await db.SaveChangesAsync();
            //成功
            if (u1.Succeeded && u2 > 0)
            {
                return 1;
            }
            //失敗
            return 0;
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 要求重新導向至外部登入提供者
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string userName, string returnUrl, bool rememberMe)
        {
            TempData["userName"] = userName;
            //var userId = await SignInManager.GetVerifiedUserIdAsync();
            var user = UserManager.FindByName(userName);
            var userId = user.Id;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
                var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
                return View(new SendCodeViewModel { userName = userName, Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
            }
            return View("Login");
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            model.userName = TempData["userName"].ToString();
            var userId = User.Identity.GetUserId();
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 產生並傳送 Token
            string token = await SignInManager.SendTwoFactorCodeAsync(model.userName, model.SelectedProvider);
            //return Content($"<script>alert('{r}')</script>");
            //if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            //{
            //    return View("Error");
            //}
            return RedirectToAction("VerifyCode", new { token = token, Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 若使用者已經有登入資料，請使用此外部登入提供者登入使用者
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

                case SignInStatus.Failure:
                default:
                    // 若使用者沒有帳戶，請提示使用者建立帳戶
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 從外部登入提供者處取得使用者資訊
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        //===================================================================================

        #region Helper

        // 新增外部登入時用來當做 XSRF 保護
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helper
    }
}