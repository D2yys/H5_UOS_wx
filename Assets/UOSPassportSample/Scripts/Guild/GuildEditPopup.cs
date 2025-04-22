using System;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
 
 namespace Unity.Passport.Sample.Scripts
{
    public class GuildEditPopup : MonoBehaviour
    {
        private enum Mode
        {
            Create = 0,
            Edit = 1
        }
        public TMP_InputField guildName;
        public TMP_InputField guildAnnouncement;
        public GameObject guildIcon;
        private GuildIcon.Type _guildIconType;
        private readonly Guild.CurrentGuildInfo _guild = new();
        public UnityEvent<Guild.CurrentGuildInfo> onEdit;
        public UnityEvent<Guild.CurrentGuildInfo> onCreate;
        private Mode _mode = Mode.Create;
        public GameObject changeIcon;
        public Toggle needApproval;
        
        /// <summary>
        /// 填充信息
        /// </summary>
        /// <param name="guild"></param>
        public void Edit(Guild.CurrentGuildInfo guild)
        {
            gameObject.SetActive(true);
            _mode = Mode.Edit;
            _guild.Id = guild.Id;
            // UI 显示
            guildName.text = guild.GuildName;
            guildAnnouncement.text = guild.Announcement;
            if (guild.GuildType == "")
            {
                SetDefaultGuildIcon(guild);
            }
            guildIcon.GetComponent<GuildIcon>().Init(guild);
            _guild.GuildType = guild.GuildType;
            needApproval.isOn = guild.EnableGainApprovalBeforeJoin;
            changeIcon.SetActive(false);
        }

        /// <summary>
        /// 获取 icon 的 index
        /// </summary>
        /// <param name="iconType"></param>
        /// <returns></returns>
        private int GetIndex(string iconType)
        {
            if (Enum.TryParse<GuildIcon.Type>(iconType, out var guildType))
            {
                int index = (int)guildType;
                return index;
            }
            return 0;
        }

        /// <summary>
        /// 清空为默认状态
        /// </summary>
        private void Clear()
        {
            // UI 清空
            guildName.text = "uos-default";
            guildAnnouncement.text = "";
            guildIcon.GetComponent<GuildIcon>().Clear();
        }

        public void Create()
        {
            gameObject.SetActive(true);
            changeIcon.SetActive(true);
            Clear();
            _mode = Mode.Create;
            needApproval.isOn = true;
            // 设置 icon
            SetDefaultGuildIcon(_guild);
            guildIcon.GetComponent<GuildIcon>().Init(_guild);
        }

        /// <summary>
        /// 设置为默认值
        /// </summary>
        /// <param name="guild"></param>
        private void SetDefaultGuildIcon(Guild.CurrentGuildInfo guild)
        {
            guild.GuildType = GuildIcon.Type.Ascendants.ToString();
        }

        public void OnConfirm()
        {
            // 数据写入
            _guild.Announcement = guildAnnouncement.text;
            _guild.GuildName = guildName.text;
            _guild.EnableGainApprovalBeforeJoin = needApproval.isOn;

            if (_mode == Mode.Create)
            {
                onCreate.Invoke(_guild);
            }

            if (_mode == Mode.Edit)
            {
                onEdit.Invoke(_guild);
            }
        }

        public void ChangeGuildIcon()
        {
            // 找到下一个值
            int currentIndex = GetIndex(_guild.GuildType);
            int maxIndex = ((int)GuildIcon.Type.Enigma - 1);
            int nextIndex = (currentIndex + 1) % maxIndex;
            var type = (GuildIcon.Type)nextIndex;
            _guild.GuildType = type.ToString();
            guildIcon.GetComponent<GuildIcon>().Init(_guild);
        }
    }
}
