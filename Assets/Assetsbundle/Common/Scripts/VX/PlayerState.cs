using UnityEngine;

public class PlayerState : MonoBehaviour
{
   
    public class ReqData
    {
        public string sentence;
        public bool denoise;
    }
    public void goon() {

    ReqData data = new ReqData();   //ʵ����

    data.sentence = "Hello World";    //��ֵ
    data.denoise = true;

        //������ת��Ϊjson�ַ���
        var jsonstring = JsonUtility.ToJson(data);
    }
}