//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using WeChatWASM;


//public class WxAds : MonoBehaviour
//{
//    public WXRewardedVideoAd ad;
//    public WXBannerAd bad;
//    public Text txtUserInfo;
//    public PlayMakerFSM PM;
//    public WeChatWASM.WXEnv env = new WXEnv();
//    private WXUserInfoButton infoButton;

//    void Start()
//    {
//        WX.InitSDK((code) =>
//        {

//            // 打印屏幕信息
//            var systemInfo = WeChatWASM.WX.GetSystemInfoSync();
//            Debug.Log($"{systemInfo.screenWidth}:{systemInfo.screenHeight}, {systemInfo.windowWidth}:{systemInfo.windowHeight}, {systemInfo.pixelRatio}");

//            // 预先创建广告实例
//            Debug.Log("初始化成功！");
//            ad = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
//            {
//                adUnitId = "xxxxxxxx" //自己申请广告单元ID
//            });
//            ad.OnError((r) =>
//            {
//                Debug.Log("ad error:" + r.errMsg);
//                PM.SendEvent("广告错误");
//            });
//            ad.OnClose((r) =>
//            {
//                PM.SendEvent("广告关闭");
//                Debug.Log("ad close:" + r.isEnded);
//            });
            

//            // 创建用户信息获取按钮，在底部1/3区域创建一个透明区域
//            // 首次获取会弹出用户授权窗口, 可通过右上角-设置-权限管理用户的授权记录
//            var canvasWith = (int)(systemInfo.screenWidth * systemInfo.pixelRatio);
//            var canvasHeight = (int)(systemInfo.screenHeight * systemInfo.pixelRatio);
//            var buttonHeight = (int)(canvasWith / 1080f * 300f);
//            infoButton = WX.CreateUserInfoButton(0, canvasHeight - buttonHeight, canvasWith, buttonHeight, "zh_CN", false);
//            infoButton.OnTap((userInfoButonRet) =>
//            {
//                Debug.Log(JsonUtility.ToJson(userInfoButonRet.userInfo));
//                txtUserInfo.text = $"nickName：{userInfoButonRet.userInfo.nickName}， avartar:{userInfoButonRet.userInfo.avatarUrl}";
//            });
//            Debug.Log("infoButton Created");
//        });

//    }

//    public void OnAdClick()
//    {
//        ad.Show();
//    }
//    public void OnShareClick()
//    {
//        WX.ShareAppMessage(new ShareAppMessageOption()
//        {
//            title = "分享标题xxx",
//            imageUrl = "https://inews.gtimg.com/newsapp_bt/0/12171811596_909/0",

//        });


//    }

//}
