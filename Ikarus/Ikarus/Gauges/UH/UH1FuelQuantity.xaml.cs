using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class UH1FuelQuantity : UserControl, IHawgTouchGauge
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

        public UH1FuelQuantity(/*string strDataImportID*/)
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
                           RotateTransform rt = new RotateTransform();

                           if (value > 0.0 && value <= 1.0)
                           {
                               double startAngle = 0.0;
                               double endAngle = 315;
                               double startValue = 0.0;
                               double endValue = 1.0;
                               rt.Angle = (startAngle - endAngle) / (startValue - endValue) * (value - startValue) + startAngle;
                           }
                           else
                           {
                               rt.Angle = 0.0;
                           }

                           this.SpeedNeedle.RenderTransform = rt;
                       }));
        }
    }
}
