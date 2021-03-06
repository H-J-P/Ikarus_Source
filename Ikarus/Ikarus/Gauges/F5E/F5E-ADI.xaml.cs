﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F5E_ADI.xaml
    /// </summary>
    public partial class F5E_ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double pitch = 0.0;
        double bank = 0.0;
        double attitudeWarningFlag = 0.0;
        double silhouette = 0.0;

        double lpitch = 0.0;
        double lbank = 0.0;
        double lattitudeWarningFlag = 1.0;
        double lsilhouette = 0.0;

        TransformGroup grp = new TransformGroup();
        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();
        TranslateTransform ttSilhouette = new TranslateTransform();

        public F5E_ADI()
        {
            InitializeComponent();
            Flagg_off.Visibility = System.Windows.Visibility.Visible;
            Flagg_course_off.Visibility = System.Windows.Visibility.Hidden;

            Side.Visibility = System.Windows.Visibility.Hidden;
            Glide.Visibility = System.Windows.Visibility.Hidden;
            Knob.Visibility = System.Windows.Visibility.Hidden;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { attitudeWarningFlag = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { silhouette = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (pitch > 0.5) pitch = 0.5;
                               if (pitch < -0.5) pitch = -0.5;

                               if (lpitch != pitch || lbank != bank)
                               {
                                   tt = new TranslateTransform()
                                   {
                                       Y = 2 * pitch * 174
                                   };
                                   rt = new RotateTransform()
                                   {
                                       Angle = bank * 180
                                   };
                                   grp = new TransformGroup();
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);

                                   Pitch.RenderTransform = grp;
                                   Bank.RenderTransform = rt;
                               }

                               if (lsilhouette != silhouette)
                               {
                                   ttSilhouette.Y = silhouette * -20;
                                   Silhouette.RenderTransform = ttSilhouette;
                               }

                               if (lattitudeWarningFlag != attitudeWarningFlag)
                                   Flagg_off.Visibility = (attitudeWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lpitch = pitch;
                               lbank = bank;
                               lattitudeWarningFlag = attitudeWarningFlag;
                               lsilhouette = silhouette;
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
