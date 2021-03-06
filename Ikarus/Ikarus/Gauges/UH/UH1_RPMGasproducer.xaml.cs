﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UH1_RPMGasproducer.xaml
    /// </summary>
    public partial class UH1_RPMGasproducer : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double gasProducerTach = 0.0;
        double gasProducerTach_U = 0.0;

        double lgasProducerTach = 0.0;
        double lgasProducerTach_U = 0.0;

        RotateTransform rtGasProducerTach = new RotateTransform();
        RotateTransform rtGasProducerTach_U = new RotateTransform();

        public UH1_RPMGasproducer()
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

                               if (vals.Length > 0) { gasProducerTach = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { gasProducerTach_U = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (gasProducerTach < 0.0) gasProducerTach = 0.0;
                               if (gasProducerTach_U < 0.0) gasProducerTach_U = 0.0;

                               if (lgasProducerTach != gasProducerTach)
                               {
                                   rtGasProducerTach.Angle = gasProducerTach * 270;
                                   GasProducerTach.RenderTransform = rtGasProducerTach;
                               }
                               if (lgasProducerTach_U != gasProducerTach_U)
                               {
                                   rtGasProducerTach_U.Angle = gasProducerTach_U * 360;
                                   GasProducerTach_U.RenderTransform = rtGasProducerTach_U;
                               }
                               lgasProducerTach = gasProducerTach;
                               lgasProducerTach_U = gasProducerTach_U;
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
