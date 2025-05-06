using CloudService;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Passport.Runtime;
using Unity.Passport.Runtime.UI;
using Unity.UOS.Auth;
using Unity.UOS.CloudSave;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WeChatWASM;
using Random = System.Random;

public class WxInit : MonoBehaviour
{

    public string[] Title;
    public string[] textureUrl;

    public WXBannerAd BannerAd;
    public WXRewardedVideoAd RewardedVideoAd;
    public WXInterstitialAd IntersAd;
    public WXCustomAd Icon1;
    public WXCustomAd Icon2;
    public WXCustomAd IconDuo1;
    public WXCustomAd IconDuo2;
    public WXCustomAd IconDuo3;

    private static WXGameClubButton clubBtn;

    public RawImage PlayerHade;
    public Text Playername;

    private string BannerAd_id = "adunit-a7a1cb5b51933274";
    private string RewardedVideoA_id = "adunit-62009f31707860cd";
    private string IntersAd_id = "adunit-30537a0deef1b783";
    private string Icon1_id = "adunit-c8e842f048f66123";
    private string Icon2_id = "adunit-5bd61669aebe5289";
    private string IconDuo1_id = "adunit-d4e24dec4916ef67";
    private string IconDuo2_id = "adunit-026484c9ddafdb74";
    private string IconDuo3_id = "adunit-3c332b666b8ce77a";

    public PlayMakerFSM Fsm;
    public Texture Hade;
    public bool WebGl = true;
    public int ChaPingLevel = 1;

  
    //��ϷȦλ��
    public int QuanTop;
    public int QuanWidth;


    public int ChangJingZhi;
    public bool ChaPingBool = false;
   

