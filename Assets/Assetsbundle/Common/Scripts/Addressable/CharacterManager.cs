using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//批量加载资源，加载完成后才可以进行下一步操作！！！
public class CharacterManager : MonoBehaviour
{
    //public GameObject m_archerObject;
    public List<AssetReference> m_Characters;
    bool m_AssetReady = false;
    int m_ToLoadCount;

    // Start is called before the first frame update
    void Start()
    {
        //ToloadCount 表数量，表数量为0时所有资源加载完成
        m_ToLoadCount = m_Characters.Count;

        //将表中的所有资源依次加载，加载完成后点击按钮调用方法 SpawnCharacter 在执行实例化，  这
        foreach (var character in m_Characters)
        {
            character.LoadAssetAsync<GameObject>().Completed += OnCharacterAssetLoaded;
        }
    }
    //根据characterTyp 这个int值，来决定实例化的物体的id。 按钮调用方法的时候可以在下面设置chaaracterType的值也就是id数
    public void SpawnCharacter(int characterType)
    {
        //Instantiate(m_archerObject);
        //m_archerObject.LoadAssetAsync<GameObject>();
        if (m_AssetReady)
        {
            Vector3 position = Random.insideUnitSphere * 5;
            position.Set(position.x, 0, position.z);
            m_Characters[characterType].InstantiateAsync(position, Quaternion.identity);
        }
        
    }

    void OnCharacterAssetLoaded(AsyncOperationHandle<GameObject> obj)
    {
        m_ToLoadCount--;
        if(m_ToLoadCount <= 0)
        {
            m_AssetReady = true;
        }
        
    }

}
