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
using System.Xml.Linq;
using DVLD.Classes;
using DVLD_Business;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DVLD.People
{
    public partial class frmListPeople : Form
    {
        private DataTable _dtAllPeople;
        private DataTable _dtPeople;

        public frmListPeople()
        {
            InitializeComponent();
        }

        private string _GetRealColumnNameInDB()
        {
            switch (cbFilter.Text)
            {
                case "Person ID":
                    return "PersonID";

                case "National No.":
                    return "NationalNo";

                case "First Name":
                    return "FirstName";

                case "Second Name":
                    return "SecondName";

                case "Third Name":
                    return "ThirdName";

                case "Last Name":
                    return "LastName";

                case "Nationality":
                    return "CountryName";

                case "Gender":
                    return "GenderCaption";

                case "Phone":
                    return "Phone";

                case "Email":
                    return "Email";

                default:
                    return "None";
            }
        }

        private void _RenameColumnsInDGV()
        {
            if (dgvShowPeopleList.Rows.Count > 0)
            {

                dgvShowPeopleList.Columns[0].HeaderText = "Person ID";
                dgvShowPeopleList.Columns[0].Width = 110;

                dgvShowPeopleList.Columns[1].HeaderText = "National No.";
                dgvShowPeopleList.Columns[1].Width = 120;


                dgvShowPeopleList.Columns[2].HeaderText = "First Name";
                dgvShowPeopleList.Columns[2].Width = 120;

                dgvShowPeopleList.Columns[3].HeaderText = "Second Name";
                dgvShowPeopleList.Columns[3].Width = 140;


                dgvShowPeopleList.Columns[4].HeaderText = "Third Name";
                dgvShowPeopleList.Columns[4].Width = 120;

                dgvShowPeopleList.Columns[5].HeaderText = "Last Name";
                dgvShowPeopleList.Columns[5].Width = 120;

                dgvShowPeopleList.Columns[6].HeaderText = "Gender";
                dgvShowPeopleList.Columns[6].Width = 120;

                dgvShowPeopleList.Columns[7].HeaderText = "Date Of Birth";
                dgvShowPeopleList.Columns[7].Width = 140;

                dgvShowPeopleList.Columns[8].HeaderText = "Nationality";
                dgvShowPeopleList.Columns[8].Width = 120;


                dgvShowPeopleList.Columns[9].HeaderText = "Phone";
                dgvShowPeopleList.Columns[9].Width = 120;


                dgvShowPeopleList.Columns[10].HeaderText = "Email";
                dgvShowPeopleList.Columns[10].Width = 170;
            }
        }

        private void _RefreshDataTablePeople()
        {
            _dtAllPeople = clsPerson.GetAllPeople();

            //only select the columns that you want to show in the grid
            _dtPeople = _dtAllPeople.DefaultView.ToTable(false, "PersonID", "NationalNo",
                                                       "FirstName", "SecondName", "ThirdName", "LastName",
                                                       "GenderCaption", "DateOfBirth", "CountryName",
                                                       "Phone", "Email");
        }

        private void _RefreshPeopleList()
        {
            _RefreshDataTablePeople();

            dgvShowPeopleList.DataSource = _dtPeople;

            lblNumberOfRecords.Text = dgvShowPeopleList.Rows.Count.ToString();
        }

        private int _GetPersonIDFromDGV()
        {
            return (int)dgvShowPeopleList.CurrentRow.Cells["PersonID"].Value;
        }

        private void _ShowDetailsPeople()
        {
            frmShowPersonDetails ShowPersonDetails = new frmShowPersonDetails(_GetPersonIDFromDGV());
            ShowPersonDetails.ShowDialog();
        }

        private void _AddNewPerson()
        {
            frmAddEditPerson AddNewPerson = new frmAddEditPerson();
            AddNewPerson.ShowDialog();
        }

        private void _UpdatePerson()
        {
            frmAddEditPerson UpdatePerson = new frmAddEditPerson(_GetPersonIDFromDGV());
            UpdatePerson.ShowDialog();
        }

        private void _DeletePerson()
        {
            if (MessageBox.Show("Are you sure you want to delete this person?", "Confirm", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (clsPerson.DeletePerson(_GetPersonIDFromDGV()))
                {
                    MessageBox.Show("Deleted Done Successfully", "Deleted",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _RefreshPeopleList();
                }
                else
                {
                    MessageBox.Show("Deleted Failed", "Failed",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void frmListPeople_Load(object sender, EventArgs e)
        {

            _RefreshPeopleList();

            cbFilter.SelectedIndex = 0; // to select `None` option by default

            _RenameColumnsInDGV();

        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {

            string FilterColumn = _GetRealColumnNameInDB();

            //Reset the filters in case nothing selected or filter value contains nothing.
            if (txtSearch.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtPeople.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvShowPeopleList.Rows.Count.ToString();
                return;
            }


            if (FilterColumn == "PersonID")
                //in this case we deal with integer not string.
                _dtPeople.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtSearch.Text.Trim());
            else
                _dtPeople.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtSearch.Text.Trim());

            lblNumberOfRecords.Text = dgvShowPeopleList.Rows.Count.ToString();

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {

            txtSearch.Visible = (cbFilter.Text != "None");

            if (txtSearch.Visible)
            {
                txtSearch.Text = "";
                txtSearch.Focus();
            }

            cbGender.Visible = (cbFilter.Text == "Gender");


            if (cbFilter.Text != "None")
                txtSearch.Visible = !(cbGender.Visible);
            else
            {
                // if the gender was female or male and then I choose None option, in that case I have to reset the filter
                _dtPeople.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvShowPeopleList.Rows.Count.ToString();
            }


            if (cbGender.Visible)
                cbGender.SelectedIndex = 0;

        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ShowDetailsPeople();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _UpdatePerson();
            _RefreshPeopleList();
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Feature Is Not Implemented Yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This Feature Is Not Implemented Yet!", "Not Ready!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _DeletePerson();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _AddNewPerson();
            _RefreshPeopleList();
        }

        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            _AddNewPerson();
            _RefreshPeopleList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvPeople_DoubleClick(object sender, EventArgs e)
        {
            _ShowDetailsPeople();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow number incase person id is selected.
            if (cbFilter.Text == "Person ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void cbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbGender.Text == "All")
            {
                _dtPeople.DefaultView.RowFilter = "";
                lblNumberOfRecords.Text = dgvShowPeopleList.Rows.Count.ToString();

                return;
            }

            _dtPeople.DefaultView.RowFilter = string.Format("[{0}] like '{1}'", "GenderCaption", cbGender.Text);

            lblNumberOfRecords.Text = dgvShowPeopleList.Rows.Count.ToString();
        }
    }
}
