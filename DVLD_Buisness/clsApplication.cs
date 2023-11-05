using System;
using System.Data;
using DVLD_DataAccess;


namespace DVLD_Business
{
    public class clsApplication
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public enum enApplicationStatus { New = 1, Canceled = 2, Completed = 3 };

        public enum enApplicationTypes
        {
            NewLocalDrivingLicense = 1,
            RenewDrivingLicense = 2,
            ReplacementForLost = 3,
            ReplacementForDamaged = 4,
            ReleaseDetainedDrivingLicense = 5,
            NewInternationalLicense = 6,
            RetakeTest = 7
        };

        public int ApplicationID { get; set; }
        public int ApplicantPersonID { get; set; }
        public clsPerson PersonInfo;
        public string ApplicationPersonFullName => clsPerson.Find(this.ApplicantPersonID).FullName;
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public clsApplicationType ApplicationTypeInfo;
        public enApplicationStatus ApplicationStatus { get; set; }
        public string StatusName
        {
            get
            {
                switch (ApplicationStatus)
                {
                    case enApplicationStatus.New:
                        return enApplicationStatus.New.ToString();

                    case enApplicationStatus.Canceled:
                        return enApplicationStatus.Canceled.ToString();

                    case enApplicationStatus.Completed:
                        return enApplicationStatus.Completed.ToString();

                    default:
                        return "Unknown";
                }
            }
        }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }
        public clsUser CreatedByUserInfo;

        public clsApplication()
        {
            this.ApplicationID = -1;
            this.ApplicantPersonID = -1;
            this.ApplicationDate = DateTime.Now;
            this.ApplicationTypeID = -1;
            this.ApplicationStatus = enApplicationStatus.New;
            this.LastStatusDate = DateTime.Now;
            this.PaidFees = -1M;
            this.CreatedByUserID = -1;

            this.Mode = enMode.AddNew;
        }

        private clsApplication(int ApplicationID, int ApplicantPersonID, DateTime ApplicationDate,
            int ApplicationTypeID, enApplicationStatus ApplicationStatus, DateTime LastStatusDate,
            decimal PaidFees, int CreatedByUserID)

        {
            this.ApplicationID = ApplicationID;
            this.ApplicantPersonID = ApplicantPersonID;
            this.PersonInfo = clsPerson.Find(ApplicantPersonID);
            this.ApplicationDate = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            this.ApplicationTypeInfo = clsApplicationType.Find(ApplicationTypeID);
            this.ApplicationStatus = ApplicationStatus;
            this.LastStatusDate = LastStatusDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            this.CreatedByUserInfo = clsUser.FindByUserID(CreatedByUserID);

            this.Mode = enMode.Update;
        }

        private bool _AddNewApplication()
        {
            this.ApplicationID = clsApplicationData.AddNewApplication(this.ApplicantPersonID,
                this.ApplicationDate, this.ApplicationTypeID, (byte)this.ApplicationStatus,
                this.LastStatusDate, this.PaidFees, this.CreatedByUserID);

            return (this.ApplicationID != -1);
        }

        private bool _UpdateApplication()
        {
            return clsApplicationData.UpdateApplication(this.ApplicationID, this.ApplicantPersonID,
                this.ApplicationDate, this.ApplicationTypeID, (byte)this.ApplicationStatus,
                this.LastStatusDate, this.PaidFees, this.CreatedByUserID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewApplication())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateApplication();
            }

            return false;
        }

        public static clsApplication FindBaseApplication(int ApplicationID)
        {
            int ApplicantPersonID = -1;
            DateTime ApplicationDate = DateTime.Now;
            int ApplicationTypeID = -1;
            byte ApplicationStatus = 0;
            DateTime LastStatusDate = DateTime.Now;
            decimal PaidFees = -1M;
            int CreatedByUserID = -1;

            bool IsFound = clsApplicationData.GetApplicationInfoByID
                (ApplicationID, ref ApplicantPersonID, ref ApplicationDate, ref ApplicationTypeID,
                ref ApplicationStatus, ref LastStatusDate, ref PaidFees, ref CreatedByUserID);

            if (IsFound)
            {
                return new clsApplication(ApplicationID, ApplicantPersonID, ApplicationDate,
                    ApplicationTypeID, (enApplicationStatus)ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID);
            }
            else
            {
                return null;
            }
        }

        public bool Cancel()
        {
            return clsApplicationData.UpdateStatus(this.ApplicationID, (byte)enApplicationStatus.Canceled);
        }

        public bool SetCompleted()
        {
            return clsApplicationData.UpdateStatus(this.ApplicationID, (byte)enApplicationStatus.Completed);
        }

        public bool Delete()
        {
            return clsApplicationData.DeleteApplication(this.ApplicationID);
        }

        public static int GetActiveApplicationIDForLicenseClass(int PersonID,
            int LicenseClassID, enApplicationTypes ApplicationTypeID)
        {
            return clsApplicationData.GetActiveApplicationIDForLicenseClass(PersonID, LicenseClassID, (int)ApplicationTypeID);
        }
    }
}
