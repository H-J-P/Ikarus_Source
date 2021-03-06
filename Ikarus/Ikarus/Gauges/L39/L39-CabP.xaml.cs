﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for L39_CabP.xaml
    /// </summary>
    public partial class L39_CabP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double cockpitAlt = 0.0;
        double pressDiff = 0.0;

        double lcockpitAlt = 0.0;
        double lpressdiff = 0.0;

        RotateTransform rtCockpitAlt = new RotateTransform();
        RotateTransform rtPressDiff = new RotateTransform();

        public L39_CabP()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            rtPressDiff.Angle =  55;
            PressureDifference.RenderTransform = rtPressDiff;
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

                               if (vals.Length > 0) { cockpitAlt = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pressDiff = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (cockpitAlt < 0.0) cockpitAlt = 0.0;
                               if (pressDiff < 0.0) pressDiff = 0.0;

                               if (lcockpitAlt != cockpitAlt)
                               {
                                   rtCockpitAlt.Angle = cockpitAlt * -328;
                                   CockpitAltitude.RenderTransform = rtCockpitAlt;
                               }
                               if (lpressdiff != pressDiff)
                               {
                                   rtPressDiff.Angle = (pressDiff * -263) + 55;
                                   PressureDifference.RenderTransform = rtPressDiff;
                               }
                               lcockpitAlt = cockpitAlt;
                               lpressdiff = pressDiff;
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
