using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private DateTime lastCheckedDate; //上次刷新日数据的日期

    private DateTime lastMonthUtc; //上次刷新月数据的日期

    TimeSpan NowOffestTime = TimeSpan.Zero;

    private void Start()
    {
        getServerTime();
        lastCheckedDate = DateTime.Parse(PlayerPrefs.GetString("LASTCHECKDATE", DateTime.MinValue.Date.ToString())); //DateTime.UtcNow.Date;
        lastMonthUtc = DateTime.Parse(PlayerPrefs.GetString("LASTMONTHDATE", DateTime.MinValue.Date.ToString()));

        //CheckAndUpdateData();
    }

    private void CheckAndUpdateData()
    {
        DateTime currentDate = GetNowTime();//DateTime.UtcNow.Date;

        if (currentDate > lastCheckedDate)
        {
            PlayerPrefs.SetString("LASTCHECKDATE", currentDate.ToString());
            UpdateDailyData();
            lastCheckedDate = currentDate;
        }

        if (currentDate.Month > lastMonthUtc.Month || currentDate.Year > lastMonthUtc.Year) //每月刷新
        {
            PlayerPrefs.SetString("LASTMONTHDATE", currentDate.ToString());
            UpdateMonthData();
            lastMonthUtc = currentDate;
        }
    }

    private void UpdateDailyData()
    {
        //在这里写需要每日刷新的数据
    }

    private void UpdateMonthData()
    {
        //在这里写需要每月刷新的数据

    }

    /// <summary>
    /// 获取实际的网络时间 切系统时间也没用
    /// </summary>
    /// <returns></returns>
    public DateTime GetNowTime()
    {
        return DateTime.Now - NowOffestTime;
    }
    //获得服务器时间
    public void getServerTime()
    {
        string url = "https://baidu.com";
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
            Debug.Log("·······------当前today是:" + Gmt);

            if (IsNewerHour(now, Gmt))
            {
                //记录一下时间差 这就是用户手动改的时间与世界时间的间隔
                //之后调用GetNowTime()就是准确时间
                NowOffestTime = now - Gmt;
               DateTime today = DateTime.Now - NowOffestTime;
                
                CheckAndUpdateData();
            }
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
