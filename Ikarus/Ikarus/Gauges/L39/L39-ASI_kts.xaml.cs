using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for L39_ASI_kts.xaml
    /// </summary>
    public partial class L39_ASI_kts : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtIAS = new RotateTransform();
        RotateTransform rtTAS = new RotateTransform();
        RotateTransform rtMach = new RotateTransform();

        double ias = 0.0;
        double tas = 0.0;
        double mach = 0.0;

        double lias = 0.0;
        double ltas = 0.0;
        double lmach = 0.0;

        public L39_ASI_kts()
        {
            InitializeComponent();

            rtIAS.Angle = 10;
            IAS.RenderTransform = rtIAS;

            rtTAS.Angle = 10;
            TAS.RenderTransform = rtTAS;

            rtMach.Angle = 11; // +rtIAS.Angle;
            MACH.RenderTransform = rtMach;
        }
        public string GetID() { return dataImportID; }

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

        public void SwitchLight(bool _on)
        {
            Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');

            if (vals.Length < 3) return;

            valueScale = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                valueScale[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
            valueScaleIndex = vals.Length;
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');

            if (vals.Length < 3) return;

            degreeDial = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                degreeDial[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
        }

        public double GetSize()
        {
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           vals = strData.Split(';');

                           try
                           {
                               if (vals.Length > 0) { ias = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { tas = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { mach = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (ias < 0.0) ias = 0.0;
                               if (tas < 0.0) tas = 0.0;
                               if (mach < 0.0) mach = 0.0;

                               if (lias != ias)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (ias >= valueScale[n] && ias <= valueScale[n + 1])
                                       {
                                           rtIAS.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (ias - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", ias.ToString(), Convert.ToInt32(rtIAS.Angle).ToString());
                                   }
                                   IAS.RenderTransform = rtIAS;
                               }

                               if (ltas != tas)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (tas >= valueScale[n] && tas <= valueScale[n + 1])
                                       {
                                           rtTAS.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (tas - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   TAS.RenderTransform = rtTAS;
                               }

                               if (lmach != mach && mach > 0.4)
                               {
                                   mach = mach - 0.4;
                                   rtMach.Angle = (mach * -220) + 11;
                                   MACH.RenderTransform = rtMach;
                               }
                               lias = ias;
                               ltas = tas;
                               lmach = mach;
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
