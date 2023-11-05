using DVLD.Applications.Detain_License;
using DVLD.DriverLicense;
using DVLD.Licenses.International_License;
using DVLD.People;
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

namespace DVLD.Applications.Release_Detained_License
{
    public partial class frmListDetainedLicenses : Form
    {

        private DataTable _dtDetainLicenses;

        public frmListDetainedLicenses()
        {
            InitializeComponent();
        }

        private string _GetRealColumnName()
        {
            switch (cbFilterBy.Text)
            {
                case "Detain ID":
                    return "DetainID";


                case "Is Released":
                    return "IsReleased";


                case "National No.":
                    return "NationalNo";


                case "Full Name":
                    return "FullName";


                case "Release Application ID":
                    return "ReleaseApplicationID";


                default:
                    return "None";
            }
        }

        private void _RefreshListInDGV()
        {
            _dtDetainLicenses = clsDetainedLicense.GetAllDetainedLicenses();

            dgvDetainedLicenses.DataSource = _dtDetainLicenses;

            lblNumberOfRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private int _GetPersonID()
        {
            // we will get the person ID from Drivers table, so I have to find DriverID first from License table then use it to find PersonID
            int LicenseID = (int)dgvDetainedLicenses.CurrentRow.Cells["LicenseID"].Value;
            return clsLicense.Find(LicenseID).DriverInfo.PersonID;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmListDetainedLicenses_Load(object sender, EventArgs e)
        {
            _RefreshListInDGV();

            cbFilterBy.SelectedIndex = 0;

            if (dgvDetainedLicenses.Rows.Count > 0)
            {
                dgvDetainedLicenses.Columns[0].HeaderText = "D.ID";
                dgvDetainedLicenses.Columns[0].Width = 90;

                dgvDetainedLicenses.Columns[1].HeaderText = "L.ID";
                dgvDetainedLicenses.Columns[1].Width = 90;

                dgvDetainedLicenses.Columns[2].HeaderText = "D.Date";
                dgvDetainedLicenses.Columns[2].Width = 160;

                dgvDetainedLicenses.Columns[3].HeaderText = "Is Released";
                dgvDetainedLicenses.Columns[3].Width = 110;

                dgvDetainedLicenses.Columns[4].HeaderText = "Fine Fees";
                dgvDetainedLicenses.Columns[4].Width = 110;

                dgvDetainedLicenses.Columns[5].HeaderText = "Release Date";
                dgvDetainedLicenses.Columns[5].Width = 160;

                dgvDetainedLicenses.Columns[6].HeaderText = "N.No.";
                dgvDetainedLicenses.Columns[6].Width = 90;

                dgvDetainedLicenses.Columns[7].HeaderText = "Full Name";
                dgvDetainedLicenses.Columns[7].Width = 330;

                dgvDetainedLicenses.Columns[8].HeaderText = "Release App.ID";
                dgvDetainedLicenses.Columns[8].Width = 150;

            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string ColumnName = _GetRealColumnName();

            if (string.IsNullOrWhiteSpace(txtFilterValue.Text.Trim()) || cbFilterBy.Text == "None")
            {
                _dtDetainLicenses.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();

                return;
            }

            if (cbFilterBy.Text == "Detain ID" || cbFilterBy.Text == "Release Application ID")
            {
                // searching for only numbers
                _dtDetainLicenses.DefaultView.RowFilter =
                    string.Format("[{0}] = {1}", ColumnName, txtFilterValue.Text.Trim());
            }
            else
            {
                _dtDetainLicenses.DefaultView.RowFilter =
                    string.Format("[{0}] like '{1}%'", ColumnName, txtFilterValue.Text.Trim());
            }

            lblNumberOfRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (cbFilterBy.Text != "None")
            {
                txtFilterValue.Clear();
                txtFilterValue.Focus();
            }

            cbIsReleased.Visible = (cbFilterBy.Text == "Is Released");

            if (cbIsReleased.Visible)
            {
                cbIsReleased.SelectedIndex = 0;
                txtFilterValue.Visible = false;
            }
        }

        private void cbIsReleased_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIsReleased.Text == "All")
            {
                _dtDetainLicenses.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();

                return;
            }

            _dtDetainLicenses.DefaultView.RowFilter =
                string.Format("[{0}] = {1}", "IsReleased", (cbIsReleased.Text == "Yes"));

            lblNumberOfRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow number incase person id or user id is selected.
            if (cbFilterBy.Text == "Detain ID" || cbFilterBy.Text == "Release Application ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(_GetPersonID());
            frm.ShowDialog();
        }

        private void PesonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowPersonDetails frm = new frmShowPersonDetails(_GetPersonID());
            frm.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LicenseID = (int)dgvDetainedLicenses.CurrentRow.Cells[1].Value;

            frmShowLicenseInfo frm = new frmShowLicenseInfo(LicenseID);
            frm.ShowDialog();

        }

        private void btnDetainLicense_Click(object sender, EventArgs e)
        {
            frmDetainLicenseApplication frm = new frmDetainLicenseApplication();
            frm.ShowDialog();
            //refresh
            frmListDetainedLicenses_Load(null, null);

        }

        private void btnReleaseDetainedLicense_Click(object sender, EventArgs e)
        {
            frmReleaseDetainedLicenseApplication frm = new frmReleaseDetainedLicenseApplication();
            frm.ShowDialog();
            //refresh
            frmListDetainedLicenses_Load(null, null);

        }

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int LicenseID = (int)dgvDetainedLicenses.CurrentRow.Cells[1].Value;

            frmReleaseDetainedLicenseApplication frm = new frmReleaseDetainedLicenseApplication(LicenseID);
            frm.ShowDialog();
            //refresh
            frmListDetainedLicenses_Load(null, null);



        }

        private void cmsApplications_Opening(object sender, CancelEventArgs e)
        {
            if (dgvDetainedLicenses.SelectedCells.Count > 0)
            {
                releaseDetainedLicenseToolStripMenuItem.Enabled = !(bool)dgvDetainedLicenses.CurrentRow.Cells[3].Value;
            }
        }
    }
}



