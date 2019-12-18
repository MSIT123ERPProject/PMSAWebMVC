using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PMSAWebMVC;
using PMSAWebMVC.Filter;
using PMSAWebMVC.Models;
using PMSAWebMVC.Services;
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
using System.Web.Security;

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
        [MyExceptionFilter]
        public ActionResult Login(string returnUrl)
        {
            //若已經登入過就導到首頁
            if (User.Identity.IsAuthenticated)
            {
                return View("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [MyExceptionFilter]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // 這不會計算為帳戶鎖定的登入失敗
            // 若要啟用密碼失敗來觸發帳戶鎖定，請變更為 shouldLockout: true
            //登入(加 cookies (在 Startup.Auth.cs 的 UseCookieAuthentication裡))
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
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

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", "Account", new { UserName = model.UserName, ReturnUrl = "", RememberMe = model.RememberMe });

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
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
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
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            string id = SignInManager.GetVerifiedUserId();
            var user = UserManager.Users.Where(x => x.Id == id).FirstOrDefault();
            switch (result)
            {
                case SignInStatus.Success:
                    if (user.LastPasswordChangedDate == null)
                    {
                        return RedirectToAction("ResetPassword", "Account");
                    }
                    else
                    {
                        return RedirectToLocal(model.ReturnUrl);
                    }
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
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                if (user == null)
                {
                    // 不顯示使用者不存在或未受確認
                    return View("ForgotPasswordConfirmation");
                }

                //// 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                //// 傳送包含此連結的電子郵件
                //string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "重設密碼", "請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設密碼");
                //return RedirectToAction("ForgotPasswordConfirmation", "Account");
                //判斷登入者為供應方、採購方
                string LoginAccId = user.UserName;
                string LognId = user.Id;
                //採購方
                if (UserManager.IsInRole(LognId, "Buyer") || UserManager.IsInRole(LognId, "Manager") ||
                    UserManager.IsInRole(LognId, "ProductionControl") || UserManager.IsInRole(LognId, "NewEmployee") ||
                    UserManager.IsInRole(LognId, "Admin") || UserManager.IsInRole(LognId, "Warehouse"))
                {
                    //採購方 忘記密碼重設密碼
                    //重設db密碼
                    //1.重設 user 密碼
                    string pwd = generateFirstPwd();
                    await UserManager.UpdateSecurityStampAsync(LognId);
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(pwd);
                    user.LastPasswordChangedDate = null;
                    await UserManager.UpdateAsync(user);

                    var emp = db.Employee.Where(x => x.EmployeeID == LoginAccId).SingleOrDefault();
                    emp.PasswordHash = user.PasswordHash;

                    // 傳送包含此連結的電子郵件
                    var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
                    string codeB = await UserManager.GenerateEmailConfirmationTokenAsync(LognId);
                    var callbackUrlB = Url.Action("ConfirmEmail", "Account", new { userId = LognId, code = codeB }, protocol: Request.Url.Scheme);
                    string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdEmailTemplate.html"));
                    // 經測試 gmail 不支援 uri data image 所以用網址傳圖比較保險
                    string img = "https://ci5.googleusercontent.com/proxy/4OJ0k4udeu09Coqzi7ZQRlKXsHTtpTKlg0ungn0aWQAQs2j1tTS6Q6e8E0dZVW2qsbzD1tod84Zbsx62gMgHLFGWigDzFOPv1qBrzhyFIlRYJWSMWH8=s0-d-e1-ft#https://app.flashimail.com/rest/images/5d8108c8e4b0f9c17e91fab7.jpg";
                    string MailBody = MembersDBService.getMailBody(tempMail, img, callbackUrlB, pwd);
                    //寄信
                    await UserManager.SendEmailAsync(LognId, "重設您的密碼", MailBody);

                    //3.更新db寄信相關欄位
                    //SendLetterDate
                    emp.SendLetterDate = DateTime.Now;
                    //SendLetterStatus
                    emp.SendLetterStatus = "S";

                    //更新狀態欄位 user emo table
                    await AccStatusResetEmp(LoginAccId);
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
                //供應方
                else if (UserManager.IsInRole(LognId, "Supplier"))
                {
                    //供應商 忘記密碼重設密碼//sa table user table
                    //重設資料庫該 user 密碼 並 hash 存入 db
                    //重設db密碼
                    //1.重設 user 密碼
                    string pwd = generateFirstPwd();
                    await UserManager.UpdateSecurityStampAsync(LognId);
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(pwd);
                    user.LastPasswordChangedDate = null;

                    var SupAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == LoginAccId).SingleOrDefault();
                    SupAcc.PasswordHash = user.PasswordHash;

                    // 傳送包含此連結的電子郵件
                    var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("PMSAWebMVC");
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(LognId);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = LognId, code = code }, protocol: Request.Url.Scheme);
                    string tempMail = System.IO.File.ReadAllText(Server.MapPath(@"~\Views\Shared\ResetPwdSupEmailTemplate.html"));
                    // 經測試 gmail 不支援 uri data image 所以用網址傳圖比較保險
                    string img = "https://ci5.googleusercontent.com/proxy/4OJ0k4udeu09Coqzi7ZQRlKXsHTtpTKlg0ungn0aWQAQs2j1tTS6Q6e8E0dZVW2qsbzD1tod84Zbsx62gMgHLFGWigDzFOPv1qBrzhyFIlRYJWSMWH8=s0-d-e1-ft#https://app.flashimail.com/rest/images/5d8108c8e4b0f9c17e91fab7.jpg";

                    string SupAccIDstr = user.UserName;
                    string MailBody = MembersDBService.getMailBody(tempMail, img, callbackUrl, pwd, SupAccIDstr);

                    //寄信
                    await UserManager.SendEmailAsync(LognId, "重設您的密碼", MailBody);

                    //3.更新db寄信相關欄位
                    //SendLetterDate
                    SupAcc.SendLetterDate = DateTime.Now;
                    //SendLetterStatus
                    SupAcc.SendLetterStatus = "S";

                    await updateTable(user, SupAcc);

                    //新增Supplier
                    var userRoles = await UserManager.GetRolesAsync(LognId);
                    if (!userRoles.Contains("Supplier"))
                    {
                        user.Roles.Clear();
                        var result = await UserManager.AddToRolesAsync(LognId, "Supplier");
                    }

                    //更新狀態欄位 user sa table
                    await AccStatusResetSup(LoginAccId);
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        private async Task AccStatusResetEmp(string EmpId)
        {
            var user = await UserManager.FindByNameAsync(EmpId);
            var emp = db.Employee.Where(x => x.EmployeeID == EmpId).SingleOrDefault();

            //emp table
            //寄信成功狀態設為 R
            emp.AccountStatus = "R";

            //4.存到資料庫
            //更新此 user table
            await updateEmpUserTable(user, emp);
        }

        //Reset 帳號為重啟時 LastPasswordChangedDate = null /  SendLetterDate / resetPwd 和 寄信
        //D -> R
        //用 SupplierAccountID
        private async Task AccStatusResetSup(string Id)
        {
            var user = await UserManager.FindByNameAsync(Id);
            var supAcc = db.SupplierAccount.Where(x => x.SupplierAccountID == Id).SingleOrDefault();

            //supAcc table
            //寄信成功狀態設為 R
            supAcc.AccountStatus = "R";

            //4.存到資料庫
            //更新此 user table var resultOfupdate =
            await updateTable(user, supAcc);
            //if (result.Succeeded && resultOfupdate > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        //==============================================================================
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
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
                var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
                return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
            }
            return View("Error");
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 產生並傳送 Token
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorCookie);
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