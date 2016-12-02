using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Ikarus
{
    public partial class UserControlExtender : UserControl
    {
        public static void LoadBmaps(string dataImportID, object imageFrame, object imageLight, bool loadImage = true, bool imageLightOff = true)
        {
            if (loadImage)
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
                                imageFrame = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));
                        }
                        if (light.Length > 4)
                        {
                            if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + light))
                                imageLight = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));
                        }
                    }
                    catch { }
                }
            }
            if (imageLightOff)
            {
                //(object)(imageLight).
            }
        }
    }
}
