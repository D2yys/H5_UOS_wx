using UnityEngine;

public class PlayerState : MonoBehaviour
{
   
    public class ReqData
    {
        public string sentence;
        public bool denoise;
    }
    public void goon() {

    ReqData data = new ReqData();   //实例化

    data.sentence = "Hello World";    //赋值
    data.denoise = true;

        //将数据转换为json字符串
        var jsonstring = JsonUtility.ToJson(data);
    }
}