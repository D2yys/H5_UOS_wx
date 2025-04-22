
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json.Linq;
using UnityEngine.AddressableAssets;

using UnityEngine.ResourceManagement.AsyncOperations;
using HutongGames.PlayMaker;
using System;

public class GetJsonData : MonoBehaviour
{
    //Fsm 是Creat 状态机   Lobby是Lobyy态机
    public PlayMakerFSM FsmCreat;
    //FsmCreat 是创建关卡时获取关卡数据用？ 可不可以合二为一
    public PlayMakerFSM FsmLobby;

    public string level;
    //任务模型和生成数量
    private string goalsModel;
    private string goalsCount;

    //普通模型和生成数量
    private string boardModel;
    private string boardCount;
    public int looplevel;


    //任务目标个数，从左起根据个数取模型作为任务生成卡片
    public int GoalsCount;
    public string str;
    public string DataTex;
    //判断是大厅获取数据还是Creat中获取数据  Fals 默认大厅  Ture Creat 中获取。
    public bool EasyBool;
    private TextAsset tex;
    public bool AddressableGet;
  //  public string Data_LoopArray = "0/0/0/0/0/0/0/0/0/0";

    string[] loopArray;
    
    public int numsid; //组id

    // 组id 0 1 2 3 4 5 8 9  ,  组的总数量，  json

    private void SetDataListId(int numsid, int listCount, JObject jo2)
    {
        //Data_LoopArray 循环的数组？？每一组的完成id完成关卡对应的数+1用于遍历哪一组的第几个关

        var numLives = FsmVariables.GlobalVariables.GetFsmString("Data_LoopArray");

        string getdata = numLives.Value;

        Debug.Log("取到的全局变量是："+getdata);

        char[] delimiter = new char[] { '/' };

        loopArray = getdata.Split(delimiter);  //拆分字符串为数组   

       
        Debug.Log("数组取值是："+loopArray[numsid]);


        //组id超出范围，重置数组对应id的值
        if (int.Parse(loopArray[numsid]) >= listCount)
        {
            loopArray[numsid] = "0";

            FsmVariables.GlobalVariables.GetFsmString("Data_LoopArray").Value ="0";

        }
        //得到json 里的 组id 对应的数据里寄存的数   ease 0 组 第几个 例如：0组第0个 
        looplevel = (int)jo2["LevelLoop"][numsid.ToString()][int.Parse(loopArray[numsid])];


        string levv = looplevel.ToString("D4");
        Debug.Log("loop-----ease: " + numsid + "------------------循环关:" + levv);

        Addressables.LoadAssetAsync<TextAsset>("Level"+levv).Completed += (hal) =>
        {

            TextAsset tex = hal.Result;

            str = tex.text;

            GetEnd();

            Addressables.Release(hal);
        };

        

    }
    //循环关卡过关后下次读取的时候就是下一个关卡
    public void LoopIDadd()
    {
      
        int i = int.Parse(loopArray[numsid])+1; //获得数组里的数+1 

        loopArray[numsid] = i.ToString();

        string data = string.Join("/", loopArray);

        

        FsmVariables.GlobalVariables.GetFsmString("Data_LoopArray").Value =data;

        Debug.Log("更新后的数组数据是："+data);
    }

    public void GetJson()

    {
        //值清空
        goalsModel = "";
        goalsCount = "";
        boardModel = "";
        boardCount = "";

        //读取的文本文件后面需要有.tex后缀
        //本地获取数据

        var numLives = FsmVariables.GlobalVariables.GetFsmInt("Data_level");
        int lv = numLives.Value;

        //循环关卡开启
        if (lv >= 2011)
        {
            

            int loop = (lv - (2010 + 1)) % ((2010 + 1) - 1001) + 1001;

            Debug.Log("---------------------循环关卡状态---------------------当前关卡数：" + lv+"计算后的关卡是："+loop+"得到计算后关卡的ease");

            //本地加载
            if (AddressableGet == false)
            {
                TextAsset newtex = Resources.Load("Assetsbundle/3DTriple Match/Resources_Ass/Triple/TextAsset/Level" + loop + "") as TextAsset;

                str = newtex.text;

                JObject jo1 = JObject.Parse(str);

                numsid = (int)jo1["ease"];

                newtex = Resources.Load("Assetsbundle/3DTriple Match/Resources_Ass/Triple/TextAsset/LevelLoop") as TextAsset; 

                str = newtex.text;

                JObject jo2 = JObject.Parse(str);


                JArray list = (JArray)jo2["LevelLoop"][numsid.ToString()];
                int count = list.Count;

                SetDataListId(numsid, count, jo2);
            }
            else
            {
                //获得某一关， 得到某一关的 ease   ease是关卡组的id . 在获得这个组总个数  
                Addressables.LoadAssetAsync<TextAsset>("Level" + loop).Completed += (hal) =>
                {


                    TextAsset tex = hal.Result;

                    str = tex.text;



                    JObject jo1 = JObject.Parse(str);

                    numsid = (int)jo1["ease"];

                    Addressables.Release(hal);


                    Addressables.LoadAssetAsync<TextAsset>("LevelLoop").Completed += (hal) =>
                    {

                        TextAsset tex = hal.Result;

                        str = tex.text;

                        JObject jo2 = JObject.Parse(str);


                        JArray list = (JArray)jo2["LevelLoop"][numsid.ToString()];//得到ease 组json

                        int count = list.Count;

                        //组id   ,  组的总数量，  json
                        SetDataListId(numsid, count, jo2);

                        Addressables.Release(hal);
                    };

                };


            }

        }

        //非循环关卡
        else
        {
            if (AddressableGet == false)
            {
                TextAsset tex = Resources.Load("Assets/Assetsbundle/3DTriple Match/Resources/TextAsset/Level" + level + "") as TextAsset;

                str = tex.text;

                GetEnd();
            }

            else

            {


                Addressables.LoadAssetAsync<TextAsset>("Level" + level).Completed += (hal) =>
                {


                    TextAsset tex = hal.Result;

                    str = tex.text;
                    GetEnd();
                 
                    Addressables.Release(hal);
                };



            }
        }

    }

    private void GetEnd()
    {
        
        JObject jo = JObject.Parse(str);
       
        int durationa = (int)jo["duration"];
        string badge = (string)jo["badge"];
        int assist = (int)jo["assist"];
        int ease = (int)jo["ease"];

        JArray a = (JArray)jo["goals"];
        foreach (JObject item in a)
        {
            string id = (string)item["id"];
            goalsModel += id + "/";

            int count = (int)item["count"];
            goalsCount += count + "/";

        }

        JArray b = (JArray)jo["board"];
        foreach (JObject item in b)
        {
            string id = (string)item["id"];
            boardModel += id + "/";

            int count = (int)item["count"];
            boardCount += count + "/";
        }
        GoalsCount = a.Count;

        DataTex = goalsModel + boardModel + "*" + goalsCount + boardCount + "*" + durationa + "*" + badge + "*" + assist + "*" + ease + "*" + GoalsCount;
        if (EasyBool == false)
        {
            FsmLobby.SendEvent("Down");
        }
        else
        {
            FsmCreat.SendEvent("Down");
        }

    }

}



