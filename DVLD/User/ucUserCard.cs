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

namespace DVLD.Controls
{
    public partial class ucUserCard : UserControl
    {

        private int _UserID = -1;
        private clsUser _User;

        public int UserID => _UserID;

        public ucUserCard()
        {
            InitializeComponent();
        }

        private void _ResetUserInfo()
        {
            lblUserID.Text = "[???]";
            lblUsername.Text = "[???]";
            lblIsActive.Text = "[???]";

            ucPersonCard1.ResetPersonInfo();
        }

        private void _FillUserInfo()
        {
            lblUserID.Text = _User.UserID.ToString();
            lblUsername.Text = _User.UserName;
            lblIsActive.Text = (_User.IsActive) ? "Yes" : "No";

            ucPersonCard1.LoadPersonInfo(_User.PersonID);
        }

        public void LoadUserInfo(int UserID)
        {
            _UserID = UserID;
            _User = clsUser.FindByUserID(UserID);

            if (_User == null)
            {
                _ResetUserInfo();

                MessageBox.Show($"No User with UserID = {UserID}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _FillUserInfo();
                ucPersonCard1.LoadPersonInfo(_User.PersonID);
            }
        }

        public void LoadUserInfo(string Username)
        {
            _User = clsUser.FindByUsername(Username);

            if (_User == null)
            {
                _ResetUserInfo();

                MessageBox.Show($"No User with Username = {Username}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _FillUserInfo();
                ucPersonCard1.LoadPersonInfo(_User.PersonID);
            }
        }
    }
}
