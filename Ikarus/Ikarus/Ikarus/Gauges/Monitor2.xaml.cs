using System;
using System.IO;
using System.Drawing.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    /// <summary>
    /// Interaction logic for Monitor2.xaml
    /// </summary>
    public partial class Monitor2 : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public System.Windows.Size Size
        {
            get
            {
                return new System.Windows.Size(300, 300);
            }
        }

        public Monitor2()
        {
            InitializeComponent();
            try { LoadFontFamily(Directory.GetCurrentDirectory() + "\\Font\\digital-7 (mono).ttf"); }
            catch (Exception e)
            {
                this.Linie1.Text = e.ToString();
            }

            //string heading = "0.012500";
            //string sHeading = "";

            //sHeading = Convert.ToInt16(360 * Convert.ToDouble(heading, CultureInfo.InvariantCulture)).ToString();

            //if (sHeading.Length == 0)
            //    sHeading = "000";
            //else if (sHeading.Length == 1)
            //    sHeading = "00" + sHeading;
            //else if (sHeading.Length == 2)
            //    sHeading = "0" + sHeading;

            this.Linie1.Text = (7300 % 3600).ToString();
            //this.Linie2.Text = Convert.ToDouble(sHeading[1].ToString(), CultureInfo.InvariantCulture).ToString();
            //this.Linie3.Text = Convert.ToDouble(sHeading[2].ToString(), CultureInfo.InvariantCulture).ToString();
        }

        public void LoadFontFamily(string fileName)
        {
            PrivateFontCollection fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(fileName);
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               if (vals.Length > 0)
                                   this.Linie1.Text = vals[0];
                               if (vals.Length > 1)
                                   this.Linie2.Text = vals[1];
                               if (vals.Length > 2)
                                   this.Linie3.Text = vals[2];
                               if (vals.Length > 3)
                                   this.Linie4.Text = vals[3];
                               if (vals.Length > 4)
                                   this.Linie5.Text = vals[4];
                               if (vals.Length > 5)
                                   this.Linie6.Text = vals[5];
                               if (vals.Length > 6)
                                   this.Linie7.Text = vals[6];
                               if (vals.Length > 7)
                                   this.Linie8.Text = vals[7];
                               if (vals.Length > 8)
                                   this.Linie9.Text = vals[8];
                               if (vals.Length > 9)
                                   this.Linie10.Text = vals[9];
                           }
                           catch (Exception e)
                           {
                               this.Linie1.Text = e.ToString();
                               return;
                           }
                       }));
        }
    }
}
