using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Passport.Runtime.UI;
using UnityEngine.Events;

namespace Unity.Passport.Sample.Scripts
{
    public class RewardItem : MonoBehaviour
    {
        public enum RewardType
        {
            Gold =0,
            Energy = 1,
            Gem = 2,
            Clover = 3,
            Glove = 4,
            Target = 5,
            Other = 6
        }
        
        // 根据需要与枚举映射
        public List<Sprite> sprites = new();
        public class Config
        {
            public RewardType Type;
            public int Count;
        }
        private bool _purchased = false;
        private bool _enoughLevel = false;
        private bool _claimed = false;

        public GameObject lockIcon; 
        public Sprite canClaimSprite; 
        private Sprite _defaultSprite;
        public GameObject claimedIcon;
        public Image rewardImage;
        public TextMeshProUGUI countText;

        private Image _image;
        private bool _initFinished;
        public UnityEvent consume;
        public Config ItemConfig;

        public void Init(Config config)
        {
            if (_initFinished) return;
            _initFinished = true;
            _image = gameObject.GetComponent<Image>();
            _defaultSprite = _image.sprite;
            gameObject.GetComponent<Button>().onClick.AddListener(Claim);
            countText.text = config.Count.ToString();
            rewardImage.sprite = sprites[(int)config.Type];
            ItemConfig = config;
        }

        /// <summary>
        /// 切换为解锁样式
        /// </summary>
        public void Unlock()
        {
            _purchased = true;
            lockIcon.SetActive(false);

            if (_enoughLevel && !_claimed)
            {
                _image.sprite = canClaimSprite;
            }
        }

        /// <summary>
        /// 升级到当前所需等级
        /// </summary>
        public void Upgrade()
        {
            _enoughLevel = true;
            if (_purchased && !_claimed)
            {
                _image.sprite = canClaimSprite;
            }
        }

        /// <summary>
        /// 领取
        /// </summary>
        private void Claim()
        {
            // 不符合条件
            if (!_purchased)
            {
                UIMessage.Show("请先购买后领取");
            }

            if (!_enoughLevel)
            {
                UIMessage.Show("请先升级后领取");
            }
            if (!_enoughLevel || !_purchased || _claimed) return;

            consume.Invoke();
        }

        /// <summary>
        /// 切换为已领取样式
        /// </summary>
        public int Claimed()
        {
            if (!_enoughLevel || !_purchased || _claimed) return 0;
            _claimed = true;
            _image.sprite = _defaultSprite; // 恢复样式
            claimedIcon.SetActive(true);
            return ItemConfig.Count;
        }

        /// <summary>
        /// 重置为初始数据
        /// </summary>
        public void ResetItem()
        {
            _claimed = false;
            _enoughLevel = false;
            _purchased = false;
            _image.sprite = _defaultSprite; // 恢复样式
            claimedIcon.SetActive(false);
            lockIcon.SetActive(true);
        }
    }

}