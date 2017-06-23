using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für WPEkran.xaml
    /// </summary>
    public partial class WPEkran : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        string displayWindow = "";
        string failLight = "";
        string memoryLight = "";
        string turnLight = "";

        public WPEkran()
        {
            InitializeComponent();

            DisplayWindow.Text = "";
            FAILLight.Text = "";
            MEMORYLight.Text = "";
            TURNLight.Text = "";
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;

            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            helper.LoadBmaps(Frame, Light);

            SwitchLight(false);

            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
        }

        public string GetID() { return dataImportID; }

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
            return 153.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            // Font LED-Board-7 oder LCDDot-TR
            //
            // DisplayWindow, 4 Zeilen a 8 Zeichen
            // FAILlight      7 Zeichen
            // MEMORYlight    7 Zeichen
            // TURNlight      7 Zeichen

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { displayWindow = vals[0]; }
                               if (vals.Length > 1) { failLight = vals[1]; }
                               if (vals.Length > 2) { memoryLight = vals[2]; }
                               if (vals.Length > 3) { turnLight = vals[3]; }

                               DisplayWindow.Text = displayWindow.Replace("0", "O");
                               FAILLight.Text = failLight.Replace("0", "O");
                               MEMORYLight.Text = memoryLight.Replace("0", "O");
                               TURNLight.Text = turnLight.Replace("0", "O");
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
