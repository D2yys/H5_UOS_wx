using UnityEngine;
using System.Collections;

//--------------------------------用于物体跟随鼠标运动
public class mmbb : MonoBehaviour
{
    Canvas canvas;
    RectTransform rectTransform;
    Vector2 pos;
    Camera _camera;
    bool state = false;
    RectTransform canvasRectTransform;
    void Start()
    {
        rectTransform = transform as RectTransform;
        canvas = GameObject.Find("Canvas(Dialog)c3").GetComponent<Canvas>();
        _camera = canvas.GetComponent<Camera>();
        canvasRectTransform = canvas.transform as RectTransform;
        Debug.Log(canvas.renderMode);
    }
    void Update()
    {


        FollowMouseMove();
    }
    public void FollowMouseMove()
    {
        //worldCamera:1.screenSpace-Camera 
        // canvas.GetComponent<Camera>() 1.ScreenSpace -Overlay 
        if (RenderMode.ScreenSpaceCamera == canvas.renderMode)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, canvas.worldCamera, out pos);
        }
        else if (RenderMode.ScreenSpaceOverlay == canvas.renderMode)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, _camera, out pos);


        }
        else
        {
            Debug.Log("请选择正确的相机模式!");
        }
        rectTransform.anchoredPosition = pos;

        //或者

        //transform.localPosition = new Vector3(pos.x, pos.y, 0);
    }
}