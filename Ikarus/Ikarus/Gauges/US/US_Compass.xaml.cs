using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for USWhiskeyCompass.xaml
    /// </summary>
    public partial class US_Compass : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;

        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double heading = 0.0;
        double pitch = 0.0;
        double bank = 0.0;
        double lheading = 0.0;
        double lpitch = 0.0;
        double lbank = 0.0;

        TransformGroup transformGroup = new TransformGroup();
        RotateTransform rtHeading = new RotateTransform();
        TranslateTransform ttHeading = new TranslateTransform();

        public US_Compass()
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
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 3);
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 3);
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
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { bank = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lheading != heading || lpitch != pitch || lbank != bank)
                               {
                                   transformGroup = new TransformGroup();
                                   ttHeading = new TranslateTransform();

                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (heading >= valueScale[n] && heading <= valueScale[n + 1])
                                       {
                                           ttHeading.X = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (heading - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   ttHeading.Y = pitch * 20;

                                   rtHeading = new RotateTransform()
                                   {
                                       Angle = bank * 30
                                   };
                                   transformGroup.Children.Add(ttHeading);
                                   transformGroup.Children.Add(rtHeading);

                                   Course.RenderTransform = transformGroup;

                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", heading.ToString(), Convert.ToInt32(ttHeading.X).ToString());
                                   }
                               }
                               lheading = heading;
                               lpitch = pitch;
                               lbank = bank;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
