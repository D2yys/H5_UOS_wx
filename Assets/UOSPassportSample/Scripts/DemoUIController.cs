using System.Collections;
using System.Collections.Generic;
using Passport;
using UnityEngine;
using Unity.Passport.Runtime.UI;
using Unity.Passport.Runtime;
using TMPro;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Linq;
using Unity.Passport.Runtime.Model;
using Unity.Passport.Sample.Scripts.AntiAddiction;
using UnityEngine.UI;
using UnityEngine.Networking;
using MessageType = Unity.Passport.Runtime.UI.MessageType;
using Unity.Passport.Sample.Scripts.Leaderboard;
using Status = Unity.Passport.Sample.Scripts.Friends.FriendItem.Status;
using Unity.UOS.Auth;

namespace Unity.Passport.Sample.Scripts
{
    public class DemoUIController : MonoBehaviour
    {
        #region Public Fields
        [Header("登录前面板")]
        public GameObject preLoginPanel;

        [Header("登录完成面板")]
        public GameObject loggedInPanel;
        [Tooltip("账号信息")] public TextMeshProUGUI userId;
        [Tooltip("头像")] public Image userAvatar;
        [Tooltip("默认头像")] public Sprite defaultAvatar;
        [Tooltip("当前选中的服务器")] public TextMeshProUGUI currentRealmText; 

        [Header("服务器选择面板")]
        public GameObject realmListContent;
        public GameObject realmSelectPanel;
        public GameObject realmPrefab;
        public static readonly UnityEvent<Realm> SelectRealm = new();
        
        [Header("创建新角色面板")]
        [Tooltip("创建新角色面板")] public GameObject createPersonaPanel;
        [Tooltip("角色名称输入框")] public GameObject createPersonaInput;
        private Persona _currentPersona;
        [TextArea]
        public string personaNameList;

        private string[] _personaNameList;

        [Header("大厅面板")] public GameObject hallPanel;
        [Tooltip("角色信息")] public TextMeshProUGUI personaName;
        [Tooltip("角色ID")] public TextMeshProUGUI personaID;

        [Header("设置菜单面板")] public GameObject settingMenuPanel;

        // TODO: entitlement
        // [Header("战令面板")] public GameObject entitlementPanel;
        // private EntitlementUIController _entitlementUIController;

        [Header("排行榜面板")] public GameObject leaderboardPanel;
        private LeaderboardUIController _leaderboardUIController;

        [Header("公会面板")] public GameObject guildPanel;

        [Header("防沉迷面板")] public GameObject antiAddictionPanel;
        private AntiAddictionUIController _antiAddictionUIController;
        public Persona Persona
        {
            get => _currentPersona; 
            private set => _currentPersona = value;
        }

        public static DemoUIController Instance
        {
            get => _instance;
            private set => _instance = value;
        } 

        #endregion

        #region Private Fields
        private string _currentRealm; // 当前选中的域

        private readonly PassportUIConfig _config = new()
        {
            AutoRotation = true, // 是否开启自动旋转
            InvokeLoginManually = false, // 是否手动启用登录面板
            Theme = PassportUITheme.Dark, // 风格主题配置
            UnityContainerId = "unity-container" // WebGL 场景下 Unity 实例容器 Id，用于挂载微信扫码 iframe
        };

        private static DemoUIController _instance;

        #endregion

        /// <summary>
        /// SDK 回调
        /// </summary>
        /// <param name="passportEvent"></param>
        private async void _callback(PassportEvent passportEvent)
        {
            switch (passportEvent)
            {
                case PassportEvent.RejectedTos:
                    Debug.Log("用户拒绝了协议");
                    ShowPreLoginPanel();
                    break;
                case PassportEvent.LoggedIn:
                    Debug.Log("完成登录");
                    break;
                case PassportEvent.Completed:
                    Debug.Log("完成全部流程");
                    await GetPlayerInfo();
                    GetRealm();
                    ShowLoggedInPanel();
                    break;
                case PassportEvent.LoggedOut:
                    Debug.Log("用户登出");
                    ShowPreLoginPanel(false);
                    ShowLoggedInPanel(false);
                    OnLoggedOut();
                    break;
            }
        }

        #region Methods for panel display control

