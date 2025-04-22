using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeBox : MonoBehaviour
{

    [SerializeField] Text text;
    public int second;
    public PlayMakerFSM Fsm;

 
    public void Stat()//显示分秒
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
        second = second - 1;    
               
        text.text = string.Format("{0:d2}:{1:d2}", (int)second / 60, (int)second % 60);
              
            if (second <= 0)
            {
               
                Fsm.SendEvent("End");
                Stop();
            }
        
    }

}
