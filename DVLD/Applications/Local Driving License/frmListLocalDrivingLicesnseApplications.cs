using DVLD.Applications;
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
using static System.Net.Mime.MediaTypeNames;
using DVLD_Business;
using DVLD.DriverLicense;
using System.Security.Cryptography;
using DVLD.Drivers;
using DVLD.Licenses.International_License;

namespace DVLD.Tests
{
    public partial class frmListLocalDrivingLicenseApplications : Form
    {
        private DataTable _dtAllLocalDrivingLicenseApplications;
        public frmListLocalDrivingLicenseApplications()
        {
            InitializeComponent();
        }

        private string _GetRealColumnNameInDB()
        {
            switch (cbFilter.Text)
            {
                case "L.D.L.AppID":
                    return "LocalDrivingLicenseApplicationID";

                case "National No.":
                    return "NationalNo";

                case "Full Name":
                    return "FullName";

                case "Status":
                    return "Status";

                default:
                    return "None";
            }
        }

        private int _GetIDFromDGV()
        {
            return (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;
        }

        private void _RefreshDGVList()
        {
            _dtAllLocalDrivingLicenseApplications = clsLocalDrivingLicenseApplication.GetAllLocalDrivingLicenseApplications();
            dgvShowLocalDrivingLicenseApplicationsList.DataSource = _dtAllLocalDrivingLicenseApplications;

            lblNumberOfRecords.Text = dgvShowLocalDrivingLicenseApplicationsList.Rows.Count.ToString();
        }

        private int _GetLicenseIDFromDataBase()
        {
            return clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(_GetIDFromDGV()).GetActiveLicenseID();
        }

        private void frmListLocalDrivingLicenseApplications_Load(object sender, EventArgs e)
        {
            _RefreshDGVList();

            if (dgvShowLocalDrivingLicenseApplicationsList.Rows.Count > 0)
            {

                dgvShowLocalDrivingLicenseApplicationsList.Columns[0].HeaderText = "L.D.L.AppID";
                dgvShowLocalDrivingLicenseApplicationsList.Columns[0].Width = 120;

                dgvShowLocalDrivingLicenseApplicationsList.Columns[1].HeaderText = "Driving Class";
                dgvShowLocalDrivingLicenseApplicationsList.Columns[1].Width = 300;

                dgvShowLocalDrivingLicenseApplicationsList.Columns[2].HeaderText = "National No.";
                dgvShowLocalDrivingLicenseApplicationsList.Columns[2].Width = 150;

                dgvShowLocalDrivingLicenseApplicationsList.Columns[3].HeaderText = "Full Name";
                dgvShowLocalDrivingLicenseApplicationsList.Columns[3].Width = 350;

                dgvShowLocalDrivingLicenseApplicationsList.Columns[4].HeaderText = "Application Date";
                dgvShowLocalDrivingLicenseApplicationsList.Columns[4].Width = 170;

                dgvShowLocalDrivingLicenseApplicationsList.Columns[5].HeaderText = "Passed Tests";
                dgvShowLocalDrivingLicenseApplicationsList.Columns[5].Width = 150;
            }

            cbFilter.SelectedIndex = 0;
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmLocalDrivingLicenseApplicationInfo frm =
                        new frmLocalDrivingLicenseApplicationInfo(_GetIDFromDGV());
            frm.ShowDialog();
            //refresh
            frmListLocalDrivingLicenseApplications_Load(null, null);

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Visible = (cbFilter.Text != "None");

            if (txtSearch.Visible)
            {
                txtSearch.Clear();
                txtSearch.Focus();
            }

            cbStatus.Visible = (cbFilter.Text == "Status");

            if (cbFilter.Text != "None")
                txtSearch.Visible = !cbStatus.Visible;

            if (cbStatus.Visible)
                cbStatus.SelectedIndex = 0;
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {

            string RealColumn = _GetRealColumnNameInDB();

            if (string.IsNullOrWhiteSpace(txtSearch.Text) || cbFilter.Text == "None")
            {
                _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = "";

                lblNumberOfRecords.Text = dgvShowLocalDrivingLicenseApplicationsList.Rows.Count.ToString();

                return;
            }

            if (cbFilter.Text == "L.D.L.AppID")
            {
                // here I'll search about ID so I'll deal with integers
                _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] = {1}", RealColumn, txtSearch.Text.Trim());
            }
            else
            {
                _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] like '{1}%'", RealColumn, txtSearch.Text.Trim());
            }

            lblNumberOfRecords.Text = dgvShowLocalDrivingLicenseApplicationsList.Rows.Count.ToString();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int LocalDrivingLicenseApplicationID = (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells[0].Value;

            frmAddUpdateLocalDrivingLicenseApplication frm =
                         new frmAddUpdateLocalDrivingLicenseApplication(LocalDrivingLicenseApplicationID);
            frm.ShowDialog();

            frmListLocalDrivingLicenseApplications_Load(null, null);
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow number incase L.D.L.AppID id is selected.
            if (cbFilter.Text == "L.D.L.AppID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void _ScheduleTest(clsTestType.enTestTypes TestType)
        {
            frmListTestAppointments frm = new frmListTestAppointments(_GetIDFromDGV(), TestType);
            frm.ShowDialog();
            //refresh
            frmListLocalDrivingLicenseApplications_Load(null, null);
        }

        private void scheduleVisionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ScheduleTest(clsTestType.enTestTypes.VisionTest);
        }

        private void scheduleWrittenTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            _ScheduleTest(clsTestType.enTestTypes.WrittenTest);
        }

        private void scheduleStreetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            _ScheduleTest(clsTestType.enTestTypes.StreetTest);
        }

