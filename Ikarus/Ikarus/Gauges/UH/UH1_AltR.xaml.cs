using System;
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
    /// Interaction logic for UH1_AltR.xaml
    /// </summary>
    public partial class UH1_AltR : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double rAltOff = 0.0;
        double rAltLo = 0.0;
        double rAltHi = 0.0;

        double rAltDigit1 = 0.0;
        double rAltDigit2 = 0.0;
        double rAltDigit3 = 0.0;
        double rAltDigit4 = 0.0;

        double rAltNeedle = 0.0;
        double rAltLoIndex = 0.0;
        double rAltHiIndex = 0.0;

        double lrAltNeedle = 0.0;
        double lrAltLoIndex = 0.0;
        double lrAltHiIndex = 0.0;

        RotateTransform rtrAltNeedle = new RotateTransform();
        RotateTransform rtrAltLoIndex = new RotateTransform();
        RotateTransform rtrAltHiIndex = new RotateTransform();

        public UH1_AltR()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            RALT_Off_Flag.Visibility = System.Windows.Visibility.Hidden;
            RALT_LO_Lamp.Visibility = System.Windows.Visibility.Hidden;
            RALT_HI_Lamp.Visibility = System.Windows.Visibility.Hidden;

            Digit4.Visibility = System.Windows.Visibility.Hidden;
            Digit3.Visibility = System.Windows.Visibility.Hidden;
            Digit2.Visibility = System.Windows.Visibility.Hidden;
            Digit1.Visibility = System.Windows.Visibility.Hidden;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { rAltNeedle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rAltOff = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rAltLo = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { rAltHi = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { rAltLoIndex = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { rAltHiIndex = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { rAltDigit4 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { rAltDigit3 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { rAltDigit2 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { rAltDigit1 = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }

                               if (rAltDigit4 == 1.0) { rAltDigit4 = 0.0; }
                               if (rAltDigit3 == 1.0) { rAltDigit3 = 0.0; }
                               if (rAltDigit2 == 1.0) { rAltDigit2 = 0.0; }
                               if (rAltDigit1 == 1.0) { rAltDigit1 = 0.0; }

                               if (lrAltNeedle != rAltNeedle)
                               {
                                   rtrAltNeedle.Angle = rAltNeedle * 360;
                                   RALT_Needle.RenderTransform = rtrAltNeedle;
                               }
                               if (lrAltLoIndex != rAltLoIndex)
                               {
                                   rtrAltLoIndex.Angle = (rAltLoIndex * 270) + 20;
                                   RALT_LO_Index.RenderTransform = rtrAltLoIndex;
                               }
                               if (lrAltHiIndex != rAltHiIndex)
                               {
                                   rtrAltHiIndex.Angle = (rAltHiIndex * 270) + (rAltLoIndex * 270) - 5;
                                   RALT_HI_Index.RenderTransform = rtrAltHiIndex;
                               }
                               RALT_Off_Flag.Visibility = rAltOff > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               RALT_LO_Lamp.Visibility = rAltLo > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               RALT_HI_Lamp.Visibility = rAltHi > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               Digit4.Visibility = rAltDigit4 > 0.0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Digit3.Visibility = rAltDigit3 > 0.0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Digit2.Visibility = rAltDigit2 > 0.0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               Digit1.Visibility = rAltOff > 0.8 ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

                               if (rAltDigit4 > 0.0)
                               {
                                   Digit3.Visibility = System.Windows.Visibility.Visible;
                                   Digit2.Visibility = System.Windows.Visibility.Visible;
                               }
                               else if (rAltDigit3 > 0.0)
                               {
                                   Digit2.Visibility = System.Windows.Visibility.Visible;
                               }

                               Digit4.Text = (rAltDigit4 * 10).ToString();
                               Digit3.Text = (rAltDigit3 * 10).ToString();
                               Digit2.Text = (rAltDigit2 * 10).ToString();
                               Digit1.Text = (rAltDigit1 * 10).ToString();

                               lrAltNeedle = rAltNeedle;
                               lrAltLoIndex = rAltLoIndex;
                               lrAltHiIndex = rAltHiIndex;
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
