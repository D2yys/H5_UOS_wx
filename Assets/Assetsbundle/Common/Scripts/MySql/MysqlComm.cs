using HutongGames.PlayMaker;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MysqlComm : MonoBehaviour
{

    public PlayMakerFSM Fsm;
    public PlayMakerFSM FSM_TiliTime;

    public Text text;
    public Text Tlilitext;
    public int second;

    //倒计时的总秒数
    int DJStime = 1800;
    public DateTime lasttime;  
    //上次登录时的时间

    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR//安卓模式帧率默认最高帧率
Application.targetFrameRate = 60;
#else
        //编辑器模式
        Application.targetFrameRate = -1;
#endif
    }


    //--------------------------------------------------------计算离线时长兑换相应得体力---------------------------------------------------------------
    public void lastTime()
    {
        var numLives = FsmVariables.GlobalVariables.GetFsmInt("Data_tili");
        int tili = numLives.Value;

        //当前时间减去计时开始的时间得到总时长秒数，总时长秒数除以体力计时的时间得到体力数，
        var lastime = FsmVariables.GlobalVariables.GetFsmString("Data_Lasttime");
        string a = lastime.Value;

        double tilishu;
        DateTime nowTime = DateTime.UtcNow.AddHours(+8);

        TimeSpan span = nowTime.Subtract(DateTime.Parse(a)).Duration();

        // 计算体力数
        tilishu = span.TotalSeconds / 1800;



        if (tilishu + tili > 5)
        {

            {

                FsmVariables.GlobalVariables.GetFsmInt("Data_tili").Value = 5;

            }
        }

        else
        {
            //离线时间很短不足30分钟，得到总离线秒数，用1800减去，得到剩余的秒数   将剩余的秒数倒计时
            if (tilishu < 1)
            {
                //倒计时总秒数（60 或1800）减去 间隔的秒数（这个时间肯定是小于倒计时总秒数的 因为体力数小于1）  = 得到剩余的计时加体力秒数，然后用这个秒数取倒计时

                second = DJStime - (int)span.TotalSeconds;
                Timer();

            }

            //离线总时长得到的体力 + 已有体力小于 5 更新体力数量,并且用总时长的余数进行倒计时
            else
            {

                second = (int)span.TotalSeconds % DJStime;
                Timer();

                //得到总体力数
                FsmVariables.GlobalVariables.GetFsmInt("Data_tili").Value += (int)tilishu;

            }

        }
    }

    //倒计时
    public void Timer()
    {
        TimerStop();
        InvokeRepeating("daojishi", 1, 1);
    }

    //停止倒计时
    public void TimerStop()
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
            // 取消调用
            CancelInvoke("daojishi");

            FsmVariables.GlobalVariables.GetFsmInt("Data_tili").Value += 1;
            var numLives = FsmVariables.GlobalVariables.GetFsmInt("Data_tili");
            int tili = numLives.Value;
            Tlilitext.text = tili.ToString();

            if (tili >= 5)
            {
                FSM_TiliTime.SendEvent("Full");
                return;
            }
            else
            {
                //更新计时起始时间从新倒计时
                FSM_TiliTime.SendEvent("Updata");
                TiliTime();
                lastTime();
            }

        }

    }


    //添加当前时间，用于体力计算
    public void TiliTime()
    {

        DateTime nowtime = DateTime.UtcNow.AddHours(+8);
        lasttime = nowtime;       
        FsmVariables.GlobalVariables.GetFsmString("Data_Lasttime").Value = lasttime.ToString(); 

    }
}



