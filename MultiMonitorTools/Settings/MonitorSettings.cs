using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorTools.Settings
{
    /// <summary>
    /// 設定
    /// </summary>
    [DataContract]
    public class MonitorSettings : INotifyPropertyChanged
    {
        [DataMember]
        public ObservableCollection<ObservableCollection<WallpaperSettings>> Settings { get; private set; }

        //private int _fieldTargetMonitor;
        ///// <summary>
        ///// Windowsのモニタ番号-1,背景をセットするモニタ
        ///// </summary>
        //[DataMember]
        //public int TargetMonitor
        //{
        //    get { return _fieldTargetMonitor; }
        //    set
        //    {
        //        if (_fieldTargetMonitor != value)
        //        {
        //            _fieldTargetMonitor = value;
        //            RaisePropertyChanged(nameof(TargetMonitor));
        //        }
        //    }
        //}

        private int _fieldActiveSetting;
        /// <summary>
        /// 使用中の設定インデックス
        /// </summary>
        [DataMember]
        public int ActiveSetting
        {
            get { return _fieldActiveSetting; }
            set
            {
                if (_fieldActiveSetting != value)
                {
                    _fieldActiveSetting = value;

                    if (this.Settings != null)
                    {
                        while (this.Settings.Count < value + 1)
                        {
                            this.AddNew(this.Settings.FirstOrDefault()?.Count ?? 1);
                        }
                    }

                    RaisePropertyChanged(nameof(ActiveSetting));
                }
            }
        }

        private int _fieldRotateDevice;
        /// <summary>
        /// 回転対象のデバイスインデックス
        /// </summary>
        [DataMember]
        public int RotateDevice
        {
            get { return _fieldRotateDevice; }
            set
            {
                if (_fieldRotateDevice != value)
                {
                    _fieldRotateDevice = value;
                    RaisePropertyChanged(nameof(RotateDevice));
                }
            }
        }

        public ObservableCollection<WallpaperSettings> Current
            => this.Settings[this.ActiveSetting];

        //public WallpaperSettings TargetMonitorSetting
        //    => this.Settings[this.ActiveSetting][this.TargetMonitor];





        public MonitorSettings(int monitorCount)
        {
            this.Settings = new ObservableCollection<ObservableCollection<WallpaperSettings>>();

            this.AddNew(monitorCount);

            //var defaultSetting = new ObservableCollection<WallpaperSettings>
            //    (Enumerable.Range(0, monitorCount).Select(x => new WallpaperSettings()));

            //this.Settings.Add(defaultSetting);

            //this.TargetMonitor = 0;
            this.RotateDevice = 0;
            this.ActiveSetting = 0;



        }

        public MonitorSettings() : this(0)
        {
        }

        public void AddNew(int monitorCount)
        {

            var defaultSetting = new ObservableCollection<WallpaperSettings>
                (Enumerable.Range(0, monitorCount).Select(x => new WallpaperSettings()));

            this.Settings.Add(defaultSetting);
        }

        public void ExpandMonitorCount(int length)
        {
            foreach (var item in this.Settings)
            {
                while (item.Count < length)
                {
                    item.Add(new WallpaperSettings());
                }
            }
        }

        public MonitorSettings Clone()
        {

            var clone = new MonitorSettings(0);
            clone.Settings.Clear();

            foreach (var list in this.Settings)
            {
                var children = new ObservableCollection<WallpaperSettings>(list.Select(x => x.Clone()));
                clone.Settings.Add(children);
            }

            clone.ActiveSetting = this.ActiveSetting;
            clone.RotateDevice = this.RotateDevice;
            //clone.TargetMonitor = this.TargetMonitor;

            return clone;

        }
        



        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
