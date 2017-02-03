using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class UH1ASIGauge : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;

        public System.Windows.Size Size
        {
            get
            {
                return new System.Windows.Size(400, 400);
            }
        }

        public string DataImportID
        {
            get
            {
                return _DataImportID;
            }
            set
            {
                _DataImportID = value;
            }
        }

        public UH1ASIGauge(/*string strDataImportID*/)
        {
            InitializeComponent();
            //_DataImportID = strDataImportID;
        }


        public void UpdateGauge(string strData)
        {
            double value = Convert.ToDouble(strData, CultureInfo.InvariantCulture);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           // (Start Angle - End Angle) / (Start Value - End Value) * (Actual Value - Start Value ) + Start Angle
                           // 150 = 346.2 = 1.0
                           // 120 = 281.7 = 0.825
                           // 80 = 195.4 = 0.55
                           // 60 = 152.3 = 0.44
                           // 50 = 135.5 = 0.395
                           // 40 = 109 = 0.32
                           // 30 = 66.5 = 0.19
                           // 20 = 23 = 0.075
                           // 0 = 0

                           RotateTransform rt = new RotateTransform();
                           double startAngle = 0 ,endAngle = 0,startValue = 0,endValue = 0;
                           rt.Angle = 0.0;
                           if (value > 0.0 && value <= 0.075)
                           {
                               startAngle = 0.0;
                               endAngle = 23.1;
                               startValue = 0.0;
                               endValue = 0.075;
                           }
                           else if (value > 0.075 && value <= 0.19)
                           {
                               startAngle = 23.1;
                               endAngle = 66.5;
                               startValue = 0.075;
                               endValue = 0.19;
                           }
                           else if (value > 0.19 && value <= 0.32)
                           {
                               startAngle = 66.5;
                               endAngle = 109.0;
                               startValue = 0.19;
                               endValue = 0.32;
                           }
                           else if (value > 0.32 && value <= 0.395)
                           {
                               startAngle = 109.0;
                               endAngle = 135.5;
                               startValue = 0.32;
                               endValue = 0.395;
                           }
                           else if (value > 0.395 && value <= 0.44)
                           {
                               startAngle = 135.5;
                               endAngle = 152.3;
                               startValue = 0.395;
                               endValue = 0.44;
                           }
                           else if (value > 0.44 && value <= 0.55)
                           {
                               startAngle = 152.3;
                               endAngle = 195.4;
                               startValue = 0.44;
                               endValue = 0.55;
                           }
                           else if (value > 0.55 && value <= 0.825)
                           {
                               startAngle = 195.4;
                               endAngle = 281.7;
                               startValue = 0.55;
                               endValue = 0.825;
                           }
                           else if (value > 0.825 && value <= 1.0)
                           {
                               startAngle = 281.7;
                               endAngle = 346.2;
                               startValue = 0.825;
                               endValue = 1.0;
                           }

                           rt.Angle = (startAngle - endAngle) / (startValue - endValue) * (value - startValue) + startAngle;
                           this.SpeedNeedle.RenderTransform = rt;
                       }));
        }
    }
}
