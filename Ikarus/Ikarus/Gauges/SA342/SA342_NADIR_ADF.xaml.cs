﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für SA342_HSI.xaml
    /// </summary>
    public partial class SA342_NADIR_ADF : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private double heading = 0.0;
        private double steadybug = 0.0;
        private double nadir = 0.0;
        private double adf = 0.0;
        private double pxFlag = 0.0;
        private double butFlag = 0.0;
        private double capFlag = 0.0;
        private double range001 = 0.0;
        private double range010 = 0.0;
        private double range100 = 0.0;

        private double lheading = 0.0;
        private double lsteadybug = 0.0;
        private double lnadir = 0.0;
        private double ladf = 0.0;
        private double lpxFlag = 1.0;
        private double lbutFlag = 1.0;
        private double lcapFlag = 1.0;
        private double lrange001 = 0.0;
        private double lrange010 = 0.0;
        private double lrange100 = 0.0;

        RotateTransform rtHeading = new RotateTransform();
        RotateTransform rtNadir = new RotateTransform();
        RotateTransform rtAdf = new RotateTransform();
        TranslateTransform ttRange001 = new TranslateTransform();
        TranslateTransform ttRange010 = new TranslateTransform();
        TranslateTransform ttRange100 = new TranslateTransform();

        public SA342_NADIR_ADF()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            Flag_CAP.Visibility = System.Windows.Visibility.Visible;
            Flag_BUT.Visibility = System.Windows.Visibility.Visible;
            Flag_PX.Visibility = System.Windows.Visibility.Visible;

            HeadingValue.Visibility = System.Windows.Visibility.Hidden;
            ADFValue.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { nadir = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { adf = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { capFlag = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { butFlag = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pxFlag = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { range100 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { range010 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { range001 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { steadybug = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }

                               if (lheading != heading)
                               {
                                   rtHeading.Angle = heading * -360;
                                   Heading.RenderTransform = rtHeading;
                               }

                               if (lnadir != nadir)
                               {
                                   if (nadir != 0)
                                       rtNadir.Angle = (nadir * 360) + 180; // {-360.0,0.0,360.0}{-1.0,0.0,1.0}
                                   else
                                       rtNadir.Angle = 0; // {-360.0,0.0,360.0}{-1.0,0.0,1.0}

                                   NARDIR.RenderTransform = rtNadir;
                               }

                               if (ladf != adf)
                               {
                                   rtAdf.Angle = adf * 360; // {-360.0,0.0,360.0}{-1.0,0.0,1.0}
                                   ADF.RenderTransform = rtAdf;
                               }

                               if (lcapFlag != capFlag)
                                   Flag_CAP.Visibility = (capFlag > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lbutFlag != butFlag)
                                   Flag_BUT.Visibility = (butFlag > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lpxFlag != pxFlag)
                                   Flag_PX.Visibility = (pxFlag > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lrange100 != range100)
                               {
                                   ttRange100.Y = range100 * -217;
                                   Range_100.RenderTransform = ttRange100;
                               }

                               if (lrange010 != range010)
                               {
                                   ttRange010.Y = range010 * -217;
                                   Range_10.RenderTransform = ttRange010;
                               }

                               if (lrange001 != range001)
                               {
                                   ttRange001.Y = range001 * -217;
                                   Range_1.RenderTransform = ttRange001;
                               }

                               //HeadingValue.Text = "Compass rose: " + heading.ToString() + " ; " + (heading * 360).ToString();
                               //ADFValue.Text =     "Needle Large:  " + nadir.ToString() + " ; " + (nadir * 360).ToString();

                               lheading = heading;
                               lnadir = nadir;
                               ladf = adf;
                               lcapFlag = capFlag;
                               lbutFlag = butFlag;
                               lpxFlag = pxFlag;
                               lrange100 = range100;
                               lrange010 = range010;
                               lrange001 = range001;
                               lsteadybug = steadybug;
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
