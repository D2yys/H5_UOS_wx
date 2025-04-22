using System;
using System.Collections.Generic;
using Passport;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts.Friends
{
    public class FriendItem : MonoBehaviour
    {
        public TextMeshProUGUI nicknameText;
        public TextMeshProUGUI statusText;
        
        // 处理好友请求按钮：接受、拒绝、拉黑
        public GameObject handleRequestButtons;
        public Button acceptRequestButton;
        public Button refuseRequestButton;
        public Button blockRequestButton;
        
        // 发送好友请求按钮
        public GameObject requestButtons;
        public Button requestButton;
        
        // 解除拉黑
        public GameObject unblockButtons;
        public Button unblockButton;
        
        // 发出的好友请求按钮
        public GameObject requestSentButtons;
        public Button revokeButton;
        
        public enum Type
        {
            FriendList = 0, // 好友列表
            RequestList = 1, // 好友请求列表
            BlockList = 2, // 黑名单列表
            RequestSentList = 3, // 发出的好友请求列表
        }

        public enum Relation
        {
            Friend = 0,
            Stranger = 1,
        }
        public enum Status
        {
            None = 0,
            Offline = 1,
            Online = 2,
        }
        
        private readonly Dictionary<Status, string> _statusDesc = new()
        {
            { Status.Offline, "离线" },
            { Status.Online, "在线" }
        };
        public class Config
        {
            public string DisplayName;
            public string Url;
            public string Status;
            public string PersonaID;
            public string Id; // 好友请求 ID / 好友关系 ID / 拉黑关系 ID
            public Type Type = Type.FriendList;
            public Relation Relation = Relation.Friend;
        }

        private FriendsUIController FriendsUIController => FriendsUIController.Instance;

        public void Init(Config config)
        {
            
            nicknameText.text = config.DisplayName;
            // 仅在好友列表展示
            bool useStatus = config.Type == Type.FriendList && config.Relation == Relation.Friend;
            if (useStatus)
            {
                statusText.text = Enum.TryParse(config.Status, out Status status) ? _statusDesc[status] : _statusDesc[Status.Offline];
            }
            statusText.gameObject.SetActive(useStatus);
            requestButtons.SetActive(config.Relation == Relation.Stranger);
            handleRequestButtons.SetActive(config.Type == Type.RequestList);
            unblockButtons.SetActive(config.Type == Type.BlockList);
            requestSentButtons.SetActive(config.Type == Type.RequestSentList);

            if (config.Relation == Relation.Stranger)
            {
                requestButton.onClick.AddListener(() =>
                {
                    FriendsUIController.SendFriendRequestInStrangerList(config);
                });
            }

            // 好友请求按钮添加点击事件处理
            if (config.Type == Type.RequestList)
            {
                acceptRequestButton.onClick.AddListener(() =>
                {
                    FriendsUIController.ApproveFriendRequest(config);
                });
                refuseRequestButton.onClick.AddListener(() =>
                {
                    FriendsUIController.RejectFriendRequest(config);
                });
                blockRequestButton.onClick.AddListener(() =>
                {
                    FriendsUIController.BlockUserForRequestList(config);
                });
            }

            // 屏蔽名单按钮添加点击事件处理
            if (config.Type == Type.BlockList)
            {
                unblockButton.onClick.AddListener(() =>
                {
                    FriendsUIController.UnblockUser(config);
                });
            }

            if (config.Type == Type.RequestSentList)
            {
                revokeButton.onClick.AddListener(() =>
                {
                    FriendsUIController.RevokeFriendRequest(config);
                });
            }
   
        }
    }
}