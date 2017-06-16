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
    /// Interaction logic for A10_RadioVHFAM.xaml
    /// </summary>
    public partial class Display_VHF : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private string frequenz = "";
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public Display_VHF()
        {
            InitializeComponent();

            Mhz10.Text = "10";
            Mhz1.Text = "0";
            Mhz01.Text = ".0";
            Mhz001.Text = "00";
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");

            if (MainWindow.editmode)
            {
                helper.MakeDraggable(this, this);
            }
        }

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
            return 262.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { frequenz = vals[0]; }

                               if (frequenz.Length > 7)
                               {
                                   frequenz = frequenz.Substring(0, 7);
                               }
                               else
                               {
                                   if (frequenz.Length == 6) frequenz = "0" + frequenz;
                                   if (frequenz.Length == 5) frequenz = "00" + frequenz;
                                   if (frequenz.Length == 4) frequenz = "000" + frequenz;
                                   if (frequenz.Length == 3) frequenz = "000." + frequenz;
                                   if (frequenz.Length == 2) frequenz = "000.0" + frequenz;
                                   if (frequenz.Length == 1) frequenz = "000.00" + frequenz;
                               }
                               if (frequenz.Length > 0)
                               {
                                   Mhz10.Text = frequenz.Substring(0, 2);
                                   Mhz1.Text = frequenz.Substring(2, 1);
                                   Mhz01.Text = frequenz.Substring(3, 2);
                                   Mhz001.Text = frequenz.Substring(5, 2);
                               }
                               else
                               {
                                   Mhz10.Text = "";
                                   Mhz1.Text = "";
                                   Mhz01.Text = "";
                                   Mhz001.Text = "";
                               }
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
