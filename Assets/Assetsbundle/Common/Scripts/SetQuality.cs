using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuality : MonoBehaviour
{
  
    // 排列顺序是0 1 2 3 4

 public void Di()//低品质  ID1
    {       

        QualitySettings.SetQualityLevel(1, true);
    }

    public void YiBan()//一般品质 ID2
    {
      
        QualitySettings.SetQualityLevel(2, true);
    }
    public void Gao()//高品质  ID3
    {
       
        QualitySettings.SetQualityLevel(3, true);
    }

}
