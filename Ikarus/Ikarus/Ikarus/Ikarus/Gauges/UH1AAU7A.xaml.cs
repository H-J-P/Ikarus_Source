using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class UH1AAU7A : UserControl, IHawgTouchGauge
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

        public UH1AAU7A(/*string strDataImportID*/)
        {
            InitializeComponent();
            //_DataImportID = strDataImportID;
        }


        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double value10000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);
                           double value1000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture);
                           double value100 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture);
                           double valuePressure = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture);
                           RotateTransform rt10000 = new RotateTransform();
                           RotateTransform rt1000 = new RotateTransform();
                           RotateTransform rt100 = new RotateTransform();
                           RotateTransform rtPressure = new RotateTransform();

                           if (value10000 > 0.0 && value10000 <= 1.0)
                           {
                               double startAngle = 0.0;
                               double endAngle = 360;
                               double startValue = 0.0;
                               double endValue = 1.0;
                               rt10000.Angle = (startAngle - endAngle) / (startValue - endValue) * (value10000 - startValue) + startAngle;
                           }
                           else
                           {
                               rt10000.Angle = 0.0;
                           }

                           if (value1000 > 0.0 && value1000 <= 1.0)
                           {
                               double startAngle = 0.0;
                               double endAngle = 360;
                               double startValue = 0.0;
                               double endValue = 1.0;
                               rt1000.Angle = (startAngle - endAngle) / (startValue - endValue) * (value1000 - startValue) + startAngle;
                           }
                           else
                           {
                               rt1000.Angle = 0.0;
                           }

                           if (value100 > 0.0 && value100 <= 1.0)
                           {
                               double startAngle = 0.0;
                               double endAngle = 360;
                               double startValue = 0.0;
                               double endValue = 1.0;
                               rt100.Angle = (startAngle - endAngle) / (startValue - endValue) * (value100 - startValue) + startAngle;
                           }
                           else
                           {
                               rt100.Angle = 0.0;
                           }

                           if (valuePressure > 0.0 && valuePressure <= 1.0)
                           {
                               double startAngle = 0.0;
                               double endAngle = 360;
                               double startValue = 0;
                               double endValue = 1.0;
                               rtPressure.Angle = (startAngle - endAngle) / (startValue - endValue) * (valuePressure - startValue) + startAngle;
                           }
                           else
                           {
                               rtPressure.Angle = 0.0;
                           }

                           this.Needle10000.RenderTransform = rt10000;
                           this.Needle1000.RenderTransform = rt1000;
                           this.Needle100.RenderTransform = rt100;
                           this.NeedlePressure.RenderTransform = rtPressure;
                       }));
        }
    }
}
