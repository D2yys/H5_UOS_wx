using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Passport.Runtime;
using System.Threading.Tasks;
using TMPro;
using Unity.Passport.Runtime.UI;

namespace Unity.Passport.Sample.Scripts.AntiAddiction
{
    public enum UserType
    {
        minor = 0,
        adult = 1
    }

    public enum AgeClass
    {
        Age08,
        Age816,
        Age1618,
        Age18,
    }

    public class AntiAddictionUIController : MonoBehaviour
    {
        [Header("玩家类型信息")] [SerializeField] private List<GameObject> userType;
        [SerializeField] private List<GameObject> userTypeInPanel;

        [Header("playable信息")] [SerializeField]
        private TextMeshProUGUI reasonText;

        [SerializeField] private TextMeshProUGUI remainTimeText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [Header("payable信息")] [SerializeField] private TextMeshProUGUI payableStatusText;
        [SerializeField] private TextMeshProUGUI payableReasonText;
        private const string HintMsg = "单笔金额金额超过上限或充值金额已达上限";
        private uint _remainSeconds = 0;
        private float _deltaTime = 0;

        [Header("模拟实名认证")] [SerializeField] private TextMeshProUGUI confirmMsg;
        [SerializeField] private GameObject mockVerifyRealnamePopup;
        // 年龄-身份证
        private Dictionary<AgeClass, string> IDCardDictionary = new()
        {
            { AgeClass.Age08, "130602202312259290" },
            { AgeClass.Age816, "532532201512255635" },
            { AgeClass.Age1618, "451222200712253955" },
            { AgeClass.Age18, "500111200512253394" }
        };

        private void LateUpdate()
        {
            if (_remainSeconds > 0)
            {
                _deltaTime += Time.deltaTime;
                int seconds = (int)Math.Floor(_deltaTime);
                if (seconds > 0)
                {
                    _deltaTime -= seconds;
                    _remainSeconds -= (uint)seconds;
                    remainTimeText.text = $"剩余游戏时间 {FormatTime(_remainSeconds)}";
                }
            }
        }

        public async Task Init()
        {
            try
            {
                await SetPlayable();
            }
            catch (PassportException e)
            {
                Debug.Log(e.ErrorMessage);
            }
        }

        private async Task SetPlayable()
        {
            Clear();
            try
            {
                var resp = await PassportFeatureSDK.AntiAddiction.CheckPlayable();
                if (Enum.TryParse(resp.UserType, out UserType type))
                {
                    var index = (int)type;
                    userType[index].SetActive(true);
                    userTypeInPanel[index].SetActive(true);
                }

                // 清空
                _deltaTime = 0;
                remainTimeText.text = "";
                // 写入
                _remainSeconds = resp.RemainingTimeInSecond;
                reasonText.text = resp.Reason;
                descriptionText.text = resp.Description;
            }
            catch (PassportException e)
            {
                Debug.Log(e.ErrorMessage);
            }
        }

        private void Clear()
        {
            foreach (var obj in userType)
            {
                obj.SetActive(false);
            }

            foreach (var obj in userTypeInPanel)
            {
                obj.SetActive(false);
            }
        }

        private string FormatTime(uint time)
        {
            var minute = time / 60;
            var seconds = (time % 60).ToString().PadLeft(2, '0');
            return $"{minute}:{seconds}";
        }

        private async Task<bool> SetPayable(uint amount)
        {
            try
            {
                var resp = await PassportFeatureSDK.AntiAddiction.CheckPayable(amount);
                payableReasonText.text = resp.Reason;
                payableStatusText.text = resp.Approved ? "允许充值" : HintMsg;

                return resp.Approved;
            }
            catch (PassportException e)
            {
                Debug.Log(e.ErrorMessage);
            }

            return false;
        }

        public async void Pay(int amount)
        {
            try
            {
                var cent = amount * 100;
                var payable = await SetPayable((uint)cent);
                if (!payable)
                {
                    UIMessage.Show(HintMsg);
                    return;
                }

                await PassportFeatureSDK.AntiAddiction.SubmitPayment((uint)cent);
                UIMessage.Show($"成功充值{amount}元");
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
            }
        }

        public async void Show()
        {
            await SetPlayable();
            await SetPayable(0);
            gameObject.SetActive(true);
        }
        
        public async void MockVerifyRealName(string age)
        {
            try
            {
                Enum.TryParse(age, out AgeClass ageClass);
                IDCardDictionary.TryGetValue(ageClass, out var idCard);
                await PassportSDK.Identity.VerifyRealName("大团结", idCard);
                await PassportSDK.Identity.SelectPersona(DemoUIController.Instance.Persona.PersonaID);

                // 刷新数据
                Show();
                mockVerifyRealnamePopup.SetActive(false);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage + "。请在当前 UOS APP 下开启「模拟实名认证」。", MessageType.Error, 10);
            }
        }
    }
}