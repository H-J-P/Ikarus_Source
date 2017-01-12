using System.Windows.Controls;


namespace HawgTouch.GaugePack.Gauges
{
    /// <summary>
    /// Interaction logic for MagentaTest.xaml
    /// </summary>
    public partial class MagentaTest : UserControl, IHawgTouchGauge
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

        public MagentaTest()
        {
            InitializeComponent();
        }
        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            //this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //           (Action)(() =>
            //           {
            //               try
            //               {
            //                   if (vals.Length > 0)
            //                       this.Linie1.Text = vals[0];
            //                   if (vals.Length > 1)
            //                       this.Linie2.Text = vals[1];
            //                   if (vals.Length > 2)
            //                       this.Linie3.Text = vals[2];
            //                   if (vals.Length > 3)
            //                       this.Linie4.Text = vals[3];
            //                   if (vals.Length > 4)
            //                       this.Linie5.Text = vals[4];
            //                   if (vals.Length > 5)
            //                       this.Linie6.Text = vals[5];
            //                   if (vals.Length > 6)
            //                       this.Linie7.Text = vals[6];
            //                   if (vals.Length > 7)
            //                       this.Linie8.Text = vals[7];
            //                   if (vals.Length > 8)
            //                       this.Linie9.Text = vals[8];
            //                   if (vals.Length > 9)
            //                       this.Linie10.Text = vals[9];
            //               }
            //               catch { return; }
            //           }));
        }
    }
}
