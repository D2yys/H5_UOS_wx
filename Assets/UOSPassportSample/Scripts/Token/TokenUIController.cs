using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Passport.Runtime;
using Unity.Passport.Runtime.UI;
using UnityEditor;

namespace Unity.Passport.Sample.Scripts.Token
{
    public class TokenUIController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField codeInput;
        [SerializeField] private GameObject rewardGetPanel;
        private RewardGetPanel _rewardGetPanel;
        public async void RedeemToken()
        {
            if (String.IsNullOrEmpty(codeInput.text))
            {
                UIMessage.Show("请输入礼包码");
                return;
            }

            try
            {
                var resp = await PassportFeatureSDK.Token.RedeemToken(codeInput.text);
                var dic = new Dictionary<string, uint>();
                foreach (var kvp in resp.TokenInstance.Resources)
                {
                    if (!dic.TryGetValue(kvp.Key, out var o))
                    {
                        dic.Add(kvp.Key, kvp.Value);
                    }
                }
                rewardGetPanel.GetComponent<RewardGetPanel>().Show(dic);
                UIMessage.Show("兑换成功");
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
            }
        }

        public void SetToken(string code)
        {
            codeInput.text = code;
        }
    }
}
