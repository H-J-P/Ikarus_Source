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
    /// Interaction logic for USWhiskeyCompass.xaml
    /// </summary>
    public partial class A10_WhiskeyCompass : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public A10_WhiskeyCompass()
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
                           double heading = 0.0;
                           double pitch = 0.0;
                           double bank = 0.0;

                           try
                           {
                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);}
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { bank = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }                                   
                           }
                           catch { return; }

                           TransformGroup transformGroup = new TransformGroup();
                           RotateTransform rtHeading = new RotateTransform();
                           TranslateTransform ttHeading = new TranslateTransform();

                           ttHeading.X = 153 * (heading < 0 ? -1 - heading : 1 - heading);
                           ttHeading.Y = pitch * 45;
                           rtHeading.Angle = bank * 45;

                           transformGroup.Children.Add(ttHeading);
                           transformGroup.Children.Add(rtHeading);

                           this.Course.RenderTransform = transformGroup;
                       }));
        }
    }
}
