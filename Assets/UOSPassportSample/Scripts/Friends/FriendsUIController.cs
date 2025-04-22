using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Passport;
using TMPro;
using Unity.Passport.Runtime;
using Unity.Passport.Runtime.UI;
using UnityEngine;
using UnityEngine.UI;
using FriendRequestStatus = Unity.Passport.Runtime.Model.FriendRequestStatus;

namespace Unity.Passport.Sample.Scripts.Friends
{
    public class FriendsUIController : MonoBehaviour
    {
        [Header("好友列表")] public GameObject friendItemPrefab;
        public Transform friendList;
        public GameObject emptyIconPrefab;
        public TextMeshProUGUI friendCountLimit; // 好友数量已使用与上限

        [Header("好友申请弹窗")] public GameObject userInfoPopup;
        public Image friendImage;
        public TextMeshProUGUI friendNickname;
        private FriendItem.Config _currentFriend;
        public GameObject requestButtons;
        public GameObject removeAndBlockButtons;

        [Header("好友搜索")] public TMP_InputField friendPersonaIDInput;
        public GameObject strangerList;
        public Transform strangerListContent;
        public GameObject searchResultList;
        public Transform searchResultListContent;

        [Header("好友申请")] public GameObject friendRequestsPopup;
        public Transform friendRequestsList;
        public TextMeshProUGUI friendRequestsCount;
        public GameObject friendRequestsAlarm;
        public Transform friendRequestsSentList;
        public TextMeshProUGUI friendRequestSentCountLimit; // 发出的好友请求数量上限

        [Header("黑名单")] public GameObject blockedUsersPopup;
        public Transform blockedUserList;

        private string RealmID => DemoUIController.Instance.Persona.RealmID;
        private string PersonaID => DemoUIController.Instance.Persona.PersonaID;
        public static FriendsUIController Instance;
        private const int Interval = 30; // 刷新间隔
        private float _deltaTime = 0;

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

        private async void LateUpdate()
        {
            _deltaTime += Time.deltaTime;
            if (_deltaTime >= Interval)
            {
                _deltaTime = 0;
                await GetFriendRequestReceivedList();
            }
        }

        public async void Init()
        {
            GetFriendList();
            SearchFriendsRandomly();
            await GetFriendRequestReceivedList();
            await GetBlacklistedUserList();
            await GetFriendRequestSentList();
            // 清空搜索结果
            ClearFriendPersonaIDInput();
            gameObject.SetActive(true);
        }

        private void Start()
        {
            gameObject.GetComponent<TabsController>().InitTabs();
        }

        #region friends list

