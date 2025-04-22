using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class jsontest : MonoBehaviour
{
    // Start is called before the first frame update
    public TextAsset a;
    public Texture ttt;
    public string key;
    public GameObject obj;
    
    public PlayMakerFSM t;
 
   
    public void load()

    {
        Addressables.LoadAssetAsync<GameObject>(key).Completed += (hal) =>
        {


            obj = hal.Result;
           

            t.SendEvent("GetOk");
           // Addressables.Release(obj);

        };
    }

    public void ShiFang() {
        Resources.UnloadUnusedAssets();

        //  Addressables.Release(obj);
    }


}
    // Update is called once per frame
 

