using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.People
{
    public partial class frmShowPersonDetails : Form
    {
        
        public frmShowPersonDetails(int PersonID)
        {
            InitializeComponent();
            ucPersonCard1.LoadPersonInfo(PersonID);

        }

        public frmShowPersonDetails(string NationalNo)
        {
            InitializeComponent();
            ucPersonCard1.LoadPersonInfo(NationalNo);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