        private void ShowPreLoginPanel(bool show = true)
        {
            preLoginPanel.SetActive(show);
            loggedInPanel.SetActive(false);
        }

        private void ShowLoggedInPanel(bool show = true)
        {
            preLoginPanel.SetActive(false);
            loggedInPanel.SetActive(show);
        }

        public void ShowRealmSelectPanel(bool show = true)
        {
            realmSelectPanel.SetActive(show);
            loggedInPanel.SetActive(!show);
        }
        
        public void BackToLoggedInPanel()
        {
            loggedInPanel.SetActive(true);
            realmSelectPanel.SetActive(false);
            createPersonaPanel.SetActive(false);
            hallPanel.SetActive(false);
            settingMenuPanel.SetActive(false);
        }

        private async void ShowHallPanel()
        {
            loggedInPanel.SetActive(false);
            createPersonaPanel.SetActive(false);
            hallPanel.SetActive(true);
            await SetPresence(Status.Online);

            // TODO: 初始化战令信息
            // _entitlementUIController.Init();
            // 初始化排行榜信息
            await _leaderboardUIController.Init();
            // 初始化防沉迷信息
            await _antiAddictionUIController.Init();
        }

        private void ShowCreatePersonaPanel()
        {
            // 生成随机名称
            createPersonaInput.GetComponent<TMP_InputField>().text = GetRandomName();
            loggedInPanel.SetActive(false);
            createPersonaPanel.SetActive(true);
        }
        #endregion

        private async void Start()
        {
            _instance = this;
            // 关闭所有面板
            CloseAllPanels();
            // _entitlementUIController = entitlementPanel.GetComponent<EntitlementUIController>();
            _leaderboardUIController = leaderboardPanel.GetComponent<LeaderboardUIController>();
            _antiAddictionUIController = antiAddictionPanel.GetComponent<AntiAddictionUIController>();
            
            Application.runInBackground = true;
            // SDK 初始化
            await PassportFeatureSDK.Initialize();
            
#if UNITY_WEIXINMINIGAME
            // external login
            string userId = "12345"; // 需要登录的外部系统的用户Id
            string personaId = "45678"; // 可选, 需要登录的 外部系统的角色ID
            string personaDisplayName = "external"; // 可选, 需要登录的角色的昵称。
            await AuthTokenManager.ExternalLogin(userId, personaId, personaDisplayName);
            _callback(PassportEvent.Completed);
#else
            // passport login
            await PassportSDK.Initialize();
            await PassportUI.Init(_config, _callback);
#endif
            
            SelectRealm.AddListener(OnSelectRealm);
        }

