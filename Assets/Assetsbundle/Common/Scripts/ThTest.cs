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
        char[] delimiter = new char[] { '/' }; // ����ָ��������Ķ�����Ҫʹ��unicode����
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
    //    myArray[myArray.Length - 1] = 4; // ���Ԫ��4
    //}
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unixʱ����Ǵ�1970-01-01 00:00:00��ʼ�������������
        // �˴�����Ϊ��λ������Ǻ��������1000
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }


}
