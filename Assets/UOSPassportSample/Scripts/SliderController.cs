using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts
{
    public class SliderController : MonoBehaviour
    {
        private Coroutine _adjustChangeCoroutine;
        private float HPAnimationSpeed = 2.0f;
        private Slider _slider;
        private float _maxValue;
        private float _toValue;
        private bool _initFinished;

        private void Init()
        {
            if (_initFinished) return;
            _initFinished = true;
            _slider = GetComponent<Slider>();
            _maxValue = _slider.maxValue;
        }

        public void SetValue(float value, bool useAnimation, Action callback = null)
        {
            Init();
            if (useAnimation || !gameObject.activeSelf)
            {
                Change(value, callback);
            }
            else
            {
                SetSliderValue(value);
                _toValue = value;
                if (callback != null)
                {
                    callback();
                }
            }
        }

        private void Change(float to, Action callback)
        {
            if (to < _toValue) return;

            // 先关掉之前的
            if (_adjustChangeCoroutine != null)
            {
                StopCoroutine(_adjustChangeCoroutine);
                SetSliderValue(_toValue);
            }

            _adjustChangeCoroutine = StartCoroutine(AdjustChange(to, callback));
        }

        IEnumerator AdjustChange(float to, Action callback)
        {
            float current = _toValue;
            _toValue = to;
            while (Mathf.Abs(to - current) > 1f)
            {
                current = Mathf.Lerp(current, to, Time.deltaTime * HPAnimationSpeed);
                SetSliderValue(current);
                yield return null;
            }

            // 精确的
            SetSliderValue(to);
            if (callback != null)
            {
                callback();
            }
        }

        private void SetSliderValue(float to)
        {
            _slider.value = to % _maxValue;
        }
    }
}