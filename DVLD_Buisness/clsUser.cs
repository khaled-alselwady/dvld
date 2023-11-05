using System;
using System.Data;
using System.Runtime.InteropServices;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int UserID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo;
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }


        public clsUser()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = "";
            this.Password = "";
            this.IsActive = true;

            this.Mode = enMode.AddNew;
        }

        private clsUser(int userID, int personID, string userName, string password,
            bool isActive)
        {
            this.UserID = userID;
            this.PersonID = personID;
            this.PersonInfo = clsPerson.Find(personID);
            this.UserName = userName;
            this.Password = password;
            this.IsActive = isActive;

            this.Mode = enMode.Update;
        }

        private bool _AddNewUser()
        {
            this.UserID = clsUserData.AddNewUser(this.PersonID, this.UserName,
                this.Password, this.IsActive);

            return (this.UserID != -1);
        }
        private bool _UpdateUser()
        {
            return clsUserData.UpdateUser(this.UserID, this.PersonID, this.UserName,
                this.Password, this.IsActive);
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateUser();
            }

            return false;
        }


        public static clsUser FindByUserID(int UserID)
        {
            int personID = -1;
            string UserName = "", Password = "";
            bool IsActive = false;

            bool IsFound = clsUserData.GetUserInfoByUserID
                (UserID, ref personID, ref UserName, ref Password, ref IsActive);

            if (IsFound)
            {
                return new clsUser(UserID, personID, UserName, Password, IsActive);
            }
            else
            {
                return null;
            }
        }
        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1;
            string UserName = "", Password = "";
            bool IsActive = false;

            bool IsFound = clsUserData.GetUserInfoByPersonID
                                (PersonID, ref UserID, ref UserName, ref Password, ref IsActive);

            if (IsFound)
                //we return new object of that User with the right data
                return new clsUser(UserID, UserID, UserName, Password, IsActive);
            else
                return null;
        }
        public static clsUser FindByUsername(string UserName)
        {
            int userID = -1, personID = -1;
            string Password = "";
            bool IsActive = false;

            bool IsFound = clsUserData.GetUserInfoByUserName
                (UserName, ref userID, ref personID, ref Password, ref IsActive);

            if (IsFound)
            {
                return new clsUser(userID, personID, UserName, Password, IsActive);
            }
            else
            {
                return null;
            }
        }
        public static clsUser FindByUsernameAndPassword(string UserName, string Password)
        {
            int userID = -1, personID = -1;
            bool IsActive = false;

            bool IsFound = clsUserData.GetUserInfoByUserNameAndPassword
                (UserName, Password, ref userID, ref personID, ref IsActive);

            if (IsFound)
            {
                return new clsUser(userID, personID, UserName, Password, IsActive);
            }
            else
            {
                return null;
            }
        }

        public static bool DeleteUser(int UserID)
        {
            return clsUserData.DeleteUser(UserID);
        }

        public static bool IsUserExists(int UserID)
        {
            return clsUserData.IsUserExists(UserID);
        }
        public static bool IsUserExists(string UserName)
        {
            return clsUserData.IsUserExists(UserName);
        }
        public static bool IsUserExists(string UserName, string Password)
        {
            return clsUserData.IsUserExists(UserName, Password);
        }
        public static bool IsUserExistsByPersonID(int PersonID)
        {
            return clsUserData.IsUserExistsByPersonID(PersonID);
        }

        public static DataTable GetAllUsers()
        {
            return clsUserData.GetAllUsers();
        }

        public bool ChangePassword(string NewPassword)
        {
            return clsUserData.ChangePassword(this.UserID, NewPassword);
        }
    }
}
