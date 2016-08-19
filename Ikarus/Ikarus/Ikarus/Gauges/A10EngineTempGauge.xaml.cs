using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class A10EngineTempGauge : UserControl, IHawgTouchGauge
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

        public A10EngineTempGauge(/*string strDataImportID*/)
        {
            InitializeComponent();
           // _DataImportID = strDataImportID;
        }


        public void UpdateGauge(string strData)
        {
            double angle = Convert.ToDouble(strData, CultureInfo.InvariantCulture);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           RotateTransform rt = new RotateTransform();
                           rt.Angle = angle * 230;
                           this.TempNeedle.RenderTransform = rt;
                       }));

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                      (Action)(() =>
                      {
                          RotateTransform rt = new RotateTransform();
                          rt.Angle = angle * 3600 + 145;
                          this.TenNeedle.RenderTransform = rt;
                      }));
        }
    }
}
