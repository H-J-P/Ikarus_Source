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
    public partial class USADIstby : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public USADIstby()
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

            // SAI_Pitch.output			            = {-1.0, 1.0}
            // SAI_Bank.output			            = {-1.0, 1.0}
            // SAI_attitude_warning_flag.output		= {0.0, 1.0}
            // SAI_manual_pitch_adjustment.output	= {-1.0, 1.0}

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double pitch = 0.0;
                           double bank = 0.0;
                           double flagOff = 0.0;
                           double manualPitch = 0.0;

                           try
                           {
                               if (vals.Length > 0) { pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { flagOff = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { manualPitch = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                           }
                           catch { return; };

                           TransformGroup grp = new TransformGroup();
                           RotateTransform rt = new RotateTransform();
                           TranslateTransform tt = new TranslateTransform();

                           tt.Y = pitch * 270;
                           rt.Angle = bank * 190;
                           grp.Children.Add(tt);
                           grp.Children.Add(rt);
                           this.bank_pitch.RenderTransform = grp;

                           //RotateTransform rtTurn = new RotateTransform();
                           //rtTurn.Angle = turn * 45;
                           this.turn.RenderTransform = rt;

                           RotateTransform rtFlagOff = new RotateTransform();
                           rtFlagOff.Angle = flagOff * 23;
                           this.flagg_off.RenderTransform = rtFlagOff;
                       }));
        }
    }
}
