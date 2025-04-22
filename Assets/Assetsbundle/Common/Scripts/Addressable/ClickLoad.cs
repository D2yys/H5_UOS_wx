using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
//单独的预制体加载
public class ClickLoad : MonoBehaviour
{
    public string PrefabName;
    public PlayMakerFSM Fsm;
    public GameObject GameObject;
 
    AsyncOperationHandle<GameObject> ggg;



    //根据传的参数判断是否释放资源和跳转
    public void LoadPrefab(string PrefabName, bool GetOK, bool Releas, string SendName)
    {
        Addressables.LoadAssetAsync<GameObject>(PrefabName).Completed += (hal) =>
                {
                    if (GameObject!= null)
                    {
                       // Destroy(GameObject);
                        smdx();
                    }

                    if (hal.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject = hal.Result;
                        ggg = hal;
                    }

                    if (Releas == true)
                    {
                        //Addressables.Release(hal);
                    }
                    if (GetOK == true)
                    {
                        Fsm.SendEvent(SendName);
                    }

                };

    }
    public void smdx()
    {
        Addressables.Release(ggg);

    }
}
