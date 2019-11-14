## 專案資料存放資料夾
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
>- parts：料件圖片    
>- products：產品圖片
## 存取帳號方法
>- 在Controller中的Action
>>- User.Identity.GetEmployee();
>>- User.Identity.GetEmployee();
>> 注意事項：建構子中User資料尚未建立，所以無法存取
>- 在Razor文件
>>- 同Action方式
