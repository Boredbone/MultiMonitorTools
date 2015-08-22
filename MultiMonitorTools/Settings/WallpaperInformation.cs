using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MultiMonitorTools.Native;

namespace MultiMonitorTools.Settings
{
    /// <summary>
    /// 背景画像のパスと表示方法のコンテナ
    /// </summary>
    [DataContract]
    public class WallpaperInformation : INotifyPropertyChanged
    {

        private string _fieldPath;
        /// <summary>
        /// 背景画像のパス
        /// </summary>
        [DataMember]
        public string Path
        {
            get { return _fieldPath; }
            set
            {
                if (_fieldPath != value)
                {
                    _fieldPath = value;
                    RaisePropertyChanged(nameof(Path));
                }
            }
        }


        private DesktopWallpaperPosition _fieldPosition;
        /// <summary>
        /// 画像の表示方法
        /// </summary>
        [DataMember]
        public DesktopWallpaperPosition Position
        {
            get { return _fieldPosition; }
            set
            {
                if (_fieldPosition != value)
                {
                    _fieldPosition = value;
                    RaisePropertyChanged(nameof(Position));
                }
            }
        }



        public WallpaperInformation(string wallpaperPath)
        {
            this.Path = wallpaperPath;
            this.Position = DesktopWallpaperPosition.Fill;

        }

        /// <summary>
        /// Deep Copy
        /// </summary>
        /// <returns></returns>
        public WallpaperInformation Clone()
        {
            return new WallpaperInformation(this.Path)
            {
                Position = this.Position,
            };
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
