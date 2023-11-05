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
    public partial class frmFindPerson : Form
    {

        // Declare a Delegate
        public delegate void GetPersonID(object sender, int PersonID);

        // Declare an event
        public event GetPersonID PersonIDBack;


        public frmFindPerson()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // Trigger the event to send data back to the caller form.
            PersonIDBack?.Invoke(this, ctrlPersonCardWithFilter1.PersonID);

            this.Close();
        }
    }
}