        /// <summary>
        /// 获取玩家信息
        /// </summary>
        private async Task GetPlayerInfo()
        {
            var userInfo = await PassportSDK.Identity.GetUserProfileInfo();
            userId.text = $"{userInfo.Name}";
            if (userInfo.AvatarUrl != "")
            {
                StartCoroutine(DownSprite(userInfo.AvatarUrl));
            }
            else
            {
                userAvatar.sprite = defaultAvatar;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            ShowPreLoginPanel(false);
            PassportUI.Login();
        }

        /// <summary>
        /// 登出
        /// </summary>
        public async void Logout()
        {
            await SetOffline();
            PassportUI.Logout();
        }

        /// <summary>
        /// 获取该应用下的域
        /// </summary>
        private async void GetRealm()
        {
            DestroyAllChildren(realmListContent.transform);
            try
            {
                var list = await PassportSDK.Identity.GetRealms();
                if (!list.Any())
                {
                    UIMessage.Show("请到网站上进行域配置(Passport -> 域管理)", MessageType.Error);
                    return;
                }

                OnSelectRealm(list[0]);
                foreach (var item in list)
                {
                    GameObject obj = Instantiate(realmPrefab, realmListContent.transform);
                    obj.GetComponent<RealmItem>().Set(item);
                }
            }
            catch (PassportException e)
            {
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 选择服务器/域
        /// </summary>
        /// <param name="realm"></param>
        private void OnSelectRealm(Realm realm)
        {
            _currentRealm = realm.RealmID;
            currentRealmText.text = realm.Name;
            BackToLoggedInPanel();
        }

        /// <summary>
        /// 开始游戏，选中角色，如果没有角色则展示新建角色面板
        /// </summary>
        public async void StartGame()
        {
            var persona = await PassportSDK.Identity.GetPersonaByRealm(_currentRealm);
            
            if (persona != null)
            {
                await OnSelectPersona(persona);
                // 展示已有角色面板
                ShowHallPanel();
            }
            else
            {
                // 展示创建新角色面板
                ShowCreatePersonaPanel();
            }
        }

        /// <summary>
        /// 在当前用户+选中的域下创建角色
        /// </summary>
        public async void CreatePersona()
        {
            var displayName = createPersonaInput.GetComponent<TMP_InputField>().text;
            if (displayName == "")
            {
                UIMessage.Show("请输入角色昵称", MessageType.Error);
                return;
            }

            try
            {
                var persona = await PassportSDK.Identity.CreatePersona(displayName, _currentRealm);
                await OnSelectPersona(persona);
                ShowHallPanel();
            }
            catch (PassportException e)
            {
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 选择某个角色
        /// </summary>
        /// <param name="persona"></param>
        private async Task OnSelectPersona(Persona persona)
        {
            try
            {
                _currentPersona = persona;
                await PassportSDK.Identity.SelectPersona(persona.PersonaID);
                personaName.text = $"{persona.DisplayName}";
                personaID.text = $"角色 ID：{persona.PersonaID}";
            }
            catch (PassportException e)
            {
                Debug.Log(e.Code);
            }
        }

        public void CopyPersonaID()
        {
            Utils.Utils.Copy(_currentPersona.PersonaID);
        }

        /// <summary>
        /// 删除 Transform 下的所有子对象
        /// </summary>
        /// <param name="parent"></param>
        private void DestroyAllChildren(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 处理用户登出逻辑
        /// </summary>
        private void OnLoggedOut()
        {
            ShowPreLoginPanel(false);
            // 清空一下信息
            userId.text = "";
            personaName.text = "";
            personaID.text = "";
        }

        IEnumerator DownSprite(string url)
        {
            // 协议替换
            url = url.Replace("http://", "https://");
            UnityWebRequest wr = new UnityWebRequest(url);
            DownloadHandlerTexture texD1 = new DownloadHandlerTexture(true);
            wr.downloadHandler = texD1;

            yield return wr.SendWebRequest();

            if (wr.error == null)
            {
                var width = texD1.texture.width;
                var height = texD1.texture.height;
                Texture2D tex = texD1.texture;

                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                userAvatar.sprite = sprite;
            }
        }

        public void ShowMessage(string msg)
        {
            UIMessage.Show(msg);
        }

        /// <summary>
        /// 关闭所有面板
        /// </summary>
        private void CloseAllPanels()
        {
            preLoginPanel.SetActive(false);
            loggedInPanel.SetActive(false);
            realmSelectPanel.SetActive(false);
            createPersonaPanel.SetActive(false);
            hallPanel.SetActive(false);
            settingMenuPanel.SetActive(false);
            // entitlementPanel.SetActive(false);
            guildPanel.SetActive(false);
        }

        private string GetRandomName()
        {
            if (_personaNameList == null)
            {
                // 生成 persona 名字列表
                _personaNameList = personaNameList.Split("、");
            }

            var index = Random.Range(0, _personaNameList.Length - 1);
            var number = Random.Range(1000, 9999);
            
            return $"{_personaNameList[index]}-{number}";
        }
        
        /// <summary>
        /// 设置玩家在线状态
        /// </summary>
        /// <param name="status"></param>
        private async Task SetPresence(Status status)
        {
            var presence = new PassportFriendPresence()
            {
                status = status.ToString(),
                Properties = new Dictionary<string, string>()
            };
            try
            {
                await PassportFeatureSDK.Friends.SetPresence(presence);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e);
            }
        }
        
        /// <summary>
        /// 设置玩家离线状态
        /// </summary>
        private async Task SetOffline()
        {
            if (_currentPersona != null)
            {
                await SetPresence(Status.Offline);
            }
            _currentPersona = null;
        }
        
        private async void OnApplicationQuit()
        {
            await SetOffline();
        }

    }
}