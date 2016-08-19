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
    public partial class Potentiometer : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureKnob = "";
        private string pictureBase = "";

        private double[] input = new double[] { };
        private double[] output = new double[] { };
        private double step = 0.05;
        private double maxDegree = 360;
        private double switchState = 0.0;
        private int windowID = 0;

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public Potentiometer()
        {
            InitializeComponent();
            Focusable = false;

            DesignFrame.Visibility = System.Windows.Visibility.Hidden;

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
                DesignFrame.Visibility = System.Windows.Visibility.Visible;

                Color color = Color.FromArgb(90, 255, 0, 0);
                LeftRec.StrokeThickness = 2.0;
                LeftRec.Stroke = new SolidColorBrush(color);
                RightRec.StrokeThickness = 2.0;
                RightRec.Stroke = new SolidColorBrush(color);
            }
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');
            input = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                input[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
            if (input.Length > 2) step = input[2];
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');

            if (vals.Length > 1)
            {
                output = new double[vals.Length];

                for (int i = 0; i < vals.Length; i++)
                {
                    output[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
                }

                maxDegree = output[1];
            }

            dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);

            if (dataRows.Length > 0)
            {
                pictureKnob = dataRows[0]["FilePictureOn"].ToString();
                pictureBase = dataRows[0]["FilePictureOff"].ToString();
            }

            try
            {
                SwitchKnob.Source = null;
                SwitchBase.Source = null;

                if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureKnob))
                    SwitchKnob.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureKnob));

                if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureBase))
                    SwitchBase.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureBase));
            }
            catch { }
        }

        public double GetSize()
        {
            return 100; // Width
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           switchState = 0.0;

                           try
                           {
                               if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                           }
                           catch { return; };

                           RotateTransform rtKnob = new RotateTransform();
                           rtKnob.Angle = switchState * maxDegree;
                           SwitchKnob.RenderTransform = rtKnob;

                           dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);

                           if (dataRows.Length > 0)
                           {
                               dataRows[0]["Value"] = switchState;
                               dataRows[0]["Event"] = false;
                           }
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

            movedByElement.MouseLeftButtonUp += (a, b) => isMousePressed = false;
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;

                moveThisElement.RenderTransform = trUsercontrol;

                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID);
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID, e.Delta);
        }

        private void SetValue(double _value, bool _event)
        {
            try
            {
                dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);

                if (dataRows.Length > 0)
                {
                    switchState = Convert.ToDouble(dataRows[0]["Value"], CultureInfo.InvariantCulture); // Old State
                }
                else
                {
                    return;
                }

                if (_value == 1.0)
                {
                    switchState += step;
                    if (switchState > 1.0) switchState = 1.0;
                }

                if (_value == -1.0)
                {
                    switchState -= step;
                    if (switchState < 0.0) switchState = 0.0;
                }

                RotateTransform rtKnob = new RotateTransform();
                rtKnob.Angle = switchState * maxDegree;
                SwitchKnob.RenderTransform = rtKnob;

                dataRows[0]["Value"] = switchState;
                dataRows[0]["Event"] = _event;
            }
            catch { }
        }

        private void RightRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(-1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void RightRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(-1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}
