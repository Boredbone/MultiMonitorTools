using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiMonitorTools.Settings;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Boredbone.Utility.Extensions;
using System.Reactive.Linq;
using Microsoft.Win32;
using MultiMonitorTools.Native;

namespace MultiMonitorTools.ViewModels
{
    class SettingWindowViewModel : ViewModelBase
    {

        private MonitorSettings Settings { get; set; }

        private ObservableCollection<WallpaperItemViewModel> _fieldCurrentList;
        public ObservableCollection<WallpaperItemViewModel> CurrentList
        {
            get { return _fieldCurrentList; }
            set
            {
                if (_fieldCurrentList != value)
                {
                    if (_fieldCurrentList != null)
                    {
                        this._fieldCurrentList.ForEach(y => y.Dispose());
                    }
                    _fieldCurrentList = value;
                    RaisePropertyChanged(nameof(CurrentList));
                }
            }
        }
        

        public ReactiveProperty<int> SettingIndex { get; }
        public ReactiveProperty<int> MonitorIndex { get; }
        public ReactiveProperty<int> DeviceIndex { get; }

        public List<string> SettingSelector { get; }
        public List<string> MonitorSelector { get; private set; }
        public List<string> DeviceSelector { get; }


        public SettingWindowViewModel()
        {

            this.Settings = AppData.Current.MonitorSettings.Clone();

            this.SettingIndex = this.Settings.ToReactivePropertyAsSynchronized(x => x.ActiveSetting).AddTo(this.unsubscribers);
            this.MonitorIndex = new ReactiveProperty<int>().AddTo(this.unsubscribers);
            this.DeviceIndex = this.Settings.ToReactivePropertyAsSynchronized(x => x.RotateDevice).AddTo(this.unsubscribers);

            this.SettingIndex.CombineLatest(this.MonitorIndex, (S, M) => new { S, M })
                .Subscribe(x =>
                {

                    if (x.M < 0)
                    {
                        return;
                    }

                    this.Settings.ExpandMonitorCount(x.M + 1);

                    this.CurrentList = new ObservableCollection<WallpaperItemViewModel>
                    (this.Settings.Current[x.M].OrientationSettings.Select(y => new WallpaperItemViewModel(y.Key, y.Value)));


                }).AddTo(this.unsubscribers);
            

            this.SettingSelector = Enumerable.Range(0, this.Settings.Settings.Count).Select(x => $"Setting No. {x + 1}").ToList();
            this.DeviceSelector = Enumerable.Range(0, AppData.Current.DeviceCount).Select(x => $"Device No. {x + 1}").ToList();

            this.SettingIndex.Subscribe(_=>
            {
                this.MonitorSelector = Enumerable.Range(0, this.Settings.Current.Count).Select(y => $"Monitor No. {y + 1}").ToList();
            }).AddTo(this.unsubscribers);
        }


        public void Commit()
        {
            AppData.Current.MonitorSettings = this.Settings;
            AppData.Current.Save();
            AppData.Current.RefreshWallpaper();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.CurrentList = null;
        }
    }

    /// <summary>
    /// 各壁紙画像の設定
    /// </summary>
    class WallpaperItemViewModel : ViewModelBase
    {

        public DisplayOrientations Orientation { get; }
        public WallpaperInformation Source { get; }

        public ReactiveCommand FileSelectCommand { get; }

        private static List<WallpaperPositionItem> _positionsList;
        public List<WallpaperPositionItem> PositionsList => _positionsList;

        public ReactiveProperty<int> PositionIndex { get; }



        public WallpaperItemViewModel(DisplayOrientations orientation, WallpaperInformation source)
        {
            this.Orientation = orientation;
            this.Source = source;

            this.FileSelectCommand = new ReactiveCommand().AddTo(this.unsubscribers);
            this.FileSelectCommand.Subscribe(x =>
            {
                //ファイルを選択しパスを取得

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FilterIndex = 1;
                openFileDialog.Filter
                    = "Image File(*.bmp, *.png, *.jpg, *.jpeg, *.gif)|*.bmp;*.png;*.jpg;*.jpeg;*.gif|All Files (*.*)|*.*";
                var result = openFileDialog.ShowDialog();
                if (result.Value)
                {
                    this.Source.Path = openFileDialog.FileName;
                }
            }).AddTo(this.unsubscribers);

            if (_positionsList == null)
            {
                //壁紙表示方法の一覧をenumから生成

                _positionsList = Enum.GetValues(typeof(DesktopWallpaperPosition))
                    .AsEnumerable<DesktopWallpaperPosition>()
                    .Select(x => new WallpaperPositionItem(x.ToString(), x))
                    .ToList();
            }

            this.PositionIndex = this.Source
                .ToReactivePropertyAsSynchronized(x => x.Position, x => this.PositionsList.FindIndex(y => y.Position == x),
                x => this.PositionsList.ContainsIndex(x) ? this.PositionsList[x].Position : DesktopWallpaperPosition.Fill)
                .AddTo(this.unsubscribers);

        }
    }

    /// <summary>
    /// 壁紙表示方法とその表示名を関連付ける
    /// </summary>
    class WallpaperPositionItem
    {
        public string Name { get; }
        public DesktopWallpaperPosition Position { get; }

        public WallpaperPositionItem(string name,DesktopWallpaperPosition position)
        {
            this.Name = name;
            this.Position = position;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
