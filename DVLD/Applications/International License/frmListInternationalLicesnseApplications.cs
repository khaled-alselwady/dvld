using DVLD.Licenses.International_License;
using DVLD.Licenses.International_Licenses;
using DVLD.People;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.Applications.International_License
{
    public partial class frmListInternationalLicenseApplications : Form
    {
        private DataTable _dtInternationalLicenses;

        public frmListInternationalLicenseApplications()
        {
            InitializeComponent();
        }

        private string _GetRealColumnNameFromDB()
        {
            switch (cbFilter.Text)
            {
                case "International License ID":
                    return "InternationalLicenseID";

                case "Application ID":
                    return "ApplicationID";

                case "Driver ID":
                    return "DriverID";

                case "Local License ID":
                    return "IssuedUsingLocalLicenseID";

                case "Is Active":
                    return "IsActive";

                default:
                    return "None";

            }
        }

        private void _RefreshDGVList()
        {
            _dtInternationalLicenses = clsInternationalLicense.GetAllInternationalLicenses();
            dgvInternationalLicenses.DataSource = _dtInternationalLicenses;
            lblNumberOfRecords.Text = dgvInternationalLicenses.Rows.Count.ToString();

            cbFilter.SelectedIndex = 0;

            if (dgvInternationalLicenses.Rows.Count > 0)
            {
                dgvInternationalLicenses.Columns[0].HeaderText = "Int.License ID";
                dgvInternationalLicenses.Columns[0].Width = 160;

                dgvInternationalLicenses.Columns[1].HeaderText = "Application ID";
                dgvInternationalLicenses.Columns[1].Width = 150;

                dgvInternationalLicenses.Columns[2].HeaderText = "Driver ID";
                dgvInternationalLicenses.Columns[2].Width = 130;

                dgvInternationalLicenses.Columns[3].HeaderText = "L.License ID";
                dgvInternationalLicenses.Columns[3].Width = 130;

                dgvInternationalLicenses.Columns[4].HeaderText = "Issue Date";
                dgvInternationalLicenses.Columns[4].Width = 180;

                dgvInternationalLicenses.Columns[5].HeaderText = "Expiration Date";
                dgvInternationalLicenses.Columns[5].Width = 180;

                dgvInternationalLicenses.Columns[6].HeaderText = "Is Active";
                dgvInternationalLicenses.Columns[6].Width = 120;

            }
        }

        private int _GetPersonID()
        {
            // Get PersonID from Driver table using DriverID from DGV
            int DriverID = (int)dgvInternationalLicenses.CurrentRow.Cells["DriverID"].Value;

            return clsDriver.FindByDriverID(DriverID).PersonID;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmListInternationalLicenseApplications_Load(object sender, EventArgs e)
        {
            _RefreshDGVList();
        }

        private void btnNewApplication_Click(object sender, EventArgs e)
        {
            frmNewInternationalLicenseApplication frm = new frmNewInternationalLicenseApplication();
            frm.ShowDialog();
            //refresh
            frmListInternationalLicenseApplications_Load(null, null);

        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int InternationalLicenseID = (int)dgvInternationalLicenses.CurrentRow.Cells[0].Value;
            frmShowInternationalLicenseInfo frm = new frmShowInternationalLicenseInfo(InternationalLicenseID);
            frm.ShowDialog();
        }

        private void PesonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowPersonDetails frm = new frmShowPersonDetails(_GetPersonID());
            frm.ShowDialog();

        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(_GetPersonID());
            frm.ShowDialog();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = cbFilter.Text != "None";

            if (cbFilter.Text != "None")
            {
                txtFilterValue.Clear();
                txtFilterValue.Focus();
            }

            cbIsActive.Visible = (cbFilter.Text == "Is Active");

            if (cbIsActive.Visible)
            {
                cbIsActive.SelectedIndex = 0;
                txtFilterValue.Visible = false;
            }
        }

        private void cbIsReleased_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIsActive.Text == "All")
            {
                _dtInternationalLicenses.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvInternationalLicenses.Rows.Count.ToString();

                return;
            }

            _dtInternationalLicenses.DefaultView.RowFilter
                = string.Format("[{0}] = {1}", "IsActive", (cbIsActive.Text == "Yes"));

            lblNumberOfRecords.Text = dgvInternationalLicenses.Rows.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string ColumnName = _GetRealColumnNameFromDB();

            if (string.IsNullOrWhiteSpace(txtFilterValue.Text.Trim()) || cbFilter.Text == "None")
            {
                _dtInternationalLicenses.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvInternationalLicenses.Rows.Count.ToString();

                return;
            }


            _dtInternationalLicenses.DefaultView.RowFilter =
                string.Format("[{0}] = {1}", ColumnName, txtFilterValue.Text.Trim());

            lblNumberOfRecords.Text = dgvInternationalLicenses.Rows.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow numbers only because all filters are numbers.
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }
    }
}
