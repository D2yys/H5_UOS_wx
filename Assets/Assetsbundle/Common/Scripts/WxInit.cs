using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using TTSDK;
using UnityEngine;
using UnityEngine.UI;


public class WxInit : MonoBehaviour
{

       
    public bool WebGl = true;
   
    //抖音广告
    private TTBannerStyle style = new TTBannerStyle();
    private TTBannerAd bannerAd = null;
    private TTInterstitialAd m_InterAdIns = null;
    private int px2dp(int px) => (int)(px * (160 / Screen.dpi));

    private string bannedId = "1d894he911gf733cg6";
    private string interstitialId = "1aoggp7eap191f63j6";
    private string videoId = "241f65cjal102ogced";
    // 激励视频
    private TTRewardedVideoAd ttRewardedVideoAd;
    public int BannerTopInt;
    public PlayMakerFSM ShopFsm; //用于分享奖励

    // 初始化微信用户头像，和名字
    public void InitWxUser()
    {

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            InitSDK();

        }
        else
        {
            WebGl = false;

        }
    }
 
    private void InitSDK()
    {
        TT.InitSDK((code, env) =>
        {
            Debug.Log("Unity message init sdk callback");
            Debug.Log("Unity message code: " + code);
            Debug.Log("Unity message HostEnum: " + env.m_HostEnum);

            CreateRewardedVideoAd();
            LoadInterstitialDY();
           
        });
       
    }

    //分享模版id
    private string[] dyShareTemplateIDs = new string[3]
{
        "5c8dd9j5ih383qo555",
        "7agurnensave7j245b",
        "1044jh597e952p11k6"
};

    //分享
    public void OnShareClick()
    {
        TTSDK.UNBridgeLib.LitJson.JsonData shareJson = new TTSDK.UNBridgeLib.LitJson.JsonData();
        int randomIndex = UnityEngine.Random.Range(0, dyShareTemplateIDs.Length);
        // 方式一：使用模版
        shareJson["templateId"] = randomIndex;
        shareJson["query"] = $"?key={randomIndex}&key2=2";
        // 方式二：使用自定义内容
        // shareJson["title"] = "测试分享标题";
        // shareJson["desc"] = "测试描述";
        // shareJson["imageUrl"] = "图片地址";
        TT.ShareAppMessage(shareJson,
            (Dictionary<string, object> data) =>
            {
                Debug.Log("玩家分享成功...");
                ShopFsm.SendEvent("分享成功");


            },
            (errMsg) =>
            {
                Debug.Log("玩家分享失败...");
                ShopFsm.SendEvent("分享失败");

            }, () =>
            {
                ShopFsm.SendEvent("取消成功");
                Debug.Log("玩家取消分享...");
               
            });
    }


    
    public void ShowBannerAd()
    {
        //return;
        Debug.Log("显示ShowBanner");

        style.width = 320;
        style.left = 10;
        style.top = BannerTopInt;
        bannerAd = TT.CreateBannerAd(bannedId, style, 60, OnAdError, OnLoaded, OnAdResize);

        void OnAdError(int code, string mes)
        {
            Debug.Log($"Banned 加载失败 code: {code} ,mes: {mes}");
        }

        void OnLoaded()
        {
            if (bannerAd != null)
            {
                bannerAd.Show();
            }
        }

        void OnAdResize(int code1, int code2)
        {
            Debug.Log($"Banned OnAdResize code: {code1} ,code2: {code2}");

            int w = style.width;
            int h = style.height;
            int sw = px2dp(Screen.width);
            int sh = px2dp(Screen.height);

            style.top = sh - h+200;
            style.left = sw / 2 - w / 2;
            style.width = w;

            bannerAd.ReSize(style);
        }


    }


    public void HideBannerAd()

    {
        if (bannerAd != null)
            bannerAd.Destroy();



    }
    public void DestroyBannerAd()
    {
        //BannerAd.Destroy();
    }


    //奖励视频
    private void CreateRewardedVideoAd()
    {
       

            LoadRewardVideoAd();
        

       
    }

    public void ShowRewardedVideoAd()
    {
        if (Application.isEditor)
        {
            {
                var numLives = FsmVariables.GlobalVariables.GetFsmGameObject("Manager");
                GameObject Manager = numLives.Value;

                Manager.GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == "Shop").SendEvent("AdGet");
            }
            return;
        }

            if (WebGl == true)
        {
            ShowAudioDY();
        }
        else
        {
            var numLives = FsmVariables.GlobalVariables.GetFsmGameObject("Manager");
            GameObject Manager = numLives.Value;

            Manager.GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == "Shop").SendEvent("AdGet");
        }

    }
    private void LoadRewardVideoAd()
    {

        DestroyRewardVideo();
        ttRewardedVideoAd = TT.CreateRewardedVideoAd(videoId, OnRewardVideoClose, OnRewardVideoAdError);
        ttRewardedVideoAd.Load();

    }

    /// <summary>
    /// 展示视频
    /// </summary>
    /// <param name="indexId"></param>
    /// <param name="Success"></param>
    /// <param name="Fial"></param>
    public void ShowAudioDY()
    {
        //rewardVideoSuccess = Success;
        //rewardVideoFial = Fial;
        if (ttRewardedVideoAd != null)
        {
            ttRewardedVideoAd.Show();
        }
        else
        {
            LoadRewardVideoAd();
        }
    }

    // 激励视频关闭回调
    private void OnRewardVideoClose(bool isSuccess, int count)
    {
        LoadRewardVideoAd();

        if (isSuccess)
        {
            var numLives = FsmVariables.GlobalVariables.GetFsmGameObject("Manager");
            GameObject Manager = numLives.Value;

            Manager.GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == "Shop").SendEvent("AdGet");
        }
        else
        {
            // 播放中途退出，不下发游戏奖励
            Debug.Log("中途退出广告不获得广告奖励");
            var numLives = FsmVariables.GlobalVariables.GetFsmGameObject("Manager");
            GameObject Manager = numLives.Value;

            Manager.GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == "Shop").SendEvent("AdGetError");
        }
    }

    // 激励视频加载出错
    private void OnRewardVideoAdError(int code, string mes)
    {
        Debug.Log($"Banned 加载失败 code: {code} ,mes: {mes}");
      
    }

    // 销毁视频
    private void DestroyRewardVideo()
    {
        if (ttRewardedVideoAd != null)
        {
            ttRewardedVideoAd.Destroy();
            ttRewardedVideoAd = null;
        }
    }


    public void ShowIntersAd()
    {

        ShowInterstitialDY();

    }


    //抖音插屏广告
    public void LoadInterstitialDY()
    {

        DesInterstitialDY();
        m_InterAdIns =TT.CreateInterstitialAd(interstitialId,
        OnInterstitialErrDY, OnInterstitialCloseDY, OnInterstitialLoadedDY);

    }

    private void OnInterstitialErrDY(int code, string errMsg)
    {

        Debug.Log($"加载插屏失败code：{{code}} , msg:{errMsg}");
        DesInterstitialDY();

    }

    private void OnInterstitialLoadedDY()
    {
 // 加载成功就展示插屏
       

    }

    //关闭就重新创建
    private void OnInterstitialCloseDY()
    {

        LoadInterstitialDY();

    }
    /// <summary>
    /// 展示插屏广告
    /// </summary>
    private void ShowInterstitialDY()
    {

        if (m_InterAdIns != null)
            m_InterAdIns.Load(); // 加载成功自动展示
        else
        {
            Debug.Log("插屏AD未创建或被销毁，创建一个");
            m_InterAdIns.Show();

        }
    }

        // 销毁插屏广告
     public void DesInterstitialDY()
        {

            if (m_InterAdIns != null)
                m_InterAdIns.Destroy();
            m_InterAdIns = null;

        }






}
