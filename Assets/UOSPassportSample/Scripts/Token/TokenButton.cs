using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts.Token
{
    public class TokenButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tokenText;
        public UnityEvent<string> onFillToken;

        private void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                onFillToken.Invoke(tokenText.text);
            });
        }
    }
}