using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;

public class DYCebian : MonoBehaviour
{

    public GameObject Prefab;
 
    /// <summary>
    /// У�������Ƿ�֧��
    /// </summary>
    /// 
    public void CheckSideBarScene()
    {

        TT.CheckScene(TTSideBar.SceneEnum.SideBar, SuccessCallBack, CompleteCallBack, ErrorCallBack);


        void SuccessCallBack(bool IsSuccess)
        {
            // ���磺��ʾ��������밴ť
            //  success?.Invoke(IsSuccess);
            
            Debug.Log($"�Ƿ�����֧�ֲ��������1��{IsSuccess}");
        }

        void CompleteCallBack()
        {
            Prefab.SetActive(true);
            Debug.Log($"�Ƿ�����֧�ֲ���� 2 CompleteCallBack");
        }
        void ErrorCallBack(int errCode, string errMsg)
        {
            Prefab.SetActive(false);
            Debug.Log($"�Ƿ�����֧�ֲ���� 3 ����id��{errCode}��������Ϣ��{errMsg}");
        }
    }
    /// <summary>
    /// �жϴӲ�������룬���Ž���
    /// </summary>
    public void OnNavigateToScene()
    {

        TTSDK.UNBridgeLib.LitJson.JsonData jsonData = new TTSDK.UNBridgeLib.LitJson.JsonData();
        jsonData["scene"] = "sidebar";
        TT.NavigateToScene(jsonData, SuccessCallBack, CompleteCallBack, ErrorCallBack);
        
        void SuccessCallBack()
        {
            // �Ӳ�������룬���Ž���
            //success?.Invoke();
            Debug.Log($"�Ӳ�������룺navigateToScene:ok");
            PlayMakerFSM Fsm = GetComponent<PlayMakerFSM>();
            Fsm.SendEvent("yesok");
           
        }

        void CompleteCallBack()
        {
            PlayMakerFSM Fsm = GetComponent<PlayMakerFSM>();
            Fsm.SendEvent("yesin");         
            Debug.Log($"�Ӳ�������� �ж���ɣ�CompleteCallBack");
        }
        void ErrorCallBack(int errCode, string errMsg)
        {
            PlayMakerFSM Fsm = GetComponent<PlayMakerFSM>();
            Fsm.SendEvent("nook");
            //fali?.Invoke();
            Debug.Log($"�Ӳ�������� ����id��{errCode}��������Ϣ��{errMsg}");
           
        }
    }
}
