using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMSAWebMVC.Utilities.YangTing
{
    public class checkPwd
    {
        public static bool isCorrectPwd(ApplicationSignInManager SignInManager, string LoginId, string pwd)
        {
            //確定密碼是否正確
            var result = SignInManager.PasswordSignIn(LoginId, pwd, false, false);

            switch (result)
            {
                case SignInStatus.Success:
                    return true;
                case SignInStatus.Failure:
                default:
                    return false;
            }
        }
    }
}