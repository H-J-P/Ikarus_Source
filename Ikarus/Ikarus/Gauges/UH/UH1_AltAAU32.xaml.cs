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
    /// Interaction logic for UH1_AltAAU32.xaml
    /// </summary>
    public partial class UH1_AltAAU32 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double altPointer = 0.0;
        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;
        double pressure1 = 0.0;
        double pressure01 = 0.0;
        double pressure001 = 0.0;
        double offFlag = 0.0;

        double laltPointer = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;
        double lpressure1 = 0.0;
        double lpressure01 = 0.0;
        double lpressure001 = 0.0;
        double loffFlag = 0.0;

        RotateTransform rtAltPointer = new RotateTransform();
        TranslateTransform ttalt10000 = new TranslateTransform();
        TranslateTransform ttalt1000 = new TranslateTransform();
        TranslateTransform ttalt100 = new TranslateTransform();
        TranslateTransform ttPressure1 = new TranslateTransform();
        TranslateTransform ttPressure01 = new TranslateTransform();
        TranslateTransform ttPressure001 = new TranslateTransform();

        public UH1_AltAAU32()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            CodeOff_flag.Visibility = System.Windows.Visibility.Visible;
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

                               if (vals.Length > 0) { altPointer = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt10000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt1000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { alt100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { pressure1 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pressure01 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { pressure001 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { offFlag = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }

                               if (laltPointer != altPointer)
                               {
                                   rtAltPointer.Angle = altPointer * 360;
                                   Pointer.RenderTransform = rtAltPointer;
                               }
                               if (lalt10000 != alt10000)
                               {
                                   ttalt10000.Y = alt10000 * -388;
                                   Alt1AAU_10000_footCount.RenderTransform = ttalt10000;
                               }
                               if (lalt1000 != alt1000)
                               {
                                   ttalt1000.Y = alt1000 * -388;
                                   Alt1AAU_1000_footCount.RenderTransform = ttalt1000;
                               }
                               if (lalt100 != alt100)
                               {
                                   ttalt100.Y = alt100 * -388;
                                   Alt1AAU_100_footCount.RenderTransform = ttalt100;
                               }
                               if (lpressure1 != pressure1)
                               {
                                   ttPressure1.Y = pressure1 * -180;
                                   AAU_32_Drum_Counter_1.RenderTransform = ttPressure1;
                               }
                               if (lpressure01 != pressure01)
                               {
                                   ttPressure01.Y = pressure01 * -180;
                                   AAU_32_Drum_Counter__1.RenderTransform = ttPressure01;
                               }
                               if (lpressure001 != pressure001)
                               {
                                   ttPressure001.Y = pressure001 * -180;
                                   AAU_32_Drum_Counter__01.RenderTransform = ttPressure001;
                               }
                               CodeOff_flag.Visibility = offFlag > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               laltPointer = altPointer;
                               lalt10000 = alt10000;
                               lalt1000 = alt1000;
                               lalt100 = alt100;
                               lpressure1 = pressure1;
                               lpressure01 = pressure01;
                               lpressure001 = pressure001;
                               loffFlag = offFlag;
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
