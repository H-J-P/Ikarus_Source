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
    /// Interaction logic for F15FuelFlow.xaml
    /// </summary>
    public partial class F15FuelFlow : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        RotateTransform rtfuelflow = new RotateTransform();
        TranslateTransform trfuel10000 = new TranslateTransform();
        TranslateTransform trfuel1000 = new TranslateTransform();
        TranslateTransform trfuel100 = new TranslateTransform();

        double fuelflow = 0.0;
        double fuelflow10000 = 0.0;
        double fuelflow1000 = 0.0;
        double fuelflow100 = 0.0;

        double lfuelflow = 0.0;
        double lfuelflow10000 = 0.0;
        double lfuelflow1000 = 0.0;
        double lfuelflow100 = 0.0;

        public F15FuelFlow()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
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

                               if (vals.Length > 0) { fuelflow = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { fuelflow10000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { fuelflow1000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { fuelflow100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (fuelflow < 0.0) fuelflow = 0.0;
                               if (fuelflow10000 < 0.0) fuelflow10000 = 0.0;
                               if (fuelflow1000 < 0.0) fuelflow1000 = 0.0;
                               if (fuelflow100 < 0.0) fuelflow100 = 0.0;

                               if (lfuelflow != fuelflow)
                               {
                                   rtfuelflow.Angle = fuelflow * 240;
                                   Fuelflow.RenderTransform = rtfuelflow;
                               }
                               if (lfuelflow10000 != fuelflow10000)
                               {
                                   trfuel10000.Y = fuelflow10000 * -270;
                                   Fuelflow_10_000.RenderTransform = trfuel10000;
                               }
                               if (lfuelflow1000 != fuelflow1000)
                               {
                                   trfuel1000.Y = fuelflow1000 * -270;
                                   Fuelflow_1_000.RenderTransform = trfuel1000;
                               }
                               if (lfuelflow100 != fuelflow100)
                               {
                                   trfuel100.Y = fuelflow100 * -270;
                                   Fuelflow_100.RenderTransform = trfuel100;
                               }
                               lfuelflow = fuelflow;
                               lfuelflow10000 = fuelflow10000;
                               lfuelflow1000 = fuelflow1000;
                               lfuelflow100 = fuelflow100;
                           }
                           catch { return; }
                       }));
        }

        private void MakeDraggable(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;
            TranslateTransform trUsercontrol = new TranslateTransform(0, 0);
            bool isMousePressed = false;

            movedByElement.MouseLeftButtonDown += (a, b) =>
            {
                isMousePressed = true;
                originalPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
            };

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                isMousePressed = false;
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;


            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
