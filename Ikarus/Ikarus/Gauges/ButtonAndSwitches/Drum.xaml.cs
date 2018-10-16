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
    public partial class Drum : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureUp = "";
        private string pictureMiddle = "";
        private string pictureDown = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        private double[] input = new double[] { };
        private double[] output = new double[] { };
        private double rotateSwitch = 0.0;

        private Switches switches = null;
        private bool touchDown = false;
        private bool mouseDown = false;

        BitmapImage bitmapImage = new BitmapImage();
        GaugesHelper helper = null;
        RotateTransform rtSwitch = new RotateTransform();

        private double minValue = 0.0;
        private double maxValue = 1.0;
        private double step = 0.05;
        private int relative = 0;
        private double switchState = 0.0;
        private double oldState = 0.0;

        public int GetWindowID() { return windowID; }

        public Drum()
        {
            InitializeComponent();
            Focusable = false;
            DesignFrame.Visibility = System.Windows.Visibility.Hidden;
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
                DesignFrame.StrokeThickness = 1.0;
                DesignFrame.Visibility = System.Windows.Visibility.Visible;
                Color color = Color.FromArgb(90, 255, 0, 0);
                UpperRec.StrokeThickness = 1.0;
                UpperRec.Stroke = new SolidColorBrush(color);
                LowerRec.StrokeThickness = 1.0;
                LowerRec.Stroke = new SolidColorBrush(color);
            }
        }

        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');

            if (vals.Length > 0) minValue = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);
            if (vals.Length > 1) maxValue = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture);
            if (vals.Length > 2) step = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture);
            if (vals.Length > 3) relative = Convert.ToInt32(vals[3], CultureInfo.InvariantCulture);

            switchState = minValue;
            oldState = minValue;
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

        private void SetContour()
        {
            int bitmapHeight = bitmapImage.PixelHeight / 2; // Jumping Jack
            int bitmapWidth = bitmapImage.PixelWidth / 2;

            DesignFrame.Height = bitmapHeight;
            DesignFrame.Width = bitmapWidth;

            SwitchUp.Height = bitmapHeight;
            SwitchUp.Width = bitmapWidth;

            SwitchMiddle.Height = bitmapHeight;
            SwitchMiddle.Width = bitmapWidth;

            SwitchDown.Height = bitmapHeight;
            SwitchDown.Width = bitmapWidth;

            UpperRec.Height = bitmapHeight / 2 - bitmapHeight / 8;
            UpperRec.Width = bitmapWidth - bitmapWidth / 4;
            UpperRec.Margin = new System.Windows.Thickness(bitmapWidth / 8, bitmapHeight / 12, 0, 0);

            LowerRec.Height = bitmapHeight / 2 - bitmapHeight / 8;
            LowerRec.Width = bitmapWidth - bitmapWidth / 8 * 2;
            LowerRec.Margin = new System.Windows.Thickness(bitmapWidth / 8, bitmapHeight / 24 + bitmapHeight / 2, 0, 0);

            if (rotateSwitch != 0)
            {
                rtSwitch.Angle = rotateSwitch;
                Switch.RenderTransform = rtSwitch;
                SwitchUp.Width = bitmapHeight;
            }
        }

        private void LoadBmaps()
        {
            if (dataImportID != "")
            {
                dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);
                bool findPictureOn = false;

                if (dataRows.Length > 0)
                {
                    pictureUp = dataRows[0]["FilePictureOn"].ToString();
                    pictureMiddle = dataRows[0]["FilePictureOff"].ToString();
                    pictureDown = dataRows[0]["FilePicture2On"].ToString();
                    rotateSwitch = Convert.ToDouble(dataRows[0]["Rotate"]);
                }

                try
                {
                    SwitchUp.Source = null;
                    SwitchMiddle.Source = null;
                    SwitchDown.Source = null;

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureUp))
                    {
                        bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureUp));
                        SwitchUp.Source = bitmapImage;
                        SetContour();
                        findPictureOn = true;
                    }

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureMiddle))
                    {
                        if (findPictureOn)
                            SwitchMiddle.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureMiddle));
                        else
                        {
                            bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureMiddle));
                            SwitchMiddle.Source = bitmapImage;
                            SetContour();
                            findPictureOn = true;
                        }
                    }
                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureDown))
                    {
                        if (findPictureOn)
                            SwitchDown.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureDown));
                        else
                        {
                            bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureDown));
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
                           try
                           {
                               if (switches == null) return;

                               if (switches.ignoreNextPackage)
                               {
                                   switches.ignoreNextPackage = false;
                                   MainWindow.getAllDscData = true;
                                   return;
                               }

                               vals = strData.Split(';');

                               if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (switchState > maxValue) switchState = maxValue;
                               if (switchState < minValue) switchState = minValue;

                               if (oldState != switchState)
                               {
                                   switches.value = switchState;
                                   switches.events = false;
                               }
                               oldState = switchState;
                           }
                           catch { return; };
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Switches", dataImportID, e.Delta);
        }

        private void SetValue(double _value)
        {
            try
            {
                if (switches == null) return;

                if (_value != 0)
                {
                    switches.events = true;
                    //switches.oldValue = switches.oldValue * -1; // ?????

                    if (_value == 1.0) switchState += step;
                    if (_value == -1.0) switchState += step * -1;
                    if (switchState > maxValue) switchState = maxValue;
                    if (switchState < minValue) switchState = minValue;

                    switchState = Convert.ToDouble(string.Format("{0:0.00000}", switchState), CultureInfo.InvariantCulture); // E-format

                    if (oldState != switchState)
                    {
                        if (relative == 0)
                        {
                            switches.value = switchState;
                        }
                        else
                        {
                            if (_value == 1.0) switches.value = step;
                            if (_value == -1.0) switches.value = step * -1;
                        }
                    }
                    oldState = switchState;
                }

                if (_value == 1.0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchMiddle.Visibility = System.Windows.Visibility.Hidden;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }

                if (_value == 0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                    SwitchMiddle.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }

                if (_value == -1.0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                    SwitchMiddle.Visibility = System.Windows.Visibility.Hidden;
                    SwitchDown.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch { return; }
        }

        private void RightRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown && !mouseDown)
            {
                SetValue(1.0);
                mouseDown = true;
            }
            touchDown = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown && !mouseDown)
            {
                SetValue(-1.0);
                mouseDown = true;
            }
            touchDown = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void RightRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            if (!touchDown && !mouseDown)
            {
                touchDown = true;
                SetValue(1.0);
            }
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            if (!touchDown && !mouseDown)
            {
                touchDown = true;
                SetValue(-1.0);
            }
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
        private void LeftRec_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            mouseDown = false;
            SetValue(0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            mouseDown = false;
            SetValue(0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
        private void RightRec_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            mouseDown = false;
            SetValue(0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void RightRec_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            mouseDown = false;
            SetValue(0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}
