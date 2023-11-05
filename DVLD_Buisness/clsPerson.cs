using System;
using System.Data;
using System.Xml.Linq;
using DVLD_DataAccess;


namespace DVLD_Business
{
    public class clsPerson
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public enum enGender { Male = 0, Female = 1 };

        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public string FullName => (string.IsNullOrEmpty(ThirdName)) ? (FirstName + " " + SecondName + " " + LastName) : (FirstName + " " + SecondName + " " + ThirdName + " " + LastName);
        public string NationalNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public short Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int NationalityCountryID { get; set; }

        public clsCountry CountryInfo;

        public string ImagePath { get; set; }


        public clsPerson()
        {
            this.PersonID = -1;
            this.FirstName = "";
            this.SecondName = "";
            this.ThirdName = "";
            this.LastName = "";
            this.NationalNo = "";
            this.DateOfBirth = DateTime.Now;
            this.Gender = 0;
            this.Address = "";
            this.Phone = "";
            this.Email = "";
            this.NationalityCountryID = -1;
            this.ImagePath = "";

            Mode = enMode.AddNew;
        }

        private clsPerson(int personID, string firstName, string secondName, string thirdName, string lastName,
            string nationalNo, DateTime dateOfBirth, short gender, string address, string phone, string email,
            int nationalityCountryID, string imagePath)
        {
            this.PersonID = personID;
            this.FirstName = firstName;
            this.SecondName = secondName;
            this.ThirdName = thirdName;
            this.LastName = lastName;
            this.NationalNo = nationalNo;
            this.DateOfBirth = dateOfBirth;
            this.Gender = gender;
            this.Address = address;
            this.Phone = phone;
            this.Email = email;
            this.NationalityCountryID = nationalityCountryID;
            this.CountryInfo = clsCountry.Find(nationalityCountryID);
            this.ImagePath = imagePath;

            Mode = enMode.Update;
        }

        private bool _AddNewPerson()
        {
            this.PersonID = clsPersonData.AddNewPerson(this.FirstName, this.SecondName, this.ThirdName, this.LastName,
                this.NationalNo, this.DateOfBirth, this.Gender, this.Address, this.Phone, this.Email,
                this.NationalityCountryID, this.ImagePath);

            return (this.PersonID != -1);
        }
        private bool _UpdatePerson()
        {
            return clsPersonData.UpdatePerson(this.PersonID, this.FirstName, this.SecondName, this.ThirdName, this.LastName,
                this.NationalNo, this.DateOfBirth, this.Gender, this.Address, this.Phone, this.Email,
                this.NationalityCountryID, this.ImagePath);
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPerson())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdatePerson();
            }

            return false;
        }


        public static clsPerson Find(int personID)
        {
            string firstName = "", secondName = "", thirdName = "", lastName = "", nationalNo = "", address = "", phone = "", email = "", imagePath = "";
            DateTime dateOfBirth = DateTime.Now;
            int nationalityCountryID = -1;
            short Gender = 0;

            bool IsFound = clsPersonData.GetPersonInfoByID
                                (
                                personID, ref firstName, ref secondName, ref thirdName,
                                ref lastName, ref nationalNo, ref dateOfBirth, ref Gender, ref address, ref phone,
                                ref email, ref nationalityCountryID, ref imagePath
                                );

            if (IsFound)
            {
                return new clsPerson(personID, firstName, secondName, thirdName, lastName, nationalNo, dateOfBirth,
                    Gender, address, phone, email, nationalityCountryID, imagePath);
            }
            else
            {
                return null;
            }
        }

        public static clsPerson Find(string nationalNo)
        {
            string firstName = "", secondName = "", thirdName = "", lastName = "", address = "", phone = "", email = "", imagePath = "";
            DateTime dateOfBirth = DateTime.Now;
            int personID = -1, nationalityCountryID = -1;
            short Gender = 0;

            bool IsFound = clsPersonData.GetPersonInfoByNationalNo
                                (
                                nationalNo, ref personID, ref firstName, ref secondName, ref thirdName,
                                ref lastName, ref dateOfBirth, ref Gender, ref address, ref phone,
                                ref email, ref nationalityCountryID, ref imagePath
                                );

            if (IsFound)
            {
                return new clsPerson(personID, firstName, secondName, thirdName, lastName, nationalNo, dateOfBirth,
                    Gender, address, phone, email, nationalityCountryID, imagePath);
            }
            else
            {
                return null;
            }
        }

        public static bool DeletePerson(int PersonID)
        {
            return clsPersonData.DeletePerson(PersonID);
        }

        public static bool IsPersonExists(int PersonID)
        {
            return clsPersonData.IsPersonExists(PersonID);
        }

        public static bool IsPersonExists(string nationalNo)
        {
            return clsPersonData.IsPersonExists(nationalNo);
        }

        public static DataTable GetAllPeople()
        {
            return clsPersonData.GetAllPeople();
        }
    }
}
