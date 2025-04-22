using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;

public class DYCebian : MonoBehaviour
{

    public GameObject Prefab;
 
    /// <summary>
    /// 校验宿主是否支持
    /// </summary>
    /// 
    public void CheckSideBarScene()
    {

        TT.CheckScene(TTSideBar.SceneEnum.SideBar, SuccessCallBack, CompleteCallBack, ErrorCallBack);


        void SuccessCallBack(bool IsSuccess)
        {
            // 比如：显示侧边栏进入按钮
            //  success?.Invoke(IsSuccess);
            
            Debug.Log($"是否宿主支持侧边栏进入1：{IsSuccess}");
        }

        void CompleteCallBack()
        {
            Prefab.SetActive(true);
            Debug.Log($"是否宿主支持侧边栏 2 CompleteCallBack");
        }
        void ErrorCallBack(int errCode, string errMsg)
        {
            Prefab.SetActive(false);
            Debug.Log($"是否宿主支持侧边栏 3 错误id：{errCode}，错误信息：{errMsg}");
        }
    }
    /// <summary>
    /// 判断从侧边栏进入，发放奖励
    /// </summary>
    public void OnNavigateToScene()
    {

        TTSDK.UNBridgeLib.LitJson.JsonData jsonData = new TTSDK.UNBridgeLib.LitJson.JsonData();
        jsonData["scene"] = "sidebar";
        TT.NavigateToScene(jsonData, SuccessCallBack, CompleteCallBack, ErrorCallBack);
        
        void SuccessCallBack()
        {
            // 从侧边栏进入，发放奖励
            //success?.Invoke();
            Debug.Log($"从侧边栏进入：navigateToScene:ok");
            PlayMakerFSM Fsm = GetComponent<PlayMakerFSM>();
            Fsm.SendEvent("yesok");
           
        }

        void CompleteCallBack()
        {
            PlayMakerFSM Fsm = GetComponent<PlayMakerFSM>();
            Fsm.SendEvent("yesin");         
            Debug.Log($"从侧边栏进入 判断完成：CompleteCallBack");
        }
        void ErrorCallBack(int errCode, string errMsg)
        {
            PlayMakerFSM Fsm = GetComponent<PlayMakerFSM>();
            Fsm.SendEvent("nook");
            //fali?.Invoke();
            Debug.Log($"从侧边栏进入 错误id：{errCode}，错误信息：{errMsg}");
           
        }
    }
}
