using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsTestType
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public enum enTestTypes
        {
            VisionTest = 1,
            WrittenTest = 2,
            StreetTest = 3
        }

        public enTestTypes TestTypeID { get; set; }
        public string TestTypeTitle { get; set; }
        public string TestTypeDescription { get; set; }
        public decimal TestTypeFees { get; set; }

        public clsTestType()
        {
            this.TestTypeID = enTestTypes.VisionTest;
            this.TestTypeTitle = string.Empty;
            this.TestTypeDescription = string.Empty;
            this.TestTypeFees = -1M;

            Mode = enMode.AddNew;
        }

        private clsTestType(enTestTypes TestTypeID, string TestTypeTitle, string TestTypeDescription, decimal TestTypeFees)
        {
            this.TestTypeID = TestTypeID;
            this.TestTypeTitle = TestTypeTitle;
            this.TestTypeDescription = TestTypeDescription;
            this.TestTypeFees = TestTypeFees;

            Mode = enMode.Update;
        }

        private bool _AddNewTestType()
        {
            this.TestTypeID = (enTestTypes)clsTestTypeData.AddNewTestType(this.TestTypeTitle, this.TestTypeDescription, this.TestTypeFees);

            return (this.TestTypeTitle != "");
        }

        private bool _UpdateTestType()
        {
            return clsTestTypeData.UpdateTestType((int)this.TestTypeID, this.TestTypeTitle, this.TestTypeDescription, this.TestTypeFees);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTestType())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateTestType();
            }

            return false;
        }

        public static clsTestType Find(enTestTypes TestTypeID)
        {
            string TestTypeTitle = string.Empty;
            string TestTypeDescription = string.Empty;
            decimal TestTypeFees = -1M;

            bool IsFound = clsTestTypeData.GetTestTypeInfoByID((int)TestTypeID, ref TestTypeTitle, ref TestTypeDescription, ref TestTypeFees);

            if (IsFound)
            {
                return new clsTestType(TestTypeID, TestTypeTitle, TestTypeDescription, TestTypeFees);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetAllTestTypes()
        {
            return clsTestTypeData.GetAllTestTypes();
        }

    }
}
