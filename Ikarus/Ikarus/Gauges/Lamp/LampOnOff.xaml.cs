using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LampOnOff : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureOn = "";
        private string pictureOff = "";
        private int windowID = 0;
        BitmapImage bitmapImage = new BitmapImage();
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public LampOnOff()
        {
            InitializeComponent();
            Focusable = false;

            DesignFrame.Visibility = System.Windows.Visibility.Hidden;

            LampOn.Visibility = System.Windows.Visibility.Hidden;
            LampOff.Visibility = System.Windows.Visibility.Visible;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Lamps");

            if (MainWindow.editmode)
            {
                helper.MakeDraggable(this, this);

                DesignFrame.Visibility = System.Windows.Visibility.Visible;

                LampOn.Visibility = System.Windows.Visibility.Visible;
                LampOff.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
        }

        public void SetOutput(string _output)
        {
        }

        private void SetContour()
        {
            int bitmapHeight = bitmapImage.PixelHeight / 2; // Jumping Jack
            int bitmapWidth = bitmapImage.PixelWidth / 2;

            DesignFrame.Height = bitmapHeight;
            DesignFrame.Width = bitmapWidth;

            LampOn.Height = bitmapHeight;
            LampOn.Width = bitmapWidth;

            LampOff.Height = bitmapHeight;
            LampOff.Width = bitmapWidth;

            UpperRec.Height = bitmapHeight; //- 10;
            UpperRec.Width = bitmapWidth; // - 10;
        }

        private void LoadBmaps()
        {
            dataRows = MainWindow.dtLamps.Select("ID=" + dataImportID);
            bool findPictureOn = false;

            if (dataRows.Length > 0)
            {
                pictureOn = dataRows[0]["FilePictureOn"].ToString();

                pictureOff = dataRows[0]["FilePictureOff"].ToString();
            }

            try
            {
                LampOn.Source = null;
                LampOff.Source = null;

                if (File.Exists(Environment.CurrentDirectory + "\\Images\\Lamps\\" + pictureOn))
                {
                    bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Lamps\\" + pictureOn));

                    SetContour();
                    LampOn.Source = bitmapImage;
                    findPictureOn = true;
                }

                if (File.Exists(Environment.CurrentDirectory + "\\Images\\Lamps\\" + pictureOff))
                {
                    if (findPictureOn)
                    {
                        LampOff.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Lamps\\" + pictureOff));
                    }
                    else
                    {
                        bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Lamps\\" + pictureOff));

                        SetContour();
                        LampOff.Source = bitmapImage;
                    }
                }
            }
            catch { }
        }

        public double GetSize()
        {
            return DesignFrame.Width;
        }

        public double GetSizeY()
        {
            return DesignFrame.Height;
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           string[] vals = strData.Split(';');

                           double lampState = 0.0;

                           try
                           {
                               if (vals.Length > 0) { lampState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                           }
                           catch { return; };

                           if (lampState > 0.08) { SetValue(1.0, false); }
                           else { SetValue(0.0, false); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtLamps, dataImportID, e.Delta);
        }

        private void SetValue(double _value, bool _event)
        {
            if (_value > 0.0)
            {
                LampOn.Visibility = System.Windows.Visibility.Visible;
                LampOff.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                LampOn.Visibility = System.Windows.Visibility.Hidden;
                LampOff.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void UpperRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_TouchDown(object sender, TouchEventArgs e)
        {
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

