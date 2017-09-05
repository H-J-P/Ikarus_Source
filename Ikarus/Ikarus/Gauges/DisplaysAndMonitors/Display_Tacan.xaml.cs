using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F5E_TacanDisplay.xaml
    /// </summary>
    public partial class Display_Tacan : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        private double value1000 = 0.0;
        private double value100 = 0.0;
        private double value10 = 0.0;
        private double value1 = 0.0;

        private int valueInt1000 = 0;
        private int valueInt100 = 0;
        private int valueInt10 = 0;
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public Display_Tacan()
        {
            InitializeComponent();
            Pos1.Text = "X";
            Pos1000.Text = " ";
            //Pos100.Text = " ";
            //Pos10.Text = " ";
            //Pos1.Text = " ";
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");

            if (MainWindow.editmode)
            {
                helper.MakeDraggable(this, this);
            }
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
            return Frame.Width;
        }

        public double GetSizeY()
        {
            return Frame.Height;
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { value1000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { value100 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { value10 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { value1 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if(value1000 == 1.0)
                               {
                                   Pos1000.Text = " ";
                               }
                               else
                               {
                                   valueInt1000 = (Convert.ToUInt16(value1000 * 10));
                                   Pos1000.Text = valueInt1000.ToString().Substring(0, 1);
                               }
                               valueInt100 = Convert.ToUInt16(value100 * 10);
                               valueInt10 = Convert.ToUInt16(value10 * 10);

                               Pos100.Text = valueInt100.ToString().Substring(0, 1);
                               Pos10.Text = valueInt10.ToString().Substring(0, 1);
                               Pos1.Text = value1 < 0.93 ? "X" : "Y";
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
