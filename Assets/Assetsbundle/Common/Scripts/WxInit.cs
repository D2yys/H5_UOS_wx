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
   
    //�������
    private TTBannerStyle style = new TTBannerStyle();
    private TTBannerAd bannerAd = null;
    private TTInterstitialAd m_InterAdIns = null;
    private int px2dp(int px) => (int)(px * (160 / Screen.dpi));

    private string bannedId = "1d894he911gf733cg6";
    private string interstitialId = "1aoggp7eap191f63j6";
    private string videoId = "241f65cjal102ogced";
    // ������Ƶ
    private TTRewardedVideoAd ttRewardedVideoAd;
    public int BannerTopInt;
    public PlayMakerFSM ShopFsm; //���ڷ�����

    // ��ʼ��΢���û�ͷ�񣬺�����
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

    //����ģ��id
    private string[] dyShareTemplateIDs = new string[3]
{
        "5c8dd9j5ih383qo555",
        "7agurnensave7j245b",
        "1044jh597e952p11k6"
};

    //����
    public void OnShareClick()
    {
        TTSDK.UNBridgeLib.LitJson.JsonData shareJson = new TTSDK.UNBridgeLib.LitJson.JsonData();
        int randomIndex = UnityEngine.Random.Range(0, dyShareTemplateIDs.Length);
        // ��ʽһ��ʹ��ģ��
        shareJson["templateId"] = randomIndex;
        shareJson["query"] = $"?key={randomIndex}&key2=2";
        // ��ʽ����ʹ���Զ�������
        // shareJson["title"] = "���Է������";
        // shareJson["desc"] = "��������";
        // shareJson["imageUrl"] = "ͼƬ��ַ";
        TT.ShareAppMessage(shareJson,
            (Dictionary<string, object> data) =>
            {
                Debug.Log("��ҷ���ɹ�...");
                ShopFsm.SendEvent("����ɹ�");


            },
            (errMsg) =>
            {
                Debug.Log("��ҷ���ʧ��...");
                ShopFsm.SendEvent("����ʧ��");

            }, () =>
            {
                ShopFsm.SendEvent("ȡ���ɹ�");
                Debug.Log("���ȡ������...");
               
            });
    }


    
    public void ShowBannerAd()
    {
        //return;
        Debug.Log("��ʾShowBanner");

        style.width = 320;
        style.left = 10;
        style.top = BannerTopInt;
        bannerAd = TT.CreateBannerAd(bannedId, style, 60, OnAdError, OnLoaded, OnAdResize);

        void OnAdError(int code, string mes)
        {
            Debug.Log($"Banned ����ʧ�� code: {code} ,mes: {mes}");
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


    //������Ƶ
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
    /// չʾ��Ƶ
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

    // ������Ƶ�رջص�
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
            // ������;�˳������·���Ϸ����
            Debug.Log("��;�˳���治��ù�潱��");
            var numLives = FsmVariables.GlobalVariables.GetFsmGameObject("Manager");
            GameObject Manager = numLives.Value;

            Manager.GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == "Shop").SendEvent("AdGetError");
        }
    }

    // ������Ƶ���س���
    private void OnRewardVideoAdError(int code, string mes)
    {
        Debug.Log($"Banned ����ʧ�� code: {code} ,mes: {mes}");
      
    }

    // ������Ƶ
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


    //�����������
    public void LoadInterstitialDY()
    {

        DesInterstitialDY();
        m_InterAdIns =TT.CreateInterstitialAd(interstitialId,
        OnInterstitialErrDY, OnInterstitialCloseDY, OnInterstitialLoadedDY);

    }

    private void OnInterstitialErrDY(int code, string errMsg)
    {

        Debug.Log($"���ز���ʧ��code��{{code}} , msg:{errMsg}");
        DesInterstitialDY();

    }

    private void OnInterstitialLoadedDY()
    {
 // ���سɹ���չʾ����
       

    }

    //�رվ����´���
    private void OnInterstitialCloseDY()
    {

        LoadInterstitialDY();

    }
    /// <summary>
    /// չʾ�������
    /// </summary>
    private void ShowInterstitialDY()
    {

        if (m_InterAdIns != null)
            m_InterAdIns.Load(); // ���سɹ��Զ�չʾ
        else
        {
            Debug.Log("����ADδ���������٣�����һ��");
            m_InterAdIns.Show();

        }
    }

        // ���ٲ������
     public void DesInterstitialDY()
        {

            if (m_InterAdIns != null)
                m_InterAdIns.Destroy();
            m_InterAdIns = null;

        }






}
