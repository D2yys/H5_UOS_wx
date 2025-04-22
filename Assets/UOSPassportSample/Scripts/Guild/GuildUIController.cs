using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guild;
using Unity.Passport.Runtime;
using Unity.Passport.Runtime.UI;
using UnityEngine;
using Passport;
using TMPro;
using UnityEngine.Events;
using Member = Guild.Member;

namespace Unity.Passport.Sample.Scripts
{
    public class GuildUIController : MonoBehaviour
    {
        [Header("当前所在公会信息")] public GameObject guildDetail;
        public GameObject emptyGuild;
        public GameObject guildInfo;
        private GuildInfo _guildInfo;

        [Header("公会成员")] [Tooltip("成员列表")] public Transform memberListContent;
        [Tooltip("成员列表项预制件")] public GameObject memberItemPrefab;
        public GameObject emptyIconPrefab;
        // 删除成员
        public static readonly UnityEvent<Member> OnDeleteGuildMember = new();
        // 管理成员权限
        public static readonly UnityEvent<Member> OnManageMemberRole = new();
        // 会长指定新会长
        public static readonly UnityEvent<Member> OnAssignOwner = new();
        private Member _selectedMember;
        [Tooltip("成员权限管理面板")] public GameObject managerMemberRolePopup;
        public TMP_Dropdown roleSelect;
        private List<GuildRole> _guildRoles;
        private string _selectedGuildRoleSlugName;
        
        [Header("创建/编辑公会")] public GameObject guildEditPopup;
        private GuildEditPopup _guildEditPopup;
        private CurrentGuildInfo _guild;

        [Header("公会列表")] public Transform guildListContent;
        public GameObject guildItemPrefab;
        public GameObject selectedGuildInfo;
        public static readonly UnityEvent<Guild.GuildInfoWithCount, GameObject> OnGuildSelect = new();
        private GameObject _selectedGuildItem;

        [Header("加入公会申请")]
        public GameObject guildReqPopup;
        public TMP_InputField requestMessage;
        public static readonly UnityEvent<Guild.GuildInfoWithCount> OnGuildReq = new();
        private Guild.GuildInfoWithCount _selectedGuildInfo;
        public GameObject guildIcon;
        public GameObject reqCountAlarm; // 入请数量提示
        private float _deltaTime = 0;
        private const float Interval = 10;

        [Header("处理公会申请列表")]
        public static readonly UnityEvent<GuildRequest> OnApproveGuildRequest = new();
        public static readonly UnityEvent<GuildRequest> OnRejectGuildRequest = new();
        public Transform reqListContent;
        public GameObject reqItemPrefab;
        public GameObject reqListPopup;

        [Header("搜索公会")] public TMP_InputField guildName;

        [Header("公会配置")] private GuildConfig _guildConfig;
        

        private Persona Persona => DemoUIController.Instance.Persona;
        private bool _initialized;

        private async void LateUpdate()
        {
            _deltaTime += Time.deltaTime;
            if (_deltaTime >= Interval)
            {
                _deltaTime = 0;
                await UpdateListGuildReqs();
            }
        }

        public async void ShowGuild()
        {
            if (!_initialized)
            {
                Init();
            }

            await GetGuildConfig();
            await GetCurrentGuild();
            gameObject.SetActive(true);
        }

        private void Init()
        {
            _initialized = true;
            _guildInfo = guildInfo.GetComponent<GuildInfo>();
            _guildEditPopup = guildEditPopup.GetComponent<GuildEditPopup>();
            OnGuildSelect.AddListener(ShowGuildDetail);
            OnGuildReq.AddListener(ShowCreateGuildReq);
            OnApproveGuildRequest.AddListener(ApproveGuildRequest);
            OnRejectGuildRequest.AddListener(RejectGuildRequest);
            OnDeleteGuildMember.AddListener(RemoveGuildMember);
            OnManageMemberRole.AddListener(ShowManageMemberRolePopup);
            OnAssignOwner.AddListener(AssignOwner);
        }

