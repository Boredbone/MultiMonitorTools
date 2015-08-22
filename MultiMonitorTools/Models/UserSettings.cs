﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Boredbone.Utility;
using MultiMonitorTools.Settings;

namespace MultiMonitorTools
{
    
    class UserSettings
    {
        private string configFileName = "settings.config";

        private XmlSettingManager<MonitorSettings> SettingXml { get; }

        public MonitorSettings MonitorSettings { get; set; }

        public int settingNum = 3;


        public UserSettings()
        {
            this.MonitorSettings = new MonitorSettings();
            this.SettingXml = new XmlSettingManager<MonitorSettings>(this.configFileName);
        }


        public void Save()
        {
            this.SettingXml.SaveXml(this.MonitorSettings);
        }
        

        public void Load(int monitorCount)
        {
            var result = this.SettingXml.LoadXml();
            var obj = result.Value;
            obj.ExpandMonitorCount(monitorCount);
            this.MonitorSettings = obj;

            while (MonitorSettings.Settings.Count < this.settingNum)
            {
                MonitorSettings.AddNew(monitorCount);
            }
        }
    }
}