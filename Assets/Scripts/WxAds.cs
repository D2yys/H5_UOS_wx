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

//            // ��ӡ��Ļ��Ϣ
//            var systemInfo = WeChatWASM.WX.GetSystemInfoSync();
//            Debug.Log($"{systemInfo.screenWidth}:{systemInfo.screenHeight}, {systemInfo.windowWidth}:{systemInfo.windowHeight}, {systemInfo.pixelRatio}");

//            // Ԥ�ȴ������ʵ��
//            Debug.Log("��ʼ���ɹ���");
//            ad = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
//            {
//                adUnitId = "xxxxxxxx" //�Լ������浥ԪID
//            });
//            ad.OnError((r) =>
//            {
//                Debug.Log("ad error:" + r.errMsg);
//                PM.SendEvent("������");
//            });
//            ad.OnClose((r) =>
//            {
//                PM.SendEvent("���ر�");
//                Debug.Log("ad close:" + r.isEnded);
//            });
            

//            // �����û���Ϣ��ȡ��ť���ڵײ�1/3���򴴽�һ��͸������
//            // �״λ�ȡ�ᵯ���û���Ȩ����, ��ͨ�����Ͻ�-����-Ȩ�޹����û�����Ȩ��¼
//            var canvasWith = (int)(systemInfo.screenWidth * systemInfo.pixelRatio);
//            var canvasHeight = (int)(systemInfo.screenHeight * systemInfo.pixelRatio);
//            var buttonHeight = (int)(canvasWith / 1080f * 300f);
//            infoButton = WX.CreateUserInfoButton(0, canvasHeight - buttonHeight, canvasWith, buttonHeight, "zh_CN", false);
//            infoButton.OnTap((userInfoButonRet) =>
//            {
//                Debug.Log(JsonUtility.ToJson(userInfoButonRet.userInfo));
//                txtUserInfo.text = $"nickName��{userInfoButonRet.userInfo.nickName}�� avartar:{userInfoButonRet.userInfo.avatarUrl}";
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
//            title = "�������xxx",
//            imageUrl = "https://inews.gtimg.com/newsapp_bt/0/12171811596_909/0",

//        });


//    }

//}
