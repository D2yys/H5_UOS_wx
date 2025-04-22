using UnityEngine;
using Guild;
using TMPro;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts
{
    public class MemberItem : MonoBehaviour
    {
        public TextMeshProUGUI memberName;
        private Member _member;
        // 权限
        public GameObject removeMembers;
        public GameObject manageMemberRole;
        public GameObject assignOwner;
        // 标识自身
        public Sprite selfSprite;
        public Image itemBackground;
        public void Init(Member member, CurrentGuildInfo guildInfo)
        {
            memberName.text = $"{member.MemberName} ({member.Role})";
            _member = member;
            // 角色对应有哪些权限
            var permissions = guildInfo.SelfRole.Permissions;
            // 移除成员权限
            removeMembers.SetActive(permissions.TryGetValue(GuildPermission.removeMembers.ToString(), out var p1) && p1);
            // 管理成员权限
            manageMemberRole.SetActive(permissions.TryGetValue(GuildPermission.manageMemberRole.ToString(), out var p2) && p2);
            assignOwner.SetActive(guildInfo.SelfRole.SlugName == "owner" && member.Role != "owner");
            
            // 是自己
            if (member.MemberId == DemoUIController.Instance.Persona.PersonaID)
            {
                // 修改背景图片
                itemBackground.sprite = selfSprite;
                // 不支持修改自身权限和移除自身
                manageMemberRole.SetActive(false);
                removeMembers.SetActive(false);
            }
        }

        /// <summary>
        /// 删除当前成员
        /// </summary>
        public void HandleDelete()
        {
            GuildUIController.OnDeleteGuildMember.Invoke(_member);
        }

        /// <summary>
        /// 管理当前成员角色
        /// </summary>
        public void HandleManageRole()
        {
            GuildUIController.OnManageMemberRole.Invoke(_member);
        }

        /// <summary>
        /// 指定当前成员为新会长
        /// </summary>
        public void HandleAssignOwner()
        {
            GuildUIController.OnAssignOwner.Invoke(_member);
        }
        
    }
}