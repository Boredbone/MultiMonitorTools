using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Boredbone.Utility
{
    /// <summary>
    /// xmlファイルへのオブジェクトの保存と読み込みを行う
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlSettingManager<T> where T : class, new()
    {
        static readonly string backUpNameHeader = "backup_";
        private string fileName;

        /// <summary>
        /// 保存するファイル名を指定してインスタンスを初期化
        /// </summary>
        /// <param name="fileName">xmlファイル名</param>
        public XmlSettingManager(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// オブジェクトをシリアライズしてxmlファイルに保存
        /// </summary>
        /// <param name="obj"></param>
#if WINDOWS_APP
        public async Task SaveXmlAsync(T obj)
#else
        public void SaveXml(T obj)
#endif
        {

            if (obj == null)
            {
                throw new ArgumentException();
            }


            try
            {

#if WINDOWS_APP
                
                //アプリ固有のフォルダにファイルを生成
                var folder = ApplicationData.Current.LocalFolder;

                var file = await folder.CreateFileAsync
                    (fileName, CreationCollisionOption.ReplaceExisting);

                //ファイルストリームからライターを生成
                using (var stream = await file.OpenStreamForWriteAsync())
                using (var xw = XmlWriter.Create(stream,
#else
                //ライターを生成
                using (var xw = XmlWriter.Create(this.fileName,
#endif
                    new XmlWriterSettings
                    {
                        Indent = true,
                        Encoding = new System.Text.UTF8Encoding(false)
                    }))
                {
                    //シリアライズして保存
                    var serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(xw, obj);
                    xw.Flush();
                }

            }
            catch
            {
                //例外はそのまま投げる
                throw;
            }
        }

        /// <summary>
        /// xmlファイルを読み込み
        /// ファイルが見つからなかった場合はオブジェクトをnewして返す
        /// 正常に読み込めたらそのファイルを自動でバックアップ
        /// </summary>
        /// <returns></returns>
#if WINDOWS_APP
        public async Task<LoadedObjectContainer<T>> LoadXmlAsync()
        {
            return await LoadXmlAsync(XmlLoadingOptions.UseBackup | XmlLoadingOptions.IgnoreNotFound);
        }
#else
        public LoadedObjectContainer<T> LoadXml()
        {
            return LoadXml(XmlLoadingOptions.UseBackup | XmlLoadingOptions.IgnoreNotFound);
        }
#endif

        /// <summary>
        /// xmlファイルを読み込み
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
#if WINDOWS_APP
        public async Task<LoadedObjectContainer<T>> LoadXmlAsync(XmlLoadingOptions options)
#else
        public LoadedObjectContainer<T> LoadXml(XmlLoadingOptions options)
#endif
        {
            

            Exception errorMessage = null;

            try
            {
                //ファイルから読み込み
#if WINDOWS_APP
                var folder = ApplicationData.Current.LocalFolder;

                var file = await folder.GetFileAsync(this.fileName);

                var loaded = await this.LoadMainAsync(folder, file);
#else
                var loaded = this.LoadMain(this.fileName);
#endif

                //自動バックアップを使用する場合、正常に読み込めたファイルを別名でコピー
                if (options.HasFlag(XmlLoadingOptions.UseBackup))
                {
                    try
                    {
#if WINDOWS_APP
                        var copied = await file.CopyAsync
                            (folder, backUpNameHeader + this.fileName, NameCollisionOption.ReplaceExisting);
#else
                        File.Copy(this.fileName, backUpNameHeader + this.fileName, true);
#endif
                    }
                    catch (Exception e)
                    {
                        return new LoadedObjectContainer<T>(loaded, e);
                    }
                }
                
                //コンテナに入れて返す
                return new LoadedObjectContainer<T>(loaded, null);
            }
            catch (FileNotFoundException)
            {
                //ファイルが存在しない場合

                if (options.HasFlag(XmlLoadingOptions.UseBackup))
                {
                    //バックアップを使用する設定の場合はスルー
                    //errorMessage = null;
                }
                else if (options.HasFlag(XmlLoadingOptions.IgnoreAllException)
                   || options.HasFlag(XmlLoadingOptions.IgnoreNotFound))
                {
                    //例外を無視する設定の場合はnewして返す
                    return new LoadedObjectContainer<T>(
                        (options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()),
                        null);
                }
                else
                {
                    //例外を投げる
                    throw;
                }
            }
            catch (Exception e)
            {
                //その他の例外

                if (options.HasFlag(XmlLoadingOptions.UseBackup))
                {
                    //例外を記憶
                    errorMessage = e;
                }
                else if (options.HasFlag(XmlLoadingOptions.IgnoreAllException))
                {
                    //例外を無視する設定の場合はnewして返す
                    return new LoadedObjectContainer<T>
                        ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), e);
                }
                else
                {
                    //例外を投げる
                    throw;
                }
            }


            //バックアップを使用する設定の場合、バックアップファイルを読み込む
#if WINDOWS_APP
            return await this.LoadBackupMainAsync(options, errorMessage);
#else
            return this.LoadBackupMain(options, errorMessage);
#endif

            //try
            //{
            //    //バックアップファイルを読み込む
            //    var loaded = this.LoadMain(backUpNameHeader + fileName);

            //    return new LoadedObjectContainer<T>(loaded, errorMessage);
            //}
            //catch (FileNotFoundException)
            //{
            //    //バックアップファイルも存在しなかった場合

            //    if (options.HasFlag(XmlLoadingOptions.IgnoreAllException)
            //       || options.HasFlag(XmlLoadingOptions.IgnoreNotFound))
            //    {
            //        //例外を無視する設定の場合はnewして返す
            //        return new LoadedObjectContainer<T>
            //            ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), errorMessage);
            //    }
            //    else
            //    {
            //        //例外を投げる
            //        throw;
            //    }
            //}
            //catch
            //{
            //    //その他の例外

            //    if (options.HasFlag(XmlLoadingOptions.IgnoreAllException))
            //    {
            //        return new LoadedObjectContainer<T>
            //            ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), errorMessage);
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
        }

        /// <summary>
        /// バックアップのxmlファイルを読み込む
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
#if WINDOWS_APP
        public async Task<LoadedObjectContainer<T>> LoadBackupXmlAsync(XmlLoadingOptions options)
        {
            return await this.LoadBackupMainAsync(options, null);
        }
#else
        public LoadedObjectContainer<T> LoadBackupXml(XmlLoadingOptions options)
        {
            return this.LoadBackupMain(options, null);

            //try
            //{
                
            //    var loaded = this.LoadMain(backUpNameHeader + this.fileName);

            //    return new LoadedObjectContainer<T>(loaded, null);
            //}
            //catch (FileNotFoundException)
            //{
            //    if (options.HasFlag(XmlLoadingOptions.IgnoreAllException)
            //       || options.HasFlag(XmlLoadingOptions.IgnoreNotFound))
            //    {
            //        return new LoadedObjectContainer<T>
            //            ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), null);
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
            //catch (Exception e)
            //{
            //    if (options.HasFlag(XmlLoadingOptions.IgnoreAllException))
            //    {
            //        return new LoadedObjectContainer<T>
            //            ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), e);
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
        }
#endif


#if WINDOWS_APP
        public async Task<LoadedObjectContainer<T>> LoadBackupMainAsync(XmlLoadingOptions options, Exception errorMessage)
        {
#else
        private LoadedObjectContainer<T> LoadBackupMain(XmlLoadingOptions options, Exception errorMessage)
#endif
        {

            try
            {
                //バックアップファイルを読み込む
#if WINDOWS_APP
                var folder = ApplicationData.Current.LocalFolder;

                var file = await folder.GetFileAsync(backUpNameHeader + fileName);

                var loaded = await this.LoadMainAsync(folder, file);
#else
                var loaded = this.LoadMain(backUpNameHeader + fileName);
#endif

                return new LoadedObjectContainer<T>(loaded, errorMessage);
            }
            catch (FileNotFoundException)
            {
                //バックアップファイルも存在しなかった場合

                if (options.HasFlag(XmlLoadingOptions.IgnoreAllException)
                   || options.HasFlag(XmlLoadingOptions.IgnoreNotFound))
                {
                    //例外を無視する設定の場合はnewして返す
                    return new LoadedObjectContainer<T>
                        ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), errorMessage);
                }
                else
                {
                    //例外を投げる
                    throw;
                }
            }
            catch (Exception e)
            {
                //その他の例外

                if (options.HasFlag(XmlLoadingOptions.IgnoreAllException))
                {
                    return new LoadedObjectContainer<T>
                        ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), errorMessage ?? e);
                }
                else
                {
                    throw;
                }
            }
        }

