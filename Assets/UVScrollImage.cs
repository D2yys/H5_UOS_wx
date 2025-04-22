using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间

public class UVScrollImage : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // 滚动速度
    public float ySpeed = 0.1f;
    private Material material; // 图片材质
    private Vector2 textureOffset = Vector2.zero; // UV偏移量
    private Vector2 textureSize; // 纹理大小（用于计算UV坐标）
    private Rect rect; // 图片的RectTransform的Rect属性

    void Start()
    {
        material = GetComponent<Image>().material; // 获取Image组件的材质
        textureSize = material.mainTexture.texelSize; // 获取纹理大小（1个像素占多少UV单位）
        rect = GetComponent<RectTransform>().rect; // 获取RectTransform的Rect属性
    }

    void Update()
    {
        textureOffset.y += scrollSpeed * Time.deltaTime; // 沿Y轴滚动（可改为X轴）
        textureOffset.x += ySpeed * Time.deltaTime;
        if (textureOffset.y > 1) // 当偏移量大于1时重置，实现循环滚动效果
            textureOffset.y = 0;
        if(textureOffset.x > 1)
            textureOffset.x = 0;

        material.SetTextureOffset("_MainTex", textureOffset); // 应用偏移量到材质的UV坐标上
    }
}