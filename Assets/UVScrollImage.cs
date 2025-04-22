using UnityEngine;
using UnityEngine.UI; // ����UI�����ռ�

public class UVScrollImage : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // �����ٶ�
    public float ySpeed = 0.1f;
    private Material material; // ͼƬ����
    private Vector2 textureOffset = Vector2.zero; // UVƫ����
    private Vector2 textureSize; // �����С�����ڼ���UV���꣩
    private Rect rect; // ͼƬ��RectTransform��Rect����

    void Start()
    {
        material = GetComponent<Image>().material; // ��ȡImage����Ĳ���
        textureSize = material.mainTexture.texelSize; // ��ȡ�����С��1������ռ����UV��λ��
        rect = GetComponent<RectTransform>().rect; // ��ȡRectTransform��Rect����
    }

    void Update()
    {
        textureOffset.y += scrollSpeed * Time.deltaTime; // ��Y��������ɸ�ΪX�ᣩ
        textureOffset.x += ySpeed * Time.deltaTime;
        if (textureOffset.y > 1) // ��ƫ��������1ʱ���ã�ʵ��ѭ������Ч��
            textureOffset.y = 0;
        if(textureOffset.x > 1)
            textureOffset.x = 0;

        material.SetTextureOffset("_MainTex", textureOffset); // Ӧ��ƫ���������ʵ�UV������
    }
}