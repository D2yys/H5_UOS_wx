using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddresableLodName : MonoBehaviour
{
    
    //根据标签和名称来加载对应的资源   Key 是名称 Lable 是标签

    public string Key;
    public string Lable_1;
    public string Lable_2;
    public Texture Texture;
    public PlayMakerFSM Fsm;
    public GameObject Clillder;
    

    public void LoadTextureName()
    {
        LoadTextureName(Key, Lable_1);

    }
    public void loadcollider()
    {
        LoadColliderName(Key, Lable_2);
    }

    void LoadTextureName(string key, string label)
    {
        Addressables.LoadAssetsAsync<Texture2D>(new List<object> { key, label }, null, Addressables.MergeMode.Intersection).Completed += TextureLoaded;
    }

    //加载完成
    void TextureLoaded(AsyncOperationHandle<IList<Texture2D>> obj)
    {
        Texture = obj.Result[0];
        loadcollider();

    }



    void LoadColliderName(string key, string label)
    {
        Addressables.LoadAssetsAsync<GameObject>(new List<object> { key, label }, null, Addressables.MergeMode.Intersection).Completed += ColliderLoaded;
    }

    void ColliderLoaded(AsyncOperationHandle<IList<GameObject>> obj)
    {
        Clillder = obj.Result[0];
        Fsm.SendEvent("Textureloadok");

    }

    //  脚本有问题，要设置加载玩贴图在加载碰撞体的操作！

}
