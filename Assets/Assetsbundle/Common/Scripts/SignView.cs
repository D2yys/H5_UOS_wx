using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using HutongGames.PlayMaker;

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

public class SignView : MonoBehaviour
{

    public Text text;

    public string[] Jiangli;
 
    int signNum;//签到次数

    DateTime today;//当前日期

    DateTime signData;   //上次签到日期

    public string json; //云函数返回时间戳

   public TimeSpan NowOffestTime = TimeSpan.Zero;

    double second;


    public void GetServerTime()//获得当前日期
    {
        getServerTime();
       // today = GetWebTime();
       // Debug.Log("今天日期是"+today);
        
    }


    private void GetEnd()//日期判断
    {

        var numLives = FsmVariables.GlobalVariables.GetFsmInt("Data_signNum");

        signNum = numLives.Value;//得到签到次数

        
        if (signNum>=7 )//签到次数等于大于7
        {
            signNum = 0;
            FsmVariables.GlobalVariables.GetFsmInt("Data_signNum").Value = signNum;

            FsmVariables.GlobalVariables.GetFsmString("Data_signTime").Value = signData.ToString();

            DataUpdata();

        }


        var time = FsmVariables.GlobalVariables.GetFsmString("Data_signTime");
        string stringtime = time.Value;//得到上次签到日期

        signData = DateTime.Parse(stringtime);


        Djstest();


    }


    public void Djstest()//上次日期与当前日期判断
    {


        if (IsOneDay(signData, today))//是同一天 ，显示下次领取倒计时
        {

            var DayAdd = today.AddDays(1);//当天+1

            DateTime currentDate = DayAdd.Date;//去除小时、分、秒

           
                LocalTime(currentDate);  //传入倒计时
            

        }
        else
        {
            text.text = ("可领取");//图标盒子按钮状态
            Stop();
        }

    }
    public void Timer()
    {
        InvokeRepeating("daojishi", 0, 1);
    }

    public void Stop()
    {
        CancelInvoke("daojishi");
    }

    // 总秒数的倒计时
    private void daojishi()
    {
        int hours = (int)second / 3600;
        int remainingSeconds = (int)second % 3600;
        int minutes = remainingSeconds / 60;
        int secs = remainingSeconds % 60;


        second = second - 1;

        text.text = string.Format("{0:d2}:{1:d2}:{2:d2}", hours, minutes, secs);


        if (second <= 0)
        {
            Stop();
            text.text = ("可领取");
        }

    }

    //数据更新
    public void DataUpdata()
    {
        GameObject Manager = GameObject.Find("Manager");

        MySendEvent scir = Manager.GetComponent<MySendEvent>();

        scir.DataUpdata();
    }


    //签到按钮点击
    public void OnSignClick()
    {
        //如果不是同一天就是可以领取，
        if (!IsOneDay(signData, today))
        {
            signNum++;

            FsmVariables.GlobalVariables.GetFsmInt("Data_signNum").Value = signNum;
            FsmVariables.GlobalVariables.GetFsmString("Data_signTime").Value = today.ToString();

            
            signData = today;
            DataUpdata();
            GetComponent<PlayMakerFSM>().SendEvent("领取成功");

        }
        else
        {
            //Debug.Log("不可领取是同一天上次领取日期：" + signData.ToString() + "当前日期：" + today.ToString());
            GetComponent<PlayMakerFSM>().SendEvent("明日可领取");
        }
    }




    //判断是否是同一天
    bool IsOneDay(DateTime t1, DateTime t2)
    {
        return (t1.Year == t2.Year &&
         t1.Month == t2.Month &&
          t1.Day == t2.Day);
    }

   
    public void LocalTime(DateTime currentDate)
    {


        TimeSpan Jstime = currentDate.Subtract(DateTime.UtcNow.AddHours(+8));//+1天时间-当前服务器时间

        second = Jstime.TotalSeconds;//倒计时总秒数

        Timer();

    }


   
    //获得服务器时间
    public void getServerTime()
    {
        string url = "https://www.baidu.com";
        StartCoroutine(IServerTime(url));
    }

    IEnumerator IServerTime(string url)
    {
        Debug.Log("开始获取“+url+”的服务器时间（GMT DATE）");
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            Dictionary<string, string> resHeaders = www.responseHeaders;
            string key = "DATE";
            string value = null;
            if (resHeaders != null && resHeaders.ContainsKey(key))
            {
                resHeaders.TryGetValue(key, out value);
            }

            if (value == null)
            {
                Debug.Log("DATE is null");
                yield break;
            }

            DateTime Gmt = GMT2Local(value);           
            DateTime now = DateTime.Now;
            Debug.Log("获得的转换后的网站时间是："+ Gmt);
            today = Gmt;  //网站的当前时间
             
            if (IsNewerHour(now, Gmt))//只要用户调的时间差在一个小时内都能接受   网站时间和当前时间比较对不上矫正
            {
                //记录一下时间差 这就是用户手动改的时间与世界时间的间隔
                //之后调用GetNowTime()就是准确时间
                NowOffestTime = now - Gmt;
                today = DateTime.Now - NowOffestTime;                 
                GetEnd();
                yield break;
            }
            GetEnd();
        }
    }

    /// <summary>
    /// GMT时间转成本地时间全世界各个时区都会自动转换
    /// </summary>
    /// <param name="gmt">字符串形式的GMT时间</param>
    /// <returns></returns>
    public DateTime GMT2Local(string gmt)
    {
        DateTime dt = DateTime.MinValue;
        try
        {
            string pattern = "";
            if (gmt.IndexOf("+0") != -1)
            {
                gmt = gmt.Replace("GMT", "");
                pattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";
            }
            if (gmt.ToUpper().IndexOf("GMT") != -1)
            {
                pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
            }
            if (pattern != "")
            {
                dt = DateTime.ParseExact(gmt, pattern, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
                dt = dt.ToLocalTime();
            }
            else
            {
                dt = Convert.ToDateTime(gmt);
            }
        }
        catch
        {
        }
        return dt;
    }
    /// <summary>
    /// time0:当下的日子
    /// time1:被比较的日子
    /// </summary>
    /// <param name="time0"></param>
    /// <param name="time1"></param>
    /// <returns></returns>
    public static bool IsNewerHour(DateTime time0, DateTime time1)
    {
        bool isNewer = false;
        if (time0 > time1)
        {
            if (time0.Year > time1.Year)
                isNewer = true;
            if (time0.Month > time1.Month)
                isNewer = true;
            if (time0.Day > time1.Day)
                isNewer = true;
            if (time0.Hour > time1.Hour)
                isNewer = true;
        }
        return isNewer;
    }



}


