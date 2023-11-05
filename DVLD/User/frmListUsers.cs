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

namespace DVLD.User
{
    public partial class frmListUsers : Form
    {
        private DataTable _dtUsers;

        public frmListUsers()
        {
            InitializeComponent();
        }

        private string _GetRealColumnNameInDB()
        {
            switch (cbFilter.Text)
            {
                case "User ID":
                    return "UserID";

                case "Person ID":
                    return "PersonID";

                case "Full Name":
                    return "FullName";

                case "UserName":
                    return "UserName";

                case "Is Active":
                    return "IsActive";

                default:
                    return "None";
            }
        }

        private void _RenameColumnsInDGV()
        {
            if (dgvShowUsersList.Rows.Count > 0)
            {
                dgvShowUsersList.Columns[0].HeaderText = "User ID";
                dgvShowUsersList.Columns[0].Width = 110;

                dgvShowUsersList.Columns[1].HeaderText = "Person ID";
                dgvShowUsersList.Columns[1].Width = 120;

                dgvShowUsersList.Columns[2].HeaderText = "Full Name";
                dgvShowUsersList.Columns[2].Width = 350;

                dgvShowUsersList.Columns[3].HeaderText = "UserName";
                dgvShowUsersList.Columns[3].Width = 120;

                dgvShowUsersList.Columns[4].HeaderText = "Is Active";
                dgvShowUsersList.Columns[4].Width = 120;
            }
        }

        private void _RefreshUsersList()
        {
            _dtUsers = clsUser.GetAllUsers();

            dgvShowUsersList.DataSource = _dtUsers;

            lblNumberOfRecords.Text = dgvShowUsersList.Rows.Count.ToString();
        }

        private int _GetUserIDFromDGV()
        {
            return (int)dgvShowUsersList.CurrentRow.Cells["UserID"].Value;
        }

        private void _AddNewUser()
        {
            frmAddUpdateUser AddNewUser = new frmAddUpdateUser();
            AddNewUser.ShowDialog();
        }

        private void _UpdateUser()
        {
            frmAddUpdateUser EditUser = new frmAddUpdateUser(_GetUserIDFromDGV());
            EditUser.ShowDialog();
        }

        private void _ShowUserDetails()
        {
            frmUserDetails UserDetails = new frmUserDetails(_GetUserIDFromDGV());
            UserDetails.ShowDialog();
        }

        private void _DeleteUser()
        {
            if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (clsUser.DeleteUser(_GetUserIDFromDGV()))
                {
                    MessageBox.Show("Deleted Done Successfully", "Deleted",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _RefreshUsersList();
                }
                else
                {
                    MessageBox.Show("Deleted Failed", "Failed",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _ChangePassword()
        {
            frmChangePassword ChangePassword = new frmChangePassword(_GetUserIDFromDGV());
            ChangePassword.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmListUsers_Load(object sender, EventArgs e)
        {
            _RefreshUsersList();

            cbFilter.SelectedIndex = 0;

            _RenameColumnsInDGV();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Visible = (cbFilter.Text != "None") && (cbFilter.Text != "Is Active");

            if (txtSearch.Visible)
            {
                txtSearch.Text = "";
                txtSearch.Focus();
            }

            cbIsActive.Visible = (cbFilter.Text == "Is Active");

            if (cbIsActive.Visible)
            {
                cbIsActive.SelectedIndex = 0; // 0 => all  
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = _GetRealColumnNameInDB();

            //Reset the filters in case nothing selected or filter value conains nothing.
            if (txtSearch.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtUsers.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvShowUsersList.Rows.Count.ToString();
                return;
            }


            if (FilterColumn != "FullName" && FilterColumn != "UserName")
                //in this case we deal with numbers not string.
                _dtUsers.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtSearch.Text.Trim());
            else
                _dtUsers.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtSearch.Text.Trim());

            lblNumberOfRecords.Text = _dtUsers.Rows.Count.ToString();
        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIsActive.Text == "All")
                _dtUsers.DefaultView.RowFilter = "";
            else
                _dtUsers.DefaultView.RowFilter = string.Format("[{0}] = {1}", "IsActive", (cbIsActive.Text == "Yes"));


            lblNumberOfRecords.Text = dgvShowUsersList.Rows.Count.ToString();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            _AddNewUser();

            frmListUsers_Load(null, null);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

            _UpdateUser();

            frmListUsers_Load(null, null);

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _AddNewUser();

            frmListUsers_Load(null, null);

        }

        private void dgvUsers_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            frmUserDetails Frm1 = new frmUserDetails((int)dgvShowUsersList.CurrentRow.Cells[0].Value);
            Frm1.ShowDialog();

        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ShowUserDetails();
        }

        private void ChangePasswordtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ChangePassword();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow number incase person id or user id is selected.
            if (cbFilter.Text == "Person ID" || cbFilter.Text == "User ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _DeleteUser();
        }
    }
}
