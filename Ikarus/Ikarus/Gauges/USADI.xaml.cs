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
    /// Interaction logic for USADI.xaml
    /// </summary>
    public partial class USADI : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public USADI()
        {
            InitializeComponent();

            //this.Flagg_course_off.Visibility = System.Windows.Visibility.Hidden;
            //this.Flagg_glide_off.Visibility = System.Windows.Visibility.Hidden;
            //this.Flagg_course_off.Visibility = System.Windows.Visibility.Hidden;
            this.Flagg_off.Visibility = System.Windows.Visibility.Visible;
        }

        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(314, 340); }
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double pitch = 0.0;
                           double bank = 0.0;
                           double slipBall = 0.0;
                           double bankSteering = 0.0;
                           double pitchSteering = 0.0;
                           double turnNeedle = 0.0;
                           double glideSlopeIndicator = 0.0;
                           double glideSlopeWarningFlag = 0.0;
                           double attitudeWarningFlag = 0.0;
                           double courceWarningFlag = 0.0;

                           try
                           {
                               if (vals.Length > 0) { pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { slipBall = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { bankSteering = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { pitchSteering = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { turnNeedle = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { glideSlopeIndicator = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { glideSlopeWarningFlag = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { attitudeWarningFlag = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { courceWarningFlag = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }
                           }
                           catch { return; };

                           TransformGroup grp = new TransformGroup();
                           RotateTransform rt = new RotateTransform();
                           TranslateTransform tt = new TranslateTransform();

                           tt.Y = pitch * -270;
                           rt.Angle = bank * -180;
                           grp.Children.Add(tt);
                           grp.Children.Add(rt);

                           this.pitch.RenderTransform = grp;
                           this.bank.RenderTransform = rt;

                           RotateTransform rtSlipball = new RotateTransform();
                           rtSlipball.Angle = slipBall * -9;
                           this.SlipBallPosition.RenderTransform = rtSlipball;

                           TranslateTransform ttTurnIndicator = new TranslateTransform();
                           ttTurnIndicator.X = turnNeedle * 40;
                           this.Turnindicator.RenderTransform = ttTurnIndicator;

                           TranslateTransform rtpitchSteering = new TranslateTransform();
                           rtpitchSteering.Y = pitchSteering * -60;
                           this.pichsteering.RenderTransform = rtpitchSteering;

                           TranslateTransform rtbanksteering = new TranslateTransform();
                           rtbanksteering.X = bankSteering * 80;
                           this.banksteering.RenderTransform = rtbanksteering;

                           TranslateTransform rtglideSlopeIndicator = new TranslateTransform();
                           rtglideSlopeIndicator.Y = glideSlopeIndicator * -55;
                           this.GlideSlopeIndicator.RenderTransform = rtglideSlopeIndicator;

                           this.Flagg_course_off.Visibility = (courceWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                           this.Flagg_glide_off.Visibility = (glideSlopeWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                           this.Flagg_off.Visibility = (attitudeWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                       }));
        }
    }
}
