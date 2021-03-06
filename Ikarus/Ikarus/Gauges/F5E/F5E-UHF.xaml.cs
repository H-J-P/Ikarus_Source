﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F5E_UHF.xaml
    /// </summary>
    public partial class F5E_UHF : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double uhf100Mhz = 0.0;
        double uhf10Mhz = 0.0;
        double uhf1Mhz = 0.0;
        double uhf01Mhz = 0.0;
        double uhf001Mhz = 0.0;

        double luhf100Mhz = 0.0;
        double luhf10Mhz = 0.0;
        double luhf1Mhz = 0.0;
        double luhf01Mhz = 0.0;
        double luhf001Mhz = 0.0;

        RotateTransform rt100Mhz = new RotateTransform();
        RotateTransform rt10Mhz = new RotateTransform();
        RotateTransform rt1Mhz = new RotateTransform();
        RotateTransform rt01Mhz = new RotateTransform();
        RotateTransform rt001Mhz = new RotateTransform();

        public F5E_UHF()
        {
            InitializeComponent();

            Konstukt.Visibility = System.Windows.Visibility.Hidden;
            FreqValue.Visibility = System.Windows.Visibility.Hidden;
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
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { uhf100Mhz = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { uhf10Mhz = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { uhf1Mhz = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { uhf01Mhz = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { uhf001Mhz = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               FreqValue.Text = uhf100Mhz.ToString();

                               if (luhf100Mhz != uhf100Mhz)
                               {
                                   if (uhf100Mhz == 0) { rt100Mhz.Angle = 0; } // A
                                   if (uhf100Mhz == 0.0) { rt100Mhz.Angle = 0; } // A
                                   if (uhf100Mhz == 0.1) { rt100Mhz.Angle = -90; } // T
                                   if (uhf100Mhz == 0.2) { rt100Mhz.Angle = -60; } // 2
                                   if (uhf100Mhz == 0.3) { rt100Mhz.Angle = -30; } // 3
                                   if (uhf100Mhz == 0.4) { rt100Mhz.Angle = 0; } // A
                                   if (uhf100Mhz == 1.0) { rt100Mhz.Angle = 0; } // A

                                   Zifferblatt100Mhz.RenderTransform = rt100Mhz;
                               }
                               if (luhf10Mhz != uhf10Mhz)
                               {
                                   rt10Mhz.Angle = uhf10Mhz * 360;
                                   Zifferblatt10Mhz.RenderTransform = rt10Mhz;
                               }
                               if (luhf1Mhz != uhf1Mhz)
                               {
                                   rt1Mhz.Angle = uhf1Mhz * 360;
                                   Zifferblatt1Mhz.RenderTransform = rt1Mhz;
                               }
                               if (luhf01Mhz != uhf01Mhz)
                               {
                                   rt01Mhz.Angle = uhf01Mhz * 360;
                                   Zifferblatt01Mhz.RenderTransform = rt01Mhz;
                               }
                               if (luhf001Mhz != uhf001Mhz)
                               {
                                   if (uhf001Mhz == 0.0) { rt001Mhz.Angle = 0; } // 00
                                   if (uhf001Mhz == 0.25) { rt001Mhz.Angle = 90; } // 25
                                   if (uhf001Mhz == 0.3) { rt001Mhz.Angle = 90; } // 25
                                   if (uhf001Mhz == 0.5) { rt001Mhz.Angle = 180; } // 50
                                   if (uhf001Mhz == 0.75) { rt001Mhz.Angle = 270; } // 75
                                   if (uhf001Mhz == 0.8) { rt001Mhz.Angle = 270; } // 75

                                   Zifferblatt001Mhz.RenderTransform = rt001Mhz;
                               }

                               luhf100Mhz = uhf100Mhz;
                               luhf10Mhz = uhf10Mhz;
                               luhf1Mhz = uhf1Mhz;
                               luhf01Mhz = uhf01Mhz;
                               luhf001Mhz = uhf001Mhz;
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
