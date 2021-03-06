﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for KA50ADIStby.xaml
    /// </summary>
    public partial class KA50ADIStby : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double roll = 0.0;
        double pitch = 0.0;
        double sideslip = 0.0;
        double failureFlag = 0.0;

        double lroll = 0.0;
        double lpitch = 0.0;
        double lsideslip = 0.0;
        double lfailureFlag = 0.0;

        RotateTransform rtRoll = new RotateTransform();
        TranslateTransform ttPitch = new TranslateTransform();
        RotateTransform rtSlipball = new RotateTransform();

        public KA50ADIStby()
        {
            InitializeComponent();

            KA50_needleOFF_ADIstb.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { roll = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { sideslip = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { failureFlag = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lroll != roll)
                               {
                                   rtRoll.Angle = roll * 180;
                                   KA50_plane_ADIstb.RenderTransform = rtRoll;
                               }
                               if (lpitch != pitch)
                               {
                                   ttPitch.Y = pitch * 155;
                                   KA50_pitch_ADIstb.RenderTransform = ttPitch;
                               }
                               if (lsideslip != sideslip)
                               {
                                   rtSlipball.Angle = sideslip * -8;
                                   KA50_needleSLIP_ADIstb.RenderTransform = rtSlipball;
                               }
                               KA50_needleOFF_ADIstb.Visibility = (failureFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lroll = roll;
                               lpitch = pitch;
                               lsideslip = sideslip;
                               lfailureFlag = failureFlag;
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
