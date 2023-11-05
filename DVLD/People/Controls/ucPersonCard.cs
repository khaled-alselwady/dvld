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
using System.Xml.Linq;
using System.IO;
using DVLD.People;

namespace DVLD.Controls
{
    public partial class ucPersonCard : UserControl
    {
        private clsPerson _Person;
        private int _PersonID = -1;

        public int PersonID => _PersonID;
        public clsPerson SelectedPersonInfo => _Person;
        
        public ucPersonCard()
        {
            InitializeComponent();
        }

        public void ResetPersonInfo()
        {
            _PersonID = -1;
            lblPersonID.Text = "[????]";
            lblNationalNo.Text = "[????]";
            lblFullName.Text = "[????]";
            lblGender.Text = "[????]";
            lblEmail.Text = "[????]";
            lblPhone.Text = "[????]";
            lblDateOfBirth.Text = "[????]";
            lblCountry.Text = "[????]";
            lblAddress.Text = "[????]";
            pbPersonImage.Image = Resources.Male_512;
            llEditPersonInfo.Enabled = false;
        }

        private void _LoadPersonImage()
        {
            if (_Person.ImagePath != "")
            {
                if (File.Exists(_Person.ImagePath))
                    pbPersonImage.ImageLocation = _Person.ImagePath;
                else
                    MessageBox.Show("Could not find this image: = " + _Person.ImagePath,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (_Person.Gender == (byte)clsPerson.enGender.Male)
                    pbPersonImage.Image = Resources.Male_512;
                else
                    pbPersonImage.Image = Resources.Female_512;
            }
        }

        private void _FillPersonInfo()
        {
            llEditPersonInfo.Enabled = true;
            _PersonID = _Person.PersonID;
            lblPersonID.Text = _Person.PersonID.ToString();
            lblFullName.Text = _Person.FullName;
            lblNationalNo.Text = _Person.NationalNo.ToString();
            lblGender.Text = (_Person.Gender == (short)clsPerson.enGender.Male) ? (clsPerson.enGender.Male.ToString()) : (clsPerson.enGender.Female.ToString());
            lblEmail.Text = _Person.Email;
            lblAddress.Text = _Person.Address;
            lblDateOfBirth.Text = _Person.DateOfBirth.ToShortDateString();
            lblPhone.Text = _Person.Phone;
            lblCountry.Text = clsCountry.Find(_Person.NationalityCountryID).CountryName;

            _LoadPersonImage();
        }

        public void LoadPersonInfo(int PersonID)
        {
            _Person = clsPerson.Find(PersonID);

            if (_Person == null)
            {
                ResetPersonInfo();

                MessageBox.Show($"No Person with PersonID = {PersonID}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            _FillPersonInfo();
        }

        public void LoadPersonInfo(string NationalNo)
        {
            _Person = clsPerson.Find(NationalNo);

            if (_Person == null)
            {
                ResetPersonInfo();

                MessageBox.Show($"No Person with NationalNo = {NationalNo}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            _FillPersonInfo();
        }

        private void llEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddEditPerson frm = new frmAddEditPerson(_PersonID);
            frm.ShowDialog();

            //refresh
            LoadPersonInfo(_PersonID);
        }
    }
}
