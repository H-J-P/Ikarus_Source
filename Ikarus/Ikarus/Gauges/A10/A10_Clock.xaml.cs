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
    /// Interaction logic for A10_Clock.xaml
    /// </summary>
    public partial class A10_Clock : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        string valueHour = "";
        string state = "";

        string hours = "";
        string minutes = "";
        string seconds = "";
        double lseconds = 0.0;

        RotateTransform rtSec = new RotateTransform();

        public A10_Clock()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);

            HourMinutesDigits.Text = "";
            SecondsDigits.Text = "";
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

                               if (vals.Length > 0)
                               {
                                   valueHour = vals[0];
                                   hours = (valueHour[0].ToString() + valueHour[1].ToString()).ToString();
                                   minutes = (valueHour[2].ToString() + valueHour[3].ToString()).ToString();
                                   seconds = (valueHour[4].ToString() + valueHour[5].ToString()).ToString();
                               }
                               if (vals.Length > 1) { state = vals[1]; }

                               if (lseconds != Convert.ToDouble(seconds, CultureInfo.InvariantCulture))
                               {
                                   rtSec.Angle = Convert.ToDouble(seconds, CultureInfo.InvariantCulture) / 60 * 360;
                                   Second.RenderTransform = rtSec;
                               }
                               SecondsDigits.Text = seconds;
                               HourMinutesDigits.Text = (hours + ":" + minutes);
                               State.Text = state;

                               lseconds = Convert.ToDouble(seconds, CultureInfo.InvariantCulture);
                           }
                           catch { return; }
                       }));
        }

        public void MakeDraggable_New(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            bool isMousePressed = false;
            System.Windows.Media.TranslateTransform transform = new System.Windows.Media.TranslateTransform(0, 0);
            moveThisElement.RenderTransform = transform;
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;

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
                if (!isMousePressed)
                {
    
                    return;
                }

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);

                transform.X += currentPoint.X - originalPoint.X;
                transform.Y += currentPoint.Y - originalPoint.Y;
            };
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
                if (!isMousePressed || !MainWindow.editmode)
                {
                    return;
                }

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
