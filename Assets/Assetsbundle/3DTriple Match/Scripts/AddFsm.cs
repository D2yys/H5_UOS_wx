using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddFsm : MonoBehaviour
{
    public PlayMakerFSM Fsm;
    public bool down = false;
    public bool AddressablesBool= false;

    void Start()
    {
        Fsm = gameObject.AddComponent<PlayMakerFSM>();

        FsmTemplate a = Resources.Load("MoBan/Cube", typeof(FsmTemplate)) as FsmTemplate;
        //Fsm.SetFsmTemplate(a);
        //Fsm.FsmName = ("状态机");
        //down = true;
        //var numLives = FsmVariables.GlobalVariables.GetFsmObject("Template_Cube");

        //FsmTemplate a = (FsmTemplate)numLives.Value;
        Fsm.SetFsmTemplate(a);
        Fsm.FsmName = ("状态机");
        down = true;

    }
}