        /// <summary>
        /// 展示创建公会面板
        /// </summary>
        public void ShowCreateGuild()
        {
            _guildEditPopup.Create();
        }

        /// <summary>
        /// 展示编辑公会面板
        /// </summary>
        public void ShowEditGuild()
        {
            _guildEditPopup.Edit(_guild);
        }

        /// <summary>
        /// 新建公会
        /// </summary>
        public async void CreateGuild(CurrentGuildInfo guild)
        {
            try
            {
                await PassportFeatureSDK.Guild.CreateGuild(guild.GuildName, 
                    GuildVisibility.Public, 
                    guild.Announcement, 
                    null, 
                    guild.GuildType,
                    guild.EnableGainApprovalBeforeJoin);
                // 关闭面板，刷新
                guildEditPopup.SetActive(false);
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                Debug.Log(e.ErrorMessage);
                UIMessage.Show(e.ErrorMessage);
                if (e.Code == ErrorCode.FailedPrecondition)
                {
                    UIMessage.Show("公会未配置，请先在 UOS Developer Portal 上开启 Passport 服务并配置公会。");
                }
            }
        }

        /// <summary>
        /// 编辑公会
        /// </summary>
        public async void UpdateGuild(CurrentGuildInfo guild)
        {
            try
            {
                await PassportFeatureSDK.Guild.UpdateGuild(guild.Id, guild.GuildName, guild.Announcement,
                    new Dictionary<string, string>(), guild.EnableGainApprovalBeforeJoin);
                // 关闭面板，刷新
                guildEditPopup.SetActive(false);
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                Debug.Log(e.ErrorMessage);
                UIMessage.Show(e.ErrorMessage);
            }
        }

