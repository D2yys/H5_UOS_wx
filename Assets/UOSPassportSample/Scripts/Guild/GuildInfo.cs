using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts
{
    public enum GuildPermission
    {
        none = 0,
        dismissGuild = 1,
        manageGuildRequests = 2,
        manageMemberRole = 3,
        removeMembers = 4,
        updateGuildInfo = 5
    }

    public class GuildInfo : MonoBehaviour
    {
        [Header("公会头像")] public GameObject guildIcon;

        [Header("公会名称、简介、人数、排名")] public TextMeshProUGUI guildName;
        public TextMeshProUGUI guildAnnouncement;
        public TextMeshProUGUI guildMemberCount;
        public TextMeshProUGUI guildRank;

        [Header("操作")] public Button guildReq;
        public GameObject dismissGuild;
        public GameObject manageGuildRequests;
        public GameObject updateGuildInfo;

        public void Init(Guild.GuildInfoWithCount guildInfo, Guild.GuildConfig config)
        {
            // 设置公会头像
            guildIcon.GetComponent<GuildIcon>().Init(guildInfo);

            guildName.text = guildInfo.GuildName;
            guildAnnouncement.text = guildInfo.Announcement;

            guildMemberCount.text = $"{guildInfo.MemberCount} / {config.MemberLimitCount}";
            if (guildInfo.Properties.TryGetValue("rank", out var rank))
            {
                guildRank.text = rank;
            }

            guildReq.onClick.AddListener(() => { GuildUIController.OnGuildReq.Invoke(guildInfo); });
        }

        public void Init(Guild.CurrentGuildInfo guildInfo, Guild.GuildConfig config)
        {
            // 设置公会头像
            guildIcon.GetComponent<GuildIcon>().Init(guildInfo);

            guildName.text = guildInfo.GuildName;
            guildAnnouncement.text = guildInfo.Announcement;

            guildMemberCount.text = $"{guildInfo.Members.Count} / {config.MemberLimitCount}";
            if (guildInfo.Properties.TryGetValue("rank", out var rank))
            {
                guildRank.text = rank;
            }

            var permissions = guildInfo.SelfRole.Permissions;
            // 解散权限
            dismissGuild.SetActive(permissions.TryGetValue(GuildPermission.dismissGuild.ToString(), out var p1) && p1);
            // 编辑权限
            updateGuildInfo.SetActive(permissions.TryGetValue(GuildPermission.updateGuildInfo.ToString(), out var p2) && p2);
            // 管理入会申请权限
            manageGuildRequests.SetActive(permissions.TryGetValue(GuildPermission.manageGuildRequests.ToString(), out var p3) && p3);
        }
    }
}