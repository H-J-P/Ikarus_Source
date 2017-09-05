using System.Windows.Controls;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WP_REV_Eng.xaml
    /// </summary>
    public partial class WPREVEng : UserControl
    {
        //private string dataImportID = "";


        public WPREVEng()
        {
            InitializeComponent();

        }
        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}
