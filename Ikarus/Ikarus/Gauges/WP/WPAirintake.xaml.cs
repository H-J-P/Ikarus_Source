using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPAirintake.xaml
    /// </summary>
    public partial class WPAirintake : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double wedge_1 = 0.0;
        double wedge_2 = 0.0;

        double lwedge_1 = 0.0;
        double lwedge_2 = 0.0;

        TranslateTransform ttWedge_1 = new TranslateTransform();
        TranslateTransform ttWedge_2 = new TranslateTransform();

        public WPAirintake()
        {
            InitializeComponent();
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

                               if (vals.Length > 0) { wedge_1 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { wedge_2 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lwedge_1 != wedge_1)
                               {
                                   ttWedge_1.Y = wedge_1 * -125;
                                   Wedge_1.RenderTransform = ttWedge_1;
                               }
                               if (lwedge_2 != wedge_2)
                               {
                                   ttWedge_2.Y = wedge_2 * -125;
                                   Wedge_2.RenderTransform = ttWedge_2;
                               }
                               lwedge_1 = wedge_1;
                               lwedge_2 = wedge_2;
                           }
                           catch { return; };
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
