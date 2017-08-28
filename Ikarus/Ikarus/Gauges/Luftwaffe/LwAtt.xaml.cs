using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für LwAtt.xaml   Author: HJP
    /// </summary>
    public partial class LwAtt : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double bank = 0.0;
        double pitch = 0.0;
        double turn = 0.0;
        double slipball = 0.0;
        double horizonCage = 0.0;

        double lbank = 0.0;
        double lpitch = 0.0;
        double lturn = 0.0;
        double lslipball = 0.0;
        double lhorizonCage = 0.0;

        TransformGroup grp = new TransformGroup();
        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();

        RotateTransform rtTurn = new RotateTransform();
        RotateTransform rtSlipball = new RotateTransform();
        RotateTransform rtHorizonCage = new RotateTransform();

        public LwAtt()
        {
            InitializeComponent();
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;

            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            helper.LoadBmaps(Frame, Light);

            SwitchLight(false);

            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
        }

        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
        }

        public void SetOutput(string _output)
        {
        }

        public double GetSize()
        {
            return Frame.Width;
        }

        public double GetSizeY()
        {
            return Frame.Height;
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { bank = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { turn = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { slipball = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { horizonCage = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (lbank != bank || lpitch != pitch)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = pitch * 35;
                                   rt.Angle = bank * 180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);
                                   Lw_ATT_Needle_hor.RenderTransform = grp;
                               }
                               if (lturn != turn)
                               {
                                   rtTurn.Angle = turn * 15;
                                   Lw_ATT_Needle_turn.RenderTransform = rtTurn;
                               }
                               if (lslipball != slipball)
                               {
                                   rtSlipball.Angle = slipball * -18;
                                   Lw_ATT_Needle_bank.RenderTransform = rtSlipball;
                               }
                               if (lhorizonCage != horizonCage)
                               {
                                   rtHorizonCage.Angle = horizonCage * 360;
                                   Lw_ATT_Bezel_cage.RenderTransform = rtHorizonCage;
                               }
                               lbank = bank;
                               lpitch = pitch;
                               lturn = turn;
                               lslipball = slipball;
                               lhorizonCage = horizonCage;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
