﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für US_AOA.xaml
    /// </summary>
    public partial class US_AOA : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double angleOfAttack = 0.0;
        double powerOffFlag = 0.0;

        double langleOfAttack = 0.0;
        double lpowerOffFlag = 0.0;

        RotateTransform rtAOA = new RotateTransform();

        public US_AOA()
        {
            InitializeComponent();

            AOA_poweroff_flag.Visibility = System.Windows.Visibility.Visible;
            AOAValue.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { angleOfAttack = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { powerOffFlag = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (angleOfAttack > 1.0) { angleOfAttack = 1.0; }
                               if (angleOfAttack < 0.0) { angleOfAttack = 0.0; }

                               if (langleOfAttack != angleOfAttack)
                               {
                                   rtAOA.Angle = angleOfAttack * -270;
                                   AOA_Units.RenderTransform = rtAOA;
                               }
                               AOA_poweroff_flag.Visibility = (powerOffFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               langleOfAttack = angleOfAttack;
                               lpowerOffFlag = powerOffFlag;
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
