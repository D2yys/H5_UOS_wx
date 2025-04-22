using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySendEvent : MonoBehaviour
{
    // 调用方法快速向状态机中发送事件
    public PlayMakerFSM MySql;
    public string EventName;
    public void DataUpdata()
    {

        MySql.SendEvent(EventName);

    }
  
}
