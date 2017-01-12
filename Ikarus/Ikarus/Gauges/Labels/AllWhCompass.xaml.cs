using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    /// <summary>
    /// Interaction logic for AllWhCompass.xaml
    /// </summary>
    public partial class AllWhCompass : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
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

        public AllWhCompass()
        {
            InitializeComponent();
        }

        public System.Windows.Size Size
        {
            get
            {
                return new System.Windows.Size(155.906, 164.41);
            }
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           // [11]  = "%.4f", 		-- KI_13_course {-1,1}
                           // [12]  = "%.4f", 		-- KI_13_pitch {-1,1}
                           // [14]  = "%.4f", 		-- KI_13_bank {-1,1}

                           double heading = 0.0;
                           double pitch = 0.0;
                           double bank = 0.0;

                           try
                           {
                               if (vals.Length > 0)
                                   heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);
                               if (vals.Length > 1)
                                   pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture);
                               if (vals.Length > 2)
                                   bank = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture);
                           }
                           catch { return; }

                           TransformGroup transformGroup = new TransformGroup();
                           RotateTransform rtHeading = new RotateTransform();
                           TranslateTransform ttHeading = new TranslateTransform();

                           ttHeading.X = 153 * (heading < 0 ? -1 - heading : 1 - heading);
                           ttHeading.Y = pitch * 180;
                           rtHeading.Angle = bank * 180;

                           transformGroup.Children.Add(ttHeading);
                           transformGroup.Children.Add(rtHeading);

                           this.course.RenderTransform = transformGroup;
                       }));
        }
    }
}
