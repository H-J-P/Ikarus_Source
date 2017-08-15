﻿using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für UH1_ADI.xaml
    /// </summary>
    public partial class UH1_ADI_Sphere : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;
        private string lightColor = "#FFFFFF"; // white

        public int GetWindowID() { return windowID; }

        double pitch = 0.0;
        double bank = 0.0;
        double bankNeedle = 0.0;
        double attitudeWarningFlag = 0.0;

        private double dPitch = 0.0;
        private double dBank = 0.0;
        private double dNull = 0.0;

        double lpitch = 0.0;
        double lbank = 0.0;
        double lattitudeWarningFlag = 1.0;

        RotateTransform rt = new RotateTransform();
        Sphere3D sphere3D;

        public UH1_ADI_Sphere()
        {
            InitializeComponent();
            Flagg_off.Visibility = System.Windows.Visibility.Visible;
            Side.Visibility = System.Windows.Visibility.Hidden;
            Glide.Visibility = System.Windows.Visibility.Hidden;

            InitialSphere();
            sphere3D.Rotate(0, 0, -90);

            directionalLight.Color = (Color)ColorConverter.ConvertFromString(lightColor);
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
            directionalLight.Color = _on ? (Color)ColorConverter.ConvertFromString("#" + MainWindow.lightOnColor) : (Color)ColorConverter.ConvertFromString(lightColor);
        }

        public void SetInput(string _input)
        {
        }

        public void SetOutput(string _output)
        {
        }

        public double GetSize()
        {
            return 283; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { bank = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { attitudeWarningFlag = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               bankNeedle = bank;

                               //if (lpitch != pitch || lbank != bank)
                               //    sphere3D.Rotate(0, pitch * -126, (bank * -180) - 90);

                               if (lpitch != pitch || lbank != bank)
                               {
                                   dPitch = pitch * -126;
                                   dBank = (bank * -180) - 90;

                                   sphere3D.Rotate(ref dNull, ref dPitch, ref dBank);
                               }
                               if (lbank != bank)
                               {
                                   rt = new RotateTransform()
                                   {
                                       Angle = bankNeedle * 180
                                   };
                                   Bank.RenderTransform = rt;
                               }

                               if (lattitudeWarningFlag != attitudeWarningFlag)
                                   Flagg_off.Visibility = (attitudeWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lpitch = pitch;
                               lbank = bank;
                               lattitudeWarningFlag = attitudeWarningFlag;
                           }
                           catch { return; };
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }

        private void InitialSphere()
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            if (File.Exists(Environment.CurrentDirectory + "\\Images\\Textures3D\\UH_ADI.png"))
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\UH_ADI.png");
            else
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\CheckerTest.jpg");

            bitmapImage.DecodePixelWidth = 1024;
            bitmapImage.EndInit();

            // declaration Sphere Object with model3Dgroup Name from XAML file and Sphere texture
            sphere3D = new Sphere3D(model3DGroup, bitmapImage);
        }
    }
}