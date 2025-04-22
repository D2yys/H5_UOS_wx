using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class hclw : MonoBehaviour
{
    public List<AssetReference> m_Prefabs;//Ҫ���ص�Ԥ��
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
            
           character.LoadAssetAsync<GameObject>().Completed += OnPrefabsAsstLoaded; //Ԥ����Ԥ����
            //aa.LoadAssetAsync<GameObject>().Completed += OnPrefabsAsstLoaded;
            //loadMapOperation = character.LoadAssetAsync<GameObject>();
            
        }



    }

    //��������idʵ������֮��
    public void m_Instantiate_index(int index)
    {

        Array = m_Prefabs[index].InstantiateAsync().Result;
       
    }

   



    //��Դ����״̬��Count=0ʱ�������
    void OnPrefabsAsstLoaded(AsyncOperationHandle<GameObject> obj)
    {
        
        
        m_ToloadCount--;

        if (m_ToloadCount <= 0)
          m_AssetsReady = true;
          Fsm.SendEvent("loadok");
        

             
        

    }
 

    //��ʼ������UI
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