#if WINDOWS_APP
        private async Task<T> LoadMainAsync(StorageFolder folder,StorageFile file)
        {
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                var serializer = new DataContractSerializer(typeof(T));
                stream.Position = 0;
                var value = serializer.ReadObject(stream);

                return (T)value;
            }
        }
#else
        private T LoadMain(string name)
        {
            using (var xr = XmlReader.Create(name))
            {
                var serializer = new DataContractSerializer(typeof(T));
                var value = serializer.ReadObject(xr);

                return (T)value;
            }
        }
#endif



    }

    /// <summary>
    /// xmlファイルから読み込まれたオブジェクトと読み込み時に発生した例外の情報
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoadedObjectContainer<T>
    {
        public T Value { get; private set; }
        public Exception Message { get; private set; }

        public LoadedObjectContainer(T value, Exception message)
        {
            this.Value = value;
            this.Message = message;
        }
    }

    [Flags]
    public enum XmlLoadingOptions
    {
        /// <summary>
        /// 発生した全ての例外を投げる
        /// </summary>
        ThrowAll = 0x00,

        /// <summary>
        /// バックアップファイルを使用する
        /// </summary>
        UseBackup = 0x01,

        /// <summary>
        /// FileNotFoundExceptionを内部で処理する
        /// </summary>
        IgnoreNotFound = 0x02,

        /// <summary>
        /// 全ての例外を内部で処理する
        /// </summary>
        IgnoreAllException = 0x04,

        /// <summary>
        /// ロードに失敗した場合はnullを返す
        /// </summary>
        ReturnNull = 0x08,
    }
}
