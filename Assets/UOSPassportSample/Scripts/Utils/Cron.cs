using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace Unity.Passport.Sample.Scripts.Utils
{
    // 支持的语言类型
    public enum LanguageType
    {
        Zh
    }

    // 原始用词
    public enum Wording
    {
        SUN = 0,
        MON,
        TUE,
        WED,
        THU,
        FRI,
        SAT,
        Every,
        Minute,
        Hour,
        DayOfMonth,
        Month,
        HourGap
    }

    public static class WordingExtension
    {
        public static LanguageType Type = LanguageType.Zh;

        private static Dictionary<Wording, string> Zh = new()
        {
            { Wording.Every, "每" },
            { Wording.Minute, "分" },
            { Wording.Hour, "点" },
            { Wording.DayOfMonth, "日" },
            { Wording.Month, "月" },
            { Wording.SUN, "周日" },
            { Wording.MON, "周一" },
            { Wording.TUE, "周二" },
            { Wording.WED, "周三" },
            { Wording.THU, "周四" },
            { Wording.FRI, "周五" },
            { Wording.SAT, "周六" },
            { Wording.HourGap, "小时" }
        };

        public static string GetString(this Wording wording)
        {
            // 根据 languageType 返回
            if (Type == LanguageType.Zh)
            {
                return Zh[wording];
            }

            return "";
        }
    }

    // 将 cron string 翻译成指定的语言
    public class Cron
    {
        // 表达式每一项的类型
        private enum Type
        {
            Minute = 0,
            Hour = 1,
            DayOfMonth = 2,
            Month = 3,
            DayOfWeek = 4,
        };

        // 表达式类型
        private const string Every = "*";

        private static string[] Format(string cron)
        {
            // 处理成标准语句
            var str = cron.Replace("CRON_TZ=Asia/Shanghai ", "");
            string[] exps = str.Split(' ');
            return exps;
        }

        public static string Translate(string cron)
        {
            // 设置语言类型
            WordingExtension.Type = LanguageType.Zh;

            // 5 位 cron 表达式：分 小时 日期 月份 星期
            // 支持的类型：小时 日期 星期

            string res = "";
            string[] exps = Format(cron);

            try
            {
                // 处理间隔类型 "@every 2h"
                if (exps[0] == "@every")
                {
                    // 获取小时数
                    var hour = exps[1].Split("h")[0];
                    return $"{Wording.Every.GetString()}{hour}{Wording.HourGap.GetString()}";
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log("解析@every字符串出错:" + string.Join("", exps));
            }

            try
            {
                // 处理周表达式
                string weekExp = exps[(int)Type.DayOfWeek];
                if (weekExp != Every)
                {
                    // 每周几循环
                    int index = int.Parse(weekExp) % 7; // 周日可能是0或7
                    res += Wording.Every.GetString() + ((Wording)index).GetString();
                }
                else
                {
                    // 处理月份表达式
                    string monthExp = exps[(int)Type.Month];
                    // 仅在没有设置每周循环的情况下生效
                    if (monthExp == Every)
                    {
                        res += Wording.Every.GetString() + Wording.Month.GetString();
                    }
                    else
                    {
                        res += monthExp + Wording.Month.GetString();
                    }

                    string dayOfMonthExp = exps[(int)Type.DayOfMonth];
                    if (dayOfMonthExp == Every)
                    {
                        res += Wording.Every.GetString() + Wording.DayOfMonth.GetString();
                    }
                    else
                    {
                        res += dayOfMonthExp + Wording.DayOfMonth.GetString();
                    }
                }

                // 处理小时表达式
                string hourExp = exps[(int)Type.Hour];
                if (hourExp == Every)
                {
                    res += Wording.Every.GetString() + Wording.Hour.GetString();
                }
                else
                {
                    res += hourExp;
                }

                string minuteExp = exps[(int)Type.Minute];
                if (minuteExp == Every)
                {
                    res += Wording.Every.GetString() + Wording.Minute.GetString();
                }
                else
                {
                    res += ":" + minuteExp.PadLeft(2, '0');
                }

                return res;
            }
            catch (Exception e)
            {
                Debug.Log("解析 CRON 表达式出错。" + e.Message);
            }

            return "";
        }
    }
}