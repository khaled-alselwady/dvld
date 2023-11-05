using DVLD.Classes;
using DVLD.Controls;
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
using static System.Net.Mime.MediaTypeNames;

namespace DVLD.Applications
{
    public partial class frmAddUpdateLocalDrivingLicenseApplication : Form
    {

        public enum enMode { AddNew = 0, Update = 1 };

        private enMode _Mode;
        private int _LocalDrivingLicenseApplicationID = -1;
        private int _SelectedPersonID = -1;
        clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;

        public frmAddUpdateLocalDrivingLicenseApplication()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        public frmAddUpdateLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            InitializeComponent();

            _Mode = enMode.Update;
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;

        }

        private void _FillComboBoxClassesName()
        {
            DataTable dtClassesName = clsLicenseClass.GetAllLicenseClassesName();

            foreach (DataRow row in dtClassesName.Rows)
            {
                cbLicenseClass.Items.Add(row["ClassName"].ToString());
            }
        }

        private void _ResetDefaultValues()
        {
            _FillComboBoxClassesName();

            if (_Mode == enMode.AddNew)
            {
                lblMode.Text = "New Local Driving License Application";
                this.Text = "New Local Driving License Application";

                _LocalDrivingLicenseApplication = new clsLocalDrivingLicenseApplication();
                ucPersonCardWithFilter1.FilterFocus();

                if (cbLicenseClass.Items.Count > 0)
                {
                    cbLicenseClass.SelectedIndex = 2;  // select class3 as a default
                }

                _FillApplicationInfo();
            }
            else
            {
                lblMode.Text = "Update Local Driving License Application";
                this.Text = "Update Local Driving License Application";

                btnSave.Enabled = true;
            }

        }

        private void _LoadData()
        {
            ucPersonCardWithFilter1.FilterEnabled = false;

            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(_LocalDrivingLicenseApplicationID);

            if (_LocalDrivingLicenseApplication == null)
            {
                MessageBox.Show("No Application with ID = " + _LocalDrivingLicenseApplicationID, "Application Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();

                return;
            }

            ucPersonCardWithFilter1.LoadPersonInfo(_LocalDrivingLicenseApplication.ApplicantPersonID);
            lblLocalDrivingLicebseApplicationID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
            lblApplicationDate.Text = clsFormat.DateToShort(_LocalDrivingLicenseApplication.ApplicationDate);
            cbLicenseClass.SelectedIndex = cbLicenseClass.FindString(clsLicenseClass.Find(_LocalDrivingLicenseApplication.LicenseClassID).ClassName);
            lblFees.Text = _LocalDrivingLicenseApplication.PaidFees.ToString("F0");
            lblCreatedByUser.Text = clsUser.FindByUserID(_LocalDrivingLicenseApplication.CreatedByUserID).UserName;

            _SelectedPersonID = _LocalDrivingLicenseApplication.ApplicantPersonID;
        }

        private void _FillApplicationData()
        {
            _LocalDrivingLicenseApplication.ApplicantPersonID = ucPersonCardWithFilter1.PersonID;
            _LocalDrivingLicenseApplication.ApplicationDate = DateTime.Now;
            _LocalDrivingLicenseApplication.ApplicationTypeID = 1;
            _LocalDrivingLicenseApplication.LastStatusDate = DateTime.Now;
            _LocalDrivingLicenseApplication.ApplicationStatus = clsApplication.enApplicationStatus.New;
            _LocalDrivingLicenseApplication.PaidFees = Convert.ToDecimal(lblFees.Text);
            _LocalDrivingLicenseApplication.CreatedByUserID = clsGlobal.CurrentUser.UserID;
        }

        private void _FillApplicationInfo()
        {
            lblApplicationDate.Text = DateTime.Now.ToShortDateString();
            lblCreatedByUser.Text = clsGlobal.CurrentUser.UserName;
            lblFees.Text = clsApplicationType.Find((int)clsApplication.enApplicationTypes.NewLocalDrivingLicense).ApplicationFees.ToString("F0");
        }

        private bool _IsPersonCorrect()
        {
            if (_SelectedPersonID == -1)
            {
                tabControl1.SelectedTab = tabPagePersonalInfo;

                MessageBox.Show("You have to select a person first!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            //if (_Mode == _enMode.AddNew && clsUser.IsUserExistsByPersonID(_PersonID))
            //{
            //    MessageBox.Show("Selected Person already has a user, choose another one", "Select another Person",
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);

            //    return false;
            //}

            return true;
        }

        private void _SaveLocalDrivingApplication(int LicenseClassID)
        {
            _FillApplicationData();
            _LocalDrivingLicenseApplication.LicenseClassID = LicenseClassID;

            if (_LocalDrivingLicenseApplication.Save())
            {
                lblLocalDrivingLicebseApplicationID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                //change form mode to update.
                _Mode = enMode.Update;

                lblMode.Text = "Update Local Driving License Application";

                MessageBox.Show("Data Saved Successfully.", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void frmAddUpdateLocalDrivingLicenseApplication_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (_Mode == enMode.Update)
            {
                _LoadData();
            }

        }

        private void btnApplicationInfoNext_Click(object sender, EventArgs e)
        {
            if (_IsPersonCorrect())
            {
                tabControl1.SelectedTab = tabPageApplicationInfo;

                if (lblCreatedByUser.Text == "[???]") // to check if the app data is Not already filled
                {
                    _FillApplicationInfo();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int LicenseClassID = clsLicenseClass.Find(cbLicenseClass.Text).LicenseClassID;

            int ApplicationID = clsApplication.GetActiveApplicationIDForLicenseClass
            (_SelectedPersonID, LicenseClassID, (clsApplication.enApplicationTypes.NewLocalDrivingLicense));

            if (ApplicationID != -1)
            {
                MessageBox.Show("Choose another License Class, the selected Person already has an" +
                                $" active application for the selected class with ID = {ApplicationID}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                cbLicenseClass.Focus();

                return;
            }

            // check if user already have issued license of the same driving class
            if (clsLicense.IsLicenseExistByPersonID(_SelectedPersonID, LicenseClassID))
            {
                MessageBox.Show("Person already has a license with the same applied driving class, choose different driving class.", "Not Allowed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            _SaveLocalDrivingApplication(LicenseClassID);
        }

        private void ctrlPersonCardWithFilter1_OnPersonSelected(int obj)
        {
            _SelectedPersonID = obj;
        }

        private void frmAddUpdateLocalDrivingLicenseApplication_Activated(object sender, EventArgs e)
        {
            ucPersonCardWithFilter1.FilterFocus();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage SelectedTabPage = tabControl1.SelectedTab;

            if (SelectedTabPage == tabPagePersonalInfo)
            {
                btnSave.Enabled = false;
            }

            if (SelectedTabPage == tabPageApplicationInfo)
            {
                tabControl1.SelectedTab = tabPagePersonalInfo;

                if (_IsPersonCorrect())
                {
                    tabControl1.SelectedTab = tabPageApplicationInfo;

                    if (lblCreatedByUser.Text == "[???]") // to check if the app data is Not already filled
                    {
                        _FillApplicationInfo();
                    }

                    btnSave.Enabled = true;
                }

            }
        }
    }
}
