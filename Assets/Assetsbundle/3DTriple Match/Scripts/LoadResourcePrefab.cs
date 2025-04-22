using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadResourcePrefab : MonoBehaviour
{
    public GameObject Prefab;
    public string key;
    public string iconkey;
    public PlayMakerFSM Fsm;
    public PlayMakerFSM ZdeFsm;
    public PlayMakerFSM IconFsm;
    public Texture Icon;
    public string GroupList;
    public bool Addressable;
    public string str;
   



    //是否加载后释放内存bool
    public bool Release =true;
    /// </summary>
    //获得文件夹中的预制体
    /// </summary>
    /// 

    //------------------------------------------------------------------------------------------ 
    //------------------------------------------------------------------------------------------ 
    //------------------------------------------------------------------------------------------ 
    public void GetPrefab()
    {
        if (Addressable == true)
        {

            Addressables.LoadAssetAsync<GameObject>(key).Completed += (hal) =>
            {


                Prefab = hal.Result;                              
                Fsm.SendEvent("GetOk");
                if (Release == true) {
                    Addressables.Release(hal);
                }


            };

        }
        else
        {
            Prefab = Resources.Load("Triple/Prefabs/" + key + "", typeof(GameObject)) as GameObject;
            //Creat 中的GetOk
            Fsm.SendEvent("GetOk");
        }
    }

    //抓大鹅获取模型
    public void ZhuaDaEGetPrefab(string EvenName)
    {
        if (Addressable == true)
        {

            Addressables.LoadAssetAsync<GameObject>(key).Completed += (hal) =>
            {


                Prefab = hal.Result;
                ZdeFsm.SendEvent(EvenName);
                if (Release == true)
                {
                    Addressables.Release(hal);
                }


            };

        }
        else
        {
            Prefab = Resources.Load("Triple/Prefabs/" + key + "", typeof(GameObject)) as GameObject;
            //Creat 中的GetOk
            ZdeFsm.SendEvent(EvenName);
        }
    }


    //获得卡片
    public void GetTexture()
    {
        if (Addressable == true)
        {

            Addressables.LoadAssetAsync<Texture>(iconkey).Completed += (hal) =>
            {


                Icon = hal.Result;

                IconFsm.SendEvent("GetOk");
                if (Release == true) { 
                Addressables.Release(hal);
                }
            };
 
        }
        else
        {
            Icon = Resources.Load("Triple/Icon/" + iconkey + "", typeof(Texture)) as Texture;
            //Card 中的GetOk
            IconFsm.SendEvent("GetOk");
        }
    }
    //获取文本
    public void GetGroupPrefab()
    {
        GroupList = "";
        if (Addressable == true)
        {

            Addressables.LoadAssetAsync<TextAsset>("A_tags").Completed += (hal) =>
            {


                TextAsset ss = hal.Result;

                str = ss.text;

                TexToJson();
                Addressables.Release(hal);
            };

        }
        else
        {
            //读取的文本文件后面需要有.tex后缀
            TextAsset tex = Resources.Load("Triple/TextAsset/A_tags") as TextAsset;
            //文本转字符串↓

            str = tex.text;
            Debug.Log(str);
            TexToJson();
        }     
    }
    
    public void TexToJson()
    {
       
        JObject jo = JObject.Parse(str);

        //从jo变量中取值↓

        // GroupList = (string)jo[key];
        JArray a = (JArray)jo[key];
        foreach (var item in a)
        {
           
            // GroupList += (string)item;
            GroupList += (string)item + "/";
        }
        Fsm.SendEvent("GroupGetOk");

    }
    public void GetPrefabNew()
    {
        if (Addressable == true) {


            Addressables.LoadAssetAsync<GameObject>(key).Completed += (hal) =>
            {


                Prefab = hal.Result;

                Fsm.SendEvent("GroupPrefabGetOK");
                if (Release == true)
                {
                    Addressables.Release(hal);
                }
            };
          
        }
        else { 
        Prefab = Resources.Load("Triple/Prefabs/" + key + "", typeof(GameObject)) as GameObject;
        //Creat 中的GetOk

        Fsm.SendEvent("GroupPrefabGetOK");
        }
    }

    public void ClearAddressbleRes()//释放所有没有引用到的资源
    {
        Debug.Log("释放未引用的资源");
        Resources.UnloadUnusedAssets();
    }
}
