using System;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Xml.Linq;
using DVLD_DataAccess;
using static System.Net.Mime.MediaTypeNames;
using static DVLD_Business.clsTestType;


namespace DVLD_Business
{
    public class clsLocalDrivingLicenseApplication : clsApplication
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int LocalDrivingLicenseApplicationID { get; set; }

        public int LicenseClassID { get; set; }
        public clsLicenseClass LicenseClassInfo;

        public string PersonFullName => base.ApplicationPersonFullName;

        public clsLocalDrivingLicenseApplication()
        {
            this.LocalDrivingLicenseApplicationID = -1;
            this.LicenseClassID = -1;

            Mode = enMode.AddNew;
        }

        private clsLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID, int ApplicationID,
            int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID,
             enApplicationStatus ApplicationStatus, DateTime LastStatusDate,
             decimal PaidFees, int CreatedByUserID, int LicenseClassID)
        {
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID; ;
            this.ApplicationID = ApplicationID;
            this.ApplicantPersonID = ApplicantPersonID;
            this.ApplicationDate = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            this.ApplicationStatus = ApplicationStatus;
            this.LastStatusDate = LastStatusDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            this.LicenseClassID = LicenseClassID;
            this.LicenseClassInfo = clsLicenseClass.Find(LicenseClassID);
            Mode = enMode.Update;
        }

        private bool _AddNewLocalDrivingLicenseApplication()
        {
            this.LocalDrivingLicenseApplicationID = clsLocalDrivingLicenseApplicationData.
                AddNewLocalDrivingLicenseApplication(this.ApplicationID, this.LicenseClassID);

            return (this.LocalDrivingLicenseApplicationID != -1);
        }

        private bool _UpdateLocalDrivingLicenseApplication()
        {
            return clsLocalDrivingLicenseApplicationData.UpdateLocalDrivingLicenseApplication
                (this.LocalDrivingLicenseApplicationID, this.ApplicationID, this.LicenseClassID);
        }

        public bool Save()
        {
            base.Mode = (clsApplication.enMode)Mode;

            if (!base.Save())
                return false;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewLocalDrivingLicenseApplication())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateLocalDrivingLicenseApplication();
            }

            return false;
        }

        public static clsLocalDrivingLicenseApplication FindByLocalDrivingAppLicenseID(int LocalDrivingLicenseApplicationID)
        {
            int ApplicationID = -1, LicenseClassID = -1;

            bool IsFound = clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByID
                            (LocalDrivingLicenseApplicationID, ref ApplicationID, ref LicenseClassID);
            if (IsFound)
            {
                //now we find the base application
                clsApplication Application = clsApplication.FindBaseApplication(ApplicationID);

                if (Application == null)
                {
                    return null;
                }

                //we return new object of that person with the right data
                return new clsLocalDrivingLicenseApplication(
                    LocalDrivingLicenseApplicationID, Application.ApplicationID,
                    Application.ApplicantPersonID, Application.ApplicationDate,
                     Application.ApplicationTypeID, Application.ApplicationStatus,
                     Application.LastStatusDate, Application.PaidFees,
                     Application.CreatedByUserID, LicenseClassID);
            }
            else
            {
                return null;
            }
        }

        public static clsLocalDrivingLicenseApplication FindByApplicationID(int ApplicationID)
        {
            // 
            int LocalDrivingLicenseApplicationID = -1, LicenseClassID = -1;

            bool IsFound = clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByApplicationID
                (ApplicationID, ref LocalDrivingLicenseApplicationID, ref LicenseClassID);


            if (IsFound)
            {
                //now we find the base application
                clsApplication Application = clsApplication.FindBaseApplication(ApplicationID);

                //we return new object of that person with the right data
                return new clsLocalDrivingLicenseApplication(
                    LocalDrivingLicenseApplicationID, Application.ApplicationID,
                    Application.ApplicantPersonID,
                                     Application.ApplicationDate, Application.ApplicationTypeID,
                                    Application.ApplicationStatus, Application.LastStatusDate,
                                     Application.PaidFees, Application.CreatedByUserID, LicenseClassID);
            }
            else
                return null;


        }

        public bool Delete()
        {
            bool IsLocalDrivingApplicationDeleted = false;
            bool IsBaseApplicationDeleted = false;

            IsLocalDrivingApplicationDeleted = clsLocalDrivingLicenseApplicationData.
                DeleteLocalDrivingLicenseApplication(this.LocalDrivingLicenseApplicationID);

            if (IsLocalDrivingApplicationDeleted)
            {
                IsBaseApplicationDeleted = base.Delete();
            }

            return IsBaseApplicationDeleted;
        }

        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            return clsLocalDrivingLicenseApplicationData.GetAllLocalDrivingLicenseApplications();
        }

        public int GetActiveLicenseID()
        {
            return clsLicense.GetActiveLicenseIDByPersonID(this.ApplicantPersonID, this.LicenseClassID);
        }

        public byte GetPassedTestCount()
        {
            return clsTest.GetPassedTestCount(this.LocalDrivingLicenseApplicationID);
        }

        public bool DoesPassAllTest()
        {
            return clsTest.DoesPassAllTest(this.LocalDrivingLicenseApplicationID);
        }

        public static byte GetPassedTestCount(int LocalDrivingLicenseApplicationID)
        {
            return clsTest.GetPassedTestCount(LocalDrivingLicenseApplicationID);
        }

        public bool IsLicenseIssued()
        {
            return (GetActiveLicenseID() != -1);
        }

        public bool DoesPassTestType(clsTestType.enTestTypes TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.
                DoesPassTestType(this.LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public bool DoesAttendTestType(clsTestType.enTestTypes TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.DoesAttendTestType
                (this.LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public byte TotalTrialsPerTest(clsTestType.enTestTypes TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.TotalTrialsPerTest
                (this.LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public bool IsThereAcActiveScheduledTest(clsTestType.enTestTypes TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.IsThereAcActiveScheduledTest
                (this.LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public static bool IsThereAcActiveScheduledTest(int LocalDrivingLicenseApplicationID,
            clsTestType.enTestTypes TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.IsThereAcActiveScheduledTest
                (LocalDrivingLicenseApplicationID, (int)TestTypeID);
        }

        public clsTest GetLastTestPerTestType(clsTestType.enTestTypes TestTypeID)
        {
            return clsTest.FindLastTestPerPersonAndLicenseClass(this.LocalDrivingLicenseApplicationID, TestTypeID);
        }

        public int IssueLicenseForTheFirstTime(string Notes, int CreatedByUserID)
        {
            #region Get DriverID
            int DriverID = -1;

            clsDriver Driver = clsDriver.FindByPersonID(this.ApplicantPersonID);

            if (Driver == null)
            {
                Driver = new clsDriver();

                Driver.PersonID = this.ApplicantPersonID;
                Driver.CreatedByUserID = CreatedByUserID;

                if (Driver.Save())
                {
                    DriverID = Driver.DriverID;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                DriverID = Driver.DriverID;
            }
            #endregion

            #region ADD License and Return LicenseID
            clsLicense License = new clsLicense();

            License.ApplicationID = this.ApplicationID;
            License.DriverID = DriverID;
            License.LicenseClassID = this.LicenseClassID;
            License.IssueDate = DateTime.Now;
            License.ExpirationDate = DateTime.Now.AddYears(this.LicenseClassInfo.DefaultValidityLength);
            License.Notes = Notes;
            License.PaidFees = this.LicenseClassInfo.ClassFees;
            License.IsActive = true;
            License.IssueReason = clsLicense.enIssueReason.FirstTime;
            License.CreatedByUserID = CreatedByUserID;

            if (License.Save())
            {
                // now we should set the application status to complete.  
                this.SetCompleted();

                return License.LicenseID;
            }
            else
            {
                return -1;
            }
            #endregion
        }

    }
}
