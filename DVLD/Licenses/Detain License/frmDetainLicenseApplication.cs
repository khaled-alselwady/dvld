using DVLD.Classes;
using DVLD.DriverLicense;
using DVLD.Licenses.Controls;
using DVLD.Licenses.International_License;
using DVLD.Licenses.International_Licenses;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.Detain_License
{
    public partial class frmDetainLicenseApplication : Form
    {

        private int _DetainID = -1;
        private int _SelectedLicenseID = -1;
        public frmDetainLicenseApplication()
        {
            InitializeComponent();
        }

        private void btnDetain_Click(object sender, EventArgs e)
        {

            if (!this.ValidateChildren())
            {
                MessageBox.Show("You should enter the fees first!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (MessageBox.Show("Are you sure you want to detain this license?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }


            _DetainID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.
                Detain(Convert.ToDecimal(txtFineFees.Text), clsGlobal.CurrentUser.UserID);

            if (_DetainID == -1)
            {
                MessageBox.Show("Failed to Detain License", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            lblDetainID.Text = _DetainID.ToString();
            MessageBox.Show("License Detained Successfully with ID = " + _DetainID.ToString(),
                "License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnDetain.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            txtFineFees.Enabled = false;
            llShowLicenseInfo.Enabled = true;
        }

        private void frmDetainLicenseApplication_Load(object sender, EventArgs e)
        {
            lblDetainDate.Text = clsFormat.DateToShort(DateTime.Now);
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected(int obj)
        {
            _SelectedLicenseID = obj;

            llShowLicenseHistory.Enabled = (_SelectedLicenseID != -1);

            if (_SelectedLicenseID == -1)
            {
                return;
            }

            lblLicenseID.Text = _SelectedLicenseID.ToString();

            //ToDo: make sure the license is not detained already.
            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.IsDetained)
            {
                MessageBox.Show("Selected License is already detained, choose another one.", "Not allowed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnDetain.Enabled = false;

                return;
            }

            txtFineFees.Focus();
            btnDetain.Enabled = true;
        }

        private void frmDetainLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory frm =
              new frmShowPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID);
            frm.ShowDialog();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm =
             new frmShowLicenseInfo(_SelectedLicenseID);
            frm.ShowDialog();
        }

        private void txtFineFees_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtFineFees.Text.Trim()))
            {
                e.Cancel = true;
                ErrorProvider1.SetError(txtFineFees, "Fees cannot be empty!");
                return;
            }
            else
            {
                ErrorProvider1.SetError(txtFineFees, null);

            };


            if (!clsValidation.IsNumber(txtFineFees.Text))
            {
                e.Cancel = true;
                ErrorProvider1.SetError(txtFineFees, "Invalid Number.");
            }
            else
            {
                ErrorProvider1.SetError(txtFineFees, null);
            };
        }
    }
}
