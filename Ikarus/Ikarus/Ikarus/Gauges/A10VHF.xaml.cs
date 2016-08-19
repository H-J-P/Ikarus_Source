using System;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class A10VHF : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;


        public System.Windows.Size Size
        {
            get
            {
                return new System.Windows.Size(400, 100);
            }
        }

        public string DataImportID
        {
            get
            {
                return _DataImportID;
            }
            set
            {
                _DataImportID = value;
            }
        }

        public A10VHF(/*string strDataImportID*/)
        {
            InitializeComponent();
            //_DataImportID = strDataImportID;
        }


        public void UpdateGauge(string strData)
        {

            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {

                           double left = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture) * 20;
                           double left2 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture) * 100;
                           double right = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture) * 10;
                           double right2 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture) * 100;

                           this.LeftT.Text = ((int)left).ToString();
                           this.Left2T.Text = ((int)left2).ToString();
                           this.RightT2.Text = ((int)right).ToString();
                           if (((int)right2) == 0)
                            this.RightT.Text = "00";
                           else
                            this.RightT.Text = ((int)right2).ToString();

                       }));
        }
    }
}
