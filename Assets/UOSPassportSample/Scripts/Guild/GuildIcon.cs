using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace Unity.Passport.Sample.Scripts
{
    public class GuildIcon : MonoBehaviour
    {
        // 公会头像类型
        public enum Type
        {
            Vanguard = 0,
            Nexus = 1,
            Ascendants = 2,
            Valiant = 3,
            Enigma = 4
        }
        
        [Header("公会头像")] public List<Sprite> guildFlags;
        public List<Sprite> guildSymbols;

        public Image guildFlag;
        public Image guildSymbol;
        private Type _type = Type.Ascendants;

        public void Init(Guild.GuildInfo guildInfo)
        {
            // 设置公会头像
            if (Enum.TryParse<Type>(guildInfo.GuildType, out var guildType))
            {
                int index = (int)guildType;
                guildFlag.sprite = guildFlags[index];
                guildSymbol.sprite = guildSymbols[index];
                _type = guildType;
            }
        }
        
        public void Init(Guild.GuildInfoWithCount guildInfo)
        {
            // 设置公会头像
            if (Enum.TryParse<Type>(guildInfo.GuildType, out var guildType))
            {
                int index = (int)guildType;
                guildFlag.sprite = guildFlags[index];
                guildSymbol.sprite = guildSymbols[index];
                _type = guildType;
            }
        }
        
        public void Init(Guild.CurrentGuildInfo guildInfo)
        {
            // 设置公会头像
            if (Enum.TryParse<Type>(guildInfo.GuildType, out var guildType))
            {
                int index = (int)guildType;
                guildFlag.sprite = guildFlags[index];
                guildSymbol.sprite = guildSymbols[index];
                _type = guildType;
            }
        }

        public void Clear()
        {
            int index = (int)Type.Ascendants;
            guildFlag.sprite = guildFlags[index];
            guildSymbol.sprite = guildSymbols[index];
        }

        public Type GetIconType()
        {
            return _type;
        }
    }
}