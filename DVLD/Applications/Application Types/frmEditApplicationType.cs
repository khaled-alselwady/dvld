using DVLD.Classes;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications
{
    public partial class frmEditApplicationType : Form
    {

        private int _ApplicationTypeID = -1;
        private clsApplicationType _ApplicationType;

        public frmEditApplicationType(int ApplicationTypeID)
        {
            InitializeComponent();

            _ApplicationTypeID = ApplicationTypeID;

        }

        private void frmEditApplicationType_Load(object sender, EventArgs e)
        {
            lblApplicationTypeID.Text = _ApplicationTypeID.ToString();

            _ApplicationType = clsApplicationType.Find(_ApplicationTypeID);

            if (_ApplicationType != null)
            {
                txtTitle.Text = _ApplicationType.ApplicationTypeTitle;
                txtFees.Text = _ApplicationType.ApplicationFees.ToString();
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid!, put the mouse over the red icon(s) to see the Error",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            _ApplicationType.ApplicationTypeTitle = txtTitle.Text.Trim();
            _ApplicationType.ApplicationFees = Convert.ToDecimal(txtFees.Text.Trim());


            if (_ApplicationType.Save())
            {
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void txtTitle_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                e.Cancel = true;
                ErrorProvider1.SetError(txtTitle, "TestTypeTitle cannot be empty!");
            }
            else
            {
                ErrorProvider1.SetError(txtTitle, null);
            };
        }

        private void txtFees_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtFees.Text.Trim()))
            {
                e.Cancel = true;
                ErrorProvider1.SetError(txtFees, "Fees cannot be empty!");
                return;
            }
            else
            {
                ErrorProvider1.SetError(txtFees, null);

            };


            if (!clsValidation.IsNumber(txtFees.Text))
            {
                e.Cancel = true;
                ErrorProvider1.SetError(txtFees, "Invalid Number.");
            }
            else
            {
                ErrorProvider1.SetError(txtFees, null);
            };

        }

    }
}
