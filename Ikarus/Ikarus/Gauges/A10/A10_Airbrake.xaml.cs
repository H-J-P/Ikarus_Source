using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

namespace HawgTouch.GaugePack.Gauges
{
    /// <summary>
    /// Interaction logic for A10_Airbrake.xaml
    /// </summary>
    public partial class A10_Airbrake : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public A10_Airbrake()
        {
            InitializeComponent();
        }
        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(155, 155); }
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double airBrake = 0.0;
                           try
                           {
                               if (vals.Length > 0) { airBrake = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }                                   
                           }
                           catch { return; }

                           RotateTransform rtairBrakeTop = new RotateTransform();
                           rtairBrakeTop.Angle = airBrake * - 60;
                           this.Airbrake_top.RenderTransform = rtairBrakeTop;

                           RotateTransform rtairBrakeButtom = new RotateTransform();
                           rtairBrakeButtom.Angle = airBrake * 60;
                           this.Airbrake_bottom.RenderTransform = rtairBrakeButtom;
                       }));
        }
    }
}
