﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 打印FPS
/// </summary>
public class FPSx : MonoBehaviour
{
    float _updateInterval = 1f;//设定更新帧率的时间间隔为1秒  
    float _accum = .0f;//累积时间  
    int _frames = 0;//在_updateInterval时间内运行了多少帧  
    float _timeLeft;
    string fpsFormat;
    public Text ttt;
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR//安卓模式帧率默认最高帧率
Application.targetFrameRate = 60;
#else
        //编辑器模式
        Application.targetFrameRate = -1;
#endif
        _timeLeft = _updateInterval;
    }

    //void OnGUI()
    //{
    //    GUI.Label(new Rect(100, 100, 200, 200), fpsFormat);
    //}

    void Update()
    {
        _timeLeft -= Time.deltaTime;
        //Time.timeScale可以控制Update 和LateUpdate 的执行速度,  
        //Time.deltaTime是以秒计算，完成最后一帧的时间  
        //相除即可得到相应的一帧所用的时间  
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;//帧数  

        if (_timeLeft <= 0)
        {
            float fps = _accum / _frames;
            //Debug.Log(_accum + "__" + _frames);  
            fpsFormat = System.String.Format("{0:F2}FPS", fps);//保留两位小数  
            
            ttt.text = fpsFormat;
            _timeLeft = _updateInterval;
            _accum = .0f;
            _frames = 0;
        }
    }
}
