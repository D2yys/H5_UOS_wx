using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MinGanGuoLv : MonoBehaviour
{

    public PlayMakerFSM Fsm;
    public string RrsulText;
    private void TestWords(string String)

    {
        Addressables.LoadAssetAsync<TextAsset>("MinGanCi").Completed += (hal) =>
        {

            TextAsset tex = hal.Result;

            string text = tex.text;

            //文本转数组
            string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            //设置敏感词库
            SensitiveWordUtil.InitSensitiveWordMap(lines);

            //传入文本返回检测后文本
            RrsulText = SensitiveWordUtil.ReplaceSensitiveWords(String);
            Fsm.SendEvent("GetOk");
            Addressables.Release(hal);
        };

    }


    //检测敏感词
    public void JianCe(string JianCeText)
    {
        TestWords(JianCeText);

    }
}
