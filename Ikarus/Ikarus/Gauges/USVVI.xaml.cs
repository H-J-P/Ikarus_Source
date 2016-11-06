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
    /// Interaction logic for USVVI.xaml
    /// </summary>
    public partial class USVVI : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public USVVI()
        {
            InitializeComponent();
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
                           double vvi = 0.0;
                           const int valueScaleIndex = 7;

                           try
                           {
                               if (vals.Length > 0) { vvi = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                           }
                           catch { return; }

                           // Variometer.input	                           = {-6000,-2000, -1000, 0, 1000,2000, 6000}
                           Double[] valueScale = new double[valueScaleIndex] { -1.0, -0.5, -0.29, 0, 0.29, 0.5, 1.0 };
                           Double[] degreeDial = new double[valueScaleIndex] { -170,  -85,   -50, 0,   50,  85, 170 };

                           RotateTransform rtVVI = new RotateTransform();

                           for (int n = 0; n < (valueScaleIndex - 1); n++)
                           {
                               if (vvi >= valueScale[n] && vvi <= valueScale[n + 1])
                               {
                                   rtVVI.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (vvi - valueScale[n]) + degreeDial[n];
                                   break;
                               }
                           }
                           this.VVI.RenderTransform = rtVVI;
                       }));
        }
    }
}
