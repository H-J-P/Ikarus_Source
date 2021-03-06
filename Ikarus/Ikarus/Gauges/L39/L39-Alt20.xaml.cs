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
    /// Interaction logic for L39_Alt20.xaml
    /// </summary>
    public partial class L39_Alt20 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double altituteKmNeedle = 0.0;
        double altituteMeterNeedle = 0.0;
        double baroPressure = 0.0;

        double laltituteKmNeedle = 0.0;
        double laltituteMeterNeedle = 0.0;
        double lbaroPressure = 0.0;

        RotateTransform rtAltituteKmNeedle = new RotateTransform();
        RotateTransform rtAltituteMeterNeedle = new RotateTransform();
        RotateTransform rtBaroPressure = new RotateTransform();

        public L39_Alt20()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);

            //rtBaroPressure.Angle = -37;
            //Pressure.RenderTransform = rtBaroPressure;
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

                               if (vals.Length > 0) { altituteKmNeedle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { altituteMeterNeedle = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { baroPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (baroPressure > 0.7686) baroPressure = 0.7686;

                               if (laltituteKmNeedle != altituteKmNeedle)
                               {
                                   rtAltituteKmNeedle.Angle = altituteKmNeedle * 360;
                                   Altimeter_km.RenderTransform = rtAltituteKmNeedle;
                               }

                               if (laltituteMeterNeedle != altituteMeterNeedle)
                               {
                                   rtAltituteMeterNeedle.Angle = altituteMeterNeedle * 360;
                                   Altimeter_m.RenderTransform = rtAltituteMeterNeedle;
                               }

                               if (lbaroPressure != baroPressure)
                               {
                                   //rtBaroPressure.Angle = (baroPressure * -373) - 37;
                                   rtBaroPressure.Angle = (baroPressure * -360);
                                   Pressure.RenderTransform = rtBaroPressure;
                               }
                               laltituteKmNeedle = altituteKmNeedle;
                               laltituteMeterNeedle = altituteMeterNeedle;
                               lbaroPressure = baroPressure;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
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
