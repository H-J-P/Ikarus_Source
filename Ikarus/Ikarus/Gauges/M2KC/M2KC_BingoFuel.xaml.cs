using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for M2KC_BingFuel.xaml
    /// </summary>
    public partial class M2KC_BingoFuel : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double bingo1000 = 0.0;
        double bingo100 = 0.0;
        double lbingo1000 = 0.0;
        double lbingo100 = 0.0;

        TranslateTransform ttBingo_1000 = new TranslateTransform();
        TranslateTransform ttBingo_100 = new TranslateTransform();

        public M2KC_BingoFuel()
        {
            InitializeComponent();

            //shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { bingo1000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bingo100 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               bingo1000 = 1 - bingo1000;
                               bingo100 = 1 - bingo100;

                               if (lbingo1000 != bingo1000)
                               {
                                   ttBingo_1000.Y = bingo1000 * -429;
                                   BingoFuel_1_000.RenderTransform = ttBingo_1000;
                               }
                               if (lbingo100 != bingo100)
                               {
                                   ttBingo_100.Y = bingo100 * -429;
                                   BingoFuel_100.RenderTransform = ttBingo_100;
                               }
                               lbingo1000 = bingo1000;
                               lbingo100 = bingo100;
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
