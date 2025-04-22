using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TemplateLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Addressables.LoadAssetAsync<FsmTemplate>("Cube").Completed += (hal) =>   //这种加载方式有延迟，会导致点击事件无法发送到状态机内，  但是模版放在Resource下 无法通过BunlDow 更新
        {

            FsmVariables.GlobalVariables.GetFsmObject("Template_Cube").Value = hal.Result;
        
        };
    }
    
}
