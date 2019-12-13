## 專案資料存放路徑
- ViewModels：存放ViewModel，請自行以Controller名稱建立資料表存放    
- Services：存放Controller使用的相關邏輯的class，請自行以Controller名稱建立資料表存放    
- Utilities：存放個人使用的方法class，依下資料夾存放
>- 仰婷 YangTing    
>- 呈穎 ChengYing    
>- 世原 ShiYuan    
>- 亞辰 YaChen    
>- 承豪 ChengHao    
>- 廷煥 TingHuan
- Common：存放共用的方法class
>- assets
>>- parts：料件圖片    
>>- products：產品圖片
## 存取帳號方法
>- 在Controller中的Action
>>- User.Identity.GetEmployee();
>>- User.Identity.GetSupplierAccount();
----
1. 取得登入者姓名
```
User.Identity.GetRealName();
```
2. 取得登入者(採購方)的資料
```
//取得存有登入者資料的物件
var emp = User.Identity.GetEmployee();
//用.的方式取得 Email 等資料
var empEmail = emp.Email;
var empId = emp.EmployeeID;
```
3. 取得登入者(供應商)的資料
```
var sup = User.Identity.GetSupplierAccount();
var supEmail = sup.Email;
var supId = sup.EmployeeID;
```
4. 取得登入者的 員工帳號(ex.CE00001)/供應商帳號(ex.SE00001)
```
//取得帳號
string LoginAccId = User.Identity.GetUserName();
```
5. 取得登入者 GUID (Id)
```
string LognId = User.Identity.GetUserId();
```
>> 注意事項：建構子中User資料尚未建立，所以無法存取
>- 在Razor文件
```
//如果是Buyer、Warehouse、Admin、Manager、ProductionControl
@if (User.IsInRole("Buyer")|| User.IsInRole("Warehouse")|| User.IsInRole("Admin")|| User.IsInRole("Manager") || User.IsInRole("ProductionControl"))
{
    //就顯示... html 標籤
}
//如果是 Supplier
@if (User.IsInRole("Supplier"))
{
    //就顯示... html 標籤
}
```
>>- 同Action方式
```
//判斷登入者是誰顯示專屬廠商
string LoginAccId = User.Identity.GetUserName();
string LognId = User.Identity.GetUserId();
//如果 LognId 這人是 Manager
if (UserManager.IsInRole(LognId, "Manager"))
{
  //就回傳 Manager 才看的到的 Json
}
//如果 LognId 這人是 Buyer
else if (UserManager.IsInRole(LognId, "Buyer"))
{
  //就回傳 Manager 才看的到的 Json
}
```
## 啟動專案問題
> Q：[DirectoryNotFoundException: 找不到路徑 bin\roslyn\csc.exe' 的一部分
> A：[重建專案]即可正常執行
## 寄信
1. 在 你的Controller 上增加 UserManager 屬性 (紅蚯蚓就加 using)
```
public class 你的Controller : BaseController
{
        //你原本的建構子不要刪掉
        public 你的Controller()
        {
        }

        //建構子多載
        public 你的Controller(ApplicationUserManager userManager)
        {
                UserManager = userManager;
        } 

        // 屬性
        private ApplicationUserManager _userManager;
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
}
```
2. 在 Action 裡寫信
```
//先找到你要寄信的人(這邊用供應商帳號找)，並儲存 user.Id
//這裡的值在資料庫的 dbo.AppUsers table
var user = UserManager.Users.Where(x => x.UserName == "SE00001").SingleOrDefault(); 
//user.Id 等等寄信方法第一個參數會用到
var userId = user.Id;

//信裡要用的變數
var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
string SupAccID = user.UserName;
string pwd = generateFirstPwd();

//寄信
UserManager.SendEmail(userId, "請重設您的密碼", $"<p>您好,您的帳號是: {SupAccIDstr}</p> 密碼: {pwd} <a href="{callbackUrl}">點此確認您的信箱</a>");
```
----
# 限制登入 Controller/Action 的人
## 供應商
```
[Authorize(Roles = "Supplier")]
```
## 採購
```
[Authorize(Roles = "Buyer")]
```
## 倉管
```
[Authorize(Roles = "Warehouse")]
```
## 採購主管
```
[Authorize(Roles = "Manager")]
```
## 生管
```
[Authorize(Roles = "ProductionControl")]
```
## 新進職員(可登入跟重設密碼但看不到其他功能)
```
[Authorize(Roles = "NewEmployee")]
```
## 系統管理員
```
[Authorize(Roles = "Admin")]
```

1. 加在Controller上
```
[Authorize(Roles = "Admin")]
public class RolesAdminController : BaseController
{
	//...
}
```

2. 加在Action上
```
[Authorize(Roles = "Admin")]
public ActionResult getAllRolesToIndexAjax()
{
	//...
}
```