        /// <summary>
        /// 获取好友列表
        /// </summary>
        private async void GetFriendList()
        {
            var list = new List<FriendItem.Config>() { };
            try
            {
                var resp = await PassportFeatureSDK.Friends.GetFriendList();

                foreach (var friendship in resp.Friends)
                {
                    list.Add(new()
                    {
                        DisplayName = friendship.DisplayName,
                        PersonaID = friendship.TargetPersonaID,
                        Type = FriendItem.Type.FriendList,
                        Id = friendship.Id,
                    });
                }

                await GetFriendPresence(list);
                SetupList(friendList, list);
                
                // 获取好友上限
                var maxCount = PassportFeatureSDK.Friends.GetMaxFriendCount();
                var usedCount = resp.Total;
                friendCountLimit.text = $"已使用 {usedCount} / 好友上限 {maxCount}";
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        #endregion

        #region friends detail panel

        /// <summary>
        /// 点击好友(陌生人)列表弹出面板
        /// </summary>
        /// <param name="friend"></param>
        private void FriendItemClickHandler(FriendItem.Config friend)
        {
            if (friend.Type != FriendItem.Type.FriendList) return;

            _currentFriend = friend;
            // 设置信息
            // friendImage.sprite
            friendNickname.text = friend.DisplayName;

            // 根据是否为好友，显示不同按钮
            requestButtons.SetActive(friend.Relation == FriendItem.Relation.Stranger);
            removeAndBlockButtons.SetActive(friend.Relation == FriendItem.Relation.Friend);

            userInfoPopup.SetActive(true);
        }

        /// <summary>
        /// 发送好友申请
        /// </summary>
        public async void SendFriendRequest()
        {
            try
            {
                await PassportFeatureSDK.Friends.SendFriendRequest(_currentFriend.PersonaID);
                UIMessage.Show("发送好友申请成功");
                userInfoPopup.SetActive(false);
                await GetFriendRequestSentList();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        /// <summary>
        /// 在陌生人列表中发送好友请求
        /// </summary>
        /// <param name="friend"></param>
        public async void SendFriendRequestInStrangerList(FriendItem.Config friend)
        {
            try
            {
                await PassportFeatureSDK.Friends.SendFriendRequest(friend.PersonaID);
                UIMessage.Show("发送好友申请成功");
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        /// <summary>
        /// 在好友列表中移除好友
        /// </summary>
        public async void RemoveFriend()
        {
            try
            {
                await PassportFeatureSDK.Friends.RemoveFriend(_currentFriend.Id);
                UIMessage.Show("移除好友成功");
                // 刷新好友列表
                GetFriendList();
                userInfoPopup.SetActive(false);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        /// <summary>
        /// 在好友列表中拉黑好友
        /// </summary>
        public async void BlacklistFriendForFriendList()
        {
            try
            {
                await PassportFeatureSDK.Friends.BlacklistUser(_currentFriend.PersonaID);
                UIMessage.Show("加入黑名单成功");
                // 刷新好友列表
                GetFriendList();
                userInfoPopup.SetActive(false);
                // 刷新拉黑列表
                await GetBlacklistedUserList();
            }
            catch (PassportException e)
            {
                Debug.Log(e.ErrorMessage);
                UIMessage.Show(e);
            }
        }

        #endregion

        #region search

        /// <summary>
        /// 搜索
        /// </summary>
        public async void Search()
        {
            var personaID = friendPersonaIDInput.text;
            if (personaID == "")
            {
                searchResultList.SetActive(false);
                strangerList.SetActive(true);
                return;
            }

            var list = new List<FriendItem.Config>() { };
            try
            {
                var resp = await PassportFeatureSDK.Friends.SearchFriends(personaID, RealmID);
                var personas = resp.Personas;
                foreach (var persona in personas)
                {
                    list.Add(new()
                    {
                        DisplayName = persona.DisplayName,
                        PersonaID = persona.PersonaID,
                        Type = FriendItem.Type.FriendList,
                        Relation = FriendItem.Relation.Stranger
                    });
                }
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }

            SetupList(searchResultListContent, list);

            searchResultList.SetActive(true);
            strangerList.SetActive(false);
        }

        public void ClearFriendPersonaIDInput()
        {
            friendPersonaIDInput.text = "";
        }

        #endregion

        #region friends requests

        /// <summary>
        /// 在好友请求列表中拉黑用户
        /// </summary>
        /// <param name="friend"></param>
        public async void BlockUserForRequestList(FriendItem.Config friend)
        {
            try
            {
                await PassportFeatureSDK.Friends.BlacklistUser(friend.PersonaID);
                UIMessage.Show("加入黑名单成功");
                // 刷新请求列表
                await GetFriendRequestReceivedList();
                // 刷新拉黑列表
                await GetBlacklistedUserList();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        /// <summary>
        /// 获取未处理的好友请求列表
        /// </summary>
        /// <returns></returns>
        private async Task GetFriendRequestReceivedList()
        {
            var list = new List<FriendItem.Config>() { };
            try
            {
                var resp = await PassportFeatureSDK.Friends.GetFriendRequestReceivedList(0, 99, FriendRequestStatus.Pending);
                var requests = resp.Requests;

                foreach (var request in requests)
                {
                    list.Add(new FriendItem.Config()
                    {
                        DisplayName = request.SourceDisplayName,
                        PersonaID = request.SourcePersonaID,
                        Id = request.Id,
                        Type = FriendItem.Type.RequestList
                    });
                }

                SetupList(friendRequestsList, list);
                friendRequestsAlarm.SetActive(list.Any());
                friendRequestsCount.text = list.Count.ToString();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        /// <summary>
        /// 获取发送的好友请求
        /// </summary>
        private async Task GetFriendRequestSentList()
        {
            var list = new List<FriendItem.Config>() { };
            try
            {
                var resp = await PassportFeatureSDK.Friends.GetFriendRequestSentList(0, 99);

                foreach (var request in resp.Requests)
                {
                    list.Add(new FriendItem.Config()
                    {
                        DisplayName = request.TargetDisplayName,
                        PersonaID = request.TargetPersonaID,
                        Id = request.Id,
                        Type = FriendItem.Type.RequestSentList
                    });
                }

                SetupList(friendRequestsSentList, list);
                
                // 获取发出的好友请求数量上限
                var maxCount = PassportFeatureSDK.Friends.GetMaxPendingFriendRequestCount();
                var usedCount = resp.Total;
                friendRequestSentCountLimit.text = $"已使用 {usedCount} / 待处理请求上限 {maxCount}";
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        /// <summary>
        /// 获取好友请求并展示好友请求面板
        /// </summary>
        public async void ShowFriendRequestsPopup()
        {
            await GetFriendRequestReceivedList();
            await GetFriendRequestSentList();
            friendRequestsPopup.SetActive(true);
        }

        /// <summary>
        /// 同意好友请求
        /// </summary>
        /// <param name="friend"></param>
        public async void ApproveFriendRequest(FriendItem.Config friend)
        {
            try
            {
                await PassportFeatureSDK.Friends.ApproveFriendRequest(friend.Id);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }

            GetFriendList();
            await GetFriendRequestReceivedList();
        }

        /// <summary>
        /// 拒绝好友请求
        /// </summary>
        /// <param name="friend"></param>
        public async void RejectFriendRequest(FriendItem.Config friend)
        {
            try
            {
                await PassportFeatureSDK.Friends.RejectFriendRequest(friend.Id);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }

            await GetFriendRequestReceivedList();
        }

        /// <summary>
        /// 撤回好友请求
        /// </summary>
        /// <param name="friend"></param>
        public async void RevokeFriendRequest(FriendItem.Config friend)
        {
            try
            {
                await PassportFeatureSDK.Friends.RevokeFriendRequest(friend.Id);
                await GetFriendRequestSentList();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        #endregion

        #region Blacklist

        /// <summary>
        /// 获取拉黑列表
        /// </summary>
        /// <returns></returns>
        private async Task GetBlacklistedUserList()
        {
            var list = new List<FriendItem.Config>() { };
            var blockingRelationships = await PassportFeatureSDK.Friends.GetBlacklistedUserList(0, 10);

            foreach (var blockingRelationship in blockingRelationships)
            {
                list.Add(new FriendItem.Config()
                {
                    DisplayName = blockingRelationship.DisplayName,
                    PersonaID = blockingRelationship.TargetPersonaID,
                    Type = FriendItem.Type.BlockList,
                    Id = blockingRelationship.ID
                });
            }

            SetupList(blockedUserList, list);
        }

        /// <summary>
        /// 展示拉黑弹窗
        /// </summary>
        public async void ShowBlockedUserPopup()
        {
            await GetBlacklistedUserList();
            blockedUsersPopup.SetActive(true);
        }

        /// <summary>
        /// 从黑名单中移除
        /// </summary>
        /// <param name="friend"></param>
        public async void UnblockUser(FriendItem.Config friend)
        {
            try
            {
                await PassportFeatureSDK.Friends.UnBlacklistUser(friend.Id);
                UIMessage.Show("从黑名单中移出成功");

                // 刷新拉黑列表
                await GetBlacklistedUserList();
                // 刷新好友列表
                GetFriendList();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        #endregion

        #region Stranger List

        /// <summary>
        /// 获取随机陌生人列表
        /// </summary>
        public async void SearchFriendsRandomly()
        {
            var list = new List<FriendItem.Config>() { };
            var resp = await PassportFeatureSDK.Friends.SearchFriendsRandomly(RealmID, 10);
            var personas = resp.Personas;
            foreach (var persona in personas)
            {
                list.Add(new()
                {
                    DisplayName = persona.DisplayName,
                    PersonaID = persona.PersonaID,
                    Type = FriendItem.Type.FriendList,
                    Relation = FriendItem.Relation.Stranger
                });
            }

            SetupList(strangerListContent, list);
        }

        #endregion

        #region Friends Presence

        /// <summary>
        /// 查询好友状态
        /// </summary>
        /// <param name="list"></param>
        private async Task GetFriendPresence(List<FriendItem.Config> list)
        {
            List<string> friendshipIDs = new();
            Dictionary<string, FriendItem.Config> friendItemMap = new();
            foreach (var friend in list)
            {
                var friendshipID = friend.Id;
                friendshipIDs.Add(friendshipID);
                friendItemMap.Add(friendshipID, friend);
            }

            try
            {
                var presenceDictionary = await PassportFeatureSDK.Friends.GetFriendPresenceList(friendshipIDs);

                foreach (var kvp in presenceDictionary)
                {
                    if (friendItemMap.TryGetValue(kvp.Key, out var friend) && friend != null)
                    {
                        friend.Status = kvp.Value.status;
                    }
                }
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }

        #endregion

        #region Utils

        private void SetupList(Transform parent, List<FriendItem.Config> friends)
        {
            DestroyChildren(parent);
            // 如果列表为空，显示空提示
            if (!friends.Any())
            {
                Instantiate(emptyIconPrefab, parent);
                return;
            }

            for (int i = 0; i < friends.Count; i += 1)
            {
                var friend = friends[i];
                var obj = Instantiate(friendItemPrefab, parent);
                obj.GetComponent<FriendItem>().Init(friend);
                obj.GetComponent<Button>().onClick.AddListener(() => { FriendItemClickHandler(friend); });
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

        #endregion

        public void OnTabSelect(int index)
        {
            if (index == 0)
            {
                // 好友列表
                GetFriendList();
            }

            if (index == 1)
            {
                // 刷新随机列表
                SearchFriendsRandomly();
            }
        }
    }
}