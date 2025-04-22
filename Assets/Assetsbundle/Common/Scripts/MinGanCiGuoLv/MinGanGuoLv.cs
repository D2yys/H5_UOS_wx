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

            //�ı�ת����
            string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            //�������дʿ�
            SensitiveWordUtil.InitSensitiveWordMap(lines);

            //�����ı����ؼ����ı�
            RrsulText = SensitiveWordUtil.ReplaceSensitiveWords(String);
            Fsm.SendEvent("GetOk");
            Addressables.Release(hal);
        };

    }


    //������д�
    public void JianCe(string JianCeText)
    {
        TestWords(JianCeText);

    }
}
