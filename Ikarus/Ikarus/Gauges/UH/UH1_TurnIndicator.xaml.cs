﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UH1_TurnIndicator.xaml
    /// </summary>
    public partial class UH1_TurnIndicator : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double turnPtr = 0.0;
        double sideSlip = 0.0;

        double lturnPtr = 0.0;
        double lsideSlip = 0.0;

        RotateTransform rtTurnPtr = new RotateTransform();
        RotateTransform rtSideSlip = new RotateTransform();

        public UH1_TurnIndicator()
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
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { turnPtr = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { sideSlip = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lturnPtr != turnPtr)
                               {
                                   rtTurnPtr.Angle = turnPtr * 35;
                                   TurnPtr.RenderTransform = rtTurnPtr;
                               }
                               if (lsideSlip != sideSlip)
                               {
                                   rtSideSlip.Angle = sideSlip * -18;
                                   SideSlip.RenderTransform = rtSideSlip;
                               }
                               lturnPtr = turnPtr;
                               lsideSlip = sideSlip;
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
