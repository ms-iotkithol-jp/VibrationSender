using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibrationSender
{
    class SensorSendModel : System.ComponentModel.INotifyPropertyChanged
    {
        private double accelX;
        private double accelY;
        private double accelZ;

        public double AccelX
        {
            get { return accelX; }
            set
            {
                accelX = value;
                OnPropertyChanged("AccelX");
            }
        }
        public double AccelY
        {
            get { return accelY; }
            set
            {
                accelY = value;
                OnPropertyChanged("AccelY");
            }
        }
        public double AccelZ
        {
            get { return accelZ; }
            set
            {
                accelZ = value;
                OnPropertyChanged("AccelZ");
            }
        }

        private int uploadMilliseconds;
        public int UploadDurationMS
        {
            get { return uploadMilliseconds; }
            set
            {
                uploadMilliseconds = value;
                OnPropertyChanged("UploadDurationMS");
            }
        }

        private int uploadCount;
        public int UploadCount
        {
            get { return uploadCount; }
            set
            {
                uploadCount = value;
                OnPropertyChanged("UploadCount");
            }
        }

        private double meanTimePerSend;
        public double MeanTimePerSend
        {
            get { return meanTimePerSend; }
            set
            {
                meanTimePerSend = value;
                OnPropertyChanged("MeanTimePerSend");
            }
        }

        private string sendStartedTime;
        public string SendStartedTime
        {
            get { return sendStartedTime; }
            set
            {
                sendStartedTime = value;
                OnPropertyChanged("SendStartedTime");
            }
        }

        private string connectionString;
        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                connectionString = value;
                OnPropertyChanged("ConnectionString");
            }
        }

        private string sbNamespace;
        public string SBNamespace
        {
            get { return sbNamespace; }
            set
            {
                sbNamespace = value;
                OnPropertyChanged("SBNamespace");
            }
        }

        private string eventHubName;
        public string EventHubName
        {
            get { return eventHubName; }
            set
            {
                eventHubName = value;
                OnPropertyChanged("EventHubName");
            }
        }

        private string accessKey;
        public string AccessKey
        {
            get { return accessKey; }
            set
            {
                accessKey = value;
                OnPropertyChanged("AccessKey");
            }
        }

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propName));
            }
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
