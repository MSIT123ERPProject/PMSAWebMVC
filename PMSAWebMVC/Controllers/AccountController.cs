using Microsoft.AspNet.Identity;
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

        //
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
            //登入(加 cookies (在 Startup.Auth.cs 的 UseCookieAuthentication裡))
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "登入嘗試失試。");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // 需要使用者已透過使用者名稱/密碼或外部登入進行登入
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
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.EmployeeID,
                    Email = model.Email,
                    RealName = model.realName
                };
                var userToEmp = new Employee
                {
                    EmployeeID = model.EmployeeID,
                    Name = model.realName,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Tel = model.Tel,
                    CompanyCode = "C00001",
                    ManagerID = null,
                    PasswordHash = "+y1MS0Wp2nZUefXbyYhiz9Tn84S8FhCbxDpCXgfNXjk=",
                    PasswordSalt = "fd357578-7784-4dea-b8c1-4d8b8d290d55",
                    AccountStatus = "R",
                    CreateDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    SendLetterStatus = null,
                    SendLetterDate = null,
                    AuthRoleID = null,
                    AuthenticateToken = null,
                    TokenCreateTime = null,
                    EnableTwoFactorAuth = null,
                    Title = null,
                };
                string pwd = generateFirstPwd();
                var result = await UserManager.CreateAsync(user, pwd);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                    // 傳送包含此連結的電子郵件
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    string mailbody = $@"
                            <tbody style='font-family: 'microsoft jhenghei', sans-serif;'>
                              <table style='width: 580px; min-width: 580px; margin: 20px auto; background-color: #eee; padding-bottom: 20px;'>
                                <tr>
                                    <td>
                                        <img src='data:image/jpeg;base64,/9j/4AAQSkZJRgABAgAAAQABAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL / 2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL / wAARCAC5AlgDASIAAhEBAxEB / 8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL / 8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4 + Tl5ufo6erx8vP09fb3 + Pn6 / 8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL / 8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3 + Pn6 / 9oADAMBAAIRAxEAPwD0PIBAHbiorhskqKdGRkAj3qIDdNweKBkvCxgZqpct5jKTnAp9y3Q + naoXmAWTj5gMAUwGwRExsU53GtK2jZFLN2FMsEEdsu7qT0qcsw3c / LnigCXdiNc9zT423Kz + tNl + 6gA6Co1YqFWgBRIdrk + tNA3AsabIdikDuabO5MYxxxQA1WZ5AvvVwsEP0FUICVkUnNWFOZZCTkAUAJH8xz2p8hzHg + tNU7FHvSOcBQe / NAgmfOFx0FIRho8nrUQDb9 / rxSo3myDsFOKACZz9oOO1T24JjZjVcY89yT1qdXxasR2BoAp2kZa4mkp1wpdlXFPszm0Y4xk9aft / fHnoOKAIPL8qIH1rJv8AL6jbJ1Gc1r3b7lVR + NZZQPrkKg8KKAOrQbY0UdQKZcBo48g04uEQetMkfeAH6UgJGbEUa46isy5fdcKFHQ1fuGK7MelZinJ3dSWoAuchhxUchOc9s04SZkJNEzKQoXrmgDSUjyl + lVJ5ADz2qyvCD6VnzEGQjNAHF + O9W + yacsCN80p5qr4F00wWX9pSLmSb7v0rm / iLd + Z4iS0B6KOPrXpel24tNKs4VGAEAx + FYpc07nQ3anYuFgoB9e1XrAjYwPU1mtGY4lOcnd0q9bthskY4rY5y1MPlUg1XyAgpZGLKOwqIMNuD2oGPuTmzxVO3UfvFIxVy75slx0FVkZSXYc8YoETgExhc04RCdHilUMhBBBGQRTY2CnJOQamibLEg0AcTZQ3lp4gmt7dJJLCJxx1EfP8AKujviV27eaxTIy67exiRgu4EAH1rZml3RRHGSRXPSl7zidFWDUVLuSKcsCetTR4ZWBHOKiYKwRunHNKjtHNjqGFdBgRwkAkdqlVz5gHvVOMEXUgZu / SrcfD54wKAK1yxN6T6CkuTnyW96mEIlknZj9Kqv80EfqrUCH3n + pBqdyRaxt6VFc4 + wnPrU4UvYDjgUDMDVmK3sMgHU1uQy7rNRWPrCMII3xwGrQs23afuHakMtucxMPaomkCiIml3Fo3 + marykm2iNMRcEyrLj1qvIMzH86ETzJ09cU5hifbTAfK4DI1KzDcD60lwoCjioznYDQBfhYNARVGE4uWFS2zMvDHg1Eq7bwn8qALM8ZMBrMiOHK + hrcbDWxGOcVgudkppAatqfmAqe5HSqmn7nlDHpVu4yX60DM9ciXFSyH5cVERtkPtUTT / Pg0ATQnBJbpSSYY8GnAhl4qPgNigY6JSDyalY4GD3qElgfanLLkYagCNvlbIp5G9c0ki5XIpIn + QimBAzdiKlgYFagc4fnoad05FICyzdqKrliwxRQBqzEKBnGelQjKxl8Ut0A88eDk4yaZO / CRA / ePNMkhkkLLtCk45JpiRmUGQDgtUzyxxpID + JoVWECRLxmgC1bEPgHsM1YwG21Vt8RpIpOccA1MJP3mPQUAWCwIGOuaj3jzCffFERHFMX52GD1agBJOZCAKbMjCHcy496mZTmRsZxWdLNO2I2b5N1AEm7a2eeKsIgFq0hzljxVY7ijNjqauy8QxpyO9AEKndIB2qS5IUZB5C1EGUPk8cZp0xUwOw5O2gRHHJ93jtk023 + d2ftyaRlKW + 8Dnb3ogBiikzn7tACc / McVIH22e3 +/ wAU9U / 0dmB / hphBDwoR05oAeU2QRqvHPNMiJLyMTx0qZzuUkfwimCPbEo7nrQBVu2CyqB3rO0tWl1qVzng8ZqzfSLHOWY / dGaZ4fYXEskwB9qAOm2jaCwyarswe5Vc9ulOaYrwKiiQm5L8cdKQEt02AeOgrMtwSqkn + KtC9J2E44xVC1QlYz / tZpgWyMsT3zinBAGUN60zJ8wE8Dk0JIZblAOmM0gNAlTG2OmKzpDiQ4GeKvKP3TCqLkB8 / 7NAHhnj9zF49Rz935D + Ga9ntchLdm5XbxXkvxYsmg1a0vQMLKhXPuOlek + HdR / tLQNNukwQYhvI9QMf0qI6SNHrE2FCTAHPAJ4qf5TnHWqNucyLjjIJNW2Gyfb7ZqzMmPQZ / hFNkVApPekzkknpwKZMCZMA0DJHXNlg9cVWt0VUlVj71bk4tefSqMR3iTB4AoAnQK3yjrVhImJbapx9KfZWkckjTuf3aDp6mrVzqdlYRpJeXUFqjHCGRwoY / jUuVhqNzgbiOS28SXRlUrlVPIrpraxa5tYpFIwR3rO1q5iuddupYyroqrGrDkHA / xNdbYxeVYQxkchBXJTlerJo7aqSoxRkvZzRwnK5x6VUcMpjOCDmunZQOoFV5LaKZSCAD6 + ldSkcbic3JHsuHLHHQ5p65RR3BGamvbOaC5LMMxkYBpEjBOPQVaJG7crvU4z1qsi74W9Q1XgoS3J9Kq25DGUDsc0APmUPYSAjlaksGEmnMM9qRcPFMB0YU3SgDbSxngjNAGbqSltKduu2o9JmL6ewHpVpoy + l3Sk5wTWVoMn7p1PqRQM2bdiYyG7rTAd1kCexp9uAQpz2IpiZWykGOjGgRNH8skLetSOR9q571WjlJSHPUGrNzgSI69aAHzncFz2pkg / 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' alt='陸行鳥自行車' />
                                    </td>                                                                                                 </td>
                                </tr>
                                <tr>
                                    <td style='line-height: 2; padding: 10px 38px 30px 38px;'>
                                        <h1 style='color: #3d5f7f; letter-spacing: 1px; margin: 0; font-size: 26px'>重設您的密碼 </h1>
                                        <p style='letter-spacing: 1px; color: #000; margin: 0;font-size: 16px'><strong>您好,</strong></p>
                                        <p style='letter-spacing: 1px; color: #000; margin: 0; font-size: 13px; margin-bottom: 25px; '>請按一下此連結確認您的帳戶，並使用下方重設密碼登入，帳號為您的員工編號，請在5分鐘內登入連結並重設密碼:</p>
                                        <p style='letter-spacing: 1px; margin: 0; margin-bottom: 30px; font-size: 16px'><strong><a href='{callbackUrl}'>按這裏!</a></strong></p>
                                        <p style='letter-spacing: 1px; margin: 0; margin-bottom: 30px; font-size: 16px'><strong>{pwd}</strong></p>
                                        <p style='letter-spacing: 1px; margin: 0; color: #aaa;' font-size: 13px>如果這不是您的帳戶，請忽略這封信，感謝您。</p>
                                    </ td>
                                </ tr>
                                </ table>
                            </ tbody> ";
                    await UserManager.SendEmailAsync(user.Id, "確認您的帳戶", mailbody);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (!string.IsNullOrWhiteSpace(userId) || code == null)
            {
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }
            return View("Error");
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

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // 不顯示使用者不存在
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
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

        // Sup 供應商===================================================================================

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult SupLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SupLogin(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                //user.
                return View(model);
            }

            // 這不會計算為帳戶鎖定的登入失敗
            // 若要啟用密碼失敗來觸發帳戶鎖定，請變更為 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "登入嘗試失試。");
                    return View(model);
            }
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