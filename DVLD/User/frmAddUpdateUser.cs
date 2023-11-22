using DVLD.Properties;
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
using DVLD.Classes;
using DVLD.People;
using DVLD.Controls;
using System.Runtime.Remoting.Messaging;

namespace DVLD.User
{
    public partial class frmAddUpdateUser : Form
    {

        private enum enMode { AddNew, Update }
        private enMode _Mode = enMode.AddNew;

        private int _UserID = -1;
        private clsUser _User;

        private int _PersonID = -1;

        public frmAddUpdateUser()
        {
            InitializeComponent();

            _Mode = enMode.AddNew;
        }

        public frmAddUpdateUser(int UserID)
        {
            InitializeComponent();

            _Mode = enMode.Update;
            _UserID = UserID;
        }

        private void _ResetDefaultValues()
        {
            if (this._Mode == enMode.AddNew)
            {
                _User = new clsUser();

                lblMode.Text = "Add New User";
                this.Text = "Add New User";

                btnSelectPerson.Enabled = true;
                btnUpdatePerson.Visible = false;
            }
            else
            {
                lblMode.Text = "Update User";
                this.Text = "Update User";

                btnSelectPerson.Enabled = false;
                btnUpdatePerson.Visible = true;
            }

            txtUsername.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            chkIsActive.Checked = true;
        }

        private void _FillFieldsWithData()
        {
            // Fill Person Info
            ucPersonCard1.LoadPersonInfo(_User.PersonID);
            _PersonID = ucPersonCard1.PersonID;

            // Fill User Info
            lblUserID.Text = _User.UserID.ToString();
            txtUsername.Text = _User.UserName.ToString();
            txtPassword.Text = _User.Password.ToString();
            txtConfirmPassword.Text = txtPassword.Text;
            chkIsActive.Checked = _User.IsActive;
        }

        private void _LoadData()
        {
            _User = clsUser.FindByUserID(_UserID);

            if (_User == null)
            {
                MessageBox.Show("No User with ID = " + _User, "User Not Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();

                return;
            }

            _FillFieldsWithData();
        }

        private void _GetPersonID(object sender, int PersonID)
        {
            _PersonID = PersonID;
        }

        private bool _IsPersonCorrect()
        {
            if (_PersonID == -1)
            {
                tabControl1.SelectedTab = tabPagePersonalInfo;

                MessageBox.Show("You have to select a person first!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            if (_Mode == enMode.AddNew && clsUser.IsUserExistsByPersonID(_PersonID))
            {
                MessageBox.Show("Selected Person already has a user, choose another one", "Select another Person",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }

        private void _SaveUser()
        {
            _User.PersonID = _PersonID;
            _User.UserName = txtUsername.Text.Trim();
            _User.Password = txtPassword.Text;
            _User.IsActive = chkIsActive.Checked;

            if (_User.Save())
            {
                lblMode.Text = "Update User";
                lblUserID.Text = _User.UserID.ToString();
                this.Text = "Update User";

                _Mode = enMode.Update;

                MessageBox.Show("Data Saved Successfully", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }

        private void frmAddUpdateUser_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (_Mode == enMode.Update)
                _LoadData();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (!this.ValidateChildren())
            {
                //Here we don't continue because the form is not valid
                MessageBox.Show("Some fields are not valid!, put the mouse over the red icon(s) to see the error",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _SaveUser();
        }

        private void txtConfirmPassword_Validating(object sender, CancelEventArgs e)
        {
            if ((!string.IsNullOrWhiteSpace(txtConfirmPassword.Text.Trim())
                && !string.IsNullOrWhiteSpace(txtPassword.Text.Trim()))
                && (txtPassword.Text.Trim() != txtConfirmPassword.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtConfirmPassword, "Password Confirmation does not match Password!");
            }
            else
            {
                errorProvider1.SetError(txtConfirmPassword, null);
            }

        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPassword, "Password cannot be blank");
            }
            else
            {
                errorProvider1.SetError(txtPassword, null);
            }

        }

        private void txtUserName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtUsername, "Username cannot be blank");
                return;
            }
            else
            {
                errorProvider1.SetError(txtUsername, null);
            }


            if ((_Mode == enMode.AddNew && clsUser.IsUserExists(txtUsername.Text.Trim())) ||
                (_Mode == enMode.Update && txtUsername.Text.Trim().ToLower() != _User.UserName.ToLower() && clsUser.IsUserExists(txtUsername.Text.Trim())))
            {
                e.Cancel = true;
                txtUsername.Focus();
                errorProvider1.SetError(txtUsername, "username is used by another user");
            }
            else
            {
                errorProvider1.SetError(txtUsername, null);
            }
        }

        private void btnPersonInfoNext_Click(object sender, EventArgs e)
        {
            if (_IsPersonCorrect())
            {
                tabControl1.SelectedTab = tabPageLoginInfo;
            }
        }
      
        private void btnSelectPerson_Click(object sender, EventArgs e)
        {
            frmFindPerson findPerson = new frmFindPerson();
            findPerson.PersonIDBack += _GetPersonID;
            findPerson.ShowDialog();

            ucPersonCard1.LoadPersonInfo(_PersonID);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage SelectedTabPage = tabControl1.SelectedTab;

            if (SelectedTabPage == tabPagePersonalInfo)
            {
                btnSave.Enabled = false;
            }

            if (SelectedTabPage == tabPageLoginInfo)
            {
                tabControl1.SelectedTab = tabPagePersonalInfo;

                if (_IsPersonCorrect())
                {
                    tabControl1.SelectedTab = tabPageLoginInfo;

                    btnSave.Enabled = true;
                }

            }
        }

        private void btnUpdatePerson_Click(object sender, EventArgs e)
        {
            frmAddEditPerson EditPerson = new frmAddEditPerson(_User.PersonID);
            EditPerson.ShowDialog();

            ucPersonCard1.LoadPersonInfo(_User.PersonID);
        }
    }
}