    public void AdInit()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            InitSDK();  //��ʼ�����
        }
    }


    //�����ĵ�¼����
    public async void Login()
    {
        //����ʼ��
     
        // ��ʼ��Passport Feature SDK
        await PassportFeatureSDK.Initialize();

        //��ʼ��CloudSaveSDK
        await CloudSaveSDK.InitializeAsync();


        // �����Ϸ���΢�ŵ�¼�ӿڣ���ȡ���token
        var externalLoginResponse = await WechatLogin();
        if (externalLoginResponse == null)
        {
            var exp = "΢�ŵ�¼ʧ�ܣ����Ժ�����";
            UIMessage.Show(exp, MessageType.Error);
            throw new Exception(exp);
        }

        // �����ȡ��token������Openid
        TokenInfo tokeninfo = new TokenInfo();
        tokeninfo.AccessToken = externalLoginResponse.personaAccessToken;
        tokeninfo.RefreshToken = externalLoginResponse.personaRefreshToken;
        tokeninfo.UserId = externalLoginResponse.persona.userID;
        Debug.Log("TokenUSERID��" + tokeninfo.UserId);
        AuthTokenManager.SaveToken(tokeninfo);
       
        //��openid��¼
        Uoslogin(tokeninfo.UserId);

       
    }

    //�ƺ���ȡ��openid Uos��¼
    public async void Uoslogin(string UserID)
    {

        //  await PassportFeatureSDK.Initialize();

        //string userId = tokeninfo.UserId; // ��Ҫ��¼���ⲿϵͳ���û�Id

        //string personaDisplayName = userName; // ��ѡ, ��Ҫ��¼�Ľ�ɫ���ǳơ�

        await AuthTokenManager.ExternalLogin(UserID, null, null, null);

        
        Fsm.SendEvent("UOS��¼");
    }

    /// <summary>
    /// ΢�ŵ�¼
    /// </summary>
    public static async Task<ExternalLoginResponse?> WechatLogin()
    {
        var tcs = new TaskCompletionSource<ExternalLoginResponse?>();
        WX.Login(new LoginOption()
        {
            success = async (res) =>
            {
                var wechatApi = new WechatAPI();
                Debug.Log("΢�Ż�ȡ code �ɹ�");
                Debug.Log(res.code);
                var externalLoginResponse = await wechatApi.WechatLogin(res.code);
                tcs.SetResult(externalLoginResponse);
            },
            fail = (err) =>
            {
                Debug.Log("΢�Ż�ȡ code ʧ��");
                Debug.Log(err.errMsg);
                tcs.SetResult(null);
            }
        });

        return await tcs.Task;
    }

    //��ȡ�û���Ȩ���������������а����Ȩ�󵯳����а񵯴�
    public void GetInfo(string typname)
    {
        GetWechatUserInfo(typname);
    }


    /// <summary>
    /// ��ȡ΢�� userinfo
    /// </summary>
    private void GetWechatUserInfo(string typname)
    {
             
        WX.GetSetting(new GetSettingOption()
        {
            fail = (err) =>
            {
                Debug.Log("get setting fail");
                Debug.Log(err.errMsg);
               
            },
            success = (res) =>
            {
                if (res.authSetting.TryGetValue("scope.userInfo", out var hasUserInfo))
                {
                    if (hasUserInfo)
                    {
                        // ����Ȩ
                        WX.GetUserInfo(new GetUserInfoOption()
                        {
                            success = async userInfoRes =>
                            {
                                Debug.Log("get user info success. nickname:");
                                Debug.Log(userInfoRes.userInfo.nickName);

                                FsmVariables.GlobalVariables.GetFsmString("Data_name").Value = userInfoRes.userInfo.nickName;
                                FsmVariables.GlobalVariables.GetFsmString("Data_Url").Value = userInfoRes.userInfo.avatarUrl;
                                FsmVariables.GlobalVariables.GetFsmString("HeadUrl").Value = userInfoRes.userInfo.avatarUrl;                              
                                StartCoroutine(LoadAvatar(userInfoRes.userInfo.avatarUrl ?? ""));

                               await UosDatUp(userInfoRes.userInfo.nickName, userInfoRes.userInfo.avatarUrl);//UOS����
                              if(typname == "���а�") {
                                PlayMakerFSM.BroadcastEvent("PaiHangBang");
                                }
                            },
                            fail = err =>
                            {
                              
                            }
                        });
                    }
                }

                // δ��Ȩ
                if (!hasUserInfo)
                {
                    // ����ȫ������ť
                    var button = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "", true);

                    button.OnTap((tapRes) =>
                    {
                        Debug.Log("press button");
                        button.Hide();
                        WX.GetUserInfo(new GetUserInfoOption()
                        {
                            success = userInfoRes =>
                            {
                                // �û���Ȩ
                                Debug.Log("get user info success. nickname:");
                                Debug.Log(userInfoRes.userInfo.nickName);
                                FsmVariables.GlobalVariables.GetFsmString("Data_name").Value = userInfoRes.userInfo.nickName;
                                FsmVariables.GlobalVariables.GetFsmString("Data_Url").Value = userInfoRes.userInfo.avatarUrl;
                                FsmVariables.GlobalVariables.GetFsmString("HeadUrl").Value = userInfoRes.userInfo.avatarUrl;

                                StartCoroutine(LoadAvatar(userInfoRes.userInfo.avatarUrl ?? ""));

                                if (typname == "���а�")
                                {
                                    PlayMakerFSM.BroadcastEvent("PaiHangBang");
                                }
                            },
                            fail = (err) =>
                            {
                                // �û�δ��Ȩ
                                Debug.Log("get user info fail");
                                Debug.Log(err.errMsg);
                              
                            }
                        });
                    });
                }
            },
        });

       
    }

    //�����û���Ϣ �ɸ��� passport �н�ɫ��Ϣ��������ɫ���ơ�ͷ���ַ���Զ������Ե�
    public async Task UosDatUp(string name, string url)
    {

        await PassportSDK.Identity.UpdatePersona(name, url, null);

    }



    /// <summary>
    /// ��ʼ��SDK
    /// </summary>
    private void InitSDK()
    {

        WX.InitSDK((code) =>
        {
            var sss = WX.GetLaunchOptionsSync(); //����û���¼�ĳ���ֵ
            ChangJingZhi = (int)sss.scene;
            CreateBannerAd();
            CreateIntersAd();
            CreateRewardedVideoAd();
            CreateIconAd1();
            CreateIcon2();
            CreateIconDuo1();
            CreateIconDuo2();
            CreateIconDuo3();         
        });
    }
  

    IEnumerator LoadAvatar(string url)
    {
        // ����ͷ��ͼƬ
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);

        yield return uwr.SendWebRequest(); // �ȴ��������

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("����UrlͼƬʧ�ܵ�ַ��" + url);

        }
        else
        {
            PlayerHade.texture  = DownloadHandlerTexture.GetContent(uwr);
        }
       
    }


    //����
    public void OnShareClick()
    {
        if (WebGl == false)
        {
            return;
        }

         Random random = new Random();

        int randIndex = random.Next(Title.Length); //�������ı�id�������������ͼƬ    

        WX.ShareAppMessage(new ShareAppMessageOption()//   �����imageUrl�ߴ糤�������5:4
        {
            title = Title[randIndex],
            imageUrl = textureUrl[randIndex],
        }); ;
    }


    // �ײ�banner���ʾ��
    private void CreateBannerAd()
    {


        var sysInfo = WX.GetSystemInfoSync();

        BannerAd = WX.CreateBannerAd(new WXCreateBannerAdParam()
        {

            adUnitId = BannerAd_id,
            adIntervals = 60,

            style = new Style()
            {

                left = (int)(sysInfo.windowWidth - 300) / 2,

                top = 0,

                width = 250,

                height = 100,
            }
        });

        BannerAd.OnError((WXADErrorResponse res) =>
        {
            Debug.Log("bannerad ������");
        });

        BannerAd.OnLoad((res) =>
        {

            Debug.Log("Banner�����سɹ�");
        });
        BannerAd.OnResize((WXADResizeResponse res) =>
        {
            //��ȡ�Ĺ����ܸ����õĲ�һ������Ҫ��̬����λ��
            BannerAd.style.top = (int)sysInfo.windowHeight - BannerAd.style.realHeight - 1;
        });

    }
    public void ShowBannerAd()
    {
        if (WebGl == false)
        {
            return;
        }
        BannerAd.Show();
        
    }


    public void HideBannerAd()

    {
        if (WebGl == false)
        {
            return;
        }
            BannerAd.Hide();
           
        
    }
    public void DestroyBannerAd()
    {
        BannerAd.Destroy();
    }


    //������Ƶ
    private void CreateRewardedVideoAd()
    {
        RewardedVideoAd = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
        {
            adUnitId = RewardedVideoA_id,
        });
        RewardedVideoAd.OnLoad((res) =>
        {
            Debug.Log("RewardedVideoAd.OnLoad:" + JsonUtility.ToJson(res));
            var reportShareBehaviorRes = RewardedVideoAd.ReportShareBehavior(new RequestAdReportShareBehaviorParam()
            {
                operation = 1,
                currentShow = 1,
                strategy = 0,
                shareValue = (int)res.shareValue,
                rewardValue = (int)res.rewardValue,
                depositAmount = 100,
            });
            Debug.Log("ReportShareBehavior.Res:" + JsonUtility.ToJson(reportShareBehaviorRes));
        });
        RewardedVideoAd.OnError((err) =>
        {
            Debug.Log("RewardedVideoAd.OnError:" + JsonUtility.ToJson(err));
        });
        RewardedVideoAd.OnClose((res) =>
        {
            Debug.Log("RewardedVideoAd.OnClose:" + JsonUtility.ToJson(res));

            if ((res != null && res.isEnded) || res == null)
            {
                // �������Ž����������·���Ϸ����

                Debug.Log("��沥����ɻ�ý���");
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


        });
        RewardedVideoAd.Load();
    }

    public void ShowRewardedVideoAd()
    {
        if (WebGl == true)
        {
            RewardedVideoAd.Show();
        }
        else
        {
            var numLives = FsmVariables.GlobalVariables.GetFsmGameObject("Manager");
            GameObject Manager = numLives.Value;

            Manager.GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == "Shop").SendEvent("AdGet");
        }

    }

    //�������
    private void CreateIntersAd()
    {
        //���ص�
        IntersAd = WX.CreateInterstitialAd(new WXCreateInterstitialAdParam()
        {
            adUnitId = IntersAd_id,

        });
        IntersAd.OnLoad((res) =>
        {

        });

    }

    //����������ʾ����
    public void ShowIntersAd()
    {
        if (WebGl == false)
        {
            return;
        }
        Thread.Sleep(1500);
        IntersAd.Show();

    }

    public void DestroyIntersAd()
    {
        if (WebGl == false)
        {
            return;
        }
        IntersAd.Destroy();
    }

    // ��3���� Duo1
    private void CreateIconDuo1()
    {
        var sysInfo = WX.GetSystemInfoSync();
        IconDuo1 = WX.CreateCustomAd(new WXCreateCustomAdParam()
        {
            adUnitId = IconDuo1_id,

            adIntervals = 120,
            style = {
                left = 0,
                top = (int)sysInfo.windowHeight -300,
            },
        });

    }

    public void IconDuo1Show()
    {
        if (WebGl == false)
        {
            return;
        }
        IconDuo1.Show();

    }
    public void IconDuo11Hid()
    {
        if (WebGl == false)
        {
            return;
        }
        IconDuo1.Hide();

    }

    //��3���� Duo2
    private void CreateIconDuo2()
    {
        var sysInfo = WX.GetSystemInfoSync();
        IconDuo2 = WX.CreateCustomAd(new WXCreateCustomAdParam()
        {
            adUnitId = IconDuo2_id,
            adIntervals = 120,
            style = {

                left = (int)sysInfo.windowWidth-60,
                top = (int)sysInfo.windowHeight -300,
            },
        });
    }
    public void IconDuo2Show()
    {
        if (WebGl == false)
        {
            return;
        }
        IconDuo2.Show();

    }
    public void IconDuo2Hid()
    {
        if (WebGl == false)
        {
            return;
        }
        IconDuo2.Hide();

    }

    //���Ŷ���� Duo3
    private void CreateIconDuo3()
    {
        var sysInfo = WX.GetSystemInfoSync();

        IconDuo3 = WX.CreateCustomAd(new WXCreateCustomAdParam()
        {
            adUnitId = IconDuo3_id,
            adIntervals = 120,
            style = {

                left =((int)sysInfo.windowWidth-400)/2+20,
                top = 60,
            },
        });

    }

    public void IconDuo3show()
    {
        if (WebGl == false)
        {
            return;
        }
        IconDuo3.Show();

    }
    public void IconDuo3Hide()
    {
        if (WebGl == false)
        {
            return;
        }
        IconDuo3.Hide();

    }

    // ���µ�����
    private void CreateIconAd1()
    {
        var sysInfo = WX.GetSystemInfoSync();
        Icon1 = WX.CreateCustomAd(new WXCreateCustomAdParam()
        {
            adUnitId = Icon1_id,
            adIntervals = 30,
            style = {
                left = 0,
                top =  (int)sysInfo.windowHeight-100,
            },
        });

    }

    public void ShowIcon1()
    {
        if (WebGl == false)
        {
            return;
        }
        Icon1.Show();


    }
    public void HideIcon1()
    {
        if (WebGl == false)
        {
            return;
        }
        Icon1.Hide();

    }

    // ���µ�����
    private void CreateIcon2()
    {
        var sysInfo = WX.GetSystemInfoSync();

        Icon2 = WX.CreateCustomAd(new WXCreateCustomAdParam()
        {
            adUnitId = Icon2_id,
            adIntervals = 30,
            style = {
                left = (int)sysInfo.windowWidth-50,
                top = (int)sysInfo.windowHeight -100,
            },
        });
        Icon2.OnLoad((res) =>
        {

        });

    }
    public void ShowIcon2()
    {
        if (WebGl == false)
        {
            return;
        }
        Icon2.Show();


    }
    public void Icon2Hid()
    {
        if (WebGl == false)
        {
            return;
        }
        Icon2.Hide();

    }

    public void DestroyRewardedVideoAd()
    {
        RewardedVideoAd.Destroy();
    }

    //������ϷȦ��ť
    public void Quan(Transform tra, Camera camera = null)
    {

        CreateClubBtn(tra, camera);

    }


    //�����������
    public void qingchu()
    {

        WX.CleanAllFileCache();


    }
    public static void CreateClubBtn(Transform tra, Camera camera = null)
    {

        if (camera == null)
        {
            camera = Camera.main;
        }

        WeChatWASM.SystemInfo systemInfo = WX.GetSystemInfoSync(); ;
        float ux = Screen.width;
        float uy = Screen.height;
        float wx = (float)systemInfo.screenWidth;
        float wy = (float)systemInfo.screenHeight;
        float sx = wx / ux;
        float sy = wy / uy;

        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, tra.position);
        //Rect r = Utility.UGUI.GetSize(tra.gameObject);
        float bw = 350 * sx;   //��
        float bh = 100 * sy; //��
        float bx = screenPoint.x * sx - bw * 0.5f;
        float by = (Screen.height - screenPoint.y) * sy - bh * 0.5f;

        Debug.Log($"CreateClubBtn++++ux:{ux} uy:{uy} wx:{wx} wy:{wy} sx:{sx} sy:{sy} bw:{bw} bh:{bh} bx:{bx} by:{by} screenX:{screenPoint.x} screenY:{screenPoint.y}");
        CreateClubBtn(new Rect(bx, by, bw, bh));



    }




    public static void CreateClubBtn(Rect rect)
    {

        if (clubBtn != null)
        {
            return;
        }
        clubBtn = WX.CreateGameClubButton(new WXCreateGameClubButtonParam()
        {
            //type = GameClubButtonType.image,
            text = "",
            //image = WeChatConfig.ClubImageUrl,
            style = new GameClubButtonStyle()
            {
                left = (int)rect.x,
                top = (int)rect.y,
                width = (int)rect.width,
                height = (int)rect.height,
            }
        });
        //clubBtn.Show();

    }


    //������ϷȦ��ť
    public void hideeClubBtn()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            clubBtn.Hide();
        }
    }
    //��ʾ��ϷȦ��ť
    public void showClubBtn()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            clubBtn.Show();
        }
    }
}
