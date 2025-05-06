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

  
    //游戏圈位置
    public int QuanTop;
    public int QuanWidth;


    public int ChangJingZhi;
    public bool ChaPingBool = false;
   

    public void AdInit()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            InitSDK();  //初始化广告
        }
    }


    //完整的登录流程
    public async void Login()
    {
        //广告初始化
     
        // 初始化Passport Feature SDK
        await PassportFeatureSDK.Initialize();

        //初始化CloudSaveSDK
        await CloudSaveSDK.InitializeAsync();


        // 调用上方的微信登录接口，获取玩家token
        var externalLoginResponse = await WechatLogin();
        if (externalLoginResponse == null)
        {
            var exp = "微信登录失败，请稍后重试";
            UIMessage.Show(exp, MessageType.Error);
            throw new Exception(exp);
        }

        // 保存获取的token，换回Openid
        TokenInfo tokeninfo = new TokenInfo();
        tokeninfo.AccessToken = externalLoginResponse.personaAccessToken;
        tokeninfo.RefreshToken = externalLoginResponse.personaRefreshToken;
        tokeninfo.UserId = externalLoginResponse.persona.userID;
        Debug.Log("TokenUSERID是" + tokeninfo.UserId);
        AuthTokenManager.SaveToken(tokeninfo);
       
        //用openid登录
        Uoslogin(tokeninfo.UserId);

       
    }

    //云函数取回openid Uos登录
    public async void Uoslogin(string UserID)
    {

        //  await PassportFeatureSDK.Initialize();

        //string userId = tokeninfo.UserId; // 需要登录的外部系统的用户Id

        //string personaDisplayName = userName; // 可选, 需要登录的角色的昵称。

        await AuthTokenManager.ExternalLogin(UserID, null, null, null);

        
        Fsm.SendEvent("UOS登录");
    }

    /// <summary>
    /// 微信登录
    /// </summary>
    public static async Task<ExternalLoginResponse?> WechatLogin()
    {
        var tcs = new TaskCompletionSource<ExternalLoginResponse?>();
        WX.Login(new LoginOption()
        {
            success = async (res) =>
            {
                var wechatApi = new WechatAPI();
                Debug.Log("微信获取 code 成功");
                Debug.Log(res.code);
                var externalLoginResponse = await wechatApi.WechatLogin(res.code);
                tcs.SetResult(externalLoginResponse);
            },
            fail = (err) =>
            {
                Debug.Log("微信获取 code 失败");
                Debug.Log(err.errMsg);
                tcs.SetResult(null);
            }
        });

        return await tcs.Task;
    }

    //获取用户授权，传入名字是排行榜等授权后弹出排行榜弹窗
    public void GetInfo(string typname)
    {
        GetWechatUserInfo(typname);
    }


    /// <summary>
    /// 获取微信 userinfo
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
                        // 已授权
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

                               await UosDatUp(userInfoRes.userInfo.nickName, userInfoRes.userInfo.avatarUrl);//UOS更新
                              if(typname == "排行榜") {
                                PlayMakerFSM.BroadcastEvent("PaiHangBang");
                                }
                            },
                            fail = err =>
                            {
                              
                            }
                        });
                    }
                }

                // 未授权
                if (!hasUserInfo)
                {
                    // 绘制全屏区域按钮
                    var button = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "", true);

                    button.OnTap((tapRes) =>
                    {
                        Debug.Log("press button");
                        button.Hide();
                        WX.GetUserInfo(new GetUserInfoOption()
                        {
                            success = userInfoRes =>
                            {
                                // 用户授权
                                Debug.Log("get user info success. nickname:");
                                Debug.Log(userInfoRes.userInfo.nickName);
                                FsmVariables.GlobalVariables.GetFsmString("Data_name").Value = userInfoRes.userInfo.nickName;
                                FsmVariables.GlobalVariables.GetFsmString("Data_Url").Value = userInfoRes.userInfo.avatarUrl;
                                FsmVariables.GlobalVariables.GetFsmString("HeadUrl").Value = userInfoRes.userInfo.avatarUrl;

                                StartCoroutine(LoadAvatar(userInfoRes.userInfo.avatarUrl ?? ""));

                                if (typname == "排行榜")
                                {
                                    PlayMakerFSM.BroadcastEvent("PaiHangBang");
                                }
                            },
                            fail = (err) =>
                            {
                                // 用户未授权
                                Debug.Log("get user info fail");
                                Debug.Log(err.errMsg);
                              
                            }
                        });
                    });
                }
            },
        });

       
    }

    //更新用户信息 可更新 passport 中角色信息，包括角色名称、头像地址、自定义属性等
    public async Task UosDatUp(string name, string url)
    {

        await PassportSDK.Identity.UpdatePersona(name, url, null);

    }



    /// <summary>
    /// 初始化SDK
    /// </summary>
    private void InitSDK()
    {

        WX.InitSDK((code) =>
        {
            var sss = WX.GetLaunchOptionsSync(); //获得用户登录的场景值
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
        // 加载头像图片
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);

        yield return uwr.SendWebRequest(); // 等待请求完成

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("加载Url图片失败地址是" + url);

        }
        else
        {
            PlayerHade.texture  = DownloadHandlerTexture.GetContent(uwr);
        }
       
    }


    //分享
    public void OnShareClick()
    {
        if (WebGl == false)
        {
            return;
        }

         Random random = new Random();

        int randIndex = random.Next(Title.Length); //获得随机的表id用来索引标题和图片    

        WX.ShareAppMessage(new ShareAppMessageOption()//   这里的imageUrl尺寸长宽比例是5:4
        {
            title = Title[randIndex],
            imageUrl = textureUrl[randIndex],
        }); ;
    }


    // 底部banner广告示例
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
            Debug.Log("bannerad 广告错误");
        });

        BannerAd.OnLoad((res) =>
        {

            Debug.Log("Banner广告加载成功");
        });
        BannerAd.OnResize((WXADResizeResponse res) =>
        {
            //拉取的广告可能跟设置的不一样，需要动态调整位置
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


    //奖励视频
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
                // 正常播放结束，可以下发游戏奖励

                Debug.Log("广告播放完成获得奖励");
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

    //插屏广告
    private void CreateIntersAd()
    {
        //过关弹
        IntersAd = WX.CreateInterstitialAd(new WXCreateInterstitialAdParam()
        {
            adUnitId = IntersAd_id,

        });
        IntersAd.OnLoad((res) =>
        {

        });

    }

    //根据名字显示插屏
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

    // 左3格子 Duo1
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

    //右3格子 Duo2
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

    //横排多格子 Duo3
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

    // 左下单格子
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

    // 右下单格子
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

    //创建游戏圈按钮
    public void Quan(Transform tra, Camera camera = null)
    {

        CreateClubBtn(tra, camera);

    }


    //清除本地数据
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
        float bw = 350 * sx;   //宽
        float bh = 100 * sy; //高
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


    //隐藏游戏圈按钮
    public void hideeClubBtn()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            clubBtn.Hide();
        }
    }
    //显示游戏圈按钮
    public void showClubBtn()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            clubBtn.Show();
        }
    }
}
