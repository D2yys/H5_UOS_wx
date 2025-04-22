using System.Collections.Generic;
using UnityEngine;

namespace Unity.Passport.Sample.Scripts
{
    public class RewardGetPanel : MonoBehaviour
    {
        [SerializeField] private GameObject rewardGetItemPrefab;
        public Transform rewardGetList; // 得到的奖品展示
        public void Show(Dictionary<string,uint> resources)
        {
            Clear();
            foreach (var kvp in resources)
            {
                var item = Instantiate(rewardGetItemPrefab, rewardGetList);
                item.GetComponent<RewardItem>().Init(new RewardItem.Config()
                {
                    Type = RewardItem.RewardType.Other,
                    Count = (int)kvp.Value  
                });
            }
            gameObject.SetActive(true);
        }
        
        private void Clear()
        {
            // 清空对象
            for (int i = 0; i < rewardGetList.childCount; i += 1)
            {
                Destroy(rewardGetList.GetChild(i).gameObject);
            }
        }
    }
}
