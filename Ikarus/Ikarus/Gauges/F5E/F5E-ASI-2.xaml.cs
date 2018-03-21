using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für F5E_ASI_2.xaml
    /// </summary>
    public partial class F5E_ASI_2 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        private double[] valueScaleMach = new double[] { };
        private double[] degreeDialMach = new double[] { };
        GaugesHelper helper = null;

        int valueScaleIndex = 0;
        const int valueScaleIndexMach = 14;

        public int GetWindowID() { return windowID; }

        RotateTransform rtASI = new RotateTransform();
        RotateTransform rtMach = new RotateTransform();
        RotateTransform rtASIMach = new RotateTransform();
        RotateTransform rtASImax = new RotateTransform();
        RotateTransform rtASIset = new RotateTransform();

        double ias = 0.0;
        double mach = 0.0;
        double iasMax = 0.0;
        double iasSet = 0.0;

        double lias = 0.0;
        double lmach = 0.0;
        double liasMax = 0.0;
        double liasSet = 0.0;

        public F5E_ASI_2()
        {
            InitializeComponent();


            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            rtASImax.Angle = -316;
            IASmax.RenderTransform = rtASImax;

            rtMach.Angle = 0;
            Mach.RenderTransform = rtMach;
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
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 4);

            // MachIndicator.input =                         { 0.5,   0.6,   0.7,   0.8,   0.9,   1.0,   1.1,   1.2,   1.3,   1.4,   1.5,   1.6, 1.7, 2.2}
            valueScaleMach = new double[valueScaleIndexMach] { 1.0, 0.929, 0.871, 0.816, 0.765, 0.727, 0.683, 0.643, 0.611, 0.582, 0.551, 0.525, 0.5, 0.4 };
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 4);

            degreeDialMach = new double[valueScaleIndexMach] { 0.0, 25.0, 46.0, 66.0, 84.0, 98.0, 114.0, 128.0, 139.0, 150.0, 161.5, 171.0, 180, 216 };
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
                           vals = strData.Split(';');

                           try
                           {
                               if (vals.Length > 0) { ias = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { mach = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { iasMax = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { iasSet = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lias != ias || lmach != mach)
                               {
                                   if (lias != ias)
                                   {
                                       for (int n = 0; n < (valueScaleIndex - 1); n++)
                                       {
                                           if (ias >= valueScale[n] && ias <= valueScale[n + 1])
                                           {
                                               rtASI.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (ias - valueScale[n]) + degreeDial[n];
                                               break;
                                           }
                                       }
                                       IAS.RenderTransform = rtASI;
                                   }

                                   if (lmach != mach)
                                   {
                                       for (int n = 0; n < (valueScaleIndexMach - 1); n++)
                                       {
                                           if (mach <= valueScaleMach[n] && mach >= valueScaleMach[n + 1])
                                           {
                                               rtMach.Angle = ((degreeDialMach[n] - degreeDialMach[n + 1]) / (valueScaleMach[n] - valueScaleMach[n + 1]) * (mach - valueScaleMach[n]) + degreeDialMach[n]);
                                               break;
                                           }
                                       }
                                   }
                                   rtASIMach.Angle = (-rtMach.Angle) + rtASI.Angle;
                                   Mach.RenderTransform = rtASIMach;

                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", ias.ToString(), Convert.ToInt32(rtASI.Angle).ToString());
                                       Cockpit.UpdateInOut(dataImportID, "2", mach.ToString(), Convert.ToInt32(rtMach.Angle).ToString());
                                   }
                               }
                               if (liasMax != iasMax)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (iasMax >= valueScale[n] && iasMax <= valueScale[n + 1])
                                       {
                                           rtASImax.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (iasMax - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   rtASImax.Angle = rtASImax.Angle - 316;
                                   IASmax.RenderTransform = rtASImax;
                               }
                               if (liasSet != iasSet)
                               {
                                   rtASIset.Angle = (iasSet * 346);
                                   IASset.RenderTransform = rtASIset;
                               }
                               lias = ias;
                               lmach = mach;
                               liasMax = iasMax;
                               liasSet = iasSet;
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
