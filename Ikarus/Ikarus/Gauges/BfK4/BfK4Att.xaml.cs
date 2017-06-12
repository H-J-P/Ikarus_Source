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
    /// Interaktionslogik für ME4KAtt.xaml
    /// </summary>
    public partial class BfK4Att : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double bank = 0.0;
        double pitch = 0.0;
        double turn = 0.0;
        double slipball = 0.0;
        double horizonCage = 0.0;

        double lbank = 0.0;
        double lpitch = 0.0;
        double lturn = 0.0;
        double lslipball = 0.0;
        double lhorizonCage = 0.0;

        RotateTransform rtTurn = new RotateTransform();
        RotateTransform rtSlipball = new RotateTransform();
        RotateTransform rtHorizonCage = new RotateTransform();
        TransformGroup grp = new TransformGroup();
        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();

        public BfK4Att()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);
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
            return 219.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { bank = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { turn = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { slipball = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { horizonCage = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (lbank != bank || lpitch != pitch)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = pitch * 35;
                                   rt.Angle = bank * 180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);
                                   Lw_ATT_Needle_hor.RenderTransform = grp;
                               }

                               if (lturn != turn)
                               {
                                   rtTurn.Angle = turn * 45;
                                   Lw_ATT_Needle_turn.RenderTransform = rtTurn;
                               }
                               if (lslipball != slipball)
                               {
                                   rtSlipball.Angle = slipball * -18;
                                   Lw_ATT_Needle_bank.RenderTransform = rtSlipball;
                               }
                               if (lhorizonCage != horizonCage)
                               {
                                   rtHorizonCage.Angle = horizonCage * 360;
                                   Lw_ATT_Bezel_cage.RenderTransform = rtHorizonCage;
                               }

                               lbank = bank;
                               lpitch = pitch;
                               lturn = turn;
                               lslipball = slipball;
                               lhorizonCage = horizonCage;
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
