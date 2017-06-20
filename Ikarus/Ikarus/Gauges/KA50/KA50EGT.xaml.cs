﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for KA50EGT.xaml
    /// </summary>
    public partial class KA50EGT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double leftEngineTemperatureHund = 0.0;
        double leftEngineTemperatureTenth = 0.0;
        double rightEngineTemperatureHund = 0.0;
        double rightEngineTemperatureTenth = 0.0;

        double lleftEngineTemperatureHund = 0.0;
        double lleftEngineTemperatureTenth = 0.0;
        double lrightEngineTemperatureHund = 0.0;
        double lrightEngineTemperatureTenth = 0.0;

        RotateTransform rtLeftEngineTemperatureHund = new RotateTransform();
        RotateTransform rtLeftEngineTemperatureTenth = new RotateTransform();
        RotateTransform rtRightEngineTemperatureHund = new RotateTransform();
        RotateTransform rtRightEngineTemperatureTenth = new RotateTransform();

        public KA50EGT()
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
            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            string frame = "";
            string light = "";

            helper.LoadBmaps(ref frame, ref light);

            try
            {
                if (frame.Length > 4)
                    Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));

                if (light.Length > 4)
                    Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));

                SwitchLight(false);
            }
            catch { }
        }

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
            return 167.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { leftEngineTemperatureHund = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { leftEngineTemperatureTenth = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rightEngineTemperatureHund = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { rightEngineTemperatureTenth = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (leftEngineTemperatureHund < 0.0) leftEngineTemperatureHund = 0.0;
                               if (rightEngineTemperatureHund < 0.0) rightEngineTemperatureHund = 0.0;

                               if (lleftEngineTemperatureHund != leftEngineTemperatureHund)
                               {
                                   rtLeftEngineTemperatureHund.Angle = leftEngineTemperatureHund * 270;
                                   KA50_needleLM_EGT.RenderTransform = rtLeftEngineTemperatureHund;
                               }
                               if (lleftEngineTemperatureTenth != leftEngineTemperatureTenth)
                               {
                                   rtLeftEngineTemperatureTenth.Angle = leftEngineTemperatureTenth * 360;
                                   KA50_needleLC_EGT.RenderTransform = rtLeftEngineTemperatureTenth;
                               }
                               if (lrightEngineTemperatureHund != rightEngineTemperatureHund)
                               {
                                   rtRightEngineTemperatureHund.Angle = rightEngineTemperatureHund * 270;
                                   KA50_needleRM_EGT.RenderTransform = rtRightEngineTemperatureHund;
                               }
                               if (lrightEngineTemperatureTenth != rightEngineTemperatureTenth)
                               {
                                   rtRightEngineTemperatureTenth.Angle = rightEngineTemperatureTenth * 360;
                                   KA50_needleRC_EGT.RenderTransform = rtRightEngineTemperatureTenth;
                               }
                               lleftEngineTemperatureHund = leftEngineTemperatureHund;
                               lleftEngineTemperatureTenth = leftEngineTemperatureTenth;
                               lrightEngineTemperatureHund = rightEngineTemperatureHund;
                               lrightEngineTemperatureTenth = rightEngineTemperatureTenth;
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
