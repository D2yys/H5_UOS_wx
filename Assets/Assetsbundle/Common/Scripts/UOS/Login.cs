using HutongGames.PlayMaker;
using Passport;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TTSDK;
using Unity.Passport.Runtime;
using Unity.Passport.Runtime.UI;
using Unity.Passport.Sample.Scripts.Leaderboard;
using Unity.UOS.Auth;
using Unity.UOS.CloudSave;
using Unity.UOS.CloudSave.Exception;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Login : MonoBehaviour
{


    [Header("排行榜面板")] public GameObject leaderboardPanel;
    private LeaderboardUIController _leaderboardUIController;
    public RawImage userAvatar;
    //[Tooltip("账号名")] public TextMeshProUGUI userId;
    public Texture defaultAvatar;
    public Text currentRealmText;
    public Text personaName;
    public Text personaID;

    public string Userid;
    private Persona _currentPersona; //不可删除
    public string LoginID;//登录ID
    public string userame;  //玩家名
    public string Topname; //排行名 
    private string _currentRealm; // 当前选中的域

    bool isLoaginDy;

    public PlayMakerFSM Fsm;


    //第一步 抖音登录权限申请 同意:获得名字和头像  不同意使用默认名字和头像   
    //抖音小游戏不需要UOS登录
    public void LoginDy()
    {

        TT.Login(OnLoginSuccessCallback, FailedCallback, true);

        void FailedCallback(string errMsg)
        {
            Debug.Log("DY --> 登录失败: " + errMsg);
        }
    }

    /// <summary>登录成功</summary>
    /// <param name="code">临时登录凭证, 有效期 3 分钟。可以通过在服务器端调用 登录凭证校验接口 换取 openid 和 session_key 等信息。</param>
    /// <param name="anonymousCode">用于标识当前设备, 无论登录与否都会返回, 有效期 3 分钟</param>
    /// <param name="isLogin">判断在当前 APP(头条、抖音等)是否处于登录状态</param>
    void OnLoginSuccessCallback(string code, string anonymousCode, bool isLogin)
    {
        Debug.Log($"登录成功，code: {code}, anonymousCode:{anonymousCode}, isLogin{isLogin}");


        TT.GetUserInfo(OnGetScUserInfoSuccessCallback, OnGetScUserInfoFailedCallback);

        void OnGetScUserInfoSuccessCallback(ref TTUserInfo scUserInfo)
        {
            Debug.Log($"登录成功获取用户信息成功，nickName: {scUserInfo.nickName}");
            Debug.Log($"登录成功获取用户信息成功，avatarUrl: {scUserInfo.avatarUrl}");

            // DownSprite(scUserInfo.avatarUrl);
            StartCoroutine(LoadAvatar(scUserInfo.avatarUrl));
            personaName.text = scUserInfo.nickName;
            isLoaginDy = true;


            // Uoslogin(scUserInfo.nickName);
        }

        void OnGetScUserInfoFailedCallback(string errMsg)
        {
            Debug.Log($"登录成功获取用户信息失败，errMsg: {errMsg}");

            //   Uoslogin("玩家");
        }


    }


    //第二步 UOS登录  使用的是随机生成的ID 和抖音登录时的玩家名
    public async void Uoslogin(string name)
    {
        Debug.Log("UOS初始化登录");
        _leaderboardUIController = leaderboardPanel.GetComponent<LeaderboardUIController>();
        await PassportFeatureSDK.Initialize();


        string userId = LoginID; // 需要登录的外部系统的用户Id      
        //string personaId = "1"; // 可选, 需要登录的 外部系统的角色ID
        //string personaDisplayName = "玩家1"; // 可选, 需要登录的角色的昵称。
        await AuthTokenManager.ExternalLogin(userId, null, name);

        _callback(PassportEvent.Completed);

    }

    private async void _callback(PassportEvent passportEvent)
    {
        switch (passportEvent)
        {

            case PassportEvent.Completed:
                Debug.Log("UOS登录成功");
                //await GetPlayerInfo();   //获取OUS玩家名称
                await CloudSave();       //初始化数据库
                GetRealm();             //登录服务器

                break;
        }
    }


    //修改UOS名称
    private async void Gaiming()
    {
        try
        {
            await PassportSDK.Identity.UpdatePersona(personaName.text);
            await PassportSDK.Identity.UpdateUserProfileInfo(personaName.text);
        }

        catch (PassportException e)
        {
            Debug.Log(e.Code);

            string log = e.ErrorMessage;

            bool ybool = log.Contains("无效的名字");
            if (ybool == true)
            {
                Debug.Log("名字无效!!!!请重新输入");

            }
        }
    }

    //获得OUS玩家信息
    private async Task GetPlayerInfo()
    {
        var userInfo = await PassportSDK.Identity.GetUserProfileInfo();
        //userId.text = $"{userInfo.Name}";

        if (userInfo.AvatarUrl != "")
        {
            StartCoroutine(LoadAvatar(userInfo.AvatarUrl));
            
        }
        else
        {
            userAvatar.texture = defaultAvatar;
        }
    }

    /// <summary>
    /// 获取该应用下的域
    /// </summary>
    private async void GetRealm()
    {
        try
        {
            var list = await PassportSDK.Identity.GetRealms();


            if (!list.Any())//服务器数量空 bug 跳出
            {
                UIMessage.Show("请到网站上进行域配置(Passport -> 域管理)", MessageType.Error);
                return;
            }

            _currentRealm = list[0].RealmID;
            currentRealmText.text = list[0].Name;
            StartGame();

        }
        catch (PassportException e)
        {
            Debug.Log(e.Code);
        }
    }


    /// <summary>
    /// 开始游戏，选中角色，如果没有角色则展示新建角色面板
    /// </summary>
    public async void StartGame()
    {

        var persona = await PassportSDK.Identity.GetPersonaByRealm(_currentRealm);

        if (persona != null)
        {

            await OnSelectPersona(persona);  //选择角色

            await _leaderboardUIController.Init();    //初始排行榜
        }
        else
        {

            // 展示创建新角色面板            
        }
    }

    /// <summary>
    /// 登录后最后一步选择某个角色 
    /// </summary>
    /// <param name="persona"></param>
    private async Task OnSelectPersona(Persona persona)
    {       
        try
        {
            _currentPersona = persona;
            await PassportSDK.Identity.SelectPersona(persona.PersonaID);
            personaName.text = $"{persona.DisplayName}";
            personaID.text = $"角色 ID：{persona.PersonaID}";
            Userid = persona.PersonaID;
            FsmVariables.GlobalVariables.GetFsmString("Data_UID").Value = Userid;  //用户id

        }
        catch (PassportException e)
        {
            Debug.Log(e.Code);
        }
    }

    //上传自己的分数
    public void TopUpData(int Score)
    {
        TopUpDa(Score);
    }

    private async void TopUpDa(int Score)
    {


        Leaderboard.UpdateScoreResponse updatedScore = await PassportFeatureSDK.Leaderboard.UpdateScore(Topname, Score);
        // 获取「最好成绩排行榜」的排行
        Leaderboard.ListLeaderboardScoresResponse resp = await PassportFeatureSDK.Leaderboard.ListLeaderboardScores(Topname);
        foreach (Leaderboard.LeaderboardMemberScore score in resp.Scores)
        {
            // 打印玩家的名称、成绩以及所属的分级
            Debug.Log(score.DisplayName);
            Debug.Log(score.Score);
            // 此处tier对应排行榜的分级配置，如：玩家分数为55，tier则根据上面配置的分级对应为「青铜」
            Debug.Log(score.Tier);
        }

    }


    //初始化数据库
    private async Task CloudSave()
    {
        try
        {

            await CloudSaveSDK.InitializeAsync();
            Fsm.SendEvent("UosInitEnd");
        }
        catch (CloudSaveClientException e)
        {
            Debug.LogErrorFormat($"failed to initialize sdk, clientEx: {e}");
            throw;
        }
        catch (CloudSaveServerException e)
        {
            Debug.LogErrorFormat($"failed to initialize sdk, serverEx: {e}");
            throw;
        }

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
            Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

            userAvatar.texture = texture;
         
        }

    }

    /// <summary>
    /// 0：数字类型、1：枚举类型;
    ///     数字类型（0）往往适用于游戏的通关分数（103分、105分），
    ///     枚举类型（1）适用于段位信息（青铜、白银）；
    /// </summary>
    private int rankDataType = 0;
    // 排行榜分区标识--(Nullable)默认值为default, 测试：test
    private string zoneId = "default";

    /// <summary>
    /// 设置排行榜分数
    /// </summary>
    public void SetImRankList(int rankValue)
    {

        if (!isLoaginDy)
        {
            LoginDy();
            Debug.Log("需要先登录再执行更新数据...");
            return;
        }
        Debug.Log("更新排行榜数据：" + rankValue);
        var paramJson = new TTSDK.UNBridgeLib.LitJson.JsonData
        {
            ["dataType"] = rankDataType,
            ["value"] = rankValue,
            //["priority"] = int.Parse(priority),
            ["zoneId"] = zoneId
        };
        Debug.Log($"SetImRankData param:{paramJson.ToJson()}");
        TT.SetImRankData(paramJson, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
                Debug.Log("设置排行榜数据成功");
                GetImRankList();  //显示排行榜
            }
            else
            {
                Debug.Log("设置排行榜数据成功");
            }
        });
    }

    /// <summary>
    /// 获取排行榜列表，调用 API 后， 根据参数自动绘制游戏好友排行榜
    /// </summary>
    public void GetImRankList()
    {
        // <param name="rankType">代表数据排序周期，day为当日写入的数据做排序；week为自然周，month为自然月，all为半年--(Require)</param>
        // <param name="dataType">由于数字类型的数据与枚举类型的数据无法同时排序，因此需要选择排序哪些类型的数据--(Require)</param>
        // <param name="relationType">选择榜单展示范围。default: 好友及总榜都展示，all：仅总榜单--(Nullable)</param>
        // <param name="suffix">数据后缀，最后展示样式为 value + suffix，若suffix传“分”，则展示 103分、104分--(Nullable)</param>
        // <param name="rankTitle">排行榜标题的文案--(Nullable)</param>
        // <param name="zoneId">排行榜分区标识--(Nullable)</param>
        // <param name="paramJson">以上参数使用json格式传入，例如"{"rankType":"week","dataType":0,"relationType":"all","suffix":"分","rankTitle":"","zoneId":"default"}"</param>
        // <param name="action">回调函数</param>

        var paramJson = new TTSDK.UNBridgeLib.LitJson.JsonData
        {
            ["rankType"] = RankType.day.ToString(),
            ["dataType"] = rankDataType,
            ["relationType"] = "default",
            ["suffix"] = "关",
            ["rankTitle"] = "巅峰排行榜",
            ["zoneId"] = zoneId,
        }; 
        Debug.Log($"巅峰排行榜 GetImRankList param:{paramJson.ToJson()}");
        TT.GetImRankList(paramJson, (isSuccess, errMsg) =>
        {
            if (isSuccess)
            {
            }
            else
            {
            }
        });

    }
    public enum RankType
    {
        // 天
        day,
        // 自然周
        week,
        // 自然月
        month,
        // 半年
        all
    }

}
