using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ButtonOffOn : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureOn = "";
        private string pictureOff = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        private double[] input = new double[] { };
        private double[] output = new double[] { };
        private double switchValue = 0.0;
        private int state = 0;
        private Switches switches = null;
        private bool touchDown = false;
        BitmapImage bitmapImage = new BitmapImage();
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public ButtonOffOn()
        {
            InitializeComponent();
            Focusable = false;

            DesignFrame.Visibility = System.Windows.Visibility.Hidden;
            SwitchUp.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();

            switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Switches");

            if (MainWindow.editmode)
            {
                helper.MakeDraggable(this, this);
                DesignFrame.Visibility = System.Windows.Visibility.Visible;

                Color color = Color.FromArgb(90, 255, 0, 0);
                UpperRec.StrokeThickness = 1.0;
                UpperRec.Stroke = new SolidColorBrush(color);
            }
        }
        public string GetID() { return dataImportID; }

        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');
            input = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                input[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');
            output = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                output[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
        }

        public void SwitchLight(bool _on)
        {
        }

        private void SetContour()
        {
            int bitmapHeight = bitmapImage.PixelHeight / 2; // Jumping Jack
            int bitmapWidth = bitmapImage.PixelWidth / 2;

            DesignFrame.Height = bitmapHeight;
            DesignFrame.Width = bitmapWidth;

            SwitchUp.Height = bitmapHeight;
            SwitchUp.Width = bitmapWidth;

            SwitchDown.Height = bitmapHeight;
            SwitchDown.Width = bitmapWidth;

            UpperRec.Height = bitmapHeight - 6;
            UpperRec.Width = bitmapWidth;
        }

        private void LoadBmaps()
        {
            if (dataImportID != "")
            {
                dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);
                bool findPictureOn = false;

                if (dataRows.Length > 0)
                {
                    pictureOn = dataRows[0]["FilePictureOn"].ToString();
                    pictureOff = dataRows[0]["FilePictureOff"].ToString();
                }

                try
                {
                    SwitchUp.Source = null;
                    SwitchDown.Source = null;

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOn))
                    {
                        bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOn));
                        SetContour();
                        SwitchUp.Source = bitmapImage;

                        findPictureOn = true;
                    }

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOff))
                    {
                        if (findPictureOn)
                        {
                            SwitchDown.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOff));
                        }
                        else
                        {
                            bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOff));
                            SetContour();
                            SwitchDown.Source = bitmapImage;
                        }
                    }
                }
                catch { }
            }
        }

        public double GetSize()
        {
            return SwitchUp.Width; // Width
        }

        public double GetSizeY()
        {
            return SwitchUp.Height; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               if (switches.ignoreNextPackage)
                               {
                                   switches.ignoreNextPackage = false;
                                   MainWindow.getAllDscData = true;

                                   return;
                               }

                               vals = strData.Split(';');

                               if (vals.Length > 0) { switchValue = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               for (int i = 0; i < input.Length; i++)
                               {
                                   if (input[i] == switchValue)
                                   {
                                       state = i;
                                       SetValue(state, false);
                                   }
                               }
                           }
                           catch { return; };
                       }));
        }

        private void SetValue(int _state, bool _event, bool dontReset = false)
        {
            try
            {
                //Switches switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));

                if (switches == null) return;

                MainWindow.refeshPopup = true;
                switches.value = output[_state];
                switches.events = _event;
                switches.dontReset = dontReset;

                if (_state > 0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    SwitchDown.Visibility = System.Windows.Visibility.Visible;
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch { return; }
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Switches", dataImportID, e.Delta);
        }

        private void UpperRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                if (SwitchUp.Visibility == System.Windows.Visibility.Visible)
                {
                    SetValue(0, true);
                }
                else
                {
                    SetValue(1, true);
                }
                if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
            }
        }

        private void UpperRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;

            if (SwitchUp.Visibility == System.Windows.Visibility.Visible)
            {
                SetValue(0, true);
            }
            else
            {
                SetValue(1, true);
            }
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
        }
    }
}
