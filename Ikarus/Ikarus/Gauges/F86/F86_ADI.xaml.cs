using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F86_ADI.xaml
    /// </summary>
    public partial class F86_ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        double attitudeIndicatorPitch = 0.0;
        double attitudeIndicatorBank = 0.0;
        double attitudeIndicatorPitchSphere = 0.0;
        double attitudeIndicatorBankNeedle = 0.0;
        double attitudeIndicatorOffFlag = 0.0;

        double lattitudeIndicatorPitch = 0.0;
        double lattitudeIndicatorBank = 0.0;
        double lattitudeIndicatorPitchSphere = 0.0;
        double lattitudeIndicatorBankNeedle = 0.0;
        double lattitudeIndicatorOffFlag = 0.0;

        TransformGroup grp = new TransformGroup();
        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();
        RotateTransform rtattitudeIndicatorBankNeedle = new RotateTransform();

        public F86_ADI()
        {
            InitializeComponent();

            AttitudeIndicatorOffFlag.Visibility = System.Windows.Visibility.Visible;
            ValueText.Visibility = System.Windows.Visibility.Hidden;
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
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { attitudeIndicatorPitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { attitudeIndicatorBank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { attitudeIndicatorPitchSphere = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { attitudeIndicatorBankNeedle = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { attitudeIndicatorOffFlag = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (lattitudeIndicatorBank != attitudeIndicatorBank || lattitudeIndicatorPitchSphere != attitudeIndicatorPitchSphere)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = attitudeIndicatorPitchSphere * -203;
                                   rt.Angle = attitudeIndicatorBank * 180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);

                                   AttitudeIndicatorPitchSphere.RenderTransform = grp;
                               }
                               if (lattitudeIndicatorPitch != attitudeIndicatorPitch || lattitudeIndicatorBankNeedle != attitudeIndicatorBankNeedle)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = attitudeIndicatorPitch * 180;
                                   rt.Angle = attitudeIndicatorBankNeedle * 180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);

                                   AttitudeIndicatorPitch.RenderTransform = grp;
                               }
                               if (lattitudeIndicatorBankNeedle != attitudeIndicatorBankNeedle)
                               {
                                   rtattitudeIndicatorBankNeedle.Angle = attitudeIndicatorBankNeedle * 180;
                                   AttitudeIndicatorBank.RenderTransform = rtattitudeIndicatorBankNeedle;
                               }

                               AttitudeIndicatorOffFlag.Visibility = attitudeIndicatorOffFlag > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               //ValueText.Text = attitudeIndicatorBank.ToString();

                               lattitudeIndicatorPitch = attitudeIndicatorPitch;
                               lattitudeIndicatorBank = attitudeIndicatorBank;
                               lattitudeIndicatorPitchSphere = attitudeIndicatorPitchSphere;
                               lattitudeIndicatorBankNeedle = attitudeIndicatorBankNeedle;
                               lattitudeIndicatorOffFlag = attitudeIndicatorOffFlag;
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
