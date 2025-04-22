using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ZBT : MonoBehaviour
{
    public List<AssetReference> m_Prefabs;//要加载的预制
    bool m_AssetsReady = false;
    int m_ToloadCount; 

    public GameObject Array;
    public PlayMakerFSM Fsm;

    // public AssetReference player;
    void Start()
    {
        m_ToloadCount = m_Prefabs.Count;
        foreach (var character in m_Prefabs)
        {

            character.LoadAssetAsync<GameObject>().Completed += OnPrefabsAsstLoaded; //预制体预加载


        }



    }

    //加载完成
    public void m_Instantiate_index(int index)
    {


        Array = m_Prefabs[index].InstantiateAsync().Result;
        Fsm.SendEvent("Init");
    }






    void OnPrefabsAsstLoaded(AsyncOperationHandle<GameObject> obj)
    {
        //Array = obj.Result;

        m_ToloadCount--;
        if (m_ToloadCount <= 0)
            m_AssetsReady = true;
        m_Instantiate_index(0);

    }

}
