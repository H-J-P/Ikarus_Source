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
    /// Interaktionslogik für AJS37_ASI.xaml
    /// </summary>
    public partial class AJS37_ASI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double ias = 0.0;
        double mach1 = 0.0;
        double mach10 = 0.0;
        double mach100 = 0.0;
        double asiOff = 0.0;
        double machOff = 0.0;

        double lias = 0.0;
        double lmach1 = 0.0;
        double lmach10 = 0.0;
        double lmach100 = 0.0;
        double lasiOff = 0.0;
        double lmachOff = 0.0;

        RotateTransform rtIas = new RotateTransform();
        TranslateTransform ttMach1 = new TranslateTransform();
        TranslateTransform ttMach10 = new TranslateTransform();
        TranslateTransform ttMach100 = new TranslateTransform();

        public AJS37_ASI()
        {
            InitializeComponent();

            ASI_OFF_flag.Visibility = System.Windows.Visibility.Visible;
            Mach_OFF_flag.Visibility = System.Windows.Visibility.Visible;

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

                               if (vals.Length > 0) { ias = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { mach1 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { mach10 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { mach100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { asiOff = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { machOff = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }

                               if (lias != ias)
                               {
                                   rtIas.Angle = ias * 325;
                                   ASI1.RenderTransform = rtIas;
                               }

                               if (mach1 != lmach1)
                               {
                                   ttMach1.Y = mach1 * -315;
                                   MACH1.RenderTransform = ttMach1;
                               }
                               if (mach10 != lmach10)
                               {
                                   ttMach10.Y = mach10 * -315;
                                   MACH10.RenderTransform = ttMach10;
                               }
                               if (mach1 != lmach1)
                               {
                                   ttMach100.Y = mach100 * -315;
                                   MACH100.RenderTransform = ttMach100;
                               }

                               if (lasiOff != asiOff)
                                   ASI_OFF_flag.Visibility = (asiOff > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lmachOff != machOff)
                                   Mach_OFF_flag.Visibility = (machOff > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lias = ias;
                               lmach1 = mach1;
                               lmach10 = mach10;
                               lmach100 = mach100;
                               lasiOff = asiOff;
                               lmachOff = machOff;
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