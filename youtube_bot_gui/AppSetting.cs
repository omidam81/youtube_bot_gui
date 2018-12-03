using System;
using System.Configuration;

namespace youtube_bot_gui
{
    public class AppSetting
    {
        public static string DirectoryPath { get { return GetKeyValue("FilePath"); } set { SetKeyValue("FilePath", value); } }

        public static int MinDelay
        {
            get { return Convert.ToInt32(GetKeyValue("minTime")); }
            set { SetKeyValue("minTime", value.ToString()); }
        }

        public static int MinViewCount
        {
            get { return Convert.ToInt32(GetKeyValue("MinViewCount")); }
            set { SetKeyValue("MinViewCount", value.ToString()); }
        }

        public static int MaxDelay
        {
            get { return Convert.ToInt32(GetKeyValue("maxTime")); }
            set { SetKeyValue("maxTime", value.ToString()); }
        }
        public static bool AutoSatrt
        {
            get { return Convert.ToBoolean(GetKeyValue("autostart")); }
            set { SetKeyValue("autostart", value.ToString()); }
        }
        private static string GetKeyValue(string name)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return configuration.AppSettings.Settings[name].Value;
        }
        private static void SetKeyValue(string name, string val)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[name].Value = val;
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");

        }

        public static int ThreadCount
        {
            get { return Convert.ToInt32(GetKeyValue("ThreadCount")); }
            set { SetKeyValue("ThreadCount", value.ToString()); }
        }



        public static int DelayMinute
        {
            get { return Convert.ToInt32(GetKeyValue("DelayMinute")); }
            set { SetKeyValue("DelayMinute", value.ToString()); }
        }

        public static int MaxF
        {
            get { return Convert.ToInt32(GetKeyValue("MaxF")); }
            set { SetKeyValue("MaxF", value.ToString()); }
        }
        public static int MinU
        {
            get { return Convert.ToInt32(GetKeyValue("MinU")); }
            set { SetKeyValue("MinU", value.ToString()); }
        }

        public static int MaxU
        {
            get { return Convert.ToInt32(GetKeyValue("MaxU")); }
            set { SetKeyValue("MaxU", value.ToString()); }
        }
    }
}
