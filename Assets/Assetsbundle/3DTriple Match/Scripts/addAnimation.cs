using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addAnimation : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Animator aaa = gameObject.AddComponent<Animator>();
       


        aaa.runtimeAnimatorController = Resources.Load("Animation/2", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
    }

  
}
