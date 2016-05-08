using System;
using System.Collections.Generic;
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
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    /// <summary>
    /// Interaction logic for USAOA.xaml
    /// </summary>
    public partial class F15AOA : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public F15AOA()
        {
            InitializeComponent();

            this.OffFlag.Visibility = System.Windows.Visibility.Visible;
            this.AOAValue.Visibility = System.Windows.Visibility.Hidden;
        }
        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(255, 255); }
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double angleOfAttack = 0.0;
                           double powerOffFlag = 0.0;

                           try
                           {
                               if (vals.Length > 0) { angleOfAttack = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { powerOffFlag = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                           }
                           catch { return; }

                           RotateTransform rtAOA = new RotateTransform();
                           rtAOA.Angle = angleOfAttack * -340;
                           this.AOA.RenderTransform = rtAOA;

                           this.OffFlag.Visibility = (powerOffFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                       }));
        }
    }
}
