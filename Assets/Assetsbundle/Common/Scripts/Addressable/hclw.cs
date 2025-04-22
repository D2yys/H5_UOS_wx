using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class hclw : MonoBehaviour
{
    public List<AssetReference> m_Prefabs;//要加载的预制
    bool m_AssetsReady = false;
    int m_ToloadCount;
    int m_PrefabsIndex = 0;
    public GameObject Array;
    public PlayMakerFSM Fsm;
    
   

    public GameObject owner;
    public GameObject start;

    // public AssetReference player;
    void Awake()
    {
       
        Caching.ClearCache();
        Caching.compressionEnabled = false;
    }
    void Start()
    {
        
        m_ToloadCount = m_Prefabs.Count;
        foreach (var character in m_Prefabs)
        {
            
           character.LoadAssetAsync<GameObject>().Completed += OnPrefabsAsstLoaded; //预制体预加载
            //aa.LoadAssetAsync<GameObject>().Completed += OnPrefabsAsstLoaded;
            //loadMapOperation = character.LoadAssetAsync<GameObject>();
            
        }



    }

    //根据数组id实例化与之体
    public void m_Instantiate_index(int index)
    {

        Array = m_Prefabs[index].InstantiateAsync().Result;
       
    }

   



    //资源加载状态，Count=0时加载完成
    void OnPrefabsAsstLoaded(AsyncOperationHandle<GameObject> obj)
    {
        
        
        m_ToloadCount--;

        if (m_ToloadCount <= 0)
          m_AssetsReady = true;
          Fsm.SendEvent("loadok");
        

             
        

    }
 

    //初始化场景UI
    public void PlayInit()
    {

        Array = m_Prefabs[1].InstantiateAsync().Result;
        Array.transform.SetParent(owner.transform, false);

    }

    public void PlayStart()
    {
       
        Array = m_Prefabs[0].InstantiateAsync().Result;
        Array.transform.SetParent(owner.transform, false);

    }

    public void PlayOut()
    {
        Destroy(start);
        Array = m_Prefabs[3].InstantiateAsync().Result;
        Array.transform.SetParent(owner.transform, false);

    }

    public void PlayBad()
    {
        Destroy(start);
        Array = m_Prefabs[2].InstantiateAsync().Result;
        Array.transform.SetParent(owner.transform, false);

    }
    public void Playgood()
    {

        Array = m_Prefabs[1].InstantiateAsync().Result;
        Array.transform.SetParent(owner.transform, false);

    }
   
}

