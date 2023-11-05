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

namespace DVLD.Tests
{


    public partial class frmEditTestType: Form
    {

        private clsTestType.enTestTypes _TestTypeID = clsTestType.enTestTypes.VisionTest;
        private clsTestType _TestType;

        public frmEditTestType(clsTestType.enTestTypes TestTypeID)
        {
            InitializeComponent();
            _TestTypeID = TestTypeID;

        }


        private void frmEditTestType_Load(object sender, EventArgs e)
        {
          
            _TestType = clsTestType.Find( _TestTypeID);

            if (_TestType != null)
            {
              

                lblTestTypeID.Text = ((int) _TestTypeID).ToString();
                txtTitle.Text = _TestType.TestTypeTitle;
                txtDescription.Text = _TestType.TestTypeDescription;
                txtFees.Text = _TestType.TestTypeFees.ToString();
            }
            
            else

            {
                MessageBox.Show("Could not find Test Type with id = " + _TestTypeID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fields are not valid!, put the mouse over the red icon(s) to see the Error", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            _TestType.TestTypeTitle = txtTitle.Text.Trim();
            _TestType.TestTypeDescription = txtDescription.Text.Trim();
            _TestType.TestTypeFees = Convert.ToDecimal(txtFees.Text.Trim());


            if (_TestType.Save())
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

        private void txtDescription_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
            {
                e.Cancel = true;
                ErrorProvider1.SetError(txtDescription, "TestTypeDescription cannot be empty!");
            }
            else
            {
                ErrorProvider1.SetError(txtDescription, null);
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
