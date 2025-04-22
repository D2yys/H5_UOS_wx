using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameLobby : MonoBehaviour
{
    public List<AssetReference> m_Prefabs;//要加载的预制
    bool m_AssetsReady = false;
    int m_ToloadCount;
    int m_PrefabsIndex = 0;

    public GameObject Array;
    public PlayMakerFSM Fsm;

    // public AssetReference player;

    //加载预制体不实例化，通过调用方法实例化预制体
    void Awake()
    {

        Caching.ClearCache();
        Caching.compressionEnabled = false;
        kaishi();
    }

    public void  kaishi()
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
        Fsm.SendEvent("LoadOK");
    }

    //实例化预制体，第一个
    public void InstantiatePrefab()
    {
        Array = m_Prefabs[0].InstantiateAsync().Result;
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
