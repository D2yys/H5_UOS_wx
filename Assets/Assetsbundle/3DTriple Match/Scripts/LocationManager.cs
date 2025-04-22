using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

/// <summary>
/// GPS位置管理器
/// </summary>
public class LocationManager : MonoBehaviour
{
    #region 单例
    public static LocationManager _Instance;
    public static LocationManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("LocationManager");
                _Instance = obj.AddComponent<LocationManager>();
                DontDestroyOnLoad(obj);
            }
            return _Instance;
        }
    }

    #endregion

    /// <summary>
    /// 超时时间
    /// 20秒
    /// </summary>
    private const float Time_Out = 20;

    /// <summary>
    /// 启动成功回调
    /// </summary>
    public event UnityAction<LocationInfo> SuccessCallback;

    /// <summary>
    /// 失败回调
    /// </summary>
    public event UnityAction<string> FailureCallback;

    /// <summary>
    /// 刷新回调
    /// </summary>
    public event UnityAction<LocationInfo> UpdateCallback;

    /// <summary>
    /// 停止回调
    /// </summary>
    public event UnityAction StoppedCallback;

    /// <summary>
    /// 是否开始
    /// </summary>
    private bool _IsStarted = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            OnUpdate(Input.location.lastData);
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            StartGPS();
        }
#endif
    }

    /// <summary>
    /// 申请权限
    /// </summary>
    public static void RequestPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string permission = UnityEngine.Android.Permission.FineLocation;
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission))
        {
            UnityEngine.Android.Permission.RequestUserPermission(permission);
        }
#endif
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        RequestPermission();
        StartGPS();
    }

    /// <summary>
    /// 开始定位
    /// </summary>
    private void StartGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            OnError("用户没有权限");
            return;
        }
        if (_IsStarted)
        {
            return;
        }
        _IsStarted = true;
        Input.location.Start();
        StartCoroutine(GetGPS());
    }
    private IEnumerator GetGPS()
    {
        float time = Time_Out;
        while (Input.location.status == LocationServiceStatus.Initializing && time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
        }

        if (time < 1)
        {
            OnError("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            OnError("Unable to determine device location");
            yield break;
        }
        else
        {
            OnStartSuccess(Input.location.lastData);
        }
    }

    /// <summary>
    /// 停止定位
    /// </summary>
    public static void StopGPS()
    {
        if (!_Instance)
        {
            return;
        }
        if (!_Instance._IsStarted)
        {
            return;
        }
        try
        {
            Input.location.Stop();
            _Instance._IsStarted = false;
        }
        catch (System.Exception)
        {
            throw;
        }
        finally
        {
            _Instance.OnStopped();
        }
    }

    /// <summary>
    /// 启动成功
    /// </summary>
    /// <param name="locationInfo"></param>
    private void OnStartSuccess(LocationInfo locationInfo)
    {
        Debug.Log("OnStartSuccess:" + locationInfo.latitude + " " + locationInfo.longitude + " " + locationInfo.altitude + " " + locationInfo.horizontalAccuracy + " " + locationInfo.timestamp);
        SuccessCallback?.Invoke(locationInfo);
    }

    /// <summary>
    /// 发生错误
    /// </summary>
    private void OnError(string errorInfo)
    {
        Debug.Log("OnError:" + errorInfo);
        FailureCallback?.Invoke(errorInfo);
        StopGPS();
    }

    /// <summary>
    /// 更新位置
    /// </summary>
    /// <param name="locationInfo"></param>
    private void OnUpdate(LocationInfo locationInfo)
    {
        UpdateCallback?.Invoke(locationInfo);
    }

    /// <summary>
    /// 停止
    /// </summary>
    private void OnStopped()
    {
        Debug.Log("OnStopped:");
        StoppedCallback?.Invoke();

        if (gameObject)
        {
            Destroy(gameObject);
            _Instance = null;
        }
    }
}

