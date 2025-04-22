using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public enum ShakeType
    {
        AxesX = 0, AxesY = 1, AxesZ = 2, AxesXYZ = 3
    }
    public ShakeType shakeType = ShakeType.AxesX;
    public Vector3 shakeRate = new Vector3(0.02f, 0.02f, 0.02f);//shakeRate
    public float shakeTime = 0.08f;//shakeTime: ����ʱ��
    public float shakeDertaTime = 0.08f;//shakeDertaTime: �ƶ�����

    public void Shakeobiect()
    {
        StartCoroutine(ShakeCoroutine());
    }

    public IEnumerator ShakeCoroutine()
    {
        var oriPosition = gameObject.transform.localPosition;
        for (float i = 0; i < shakeTime; i += shakeDertaTime)
        {
            switch (shakeType)
            {
                case ShakeType.AxesX:
                    gameObject.transform.localPosition = oriPosition + Random.Range(-shakeRate.x, shakeRate.x) * Vector3.right;
                    break;
                case ShakeType.AxesY:
                    gameObject.transform.localPosition = oriPosition +
                    Random.Range(-shakeRate.y, shakeRate.y) * Vector3.up;
                    break;
                case ShakeType.AxesZ:
                    gameObject.transform.localPosition = oriPosition + Random.Range(-shakeRate.z, shakeRate.z) * Vector3.forward;
                    break;
                case ShakeType.AxesXYZ:
                    gameObject.transform.localPosition = oriPosition +
                    Random.Range(-shakeRate.x, shakeRate.x) * Vector3.right +
                    Random.Range(-shakeRate.y, shakeRate.y) * Vector3.up +
                    Random.Range(-shakeRate.z, shakeRate.z) * Vector3.forward;
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(shakeDertaTime);
        }
        gameObject.transform.localPosition = oriPosition;
    }

    public float DelayTime = 2f;
    bool mIsPlayShake = false;
    float mShakeLeftTime = 0;

    [ContextMenu("Shake")]
    public void PlayShake()
    {
        mShakeLeftTime = DelayTime;
        mIsPlayShake = true;
    }

    void Update()
    {
        if (mIsPlayShake)
        {
            mShakeLeftTime -= Time.deltaTime;
            if (mShakeLeftTime <= 0)
            {
                mShakeLeftTime = 0;
                mIsPlayShake = false;
                Shakeobiect();
            }
        }
    }
}