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

namespace DVLD.Drivers
{
    public partial class frmListDrivers : Form
    {
        private DataTable _dtAllDrivers;

        public frmListDrivers()
        {
            InitializeComponent();
        }

        private string _GetRealColumnNameFromDB()
        {
            switch (cbFilterBy.Text)
            {
                case "Driver ID":
                    return "DriverID";


                case "Person ID":
                    return "PersonID";


                case "National No.":
                    return "NationalNo";


                case "Full Name":
                    return "FullName";


                default:
                    return "None";
            }
        }

        private void _RefreshDriversList()
        {
            _dtAllDrivers = clsDriver.GetAllDrivers();
            dgvShowDriverList.DataSource = _dtAllDrivers;

            lblRecordsCount.Text = dgvShowDriverList.Rows.Count.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmListDrivers_Load(object sender, EventArgs e)
        {
            _RefreshDriversList();

            cbFilterBy.SelectedIndex = 0;

            if (dgvShowDriverList.Rows.Count > 0)
            {
                dgvShowDriverList.Columns[0].HeaderText = "Driver ID";
                dgvShowDriverList.Columns[0].Width = 110;

                dgvShowDriverList.Columns[1].HeaderText = "Person ID";
                dgvShowDriverList.Columns[1].Width = 110;

                dgvShowDriverList.Columns[2].HeaderText = "National No.";
                dgvShowDriverList.Columns[2].Width = 70;

                dgvShowDriverList.Columns[3].HeaderText = "Full Name";
                dgvShowDriverList.Columns[3].Width = 300;

                dgvShowDriverList.Columns[4].HeaderText = "Date";
                dgvShowDriverList.Columns[4].Width = 180;

                dgvShowDriverList.Columns[5].HeaderText = "Active Licenses";
                dgvShowDriverList.Columns[5].Width = 110;
            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (cbFilterBy.Text != "None")
            {
                txtFilterValue.Clear();
                txtFilterValue.Focus();
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string RealColumnName = _GetRealColumnNameFromDB();

            if (string.IsNullOrWhiteSpace(txtFilterValue.Text.Trim()) ||
                cbFilterBy.Text == "None")
            {
                _dtAllDrivers.DefaultView.RowFilter = "";

                lblRecordsCount.Text = dgvShowDriverList.Rows.Count.ToString();

                return;
            }

            if (cbFilterBy.Text == "Driver ID" || cbFilterBy.Text == "Person ID")
            {
                _dtAllDrivers.DefaultView.RowFilter = string.Format("[{0}] = {1}", RealColumnName, txtFilterValue.Text.Trim());
            }
            else
            {
                _dtAllDrivers.DefaultView.RowFilter = string.Format("[{0}] like '{1}%'", RealColumnName, txtFilterValue.Text.Trim());
            }

            lblRecordsCount.Text = dgvShowDriverList.Rows.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow number in case person id or driver id is selected.
            if (cbFilterBy.Text == "Driver ID" || cbFilterBy.Text == "Person ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = (int)dgvShowDriverList.CurrentRow.Cells[1].Value;
            frmShowPersonDetails frm = new frmShowPersonDetails(PersonID);
            frm.ShowDialog();
            //refresh
            frmListDrivers_Load(null, null);

        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = (int)dgvShowDriverList.CurrentRow.Cells[1].Value;


            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(PersonID);
            frm.ShowDialog();
        }
    }
}
