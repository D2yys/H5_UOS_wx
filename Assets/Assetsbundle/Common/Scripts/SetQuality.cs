using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuality : MonoBehaviour
{
  
    // ����˳����0 1 2 3 4

 public void Di()//��Ʒ��  ID1
    {       

        QualitySettings.SetQualityLevel(1, true);
    }

    public void YiBan()//һ��Ʒ�� ID2
    {
      
        QualitySettings.SetQualityLevel(2, true);
    }
    public void Gao()//��Ʒ��  ID3
    {
       
        QualitySettings.SetQualityLevel(3, true);
    }

}
