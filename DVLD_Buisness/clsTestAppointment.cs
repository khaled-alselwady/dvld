using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsTestAppointment
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int TestAppointmentID { get; set; }
        public clsTestType.enTestTypes TestTypeID { get; set; }
        public int LocalDrivingLicenseApplicationID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }
        public bool IsLocked { get; set; }
        public int RetakeTestApplicationID { get; set; }
        public clsApplication RetakeTestAppInfo { get; set; }

        public int TestID => _GetTestID();

        public clsTestAppointment()
        {
            this.TestAppointmentID = -1;
            this.TestTypeID = clsTestType.enTestTypes.VisionTest;
            this.LocalDrivingLicenseApplicationID = -1;
            this.AppointmentDate = DateTime.Now;
            this.PaidFees = -1M;
            this.CreatedByUserID = -1;
            this.IsLocked = false;
            this.RetakeTestApplicationID = -1;

            Mode = enMode.AddNew;
        }

        private clsTestAppointment(int TestAppointmentID, clsTestType.enTestTypes TestTypeID, int LocalDrivingLicenseApplicationID,
            DateTime AppointmentDate, decimal PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestApplicationID)
        {
            this.TestAppointmentID = TestAppointmentID;
            this.TestTypeID = TestTypeID;
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this.AppointmentDate = AppointmentDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsLocked = IsLocked;
            this.RetakeTestApplicationID = RetakeTestApplicationID;
            this.RetakeTestAppInfo = clsApplication.FindBaseApplication(RetakeTestApplicationID);

            Mode = enMode.Update;
        }

        private bool _AddNewTestAppointment()
        {
            this.TestAppointmentID = clsTestAppointmentData.AddNewTestAppointment((int)this.TestTypeID,
                this.LocalDrivingLicenseApplicationID, this.AppointmentDate, this.PaidFees,
                this.CreatedByUserID, this.RetakeTestApplicationID);

            return (this.TestAppointmentID != -1);
        }

        private bool _UpdateTestAppointment()
        {
            return clsTestAppointmentData.UpdateTestAppointment(this.TestAppointmentID,
                (int)this.TestTypeID, this.LocalDrivingLicenseApplicationID, this.AppointmentDate,
                this.PaidFees, this.CreatedByUserID, this.IsLocked, this.RetakeTestApplicationID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTestAppointment())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateTestAppointment();
            }

            return false;
        }

        public static clsTestAppointment Find(int TestAppointmentID)
        {
            int TestTypeID = -1;
            int LocalDrivingLicenseApplicationID = -1;
            DateTime AppointmentDate = DateTime.Now;
            decimal PaidFees = -1M;
            int CreatedByUserID = -1;
            bool IsLocked = false;
            int RetakeTestApplicationID = -1;

            bool IsFound = clsTestAppointmentData.GetTestAppointmentInfoByID(TestAppointmentID,
                ref TestTypeID, ref LocalDrivingLicenseApplicationID, ref AppointmentDate,
                ref PaidFees, ref CreatedByUserID, ref IsLocked, ref RetakeTestApplicationID);

            if (IsFound)
            {
                return new clsTestAppointment(TestAppointmentID, (clsTestType.enTestTypes)TestTypeID,
                    LocalDrivingLicenseApplicationID, AppointmentDate, PaidFees, CreatedByUserID,
                    IsLocked, RetakeTestApplicationID);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetApplicationTestAppointmentsPerTestType(clsTestType.enTestTypes TestTypeID,
            int LocalDrivingLicenseApplicationID)
        {
            return clsTestAppointmentData.GetApplicationTestAppointmentsPerTestType((int)TestTypeID, LocalDrivingLicenseApplicationID);
        }

        private int _GetTestID()
        {
            return clsTestAppointmentData.GetTestID(this.TestAppointmentID);
        }

    }
}
