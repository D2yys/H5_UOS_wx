using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class CgwellAddMask : MonoBehaviour
{
    Material _material = null;
   
    public Vector2 Main_Tiling = new Vector2(1f, 1f);
    public Vector2 Main_Offset = new Vector2(0, 0);
   
    private Image _image;
    

    // Start is called before the first frame update
    void Start()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }
        SetMaterialState();
    }

    void SetMaterialState()
    {
        if(_image == null) return;

        _material = _image.material;     
        _material.SetTextureScale("_MainTex",Main_Tiling);
        _material.SetTextureOffset("_MainTex",Main_Offset);
      
    }

    // Update is called once per frame
    void Update()
    {
        if(_image != null && _image.material != null)
            SetMaterialState();
    }
    
    void OnDestroy()
    {
        if (_material != null)
        {
            DestroyImmediate(_material);
        }
    }
}
