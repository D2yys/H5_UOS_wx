using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IsNotWebGl : MonoBehaviour
{
    // Start is called before the first frame update
    public string FsmName;
    void Start()
    {

        if (Application.platform == RuntimePlatform.WindowsEditor)

        {
            GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == FsmName).SendEvent("UnityEditor");

        }
        else {
            GetComponents<PlayMakerFSM>().First(fsm => fsm.Fsm.Name == FsmName).SendEvent("WebGl");
        }
      
    }
}

    
 
