﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPASI10.xaml
    /// </summary>
    public partial class MIG21_ASI10 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double ias = 0.0;
        double mach = 0.0;
        double tas = 0.0;

        double lias = 0.0;
        double lmach = 0.0;
        double ltas = 0.0;

        RotateTransform rtIAS = new RotateTransform();
        RotateTransform rtmach = new RotateTransform();

        public MIG21_ASI10()
        {
            InitializeComponent();

            //IAS_M.Visibility = System.Windows.Visibility.Hidden;
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
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 3);
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 3);
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

                               if (vals.Length > 0) { ias = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { mach = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { tas = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lias != ias)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
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
                               // 101   TAS_indicator.input  = { 0.0,  167,   278,  417,  555,  833 };
                               //       TAS_indicator.output = { 0.0, 0.20, 0.309, 0.49, 0.67, 1.00 };

                               // 102   M_indicator.input    = { 0.0,   0.6,   1.0, 1.8,  2.0,  3.0 };
                               //       M_indicator.output   = { 0.0, 0.202, 0.312, 0.6, 0.66, 1.00 };

                               mach = (ias > 0.5) ? 1.0 : 0.0;

                               if (lmach != mach)
                               {
                                   rtmach.Angle = mach * 30;
                                   IAS_M.RenderTransform = rtmach;
                               }
                               lias = ias;
                               lmach = mach;
                               ltas = tas;
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
