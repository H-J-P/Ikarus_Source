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
    /// Interaction logic for F86_ADI.xaml
    /// </summary>
    public partial class F86_ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double attitudeIndicatorPitch = 0.0;
        double attitudeIndicatorBank = 0.0;
        double attitudeIndicatorPitchSphere = 0.0;
        double attitudeIndicatorBankNeedle = 0.0;
        double attitudeIndicatorOffFlag = 0.0;

        double lattitudeIndicatorPitch = 0.0;
        double lattitudeIndicatorBank = 0.0;
        double lattitudeIndicatorPitchSphere = 0.0;
        double lattitudeIndicatorBankNeedle = 0.0;
        double lattitudeIndicatorOffFlag = 0.0;

        TransformGroup grp = new TransformGroup();
        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();
        RotateTransform rtattitudeIndicatorBankNeedle = new RotateTransform();

        public F86_ADI()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            AttitudeIndicatorOffFlag.Visibility = System.Windows.Visibility.Visible;
            ValueText.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { attitudeIndicatorPitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { attitudeIndicatorBank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { attitudeIndicatorPitchSphere = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { attitudeIndicatorBankNeedle = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { attitudeIndicatorOffFlag = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (lattitudeIndicatorBank != attitudeIndicatorBank || lattitudeIndicatorPitchSphere != attitudeIndicatorPitchSphere)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = attitudeIndicatorPitchSphere * -203;
                                   rt.Angle = attitudeIndicatorBank * 180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);

                                   AttitudeIndicatorPitchSphere.RenderTransform = grp;
                               }
                               if (lattitudeIndicatorPitch != attitudeIndicatorPitch || lattitudeIndicatorBankNeedle != attitudeIndicatorBankNeedle)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = attitudeIndicatorPitch * 180;
                                   rt.Angle = attitudeIndicatorBankNeedle * 180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);

                                   AttitudeIndicatorPitch.RenderTransform = grp;
                               }
                               if (lattitudeIndicatorBankNeedle != attitudeIndicatorBankNeedle)
                               {
                                   rtattitudeIndicatorBankNeedle.Angle = attitudeIndicatorBankNeedle * 180;
                                   AttitudeIndicatorBank.RenderTransform = rtattitudeIndicatorBankNeedle;
                               }

                               AttitudeIndicatorOffFlag.Visibility = attitudeIndicatorOffFlag > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               //ValueText.Text = attitudeIndicatorBank.ToString();

                               lattitudeIndicatorPitch = attitudeIndicatorPitch;
                               lattitudeIndicatorBank = attitudeIndicatorBank;
                               lattitudeIndicatorPitchSphere = attitudeIndicatorPitchSphere;
                               lattitudeIndicatorBankNeedle = attitudeIndicatorBankNeedle;
                               lattitudeIndicatorOffFlag = attitudeIndicatorOffFlag;
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
