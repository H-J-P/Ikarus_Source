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
    /// Interaktionslogik für MasterCaution.xaml
    /// </summary>
    public partial class MasterCaution : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public MasterCaution()
        {
            InitializeComponent();
        }

        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(39, 87); }
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double value = 0.0;

                           try { if (vals.Length > 0) { value = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); } }
                           catch { return; }

                           this.MasterCaution2.Visibility = value > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                       }));
        }
    }
}
