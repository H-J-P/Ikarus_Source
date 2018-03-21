using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_Hover.xaml
    /// </summary>
    public partial class MI8_Hover : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double hover_x = 0.0;
        double hover_y = 0.0;
        double hover_z = 0.0;
        double hover_lamp_off = 0.0;

        double lhover_x = 0.0;
        double lhover_y = 0.0;
        double lhover_z = 0.0;
        double lhover_lamp_off = 0.0;

        TranslateTransform tthover_x = new TranslateTransform();
        TranslateTransform ttHover_y = new TranslateTransform();
        RotateTransform rtHover_z = new RotateTransform();

        public MI8_Hover()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            diss15_hover_lamp_off.Visibility = System.Windows.Visibility.Hidden;
            FirstValue.Visibility = System.Windows.Visibility.Hidden;
            SecondValue.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { hover_x = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { hover_y = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { hover_z = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { hover_lamp_off = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               FirstValue.Text = "x " + hover_x.ToString();
                               SecondValue.Text = "y " + hover_y.ToString();

                               if (lhover_x != hover_x)
                               {
                                   if (hover_x > 0.0)
                                   {
                                       tthover_x.Y = hover_x * (-50 / 3.6) * 2;
                                       diss15_hover_x_plus.RenderTransform = tthover_x;
                                   }
                                   else
                                   {
                                       tthover_x.Y = hover_x * (25 / 3.6) * 2;
                                       diss15_hover_x_minus.RenderTransform = tthover_x;
                                   }
                               }
                               if (lhover_y != hover_y)
                               {
                                   if (hover_y > 0.0)
                                   {
                                       ttHover_y.X = hover_y * -30 / 2;
                                       diss15_hover_y_plus.RenderTransform = ttHover_y;
                                   }
                                   else
                                   {
                                       ttHover_y.X = hover_y * 20 / 2;
                                       diss15_hover_y_minus.RenderTransform = ttHover_y;
                                   }
                               }
                               if (lhover_z != hover_z)
                               {
                                   rtHover_z.Angle = hover_z * 40;
                                   diss15_hover_z.RenderTransform = rtHover_z;
                               }
                               diss15_hover_lamp_off.Visibility = (hover_lamp_off > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lhover_x = hover_x;
                               lhover_y = hover_y;
                               lhover_z = hover_z;
                               lhover_lamp_off = hover_lamp_off;
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
