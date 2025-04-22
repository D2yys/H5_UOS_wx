using System.Collections;
using System.Collections.Generic;
using Guild;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Passport.Sample.Scripts
{
    public class GuildItem : MonoBehaviour
    {
        public GameObject guildIcon;
        public TextMeshProUGUI guildName;
        public TextMeshProUGUI guildMemberCount;
        public GameObject imageFocus;
        
        public void Init(Guild.GuildInfoWithCount guild)
        {
            guildIcon.GetComponent<GuildIcon>().Init(guild);
            guildName.text = guild.GuildName;
            // guildMemberCount.text = $"{guild.Members.Count} / {guild.MemberCount}";
            gameObject.GetComponent<Button>().onClick.AddListener(
                () => { GuildUIController.OnGuildSelect.Invoke(guild, gameObject); });
        }

        public void Focus(bool focus = true)
        {
            imageFocus.SetActive(focus);
        }
    }
}