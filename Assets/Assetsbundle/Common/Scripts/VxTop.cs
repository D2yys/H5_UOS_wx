//using LitJson;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Networking;
//using WeChatWASM;


//public class VxTop : MonoBehaviour
//{
  
   
//    //�ƺ�������
//    public string YhsName;
//    //�¼���
//    public string EventName;
//    string Json; 
//    public string[] List_name;
//    public string[] List_level;
//    public string[] List_iconid;
//    public string[] List_iconKuang;
//    public string[] List_zdeIZInt;
//    public string[] List_zdeIZKLL;

//    public Texture[] List_Hade;
//    public string[] List_openid;   //�����ж��Լ�������û����1000����
//                                   //
//    public Texture nullTexture; //Ĭ��ͷ��

//    public string[] ListUrl;

//    public class ReqData
//    {
//        public int ranking;
//    }



//    //ȡ����
//    public void GetTopData(string HanShuName)
//    {

//        Debug.Log("����΢�Ż�ȡ�������ݺ���");

//        WXBase.cloud.Init(new CallFunctionInitParam()
//        {
//            env = "d2yy-7gsz0lyd20634ff3",

//            traceUser = false
//        });


//        ReqData P = new ReqData();

//        P.ranking = 100;



//        WX.cloud.CallFunction(new CallFunctionParam()
//        {
//            name = HanShuName,
            

//            data = JsonUtility.ToJson(P),


//            success = (res) =>
//            {
                 
//                Json = res.result;

//                var data = JsonMapper.ToObject(res.result);

               
//                if (data.ContainsKey("data"))
//                {                               

//                }
//                //���ݻ�ȡ����   
//                else
//                {
                  

//                }

//            },

//            //�����ƺ���ʧ��

//            fail = (res) =>
//            {

              

//            },
//            //ȫ������
//            complete = (res) =>
//            {
              

//                LoadEnd();
//            }

//        });;

//    }
//    private void LoadEnd()
//    {


//        JObject jsonData = JObject.Parse(Json);        
//        JArray Ja = (JArray)jsonData["data"];

//        List_name = new string[0];
//        List_level = new string[0];
//        List_iconid = new string[0];
//        List_openid = new string[0];
//        List_Hade = new Texture[0];
//        ListUrl = new string[0];
//        List_iconKuang = new string[0];
//        List_zdeIZInt = new string[0];
//        List_zdeIZKLL = new string[0];
//        for (int i = 0; i < Ja.Count; i++)
//        {

//            Array.Resize(ref List_iconid, List_iconid.Length + 1);
//            List_iconid[List_iconid.Length - 1] = ((string)jsonData["data"][i]["gamedata"]["Iconid"]).Replace("int", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty); ;

//            Array.Resize(ref List_iconKuang, List_iconKuang.Length + 1);
//            List_iconKuang[List_iconKuang.Length - 1] = ((string)jsonData["data"][i]["gamedata"]["IconKuang"]).Replace("int", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

//            Array.Resize(ref List_openid, List_openid.Length + 1);
//            List_openid[List_openid.Length - 1] = (string)jsonData["data"][i]["openid"];
          
//            Array.Resize(ref List_name, List_name.Length + 1);
//            List_name[List_name.Length - 1] = ((string)jsonData["data"][i]["gamedata"]["name"]).Replace("string", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

//            Array.Resize(ref List_level, List_level.Length + 1);
//            List_level[List_level.Length - 1] = ((string)jsonData["data"][i]["gamedata"]["level"]).Replace("int", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

//            Array.Resize(ref ListUrl, ListUrl.Length + 1);
//            ListUrl[ListUrl.Length - 1] = ((string)jsonData["data"][i]["gamedata"]["Data_HaidUrl"]).Replace("string", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

//            Array.Resize(ref List_zdeIZInt, List_zdeIZInt.Length + 1);
//            List_zdeIZInt[List_zdeIZInt.Length - 1] = ((string)jsonData["data"][i]["gamedata"]["Data_zdeIZInt"]).Replace("int", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

//            Array.Resize(ref List_zdeIZKLL, List_zdeIZKLL.Length + 1);
//            List_zdeIZKLL[List_zdeIZKLL.Length - 1] = ((string)jsonData["data"][i]["gamedata"]["Data_zdeIZKLL"]).Replace("int", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

//        }
       
//        StartCoroutine(LoadImages());
       
        

//    }


//    IEnumerator LoadImages()
//    {

//        foreach (var url in ListUrl)
//        {
//            if (url.Length < 10)
//            {
//                Array.Resize(ref List_Hade, List_Hade.Length + 1);
//                List_Hade[List_Hade.Length - 1] = nullTexture;
                
//                continue;
//            }
                    
           
//            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
//            yield return uwr.SendWebRequest(); // �ȴ��������

//            if (uwr.result != UnityWebRequest.Result.Success)
//            {
              
//                Array.Resize(ref List_Hade, List_Hade.Length + 1);
//                List_Hade[List_Hade.Length - 1] = nullTexture;
//                continue; // �����д����ͼƬ
//            }
//            else
//            {             
//                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
               
//                Array.Resize(ref List_Hade, List_Hade.Length + 1);
//                List_Hade[List_Hade.Length - 1] = texture;

                
//            }
//        }
//        GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == "Top").SendEvent(EventName);
//    }
//}


