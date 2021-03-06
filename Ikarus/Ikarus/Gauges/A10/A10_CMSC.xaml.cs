﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_CMSC.xaml
    /// </summary>
    public partial class A10_CMSC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        string chaff = "";
        string jmr = "";
        string mws = "";
        double missileLaunchIndicator = 0.0;
        double priorityStatusIndicator = 0.0;
        double unknownStatusIndicator = 0.0;

        public A10_CMSC()
        {
            InitializeComponent();

            CHAFF.Text = "";
            FLARE.Text = "";
            MWS.Text = "";
            JMR.Text = "";
            FLARE.Visibility = System.Windows.Visibility.Hidden;

            Missile_Launch.Visibility = System.Windows.Visibility.Hidden;
            Priority.Visibility = System.Windows.Visibility.Hidden;
            Unknown.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { chaff = vals[0]; }
                               if (vals.Length > 1) { jmr = vals[1]; }
                               if (vals.Length > 2) { mws = vals[2]; }
                               if (vals.Length > 3) { missileLaunchIndicator = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { priorityStatusIndicator = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { unknownStatusIndicator = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }

                               CHAFF.Text = chaff;
                               FLARE.Text = jmr;
                               MWS.Text = mws;
                               JMR.Text = jmr;  ///????

                               Missile_Launch.Visibility = (missileLaunchIndicator > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Priority.Visibility = (priorityStatusIndicator > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Unknown.Visibility = (unknownStatusIndicator > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                           }
                           //catch { return; }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
