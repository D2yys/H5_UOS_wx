using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ThTest : MonoBehaviour
{

    public int[] myArray;

    DateTime today;



    void Start()
    {
        string inputString = "0/0/0/0/0/0/0/0/0/0";
        char[] delimiter = new char[] { '/' }; // 定义分隔符，中文逗号需要使用unicode编码
        string[] stringArray = inputString.Split(delimiter);

        foreach (string str in stringArray)
        {
            Debug.Log(str);
        }
    }

    //private void Start()
    //{
    //    today = UnixTimeStampToDateTime(1711728415225/ 1000);

    //    Debug.Log(today);
    //}

    //private void Start()
    //{
    //    Array.Resize(ref myArray, myArray.Length + 1);
    //    myArray[myArray.Length - 1] = 4; // 添加元素4
    //}
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix时间戳是从1970-01-01 00:00:00开始的秒数或毫秒数
        // 此处以秒为单位，如果是毫秒则除以1000
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }


}
