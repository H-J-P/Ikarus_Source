﻿using System;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

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
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + dataImportID);

            if (dataRows.Length > 0)
            {
                string frame = dataRows[0]["ImageFrame"].ToString();
                string light = dataRows[0]["ImageLight"].ToString();

                try
                {
                    if (frame.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame))
                            Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));
                    }
                    if (light.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + light))
                            Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));
                    }
                    SwitchLight(false);
                }
                catch { }
            }
        }

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
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
