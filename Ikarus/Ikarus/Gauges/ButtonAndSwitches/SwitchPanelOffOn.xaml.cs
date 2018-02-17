using System;
using System.Data;
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
    public partial class SwitchPanelOffOn : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureOn = "";
        private string pictureOff = "";
        private int windowID = 0;
        private int panelID = 0;
        private int _on = 0;
        private string[] vals = new string[] { };
        private bool touchDown = false;
        BitmapImage bitmapImage = new BitmapImage();
        GaugesHelper helper = null;
        RotateTransform rtSwitch = new RotateTransform();
        private double rotateSwitch = 0.0;

        public int GetWindowID() { return windowID; }

        public SwitchPanelOffOn()
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
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Switches");

            if (MainWindow.editmode)
            {
                helper.MakeDraggable(this, this);
                DesignFrame.StrokeThickness = 2.0;
                DesignFrame.Visibility = System.Windows.Visibility.Visible;
                Color color = Color.FromArgb(90, 255, 0, 0);
                UpperRec.StrokeThickness = 2.0;
                UpperRec.Stroke = new SolidColorBrush(color);
                LowerRec.StrokeThickness = 2.0;
                LowerRec.Stroke = new SolidColorBrush(color);
            }
        }
        public string GetID() { return dataImportID; }

        public void SetInput(string _input)
        {
        }

        public void SwitchLight(bool _on)
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

            SwitchUp.Height = bitmapHeight;
            SwitchUp.Width = bitmapWidth;

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
                DesignFrame.Width = bitmapHeight / 1.2;
                rtSwitch.Angle = rotateSwitch;
                Switch.RenderTransform = rtSwitch;
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
                    pictureOn = dataRows[0]["FilePictureOn"].ToString();
                    pictureOff = dataRows[0]["FilePictureOff"].ToString();
                    rotateSwitch = Convert.ToDouble(dataRows[0]["Rotate"]);
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

                if (_on == 1)
                {
                    if (MainWindow.cockpitWindows.Count > panelID) MainWindow.cockpitWindows[panelID].Visibility = System.Windows.Visibility.Visible;
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    if (panelID > 0)
                    {
                        if (MainWindow.cockpitWindows.Count > panelID) MainWindow.cockpitWindows[panelID].Visibility = System.Windows.Visibility.Hidden;
                        SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                        SwitchDown.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }

        public double GetSize()
        {
            return DesignFrame.Width; // Width
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
                           //string[] vals = strData.Split(';');
                           //double switchState = 0.0;

                           //try
                           //{
                           //    if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                           //}
                           //catch { return; };

                           //if (switchState > 0.8) { SetValue(1.0, false); }
                           //else { SetValue(0.0, false); }
                       }));
        }

        private void SetValue(double _value)
        {
            try
            {
                dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);
                MainWindow.refeshPopup = true;

                if (dataRows.Length > 0)
                {
                    try
                    {
                        panelID = Convert.ToInt16(dataRows[0]["Input"]) - 1;  // Switch To Panel
                    }
                    catch { panelID = 99999; }

                    if (panelID < 0) panelID = 99999;
                }

                if (_value > 0.0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;

                    if (panelID != 0 && panelID != 99999)
                    {
                        if (MainWindow.cockpitWindows.Count > panelID) MainWindow.cockpitWindows[panelID].Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                    SwitchDown.Visibility = System.Windows.Visibility.Visible;

                    if (panelID != 0 && panelID != 99999)
                    {
                        if (MainWindow.cockpitWindows.Count > panelID) MainWindow.cockpitWindows[panelID].Visibility = System.Windows.Visibility.Hidden;
                    }
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
                MainWindow.refeshPopup = true;
                SetValue(1.0);
                if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
            }
        }

        private void LowerRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (!touchDown)
            {
                MainWindow.refeshPopup = true;
                SetValue(0.0);
                if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
            }
        }

        private void UpperRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            MainWindow.refeshPopup = true;

            touchDown = true;
            SetValue(1.0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LowerRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            MainWindow.refeshPopup = true;

            touchDown = true;
            SetValue(0.0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LowerRec_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}
