using System;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    public partial class A10TACAN : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;


        public System.Windows.Size Size
        {
            get
            {
                return new System.Windows.Size(270, 100);
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

        public A10TACAN(/*string strDataImportID*/)
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

                           double left = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);
                           double middle = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture) * 10;
                           double right = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture) * 10;
                           double xy = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture);
                           if (left != 1.0)
                               this.LeftT.Text = (left * 10).ToString();
                           else
                               this.LeftT.Text = "";

                           this.MiddleT.Text = ((int)middle).ToString();
                           this.RightT.Text = ((int)right).ToString();

                           if (xy == 1.0)
                               this.XY.Text = "Y";
                           else
                               this.XY.Text = "X";

                       }));
        }
    }
}
