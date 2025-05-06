//    (c) Jean Fabre, 2011-2015 All rights reserved.
//    http://www.fabrejean.net

// INSTRUCTIONS
// Drop a PlayMakerHashTableProxy script onto a GameObject, and define a unique name for reference if several PlayMakerHashTableProxy coexists on that GameObject.
// In this Action interface, link that GameObject in "hashTableObject" and input the reference name if defined. 
// Note: You can directly reference that GameObject or store it in an Fsm variable or global Fsm variable

using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.UOS.CloudSave.Model.Files;
using Unity.UOS.CloudSave;
using Unity.UOS.CloudSave.Exception;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Easy Save 2")]
    [Tooltip("Loads a PlayMaker HashTable Proxy component using EasySave")]
    public class HashTableEasyLoad : HashTableActions
    {

        [ActionSection("Set up")]

        [RequiredField]
        [Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
        [CheckForComponent(typeof(PlayMakerHashTableProxy))]
        public FsmOwnerDefault gameObject;

        [Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
        [UIHint(UIHint.FsmString)]
        public FsmString reference;

        [ActionSection("Easy Save Set Up")]

        [Tooltip("A unique tag for this save. For example, the object's name if no other objects use the same name. Leave to none or empty, to use the GameObject Name + Fsm Name + array Reference as tag.")]
        public FsmString uniqueTag = "";

        [RequiredField]
        [Tooltip("The name of the file that we'll create to store our data.")]
        public FsmString saveFile = "defaultES2File.txt";

        [Tooltip("Whether the data we are loading is stored in the Resources folder.")]
        public FsmBool loadFromResources;


        public FsmEvent JsonNull;
        public FsmEvent End;     //注意等待回调的脚本不能用默认Finish动作  ，代码执行完成未回调成功会直接跳过
        public FsmBool cloud;   //不勾选就是本地数据加载，勾选是云数据加载


        string json = "null";

        public class ReqData
        {
            public int coin;
            public int tili;

        }
        public override void Reset()
        {
            gameObject = null;
            reference = null;
            JsonNull = null;
            End = null;

            uniqueTag = new FsmString() { UseVariable = true };

            saveFile = "defaultES2File.txt";

            loadFromResources = false;
            cloud = false;
        }

        public override void OnEnter()
        {

            if (SetUpHashTableProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
                LoadHashTable();

            Finish();
        }


        public async void LoadHashTable()
        {
            if (!isProxyValid())
                return;

            string _tag = uniqueTag.Value;

            if (string.IsNullOrEmpty(_tag))
            {
                _tag = Fsm.GameObjectName + "/" + Fsm.Name + "/hashTable/" + reference;
            }

            ES2Settings loadSettings = new ES2Settings();

            if (loadFromResources.Value)
            {
                loadSettings.saveLocation = ES2Settings.SaveLocation.Resources;
            }

            //本地数据加载
            if (cloud.Value == false)
            {

                Dictionary<string, string> _dict = ES2.LoadDictionary<string, string>(saveFile.Value + "?tag=" + _tag);

                string json = JsonConvert.SerializeObject(_dict);

                proxy.hashTable.Clear();

                foreach (string key in _dict.Keys)
                {
                    // Debug.Log(_dict[key]);
                    proxy.hashTable[key] = PlayMakerUtils.ParseValueFromString(_dict[key]);
                }

                Fsm.Event(End);
            }
            else //云数据
            {
                UosGet();

            }

        }


        public async void UosGet()
        {
            // 获取指定命名空间和角色下单存档的元数据信息，为空表示该角色在此命名空间下无单存档
            string targetNamespace = "data"; // 存档的命名空间
            try
            {
                SaveItem saveItem = await CloudSaveSDK.Instance.Files.GetLinearAsync(targetNamespace);
                             
               
                if (saveItem != null) //有数据
                {                    
                    FsmVariables.GlobalVariables.GetFsmString("Data_Saveid").Value = saveItem.SaveId;  //得到存储文件的id

                    Dictionary<string, string> uosdic = saveItem.Properties;

                    foreach (string key in uosdic.Keys)
                    {
                        //Debug.Log(_dict_new[key]);
                        proxy.hashTable[key] = PlayMakerUtils.ParseValueFromString(uosdic[key]);
                    }

                    Fsm.Event(End);
                    Debug.Log("服务器有数据");

                }
                else //没有数据
                {

                    // 对指定命名空间和角色下的单存档进行存档操作。若之前无单存档，会创建一个新的单存档。成功后返回存档Id

                    UpdateOptions data = new UpdateOptions();
                    data.Name = "data";                   
                    string _tag = uniqueTag.Value;

                    if (string.IsNullOrEmpty(_tag))
                    {
                        _tag = Fsm.GameObjectName + "/" + Fsm.Name + "/hashTable/" + reference;
                    }
                    Dictionary<string, string> _dict = ES2.LoadDictionary<string, string>(saveFile.Value + "?tag=" + _tag);
                    data.Properties = _dict;  //数据

                    try
                    {
                        Debug.Log("尝试UOS数据上传");
                        string saveId = await CloudSaveSDK.Instance.Files.SaveLinearAsync("data", data);

                        FsmVariables.GlobalVariables.GetFsmString("Data_Saveid").Value = saveId;

                        //Debug.Log("-------保存的id是：：" +saveId);
                    }
                    catch (CloudSaveClientException e)
                    {
                        Debug.LogErrorFormat("failed to create or update linear save, clientEx: {0}", e);
                        throw;
                    }
                    catch (CloudSaveServerException e)
                    {
                        Debug.LogErrorFormat("failed to create or update linear save, serverEx: {0}", e);
                        throw;
                    }

                    Fsm.Event(JsonNull);
                }

                // LoadEnd();


            }
            catch (CloudSaveClientException e)
            {
                Debug.LogErrorFormat("failed to get metadata of linear save, clientEx: {0}", e);
                throw;
            }
            catch (CloudSaveServerException e)
            {
                Debug.LogErrorFormat("failed to get metadata of linear save, serverEx: {0}", e);
                throw;
            }

        }
    }

}
