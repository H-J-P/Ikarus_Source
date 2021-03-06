﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AV8BNA_Chrono.xaml
    /// </summary>
    public partial class AV8BNA_Chrono : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double cronoMinutes = 0.0;
        double cronoSeconds = 0.0;

        double lcronoMinutes = 0.0;
        double lcronoSeconds = 0.0;

        RotateTransform rtCronoMinutes = new RotateTransform();
        RotateTransform rtCronoSeconds = new RotateTransform();

        public AV8BNA_Chrono()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { cronoMinutes = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { cronoSeconds = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lcronoMinutes != cronoMinutes)
                               {
                                   rtCronoMinutes.Angle = cronoMinutes * 360;
                                   MINUTES.RenderTransform = rtCronoMinutes;
                               }
                               if (lcronoSeconds != cronoSeconds)
                               {
                                   rtCronoSeconds.Angle = cronoSeconds * 360;
                                   SECONDS.RenderTransform = rtCronoSeconds;
                               }
                               lcronoMinutes = cronoMinutes;
                               lcronoSeconds = cronoSeconds;
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
