using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddFsm : MonoBehaviour
{
    public PlayMakerFSM Fsm;
    public bool down = false;
    public bool AddressablesBool= false;

    void Start()
    {
        Fsm = gameObject.AddComponent<PlayMakerFSM>();

        //FsmTemplate a = Resources.Load("MoBan/Cube", typeof(FsmTemplate)) as FsmTemplate;
        //Fsm.SetFsmTemplate(a);
        //Fsm.FsmName = ("状态机");
        //down = true;
        var numLives = FsmVariables.GlobalVariables.GetFsmObject("Template_Cube");

        FsmTemplate a = (FsmTemplate)numLives.Value;
        Fsm.SetFsmTemplate(a);
        Fsm.FsmName = ("状态机");
        down = true;


        //Addressables.LoadAssetAsync<FsmTemplate>("Cube").Completed += (hal) =>   //这种加载方式有延迟，会导致点击事件无法发送到状态机内，  但是模版放在Resource下 无法通过BunlDow 更新
        //{

        //    Fsm = gameObject.AddComponent<PlayMakerFSM>();
        //    Fsm.FsmName = "状态机";
        //    FsmTemplate a = hal.Result;
        //   Fsm.SetFsmTemplate(a);   //模版文件要在状态机开始运行前设置好  

        //    down = true;
        //};



    }
}
