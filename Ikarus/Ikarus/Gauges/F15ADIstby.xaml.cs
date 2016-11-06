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
    /// Interaction logic for USADIstby.xaml
    /// </summary>
    public partial class F15ADIstby : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public F15ADIstby()
        {
            InitializeComponent();

            RotateTransform rtFlagOff = new RotateTransform();
            rtFlagOff.Angle = 12;
            this.flagg_off.RenderTransform = rtFlagOff;
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
                           double pitch = 0.0;
                           double bank = 0.0;
                           double attitudeWarningFlag = 0.0;
                           double manualPitch = 0.0;

                           try
                           {
                               if (vals.Length > 0) { pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { attitudeWarningFlag = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { manualPitch = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                           }
                           catch { return; };

                           TransformGroup grp = new TransformGroup();
                           RotateTransform rt = new RotateTransform();
                           TranslateTransform tt = new TranslateTransform();

                           tt.Y = pitch * 270;
                           rt.Angle = bank * 180;
                           grp.Children.Add(tt);
                           grp.Children.Add(rt);
                           this.bank_pitch.RenderTransform = grp;

                           //RotateTransform rtTurn = new RotateTransform();
                           //rtTurn.Angle = turn * 45;
                           this.turn.RenderTransform = rt;

                           RotateTransform rtFlagOff = new RotateTransform();
                           rtFlagOff.Angle = attitudeWarningFlag * 12;
                           this.flagg_off.RenderTransform = rtFlagOff;
                       }));
        }
    }
}
