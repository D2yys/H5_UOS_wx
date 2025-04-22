using UnityEngine;

namespace Unity.Passport.Sample.Scripts
{
    [ExecuteInEditMode]
    public class BGScaler : MonoBehaviour
    {
        //图片原大小(压缩前的)
        public Vector2 textureOriginSize = new Vector2(2048, 1024);

        // Start is called before the first frame update
        void Start()
        {
            Scaler();
        }
        #if UNITY_EDITOR
        void Update()
        {
            Scaler();
        }
        #endif

        //适配
        void Scaler()
        {
            //当前画布尺寸
            Vector2 canvasSize = gameObject.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            //当前画布长宽比
            float screenxyRate = canvasSize.x / canvasSize.y;

            //背景图片尺寸
            Vector2 bgSize = textureOriginSize;
            //背景图片长宽比
            float texturexyRate = bgSize.x / bgSize.y;

            RectTransform rt = (RectTransform)transform;

            if (texturexyRate > screenxyRate)
            {
                //背景图片x偏长,需要适配y
                int newSizeY = Mathf.CeilToInt(canvasSize.y);
                int newSizeX = Mathf.CeilToInt((float)newSizeY / bgSize.y * bgSize.x);
                rt.sizeDelta = new Vector2(newSizeX, newSizeY);
            }
            else
            {
                int newVideoSizeX = Mathf.CeilToInt(canvasSize.x);
                int newVideoSizeY = Mathf.CeilToInt((float)newVideoSizeX / bgSize.x * bgSize.y);
                rt.sizeDelta = new Vector2(newVideoSizeX, newVideoSizeY);
            }
        }
    }
};