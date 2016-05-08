using System;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_RadioILS.xaml
    /// </summary>
    public partial class A10_RadioILS : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public A10_RadioILS()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);

            Frequenz.Text = "";

            KHz.Visibility = System.Windows.Visibility.Hidden;
            MHz.Visibility = System.Windows.Visibility.Hidden;
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
            string[] vals = _output.Split(',');
            
        }

        public double GetSize()
        {
            return 178.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           // 251;252;253;254;255

                           // ILS_window_wheel_MHz.arg_number	= 251
                           // ILS_window_wheel_MHz.input		= {8.0, 11.0}
                           // ILS_window_wheel_MHz.output		= {0.0,  0.3}

                           // ILS_window_wheel_KHz.arg_number	= 252
                           // ILS_window_wheel_KHz.input		= {10.0, 15.0, 30.0, 35.0, 50.0, 55.0, 70.0, 75.0, 90.0, 95.0}
                           // ILS_window_wheel_KHz.output		= { 0.0,  0.1,  0.2,  0.3,  0.4,  0.5,  0.6,  0.7,  0.8,  0.9}

                           string outputMHz = "";
                           string outputKHz = "";

                           string inputMHz = "000";
                           string inputKHz = "00";

                           try
                           {
                               if (vals.Length > 0) { outputMHz = vals[0]; }
                               if (vals.Length > 1) { outputKHz = vals[1]; }

                               switch (outputKHz)
                               {
                                   case "0.0":
                                       inputKHz = "10";
                                       break;
                                   case "0.1":
                                       inputKHz = "15";
                                       break;
                                   case "0.2":
                                       inputKHz = "30";
                                       break;
                                   case "0.3":
                                       inputKHz = "35";
                                       break;
                                   case "0.4":
                                       inputKHz = "50";
                                       break;
                                   case "0.5":
                                       inputKHz = "55";
                                       break;
                                   case "0.6":
                                       inputKHz = "70";
                                       break;
                                   case "0.7":
                                       inputKHz = "75";
                                       break;
                                   case "0.8":
                                       inputKHz = "90";
                                       break;
                                   case "0.9":
                                       inputKHz = "95";
                                       break;
                               }

                               switch (outputMHz)
                               {
                                   case "":
                                       inputMHz = "";
                                       break;
                                   case "0.0":
                                       inputMHz = "108";
                                       break;
                                   case "0.1":
                                       inputMHz = "109";
                                       break;
                                   case "0.2":
                                       inputMHz = "110";
                                       break;
                                   case "0.3":
                                       inputMHz = "111";
                                       break;
                               }
                           }
                           catch { return; }

                           Frequenz.Text = (inputMHz != "") ? inputMHz + "." + inputKHz : "";
                           MHz.Text = vals[0];
                           KHz.Text = vals[1];
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

                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
