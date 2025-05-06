using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts.Leaderboard
{
    public class RankItem : MonoBehaviour
    {
        public TextMeshProUGUI nicknameText;
        public TextMeshProUGUI tierText;
        private LeaderboardUIController LeaderboardUIController => LeaderboardUIController.Instance;
        public List<GameObject> rankStatusList = new();
        public RawImage Hade;

        public const int RankSize = 10; // 大于10意味着未上榜
        private const int MaxMedalRank = 3; // 最大的可获得奖牌的排名
        public class Config
        {
            public string Alias;
            public string Url;
            public string Id;
            public ulong Rank; // 排名
            public double Score; // 分数
            public string Tier; // 等级
            public Texture hadeT;  //头像图片

        }

        // 与 rankStatusList 的顺序一一对应
        private enum RankStatus
        {
            NotRanked = 0,
            Gold = 1,
            Sliver = 2,
            Bronze = 3,
            SelfDefined = 4,
        }

        public void Init(Config config)
        {

            nicknameText.text = config.Alias;

            // 设置等级名称
            var text = $"{config.Score}";
            //设置头像            
            if (config.Url.Length != 0) { 
            StartCoroutine(LoadAvatar(config.Url));
            }

            if (config.Tier != "")
            {
                text += $"（{config.Tier}）";
            }
            tierText.text = text + "分";


            // 设置排名
            if (config.Rank <= 0)
            {
                Debug.LogError("错误的排名");
            }
            else if (config.Rank > RankSize)
            {
                // 未上榜
                rankStatusList[(int)RankStatus.NotRanked].SetActive(true);
            }
            else if (config.Rank > MaxMedalRank)
            {
                // 自定义排名
                var obj = rankStatusList[(int)RankStatus.SelfDefined];
                var textComp = obj.GetComponent<TextMeshProUGUI>();
                textComp.text = config.Rank.ToString();
                obj.SetActive(true);
            }
            else
            {
                // 金银铜牌
                rankStatusList[(int)config.Rank].SetActive(true);
            }


        }

        public void Clear()
        {
            nicknameText.text = "";
            tierText.text = "";
            foreach (var obj in rankStatusList)
            {
                obj.SetActive(false);
            }
        }

        IEnumerator LoadAvatar(string url)
        {
            // 加载头像图片
            Debug.Log("头像的Url地址是" + url);
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);

            yield return uwr.SendWebRequest(); // 等待请求完成

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("加载Url图片失败地址是" + url);

            }
            else

            {

                Hade.texture = DownloadHandlerTexture.GetContent(uwr);

                Debug.Log("获得成功");

            }

        }
    }
}