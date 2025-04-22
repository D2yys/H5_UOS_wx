using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTime : MonoBehaviour
{

    [SerializeField] Text text;
 
    public PlayMakerFSM Fsm;
    public DateTime EndTime;
    int pdbool;  //�״�����bool
    public int TimeHours;//����ʱ��Сʱ����
    public bool TimeEnd;
    TimeSpan span;
    public string Savekey;
    public string EndString; //���ս���ʱ��

     void Awake()
    {
        PlayTime();
    }

    public void PlayTime()//��ʾʱ���� ȡֵ��Ĭ��ֵ���ж��ǵ�һ��ȡֵ    bool 0���״Σ�1�����״�
    {

        if (PlayerPrefs.GetString(Savekey, EndString) == "" && PlayerPrefs.GetInt(Savekey, pdbool) == 0)
        {
            PlayerPrefs.SetInt(Savekey, 1);
            DateTime a = DateTime.UtcNow.AddHours(+8).AddHours(TimeHours);//ǰʱ��+24Сʱ
            EndString = a.ToString();
            PlayerPrefs.SetString(Savekey, EndString); //�������ʱ��
            span = DateTime.UtcNow.AddHours(+8).Subtract(DateTime.Parse(EndString)).Duration(); //�����ʱ��
           
            InvokeRepeating("ShopDjs", 0, 1);
        }
        else
        {
            EndString = PlayerPrefs.GetString(Savekey, "defaultValue");
            DateTime nowTime = DateTime.UtcNow.AddHours(+8);
             span = nowTime.Subtract(DateTime.Parse(EndString)).Duration(); //�����ʱ��
                                                                            //
            if (span.TotalSeconds > 0)
            {
                InvokeRepeating("ShopDjs", 0, 1);
            }
            else
            {
                TimeEnd = true;
                Fsm.SendEvent("End");
               
            }

        }
    }

    public void Stop()
    {
        CancelInvoke("ShopDjs");
        
    }


    private void ShopDjs()
    {      

        span = DateTime.UtcNow.AddHours(+8).Subtract(DateTime.Parse(EndString)).Duration();
        string sj = string.Format("{0:d2}:{1:d2}:{2:d2}", span.Hours, span.Minutes, span.Seconds);
        text.text = "���ʧЧʱ��" + sj;     
        if (span.TotalSeconds <= 0)
        {
            Stop();
            TimeEnd = true;
            Fsm.SendEvent("End");
        }
    }
}
