
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
//using WeChatWASM;


[System.Serializable]
public class OpenDataMessage
{
    // type 用于表明时间类型
    public string type;

    public int score;
}

public class WXRank : MonoBehaviour
{
    /// <summary>
    /// 显示排行榜数据内容
    /// </summary>
    public RawImage RankBody;

    /// <summary>
    /// 排行榜
    /// </summary>
    public GameObject RankingBox;
    public GameObject RankMask;
    public GameObject OBJ;
    public int score;

    //public  void ShowTop()
    //{
       
    //    GetSettingOption getSettingOption = new GetSettingOption()
    //    {
    //        success = GetSettingSuccess,
    //        fail = (result) => { Debug.Log($"获取设置信息失败 {result.errMsg}"); }
    //    };
    //    WX.GetSetting(getSettingOption);
    //}

    ///// <summary>
    ///// 渲染排行榜
    ///// </summary>
    //private void ShowOpenData()
    //{
   

    //    CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
    //    var referenceResolution = scaler.referenceResolution;
    //    var p = RankBody.transform.position;

    //    WX.ShowOpenData(RankBody.texture, (int)p.x, Screen.height - (int)p.y, (int)((Screen.width / referenceResolution.x) * RankBody.rectTransform.rect.width), (int)((Screen.width / referenceResolution.x) * RankBody.rectTransform.rect.height));
    //    OBJ.SetActive(true);
    //}


    ///// <summary>
    ///// 获取玩家配置成功
    ///// </summary>
    ///// <param name="result"></param>
    //private void GetSettingSuccess(GetSettingSuccessCallbackResult result)
    //{
    //    if (!result.authSetting.ContainsKey("scope.userInfo") || !result.authSetting["scope.userInfo"])
    //    {
    //        Debug.Log("生成按钮开始请求获取用户信息");

    //        //此处设置虚拟按钮大小 
    //        WXUserInfoButton wxUserInfoButton = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "zh_CN", true);
    //        //wxUserInfoButton.Show();
    //        wxUserInfoButton.OnTap((data) =>
    //        {
    //            if (data.errCode == 0)
    //            {
    //                //获取成功
    //                Debug.Log($"用户同意授权 用户名：{data.userInfo.nickName} 用户头像{data.userInfo.avatarUrl}");
    //               // setnameAsync(data.userInfo.nickName, data.userInfo.avatarUrl); //设置UOS中的名字和头像地址
    //            }
    //            else
    //            {
    //                Debug.Log("用户拒绝授权");
                                        
    //            }
    //            wxUserInfoButton.Hide();
    //        });
    //    }
    //    else
    //    {
    //        Debug.Log("已获取过权限");
    //        GetUserInfoOption getUserInfoOption = new GetUserInfoOption()
    //        {
    //            lang = "zh_CN",
    //            withCredentials = false,
    //            success = GetUserInfoSuccess,
    //            fail = (result) => {

    //                Show(score);
    //                Debug.Log($"获取玩家信息失败 {result.errMsg}"); 
    //            }
    //        };
    //        WX.GetUserInfo(getUserInfoOption);
    //    }
    //}
    ////更新角色名和头像地址
    //private async Task setnameAsync(string name,string url)
    //{
    //  //  await PassportSDK.Identity.UpdatePersona(name, url);
    //}
    ///// <summary>
    ///// 获取玩家信息成功回调
    ///// </summary>
    ///// <param name="data"></param>
    //private void GetUserInfoSuccess(GetUserInfoSuccessCallbackResult data)
    //{
    //    Show(score);
    //    Debug.Log($"用户名：{data.userInfo.nickName} 用户头像{data.userInfo.avatarUrl}");
    //}

    ///// <summary>
    ///// 显示排行榜
    ///// </summary>
    //public void Show(int Score)
    //{


    //    SetUserScore(Score);

    //    ShowOpenData();

    //    //显示排行榜
    //    OpenDataMessage msgData = new OpenDataMessage();
    //    msgData.type = "showFriendsRank";
    //    string msg = JsonUtility.ToJson(msgData);
    //    WX.GetOpenDataContext().PostMessage(msg);
    //}
    //public void ShareGroupRank()
    //{
    //    // 分享信息到群聊
    //    WX.ShareAppMessage(new ShareAppMessageOption()
    //    {
    //        // 标题
    //        title = "在吗在吗？吗喽",
    //        // 分享的路径参数，与OnShow约定进行判断
    //        query = "action=show_group_rank",
    //        // 封面 - 后续可更换为自己的，但需要在微信公众平台配置
    //        // https://developers.weixin.qq.com/minigame/dev/guide/open-ability/share/share.html
    //        imageUrl = "https://mmgame.qpic.cn/image/5f9144af9f0e32d50fb878e5256d669fa1ae6fdec77550849bfee137be995d18/0",
    //    });
    //}

    ///// <summary>
    ///// 设置开放域数据 （要排行的数据）
    ///// </summary>
    ///// <param name="msgData"></param>
    //public void SetUserScore(int Score)
    //{
    //    //Debug.Log("设置数据");
    //    OpenDataMessage message = new OpenDataMessage();
    //    message.type = "setUserRecord";
    //    message.score = Score;
    //    string msg = JsonUtility.ToJson(message);
    //    WX.GetOpenDataContext().PostMessage(msg);
    //   // Debug.Log("每日杀敌数：" + message.score);
    //}
    //public void SetUserZDE(int Score)
    //{
    //    OpenDataMessage message = new OpenDataMessage();
    //    message.type = "setUserRecord";
    //    message.score = Score;
    //    string msg = JsonUtility.ToJson(message);
    //    WX.GetOpenDataContext().PostMessage(msg);

    //}


    ///// <summary>
    ///// 隐藏排行榜
    ///// </summary>
    //public void Hide()
    //{
    //    //gameObject.SetActive(false);        
    //    WX.HideOpenData();
    //}
   

}