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
        Addressables.LoadAssetAsync<FsmTemplate>("Cube").Completed += (hal) =>   //���ּ��ط�ʽ���ӳ٣��ᵼ�µ���¼��޷����͵�״̬���ڣ�  ����ģ�����Resource�� �޷�ͨ��BunlDow ����
        {

            FsmVariables.GlobalVariables.GetFsmObject("Template_Cube").Value = hal.Result;
        
        };
    }
    
}
