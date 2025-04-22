using HutongGames.PlayMaker;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class myConfig : MonoBehaviour
{
    // Start is called before the first frame update

    public TextAsset Config;
    public TextAsset Payment;
    public TextAsset Mission;

    // Update is called once per frame
    int value;
    double valued;
    bool configbool;
    public int[] dataint;
    string stringdata;
    float flott;  
    public Text text;

    public List<float> Group;
    public List<float> SkillFloat;//储存的的过关时间比值
    public int PlayerGroup;
    public string DayString; // 周活动的日期
    public int MissinId; //活动时的图标id
    public string MissinTime;// 活动剩余的时间 天数小时
    public myConfig Cf;
    public void GetPlayerLevel(float flo)
    {
        JObject jsonData = JObject.Parse(Config.text);

        JArray Ja = (JArray)jsonData["Config"]["SkillGroupTracking"];

        JArray Multiplier = (JArray)jsonData["Config"]["Game"]["BoardItemsMultiplier"];

        var numLives = FsmVariables.GlobalVariables.GetFsmInt("Data_level");

       int level = numLives.Value;

        int StartLevel = (int)Ja[0]["StartLevel"];
        int EndLevel = (int)Ja[0]["EndLevel"];

       // Debug.Log("当前关卡:" + level + "百分比:" + flo + "起始关:" + StartLevel + "结束关:" + EndLevel + "fffff" + flott);
        if (level >= StartLevel && level < EndLevel)
        {

            SkillFloat.Add(flo);
            Debug.Log("flo" + flo);

        }
        else
        {
            float _float = 0;

            if (level == EndLevel)
            {

                for (int i = 0; i < SkillFloat.Count; i++)
                {
                    _float += SkillFloat[i];

                }
                Debug.Log("总float:" + _float);
                float _floatb = 1 - (_float / (EndLevel - StartLevel));//取平均用时值
                Debug.Log("平均float:" + _float);

                Group.Add((float)Ja[0]["Group1"]);//0.4
                Group.Add((float)Ja[0]["Group2"]);//0.3
                Group.Add((float)Ja[0]["Group3"]);//0.22
                Group.Add((float)Ja[0]["Group4"]);//0.15
                Group.Add((float)Ja[0]["Group5"]);//0
                Debug.Log("组ID：" + Group.Count);

                int count = Group.Count;

                for (int i = 0; i < count; i++)
                {
                    
                    Debug.Log("用户组参数设置i=:" + i + "平均值" + _float + "Group 值" + Group[i]);
                    if (_float > Group[i])
                    {
                        FsmVariables.GlobalVariables.GetFsmInt("Data_PlayerGroup").Value = i;
                      
                        flott = (float)Multiplier[i]["Value"];

                        FsmVariables.GlobalVariables.GetFsmFloat("Data_PlayerGroup_Multiplier").Value = flott;

                        Debug.Log("用户组参数设置:组ID"+i+" 难度系数:"+flott );
                        break;
                    }

                }
            }

        }

    }
    public void PaymentJson(string ItemName, string key,string text)
    {
        


        JObject jsonData = JObject.Parse(text);

        string root1 = "Payment";
        string root2 = "Items";


        if (key != "CooldownTime")
        {
            value = (int)jsonData[root1][root2][ItemName][key];
            SetFsmVariabInt();
        }
        else
        {
            valued = (double)jsonData[root1][root2][ItemName][key];
            SetFsmVariabFloat();
        }

    }


    public void PaymentData(string ItemName,string key) {

   
        PaymentJson(ItemName, key,Payment.text);
    }


    

    //关卡数据
    public void ConfigJson(string Title, string listkey)
    {
        string str = Config.text;

        JObject jsonData = JObject.Parse(str);
        string root1 = "Config";
        string root2 = "Game";
        // Debug.Log("执行");

        if (Title != "LevelCompletRewards")
        {

            try
            {
                value = (int)jsonData[root1][root2][Title];
                SetFsmVariabInt();
            }
            catch
            {
                try
                {
                    valued = (double)jsonData[root1][root2][Title];
                    SetFsmVariabFloat();
                }
                catch
                {
                    //   configbool = (bool)Items[Title];
                }
            }
        }

        else
        {
            stringdata = ("");

            JArray Ja = (JArray)jsonData[root1][root2][Title];

            if (listkey != "Reward")
            {



                for (int i = 0; i < Ja.Count; i++)
                {


                    string a = (string)jsonData[root1][root2][Title][i][listkey];

                    stringdata += a + "/";


                }
            }
            else
            {


                for (int i = 0; i < Ja.Count; i++)
                {
                    string a = (string)jsonData[root1][root2][Title][i][listkey][0]["Count"];

                    stringdata += a + "/";


                }
            }

            SetFsmString();

        }
    }
    //任意表取任意值
    public void ConfigJsonRoot(string root1, string root2, string Title, string listkey)
    {
        string str = Config.text;
        if (root1 == "TreasureMission")
        {
            str = Mission.text;
            // Debug.Log("++++++" + str);
        }
        if (root1 == "Payment")
        {
            str = Payment.text;
        }
        JObject jsonData = JObject.Parse(str);

        stringdata = ("");

        if (listkey.Length == 0)
        {

            try
            {

                value = (int)jsonData[root1][root2][Title];
                SetFsmVariabInt();

            }
            catch
            {

                try
                {

                    valued = (double)jsonData[root1][root2][Title];
                    SetFsmVariabFloat();

                }
                catch
                {

                    try
                    {

                        configbool = (bool)jsonData[root1][root2][Title];

                    }
                    catch
                    {

                        try
                        {
                            JArray Ja = (JArray)jsonData[root1][root2][Title];

                            for (int i = 0; i < Ja.Count; i++)
                            {


                                string a = (string)jsonData[root1][root2][Title][i];

                                stringdata += a + "/";


                            }

                        }
                        catch
                        {
                            //根节点取数组
                            try
                            {
                                JArray Ja = (JArray)jsonData[root1][root2];
                                for (int i = 0; i < Ja.Count; i++)
                                {


                                    string a = (string)jsonData[root1][root2][i][Title];

                                    stringdata += a + "/";


                                }
                            }
                            catch { Debug.Log("--------------------------取值失败----------------------------"); }

                        }

                    }
                }
            }

        }

        else
        {
            try
            {
                value = (int)jsonData[root1][root2][Title][listkey];
                SetFsmVariabInt();
            }
            catch
            {

                JArray Ja = (JArray)jsonData[root1][root2][Title];
                try
                {


                    for (int i = 0; i < Ja.Count; i++)
                    {


                        string a = (string)jsonData[root1][root2][Title][i][listkey];

                        stringdata += a + "/";


                    }
                }
                catch
                {


                    for (int i = 0; i < Ja.Count; i++)
                    {


                        string a = (string)jsonData[root1][root2][Title][i][listkey][0]["Count"];

                        stringdata += a + "/";


                    }
                }


            }
        }
        SetFsmString();
    }

    //获取周活动奖励配置
    public void ConfMission(int GetId)
    {
        string str = Mission.text;
        string root1 = "TreasureMission";
        string root2 = "TreasureMissionData";

        JObject jsonData = JObject.Parse(str);

        int GoalSize = (int)jsonData[root1][root2][GetId]["GoalSize"];
        string Type = (string)jsonData[root1][root2][GetId]["Reward"][0]["Type"];

        int Count = (int)jsonData[root1][root2][GetId]["Reward"][0]["Count"];

        stringdata = GoalSize + "*" + Type + "*" + Count;

        SetFsmString();
    }
    //获取活动日期
    public void MissionDay()
    {
        int NowYear = DateTime.Now.Year;
        Debug.Log("当前年份是:"+NowYear);
        DateTime current = new DateTime(NowYear, 1, 1); // 设置当前年分的开始日期 1月1日
        int hdid = 0; //活动id
        while (current.Year == NowYear)//遍历
        {
            if (current.DayOfWeek == DayOfWeek.Friday)//是否是周五
            {
               // Debug.Log("是周五:" + current.DayOfWeek);
                if (hdid <6)  // 活动id的数量
                {
                    hdid = hdid+1;
                  
                    MissinId = hdid; //活动时的图标id
                }
                else {
                    hdid = 0;
                }
                TimeSpan SpanDay = DateTime.UtcNow.AddHours(+8).Subtract(current);// 用周五的日期减去当前时间判断是否在活动时间内

                if (SpanDay.TotalDays < 7 && SpanDay.TotalDays > 0)// 大于0小于7在活动期间内
                {
                    DayString = current.ToString("yyyy-MM-dd");//获得这个周五的日期

                    var numLives = FsmVariables.GlobalVariables.GetFsmString("Data_MissnTimeTset");//获得测试时间数据
                    string myString = numLives.Value;
                    DateTime test;
                    try { test = DateTime.Parse(myString); }
                    catch
                    {
                        test = DateTime.Parse(DayString);

                    }
                    

                    if (current.Date != test.Date)
                    {
                        FsmVariables.GlobalVariables.GetFsmString("Data_MissnTimeTset").Value = DayString; //记录是否同一天的测试时间
                        FsmVariables.GlobalVariables.GetFsmInt("Data_GetID").Value = 0;
                        FsmVariables.GlobalVariables.GetFsmInt("Data_GetModelInt").Value = 0;
                    }

                    Debug.Log("活动的开始日期是:" + DayString+"活动id是："+MissinId);
                    break;
                }                
              
            }
            current = current.AddDays(1); // 天数递增
        }


        StartDateTime();//用获得的时间计时
        

    }

    //活动倒计时
    public void StartDateTime()
    {

        CancelInvoke("MissionDayTime");
        InvokeRepeating("MissionDayTime", 0, 600);//间隔时长
    }

    public void TimeEnd()//活动重置
    {
        FsmVariables.GlobalVariables.GetFsmInt("Data_GetID").Value = 0;
        FsmVariables.GlobalVariables.GetFsmInt("Data_GetModelInt").Value = 0;
        CancelInvoke("MissionDay()");//???????是否是错误

    }

    private void MissionDayTime()  //时间显示，天，小时数
    {
        DateTime day = DateTime.Parse(DayString);

        DateTime EndDay = day.AddDays(7);

        TimeSpan sc = EndDay.Subtract(DateTime.UtcNow.AddHours(+8));

        text.text = (int)sc.TotalDays + "天 " + (int)sc.TotalHours % 24 + "小时 ";
        MissinTime = text.text;

        //text.text = (int)sc.TotalDays + "d " + (int)sc.TotalHours % 24 + "h " + (int)sc.TotalMinutes % 60 + "m " + (int)sc.TotalSeconds % 60 + "s";

        if (sc.TotalSeconds <= 0)
        {
            TimeEnd();//时间到
        }

    }

    //更新全局数据Int值
    private void SetFsmVariabInt()
    {
        FsmVariables.GlobalVariables.GetFsmInt("Data_Confjg_Int").Value = value;

    }
    //更新全局数据Float值
    private void SetFsmVariabFloat()
    {
        float valuef = (float)valued;

        FsmVariables.GlobalVariables.GetFsmFloat("Data_Confjg_Float").Value = valuef;

    }

    private void SetFsmArray()
    {
        FsmVariables.GlobalVariables.GetFsmArray("Data_Confjg_ArrayInt").intValues = dataint;

    }

    //设置全局字符串
    private void SetFsmString()
    {
        FsmVariables.GlobalVariables.GetFsmString("Data_Confjg_String").Value = stringdata;

    }
}