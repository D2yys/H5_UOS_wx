using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using UnityEngine.UI;
//using WeChatWASM;

public class SceneLoad : MonoBehaviour
{
    public string SceneName;
    public Text Test;
    public Scrollbar bar;
   
  
    public void StartLoad()
    {
        Test.text = "开始加载";
        StartCoroutine(LoadScene());
        
    }

    IEnumerator LoadScene()
    {

        var handle = Addressables.LoadSceneAsync(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, false);

        if (handle.Status == AsyncOperationStatus.Failed)
        {
            //加载失败
            Debug.LogError("场景加载异常：" + handle.OperationException.ToString());
            Test.text = ("场景加载失败请检查网络");
            yield break;

        }
        while (!handle.IsDone)
        {
           
            float bfb = handle.GetDownloadStatus().Percent*100;
            int intbfb = (int)bfb;
           
            Test.text = intbfb.ToString()+"%";     
            
            bar.size = handle.GetDownloadStatus().Percent;
            yield return null;
        }
        bar.size = 0.9f;
        Test.text = ("初始化中");
        Debug.Log("场景加载完成开始激活");
       
        handle.Result.ActivateAsync(); //场景激活
    }
    //清除本地缓存
    public void QingHuanCun()
    {

       // WX.CleanAllFileCache();

    }

}
