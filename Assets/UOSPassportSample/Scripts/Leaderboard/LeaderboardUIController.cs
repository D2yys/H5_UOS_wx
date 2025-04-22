using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Passport;
using TMPro;
using UnityEngine;
using Unity.Passport.Runtime;
using Unity.Passport.Runtime.UI;
using Unity.Passport.Sample.Scripts.Utils;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using HutongGames.PlayMaker;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts.Leaderboard
{
    public class LeaderboardUIController : MonoBehaviour
    {
        public static LeaderboardUIController Instance;
        public GameObject rankItemPrefab;
        public GameObject rankTabPrefab;
        public GameObject rankPanelPrefab;

        public Transform tabs;
        public Transform panels;
        public UnityEvent onConfigGenned = new();

        public Text hintText;
        // 排行榜配置列表
        private readonly List<Config> _rankConfigList = new();

        [System.Serializable]
        private class Config
        {
            public Transform listContent;
            public string leaderboardSlugName;
            public string name;
            public GameObject rankSelf; // 玩家自身排行
            public bool useBucket = false; // 是否获取随机小榜
            public TextMeshProUGUI cronText; // 更新周期文本
            public int size = 10; // 榜单的大小
            public string scheduleText; // 重置周期设置
            public TabController tabController;
            public string BucketStrategy; // 榜单的小榜策略
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        
        
        
        public async Task Init(bool updateScore = true)
        {
            await GenConfigList();
            var numLives = FsmVariables.GlobalVariables.GetFsmInt("Data_level");
            
            int score = numLives.Value;

            foreach (var rankConfig in _rankConfigList)
            {
                if (updateScore)
                {
                    // 更新玩家分数
                    await UpdateScoreOnLoggedIn(rankConfig, score);
                }
                UpdateLeaderboardByConfig(rankConfig);
            }
        }
        
        private void UpdateLeaderboardByConfig(Config leaderboard)
        {
            // 获取排行榜成绩
            if (leaderboard.useBucket)
            {
                GetLeaderboardBucket(leaderboard);
            }
            else
            {
                GetLeaderboard(leaderboard);
            }
            
            // 设置更新频率文案
            if (leaderboard.cronText != null)
            {
                var schedule = leaderboard.scheduleText;
                leaderboard.cronText.text = schedule != "" ?  $"榜单{Cron.Translate(schedule)}重置" : "榜单无重置计划";
            }
        }

        /// <summary>
        /// 登录奖励分数
        /// </summary>
        private async Task UpdateScoreOnLoggedIn(Config leaderboard, int score = 0)
        {
            try
            {
                if (leaderboard.BucketStrategy == "Customized")
                {
                    await PassportFeatureSDK.Leaderboard.UpdateScoreWithBucketId(leaderboard.leaderboardSlugName, score, "bucket1");
                }
                else
                {
                    await PassportFeatureSDK.Leaderboard.UpdateScore(leaderboard.leaderboardSlugName, score);
                }
               // UIMessage.Show($"经验值+{score}");
            }
            catch (PassportException e)
            {
                Debug.Log(e.Code);
            }
        }

        private async void GetLeaderboard(Config leaderboard)
        {
            Transform listContent = leaderboard.listContent;
            string leaderboardSlugName = leaderboard.leaderboardSlugName;
            List<RankItem.Config> list = new();
            try
            {
                DestroyChildren(listContent);
                var resp = await PassportFeatureSDK.Leaderboard.ListLeaderboardScores(leaderboardSlugName);
                foreach (var score in resp.Scores)
                {
                    list.Add(new RankItem.Config()
                    {
                        Alias = score.DisplayName,
                        Rank = score.Rank,
                        Score = score.Score,
                        Tier = score.Tier,
                    });
                }
                SetupLeaderboardList(listContent, list.GetRange(0, Math.Min(leaderboard.size, list.Count)));
                // 设置自身分数
                SetupSelfRank(leaderboard);
            }
            catch (PassportException e)
            {
                Debug.Log(e.Code);
                Debug.Log(e.Message);
            }
        }

        /// <summary>
        /// 销毁所有子项
        /// </summary>
        /// <param name="parent"></param>
        private void DestroyChildren(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i += 1)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private void SetupLeaderboardList(Transform parent, List<RankItem.Config> ranks)
        {
            // 如果列表为空，显示空提示
            if (!ranks.Any())
            {
                return;
            }

            for (int i = 0; i < ranks.Count; i += 1)
            {
                var rank = ranks[i];
                var obj = Instantiate(rankItemPrefab, parent);
                obj.GetComponent<RankItem>().Init(rank);
            }
        }
        
        #region Methods For Self Rank

        /// <summary>
        /// 设置自身排行
        /// </summary>
        /// <param name="leaderboard"></param>
        private async void SetupSelfRank(Config leaderboard)
        {
            var item = leaderboard.rankSelf.GetComponent<RankItem>();
            // 先清空
            item.Clear();
            try
            {
                var resp = await PassportFeatureSDK.Leaderboard.GetScore(leaderboard.leaderboardSlugName, 0);
                if (resp.Scores.Any())
                {
                    var score = resp.Scores[0];
                    RankItem.Config config = new()
                    {
                        Alias = score.DisplayName,
                        Rank = score.Rank,
                        Score = score.Score,
                        Tier = score.Tier,
                    };
                    
                    item.Init(config);
                }
            }
            catch (PassportException e)
            {
                Debug.Log(e.Code);
                Debug.Log(e.Message);
            }

        }
        #endregion
        
        #region Methods For Leaderboard Bucket
        private async void GetLeaderboardBucket(Config leaderboard)
        {
            Transform listContent = leaderboard.listContent;
            string leaderboardSlugName = leaderboard.leaderboardSlugName;
            List<RankItem.Config> list = new();
            try
            {
                DestroyChildren(listContent);
                var resp = await PassportFeatureSDK.Leaderboard.GetBucket(leaderboardSlugName);
                ulong rank = 0;
                ulong count = 0;
                double lastScore = -1;
                var selfItem = leaderboard.rankSelf.GetComponent<RankItem>();
                selfItem.Clear();
                foreach (var score in resp.Scores)
                {
                    count += 1;
                    if (lastScore < score.Score)
                    {
                        rank += 1;
                    }
                    else
                    {
                        rank = count;
                    }
                    var rankItem = new RankItem.Config()
                    {
                        Alias = score.DisplayName,
                        Rank = rank,
                        Score = score.Score,
                        Tier = score.Tier,
                    };
                    lastScore = score.Score;
                    list.Add(rankItem);
                    
                    // 设置自身分数
                    if (score.MemberId == DemoUIController.Instance.Persona.PersonaID)
                    {
                        selfItem.Init(rankItem);
                    }
                }

                var lastIndex = Math.Min(leaderboard.size, list.Count);
                SetupLeaderboardList(listContent, list.GetRange(0, lastIndex));
            }
            catch (PassportException e)
            {
                Debug.Log(e.Code);
                Debug.Log(e.Message);
            }

        }
        #endregion

        #region Methods For Leaderboard Configs
        private async Task GenConfigList()
        {
            // 先清空
            DestroyChildren(tabs);
            DestroyChildren(panels);
            _rankConfigList.Clear();
            
            // 获取排行榜列表
            try
            {
                var resp = await PassportFeatureSDK.Leaderboard.GetLeaderboards();
                var leaderboards = resp.Leaderboards;
                if (!leaderboards.Any())
                {
                    Debug.LogError("当前 uos app 未配置排行榜，请到 https://uos.unity.cn/ 上创建排行榜");
                    return;
                }

                const int maxSize = 3;
                var maxIndex = Math.Min(leaderboards.Count, maxSize) - 1;
                hintText.text = leaderboards.Count > maxSize ? $"共有{leaderboards.Count}个榜单，仅展示最新{maxSize}个" : "";

                // 生成配置列表
                for (int i = 0; i <= maxIndex; i += 1)
                {
                    var leaderboard = leaderboards[i];
                    var config = new Config
                    {
                        leaderboardSlugName = leaderboard.SlugName,
                        name = leaderboard.DisplayName,
                        size = 10,
                        useBucket = leaderboard.HasBucketSize,
                        scheduleText = leaderboard.ResetSchedule,
                        BucketStrategy = leaderboard.BucketStrategy.ToString()
                    };

                    // 生成 Tab
                    var rankTab = Instantiate(rankTabPrefab, tabs);
                    var controller = rankTab.GetComponent<TabController>();
                    controller.SetText(config.name);

                    // 生成 panel
                    var rankPanel = Instantiate(rankPanelPrefab, panels);
                    var t1 = rankPanel.transform;


                    // 获取列表 transform
                    var listContent = t1.Find("Mask/Content");
                    config.listContent = listContent;

                    // 获取自身排行对象
                    var rankSelf = t1.Find("RankSelf").gameObject;
                    config.rankSelf = rankSelf;

                    // 获取重置周期文本对象
                    var cronText = t1.Find("CronText").gameObject.GetComponent<TextMeshProUGUI>();
                    config.cronText = cronText;

                    // 设置 tab 高亮
                    config.tabController = controller;

                    _rankConfigList.Add(config);
                }

                onConfigGenned.Invoke();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        #endregion

        public async void ShowLeaderboard()
        {
            await Init(false);
            gameObject.SetActive(true);
        }
    }
}
