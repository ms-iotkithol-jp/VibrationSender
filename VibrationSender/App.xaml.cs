using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VibrationSender
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        // TODO:
        // 作成したイベントハブのサービスバス名前空間の“接続文字列”で“[Service Bus Connection String]”を置き換えてください
        public static string ServiceBusConnectionString = "[Service Bus Connection String]";
        // TODO:
        // 作成したイベントハブの名前で“[EventHub Name]”を置き換えてください
        public static string EventHubName = "[EventHub Name]";
        // TODO:
        // 適当な文字列で“[DeviceId]”を置き換えてください。
        // 同時に複数このアプリを実行する場合は、異なる文字列にしてください
        public static string DeviceId = "[DeviceId]";
    }
}