        /// <summary>
        /// 查询所在公会
        /// </summary>
        private async Task GetCurrentGuild()
        {
            try
            {
                var resp = await PassportFeatureSDK.Guild.GetCurrentGuild();
                
                bool empty = !resp.Guilds.Any();
                emptyGuild.SetActive(empty);
                guildDetail.SetActive(!empty);

                if (empty)
                {
                    ListRandomGuilds();
                    _guild = null;
                    return;
                }
                
                _guild = resp.Guilds[0];
                _guildInfo.Init(_guild, _guildConfig);
                var members = new List<Member>();
                foreach (var member in _guild.Members)
                {
                    members.Add(member);
                }
                
                SetupMemberList(memberListContent, members);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 解散公会
        /// </summary>
        public async void DismissGuild()
        {
            try
            {
                await PassportFeatureSDK.Guild.DismissGuild(_guild.Id);
                UIMessage.Show("当前公会已解散");
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 获取随机公会列表
        /// </summary>
        public async void ListRandomGuilds()
        {
            try
            {
                selectedGuildInfo.SetActive(false);
                guildName.text = "";
                var resp = await PassportFeatureSDK.Guild.ListRandomGuilds();
                List<Guild.GuildInfoWithCount> list = new();
                foreach (var guild in resp.Guilds)
                {
                    list.Add(guild);
                }

                SetupGuildList(guildListContent, list);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 展示选中的公会详情
        /// </summary>
        /// <param name="guild"></param>
        /// /// <param name="guildItem"></param>
        private void ShowGuildDetail(Guild.GuildInfoWithCount guild, GameObject guildItem)
        {
            // 设置 focus
            if (_selectedGuildItem != null)
            {
                _selectedGuildItem.GetComponent<GuildItem>().Focus(false);
            }

            _selectedGuildItem = guildItem;
            guildItem.GetComponent<GuildItem>().Focus();

            selectedGuildInfo.GetComponent<GuildInfo>().Init(guild, _guildConfig);
            selectedGuildInfo.SetActive(true);
        }

        /// <summary>
        /// 退出当前公会
        /// </summary>
        public async void LeaveGuild()
        {
            try
            {
                await PassportFeatureSDK.Guild.LeaveGuild(_guild.Id);
                UIMessage.Show("已退出当前公会");
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 展示申请入会弹窗
        /// </summary>
        private void ShowCreateGuildReq(Guild.GuildInfoWithCount guild)
        {
            requestMessage.text = "";
            guildIcon.GetComponent<GuildIcon>().Init(guild);

            guildReqPopup.SetActive(true);
            _selectedGuildInfo = guild;
        }

        /// <summary>
        /// 申请入会
        /// </summary>
        public async void ConfirmCreateGuildReq()
        {
            try
            {
                await PassportFeatureSDK.Guild.JoinGuild(_selectedGuildInfo.Id, requestMessage.text);
                UIMessage.Show("入会申请已发送");
                await GetCurrentGuild();
                guildReqPopup.SetActive(false);
            } 
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 更新接收到的入会申请的数量
        /// </summary>
        /// <returns></returns>
        private async Task<List<GuildRequest>> UpdateListGuildReqs()
        {
            List<GuildRequest> list = new();
            // 检查当前公会不为空
            if (_guild == null)
            {
                reqCountAlarm.SetActive(false);
                return list;
            }
            // 检查权限
            var permissions = _guild.SelfRole.Permissions;
            if (!(permissions.TryGetValue(GuildPermission.manageGuildRequests.ToString(), out var p3) && p3))
            {
                reqCountAlarm.SetActive(false);
                return list;
            }
            
            var resp = await PassportFeatureSDK.Guild.ListJoinGuildRequests(_guild.Id);
            foreach (var req in resp.GuildRequests)
            {
                list.Add(req);
            }
            
            reqCountAlarm.SetActive(list.Any());
            if (list.Any())
            {
                reqCountAlarm.GetComponentInChildren<TextMeshProUGUI>().text = list.Count.ToString();
            }

            return list;
        }

        /// <summary>
        /// 获取入会申请列表
        /// </summary>
        public async void ListGuildReqs()
        {
            try
            {
                var list = await UpdateListGuildReqs();
                SetupGuildReqList(reqListContent, list);
                reqListPopup.SetActive(true);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }
        
        /// <summary>
        /// 通过加入公会申请
        /// </summary>
        /// <param name="req"></param>
        private async void ApproveGuildRequest(GuildRequest req)
        {
            try
            {
                await PassportFeatureSDK.Guild.ApproveJoinGuildRequest(_guild.Id, req.Id.ToString());
                UIMessage.Show("请求已通过");
                ListGuildReqs();
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }
        
        /// <summary>
        /// 拒绝加入公会申请
        /// </summary>
        /// <param name="req"></param>
        private async void RejectGuildRequest(GuildRequest req)
        {
            try
            {
                await PassportFeatureSDK.Guild.RejectJoinGuildRequest(_guild.Id, req.Id.ToString());
                UIMessage.Show("请求已拒绝");
                ListGuildReqs();
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }
        
        /// <summary>
        /// 移除公会成员
        /// </summary>
        private async void RemoveGuildMember(Member member)
        {
            try
            {
                await PassportFeatureSDK.Guild.RemoveGuildMember(_guild.Id, member.MemberId);
                UIMessage.Show("移除成功");
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 编辑成员权限弹框
        /// </summary>
        private async void ShowManageMemberRolePopup(Member member)
        {
            try
            {
                _selectedMember = member;
                var resp = await PassportFeatureSDK.Guild.ListGuildRoles();
                var options = new List<TMP_Dropdown.OptionData>();
                _guildRoles = new();
                
                int selectedIndex = 0;
                for(int i = 0; i < resp.GuildRoles.Count; i += 1)
                {
                    var role = resp.GuildRoles[i];
                    options.Add(new()
                    {
                        text = role.DisplayName,
                    });
                    _guildRoles.Add(role);
                    if (role.SlugName == member.Role)
                    {
                        selectedIndex = i;
                    }
                }
                // 设置选择框当前值和选项
                roleSelect.value = selectedIndex;
                roleSelect.options = options;
                
                managerMemberRolePopup.SetActive(true);
            } 
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 编辑公会成员权限
        /// </summary>
        public async void ConfirmManageMemberRole()
        {
            try
            {
                var role = _guildRoles[roleSelect.value].SlugName;
                await PassportFeatureSDK.Guild.ManageMemberRole(_guild.Id, _selectedMember.MemberId, role);
                UIMessage.Show("修改成功");
                managerMemberRolePopup.SetActive(false);
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 搜索公会
        /// </summary>
        public async void SearchGuilds()
        {
            try
            {
                if (guildName.text == "")
                {
                    return;
                }
                var resp = await PassportFeatureSDK.Guild.SearchGuilds(guildName.text);
                List<Guild.GuildInfoWithCount> list = new();
                foreach (var guild in resp.Guilds)
                {
                    list.Add(guild);
                }
                SetupGuildList(guildListContent, list);
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }

        /// <summary>
        /// 清空搜索框并展示随机公会列表
        /// </summary>
        public void ClearAndSearchRandom()
        {
            if (guildName.text != "")
            {
                guildName.text = "";
                ListRandomGuilds();
            }
        }
        
        /// <summary>
        /// 会长指定任意成员为会长
        /// </summary>
        private async void AssignOwner(Member member)
        {
            try
            {
                await PassportFeatureSDK.Guild.AssignOwner(_guild.Id, member.MemberId);
                UIMessage.Show("会长转让成功");
                await GetCurrentGuild();
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }
        
        /// <summary>
        /// 获取公会配置
        /// </summary>
        private async Task GetGuildConfig()
        {
            try
            {
                var config = await PassportFeatureSDK.Guild.GetGuildConfig();
                _guildConfig = config.GuildConfig;
            }
            catch (PassportException e)
            {
                UIMessage.Show(e.ErrorMessage);
                Debug.Log(e.ErrorMessage);
                Debug.Log(e.Code);
            }
        }
        
        #region Tools
        /// <summary>
        /// 挂载公会成员列表
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="items"></param>
        private void SetupMemberList(Transform parent, List<Member> items)
        {
            DestroyChildren(parent);
            // 如果列表为空，显示空提示
            if (!items.Any())
            {
                Instantiate(emptyIconPrefab, parent);
                return;
            }

            for (int i = 0; i < items.Count; i += 1)
            {
                var item = items[i];
                var obj = Instantiate(memberItemPrefab, parent);
                obj.GetComponent<MemberItem>().Init(item, _guild);
                // obj.GetComponent<Button>().onClick.AddListener(() => { FriendItemClickHandler(friend); });
            }
        }

        /// <summary>
        /// 挂载公会列表
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="items"></param>
        private void SetupGuildList(Transform parent, List<Guild.GuildInfoWithCount> items)
        {
            DestroyChildren(parent);
            // 如果列表为空，显示空提示
            if (!items.Any())
            {
                Instantiate(emptyIconPrefab, parent);
                return;
            }

            for (var i = 0; i < items.Count; i += 1)
            {
                var item = items[i];
                var obj = Instantiate(guildItemPrefab, parent);
                obj.GetComponent<GuildItem>().Init(item);
                if (i == 0)
                {
                    ShowGuildDetail(item, obj);
                }
            }
        }

        /// <summary>
        /// 挂载入会申请列表
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="items"></param>
        private void SetupGuildReqList(Transform parent, List<GuildRequest> items)
        {
            DestroyChildren(parent);
            // 如果列表为空，显示空提示
            if (!items.Any())
            {
                Instantiate(emptyIconPrefab, parent);
                return;
            }

            for (int i = 0; i < items.Count; i += 1)
            {
                var item = items[i];
                var obj = Instantiate(reqItemPrefab, parent);
                obj.GetComponent<ReqItem>().Init(item);
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
    }
}