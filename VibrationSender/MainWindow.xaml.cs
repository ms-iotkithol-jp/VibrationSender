using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VibrationSender
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private SensorSendModel sensorSendModel;
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensorSendModel = new SensorSendModel();
            sensorSendModel.UploadDurationMS = 1000;
            sensorSendModel.ConnectionString = App.ServiceBusConnectionString;
            sensorSendModel.EventHubName = App.EventHubName;

            
            this.opPanel.DataContext = sensorSendModel;
            measureTimer = new DispatcherTimer();
            accelerometer = Windows.Devices.Sensors.Accelerometer.GetDefault();
            if (accelerometer == null)
            {
                nonSensorRandom = new Random(DateTime.Now.Millisecond);
                measureTimer.Interval = TimeSpan.FromMilliseconds(1000);
            }
            else
            {
                measureTimer.Interval = TimeSpan.FromMilliseconds(accelerometer.ReportInterval);
            }
            measureTimer.Tick += measureTimer_Tick;
            measureTimer.Start();
        }

        Random nonSensorRandom;
        void measureTimer_Tick(object sender, EventArgs e)
        {
            lock (sensorSendModel)
            {
                if (accelerometer != null)
                {
                    var accel = accelerometer.GetCurrentReading();
                    sensorSendModel.AccelX = accel.AccelerationX;
                    sensorSendModel.AccelY = accel.AccelerationY;
                    sensorSendModel.AccelZ = accel.AccelerationZ;
                }
                else
                {
                    sensorSendModel.AccelX = nonSensorRandom.NextDouble();
                    sensorSendModel.AccelY = nonSensorRandom.NextDouble();
                    sensorSendModel.AccelZ = nonSensorRandom.NextDouble();
                }
            }
        }

        void accelerometer_ReadingChanged(Windows.Devices.Sensors.Accelerometer sender, Windows.Devices.Sensors.AccelerometerReadingChangedEventArgs args)
        {
            lock (sensorSendModel)
            {
                sensorSendModel.AccelX = args.Reading.AccelerationX;
                sensorSendModel.AccelY = args.Reading.AccelerationY;
                sensorSendModel.AccelZ = args.Reading.AccelerationZ;
            }
        }

        private DispatcherTimer measureTimer;
        private DispatcherTimer uploadTimer;
        private Windows.Devices.Sensors.Accelerometer accelerometer;

        private async void buttonSendEH_Click(object sender, RoutedEventArgs e)
        {
            if (uploadTimer == null)
            {
                uploadTimer = new DispatcherTimer();
                uploadTimer.Tick += uploadTimer_Tick;
            }
            uploadTimer.Interval = TimeSpan.FromMilliseconds(sensorSendModel.UploadDurationMS);

            if (buttonSendEH.Content.ToString() == "Send")
            {
                if (ehClient == null)
                {
                    var nsManager = Microsoft.ServiceBus.NamespaceManager.CreateFromConnectionString(sensorSendModel.ConnectionString);
                    var eh = await nsManager.GetEventHubAsync(sensorSendModel.EventHubName);
                    partitionId = eh.PartitionIds[0];
                    ehClient = EventHubClient.CreateFromConnectionString(sensorSendModel.ConnectionString, sensorSendModel.EventHubName);

                    sensorSendModel.UploadCount = 0;
                    sendingTicks = TimeSpan.FromMilliseconds(0);

                    startedTimestamp = DateTime.Now;
                    sensorSendModel.SendStartedTime = startedTimestamp.ToString("yyyy-MM-dd hh:mm:ss.fff");
                }
                uploadTimer.Start();

                buttonSendEH.Content = "Stop";
            }
            else
            {
                uploadTimer.Stop();
                buttonSendEH.Content = "Send";
            }
        }

        EventHubClient ehClient;
        string partitionId;
        DateTime startedTimestamp;
        TimeSpan sendingTicks;
        void uploadTimer_Tick(object sender, EventArgs e)
        {
            buttonSendEH.IsEnabled = false;
            uploadTimer.Stop();
            sensorSendModel.UploadCount++;
            var currentTime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fff");
            var content = "{\"accelx\":" + sensorSendModel.AccelX +
            ",\"accely\":" + sensorSendModel.AccelY +
            ",\"accelz\":" + sensorSendModel.AccelZ +
            ",\"time\":\"" + currentTime + "\""+
            ",\"deviceid\":\"" + App.DeviceId + "\"}";
            var data = new EventData(UTF8Encoding.UTF8.GetBytes(content));
            data.PartitionKey = partitionId;

            var pre = DateTime.Now;
            ehClient.SendAsync(data).Wait();
            var post = DateTime.Now;
            var delta= post - pre;
            sendingTicks += delta;
            sensorSendModel.MeanTimePerSend= sendingTicks.TotalMilliseconds / sensorSendModel.UploadCount;
            uploadTimer.Start();
            buttonSendEH.IsEnabled = true;
        }
    }
}
