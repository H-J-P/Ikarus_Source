using System;
using System.Data;
using System.Windows;

namespace Ikarus
{
    public partial class MessageBoxWindow : Window
    {
        private static int index = 0;
        private static string name = "";
        private static string posX = "";
        private static string posY = "";
        private static string height = "";
        private static string width = "";
        private static string background = "";
        private static string backgroundNight = "";

        public MessageBoxWindow(int _index)
        {
            try
            {
                index = _index;

                DataRow[] dataRows = MainWindow.dtWindows.Select("WindowID=" + index.ToString());

                if (dataRows.Length > 0)
                {
                    name = dataRows[0]["Name"].ToString();
                    posX = dataRows[0]["PosX"].ToString();
                    posY = dataRows[0]["PosY"].ToString();
                    height = dataRows[0]["Height"].ToString();
                    width = dataRows[0]["Width"].ToString();
                    background = dataRows[0]["Background"].ToString();
                    backgroundNight = dataRows[0]["BackgroundNight"].ToString();
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Dialog Panel: " + ex.ToString()); }

            Start();
        }

        public MessageBoxWindow()
        {
            Start();
        }

        private void Start()
        {
            InitializeComponent();

            tbWindowID.Text = index.ToString();
            tbDesc.Text = name;
            tbPosX.Text = posX;
            tbPosY.Text = posY;
            tbHeight.Text = height;
            tbWidth.Text = width;
            tbBackground.Text = background;
            tbBackgroundNight.Text = backgroundNight;

            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
            Close();
        }

        private string FileSelectDialog(string defaultExt, string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = MainWindow.currentDirectory + "\\Images\\Backgounds";

            dlg.DefaultExt = defaultExt;
            dlg.Filter = filter;

            var result = dlg.ShowDialog();
            if (result == false) { return ""; }

            return dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1);
        }

        private void UpdateData()
        {
            try
            {
                DataRow[] dataRows = MainWindow.dtWindows.Select("WindowID=" + index.ToString());

                if (dataRows.Length > 0)
                {
                    dataRows[0]["WindowID"] = Convert.ToInt16(tbWindowID.Text);
                    dataRows[0]["Name"] = tbDesc.Text;
                    dataRows[0]["PosX"] = tbPosX.Text;
                    dataRows[0]["PosY"] = tbPosY.Text;
                    dataRows[0]["Height"] = tbHeight.Text;
                    dataRows[0]["Width"] = tbWidth.Text;
                    dataRows[0]["Background"] = tbBackground.Text;
                    dataRows[0]["BackgroundNight"] = tbBackgroundNight.Text;
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Transmit Panel: " + ex.ToString()); }
        }

        private void ImageOnSelect_Click(object sender, RoutedEventArgs e)
        {
            background = FileSelectDialog(".png", "PNG files (*.png)|*.png|All files (*.*)|*.*");

            if (background != "") { tbBackground.Text = background; }
        }

        private void ImageOnSelect2_Click(object sender, RoutedEventArgs e)
        {
            background = FileSelectDialog(".png", "PNG files (*.png)|*.png|All files (*.*)|*.*");

            if (background != "") { tbBackgroundNight.Text = background; }
        }
    }
}
