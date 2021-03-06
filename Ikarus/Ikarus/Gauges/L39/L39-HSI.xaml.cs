﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPHSI_early.xaml
    /// </summary>
    public partial class L39_HSI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double heading = 0.0;
        double requiredHeading = 0.0;
        double bearingNeedle = 0.0;
        double warningFlagG = 0.0;
        double warningFlagK = 0.0;
        double glide = 0.0;
        double side = 0.0;

        double lheading = 0.0;
        double lrequiredHeading = 0.0;
        double lbearingNeedle = 0.0;
        double lwarningFlagG = 0.0;
        double lwarningFlagK = 0.0;
        double lglide = 0.0;
        double lside = 0.0;

        RotateTransform rtHeading = new RotateTransform();
        RotateTransform rtRequiredHeading = new RotateTransform();
        RotateTransform rtBearingNeedle = new RotateTransform();
        TranslateTransform ttGlide = new TranslateTransform();
        TranslateTransform ttSide = new TranslateTransform();

        public L39_HSI()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            Flagg_G.Visibility = System.Windows.Visibility.Visible;
            Flagg_K.Visibility = System.Windows.Visibility.Visible;
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
                               if (vals.Length > 1) { requiredHeading = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { bearingNeedle = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { warningFlagG = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { warningFlagK = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { glide = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { side = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               if (lheading != heading)
                               {
                                   rtHeading.Angle = heading * -360;
                                   Heading.RenderTransform = rtHeading;
                               }

                               if (lrequiredHeading != requiredHeading)
                               {
                                   rtRequiredHeading.Angle = (requiredHeading * -360); // + (heading * -360);
                                   RequiredHeading.RenderTransform = rtRequiredHeading;
                               }

                               if (lbearingNeedle != bearingNeedle)
                               {
                                   rtBearingNeedle.Angle = bearingNeedle * 360;
                                   BearingPointer.RenderTransform = rtBearingNeedle;
                               }

                               Flagg_G.Visibility = warningFlagG > 0.3 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Flagg_K.Visibility = warningFlagK > 0.3 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lglide != glide)
                               {
                                   ttGlide.Y = glide * -30;
                                   Glide.RenderTransform = ttGlide;
                               }
                               if (lside != side)
                               {
                                   ttSide.X = side * 30;
                                   Side.RenderTransform = ttSide;
                               }
                               lheading = heading;
                               lrequiredHeading = requiredHeading;
                               lbearingNeedle = bearingNeedle;
                               lwarningFlagG = warningFlagG;
                               lwarningFlagK = warningFlagK;
                               lglide = glide;
                               lside = side;
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