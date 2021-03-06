﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG21_HydraulicPressure.xaml
    /// </summary>
    public partial class MIG21_HydraulicPressure : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double primaryPressure = 0.0;
        double secondaryressure = 0.0;

        double lprimaryPressure = 0.0;
        double lsecondaryressure = 0.0;

        RotateTransform rtprimaryPressure = new RotateTransform();
        RotateTransform rtsecondaryPressure = new RotateTransform();

        public MIG21_HydraulicPressure()
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
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { primaryPressure = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { secondaryressure = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lprimaryPressure != primaryPressure)
                               {
                                   rtprimaryPressure.Angle = primaryPressure * 135;
                                   Main_hydro_system.RenderTransform = rtprimaryPressure;
                               }
                               if (lsecondaryressure != secondaryressure)
                               {
                                   rtsecondaryPressure.Angle = secondaryressure * -135;
                                   Command_hydro_system.RenderTransform = rtsecondaryPressure;
                               }
                               lprimaryPressure = primaryPressure;
                               lsecondaryressure = secondaryressure;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
