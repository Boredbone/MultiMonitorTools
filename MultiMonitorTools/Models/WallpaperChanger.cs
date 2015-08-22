using System;
using System.Threading;
using System.Threading.Tasks;
using MultiMonitorTools.Settings;
using MultiMonitorTools.Native;
using Boredbone.Utility.Extensions;
using System.Reactive.Disposables;
using System.Windows.Forms;

namespace MultiMonitorTools.Models
{
    /// <summary>
    /// 壁紙の設定を行うクラス
    /// </summary>
    public class WallpaperChanger
    {
        /// <summary>
        /// システムが把握しているディスプレイの総数
        /// </summary>
        public int MonitorCount { get; set; }

        private int blackoutTime = 150;
        

        public WallpaperChanger()
        {
            var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
            MonitorCount = (int)wallpaper.GetMonitorDevicePathCount();
        }

        /// <summary>
        /// ディスプレイを回転した後、画面の向きに対応した壁紙を設定する
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="increment"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async Task RotateAndChangeWallPaperAsync(MonitorSettings settings, int increment, IObserver<int> progress)
        {
            if (MonitorCount == 0)
            {
                return;
            }

            //画面を回転させるオブジェクトを取得
            var rotate = DisplayRotate.Generate(settings.RotateDevice % MonitorCount);

            if (rotate == null)
            {
                return;
            }
            

            //回転中は画面がちらつくので暗転させる
            using (increment != 0 ? new DisplayBlackOut(blackoutTime) : Disposable.Empty)//
            {
                await Task.Delay(100);

                //画面を回転
                rotate.Rotate(increment);

                await Task.Delay(100);

                //壁紙を設定
                this.RefreshWallpaper(settings);

                await Task.Delay(1000);
            }

            progress.OnNext(1);

            if (increment != 0)
            {
                //壁紙が正しく変更されないことがあるので、時間がたったら再度壁紙を設定
                await Task.Delay(2500);
                this.RefreshWallpaper(settings);
            }

            progress.OnCompleted();

            /*
            await Task.Run(async () =>
            {

                //回転中は画面がちらつくので暗転させる
                using (increment != 0 ? new DisplayBlackOut(blackoutTime) : Disposable.Empty)
                {
                    //画面を回転
                    rotate.Rotate(increment);

                    //壁紙を設定
                    this.RefreshWallpaper(settings);
                }

                progress.OnNext(1);

                if (increment != 0)
                {
                    //壁紙が正しく変更されないことがあるので、時間がたったら再度壁紙を設定
                    await Task.Delay(3000);
                    this.RefreshWallpaper(settings);
                }

                progress.OnCompleted();
            });*/
        }

        /// <summary>
        /// 全モニタの壁紙を再設定
        /// </summary>
        /// <param name="settings"></param>
        public void RefreshWallpaper(MonitorSettings settings)
        {
            for (int monitorIndex = 0; monitorIndex < MonitorCount; monitorIndex++)
            {
                var mode = new DeviceMode(monitorIndex);

                //デバイス情報を取得できないか、設定が存在しない場合はスキップ
                if (!mode.IsSucceeded || !settings.Current.ContainsIndex(monitorIndex))
                {
                    continue;
                }

                //壁紙のファイルパスと表示方法の設定を読み込み
                var setting = settings.Current[monitorIndex]
                    .OrientationSettings[(DisplayOrientations)mode.Mode.dmDisplayOrientation];

                //壁紙を設定
                if (setting != null && setting.Path != null && setting.Path.Length > 0)
                {
                    this.ChangeWallpaper(monitorIndex, setting.Path, setting.Position);
                }
            }
        }

        /// <summary>
        /// デスクトップの壁紙を変更
        /// </summary>
        /// <param name="monitorIndex">モニタ番号</param>
        /// <param name="filePath">画像ファイルのパス</param>
        /// <param name="position">壁紙の表示方法</param>
        public void ChangeWallpaper(int monitorIndex, string filePath, DesktopWallpaperPosition position)
        {

            //壁紙を設定するためのオブジェクトを生成
            var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());

            //モニタのデバイスパスを取得
            var id = wallpaper.GetMonitorDevicePathAt((uint)monitorIndex);

            try
            {
                //壁紙を変更
                wallpaper.SetWallpaper(id, filePath);
            }
            catch (Exception)
            {
                //TODO 例外処理
                //ファイルが存在しない場合
                //壁紙の設定に失敗した場合
            }

            //壁紙の表示方法を設定
            wallpaper.SetPosition(position);

        }
    }
}
