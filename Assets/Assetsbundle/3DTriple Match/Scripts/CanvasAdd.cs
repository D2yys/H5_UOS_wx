using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAdd : MonoBehaviour
{


    Transform[] allChild;

    int add = 10;
    
    public GameObject Dialog;

    public void CavasAdd(int count)
    {
        if (Dialog == null) {
            Dialog = this.gameObject;
        }
        Transform parent = Dialog.transform.parent;
        Canvas Dc = Dialog.GetComponent<Canvas>();
        if (Dc == null) { 
          Dc = Dialog.AddComponent<Canvas>();
        }

        GraphicRaycaster Grap = Dialog.AddComponent<GraphicRaycaster>();


        allChild = Dialog.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChild)
        {
            Canvas Commont = child.GetComponent<Canvas>();
            if (Commont != null)
            {
                
                Commont.overrideSorting = true; 
                
                Commont.sortingOrder += add + count;

                Debug.Log(child.name);
            }
            
            SpriteRenderer SpRender = child.GetComponent<SpriteRenderer>();
            if (SpRender != null)
            {
                SpRender.sortingOrder += add + count + 1;
            }
            ParticleSystemRenderer lizi = child.GetComponent<ParticleSystemRenderer>();
            if (lizi != null)
            {
                lizi.sortingOrder += add + count;

            }

        }

    }
}