        private void btnAddNewApplication_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicenseApplication frm = new frmAddUpdateLocalDrivingLicenseApplication();
            frm.ShowDialog();
            //refresh
            frmListLocalDrivingLicenseApplications_Load(null, null);
        }

        private void issueDrivingLicenseFirstTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int LocalDrivingLicenseApplicationID = (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells[0].Value;
            frmIssueDriverLicenseFirstTime frm = new frmIssueDriverLicenseFirstTime(LocalDrivingLicenseApplicationID);
            frm.ShowDialog();
            //refresh
            frmListLocalDrivingLicenseApplications_Load(null, null);
        }

        private void cmsApplications_Opening(object sender, CancelEventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells[0].Value;
            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication =
                    clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID
                                                    (LocalDrivingLicenseApplicationID);

            int TotalPassedTests = (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells[5].Value;

            bool LicenseExists = LocalDrivingLicenseApplication.IsLicenseIssued();

            //Enabled only if person passed all tests and Does not have license. 
            issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = (TotalPassedTests == 3) && !LicenseExists;

            showLicenseToolStripMenuItem.Enabled = LicenseExists;
            editToolStripMenuItem.Enabled = !LicenseExists && (LocalDrivingLicenseApplication.ApplicationStatus == clsApplication.enApplicationStatus.New);
            ScheduleTestsMenu.Enabled = !LicenseExists;

            //Enable/Disable Cancel Menu Item
            //We only cancel the applications with status=new.
            CancelApplicationToolStripMenuItem.Enabled = (LocalDrivingLicenseApplication.ApplicationStatus == clsApplication.enApplicationStatus.New);

            //Enable/Disable Delete Menu Item
            //We only allow delete incase the application status is new not complete or Cancelled.
            DeleteApplicationToolStripMenuItem.Enabled =
                (LocalDrivingLicenseApplication.ApplicationStatus == clsApplication.enApplicationStatus.New);

            //Enable Disable Schedule Menu and it's sub Menu
            bool PassedVisionTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestTypes.VisionTest);
            bool PassedWrittenTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestTypes.WrittenTest);
            bool PassedStreetTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestTypes.StreetTest);

            ScheduleTestsMenu.Enabled = (!PassedVisionTest || !PassedWrittenTest || !PassedStreetTest) && (LocalDrivingLicenseApplication.ApplicationStatus == clsApplication.enApplicationStatus.New);

            if (ScheduleTestsMenu.Enabled)
            {
                //To Allow Schedule vision test, Person must not passed the same test before.
                scheduleVisionTestToolStripMenuItem.Enabled = !PassedVisionTest;

                //To Allow Schedule written test, Person must pass the vision test and must not passed the same test before.
                scheduleWrittenTestToolStripMenuItem.Enabled = PassedVisionTest && !PassedWrittenTest;

                //To Allow Schedule street test, Person must pass the vision * written tests, and must not passed the same test before.
                scheduleStreetTestToolStripMenuItem.Enabled = PassedVisionTest && PassedWrittenTest && !PassedStreetTest;

            }
        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells[0].Value;

            int LicenseID = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(
               LocalDrivingLicenseApplicationID).GetActiveLicenseID();

            if (LicenseID != -1)
            {
                frmShowLicenseInfo frm = new frmShowLicenseInfo(LicenseID);
                frm.ShowDialog();

            }
            else
            {
                MessageBox.Show("No License Found!", "No License", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }

        private void CancelApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure do want to cancel this application?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            int LocalDrivingLicenseApplicationID = (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            if (LocalDrivingLicenseApplication != null)
            {
                if (LocalDrivingLicenseApplication.Cancel())
                {
                    MessageBox.Show("Application Cancelled Successfully.", "Cancelled",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //refresh the form again.
                    frmListLocalDrivingLicenseApplications_Load(null, null);
                }
                else
                {
                    MessageBox.Show("Could not cancel application.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure do want to delete this application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            int LocalDrivingLicenseApplicationID = (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            if (LocalDrivingLicenseApplication != null)
            {
                if (LocalDrivingLicenseApplication.Delete())
                {
                    MessageBox.Show("Application Deleted Successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //refresh the form again.
                    frmListLocalDrivingLicenseApplications_Load(null, null);
                }
                else
                {
                    MessageBox.Show("Could not delete application, other data depends on it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvShowLocalDrivingLicenseApplicationsList.CurrentRow.Cells[0].Value;
            clsLocalDrivingLicenseApplication localDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(localDrivingLicenseApplication.ApplicantPersonID);
            frm.ShowDialog();
        }

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStatus.Text == "All")
            {
                _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvShowLocalDrivingLicenseApplicationsList.Rows.Count.ToString();

                return;
            }

            _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter =
                string.Format("[{0}] like '{1}%'", "Status", cbStatus.Text);

            lblNumberOfRecords.Text = dgvShowLocalDrivingLicenseApplicationsList.Rows.Count.ToString();
        }
    }
}
