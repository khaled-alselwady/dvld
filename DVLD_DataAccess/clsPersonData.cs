using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsPersonData
    {
        public static bool GetPersonInfoByID(int PersonID, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref string NationalNo, ref DateTime DateOfBirth,
            ref short Gender, ref string Address, ref string Phone, ref string Email,
            ref int NationalityCountryID, ref string ImagePath)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select * from People where PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    IsFound = true;

                    FirstName = (string)reader["FirstName"];
                    SecondName = (string)reader["SecondName"];
                    LastName = (string)reader["LastName"];
                    NationalNo = (string)reader["NationalNo"];
                    DateOfBirth = (DateTime)reader["DateOfBirth"];
                    Gender = (byte)(reader["Gender"]);
                    Address = (string)reader["Address"];
                    Phone = (string)reader["Phone"];
                    NationalityCountryID = (int)reader["NationalityCountryID"];

                    // these columns allow null in database so we should handle null
                    ThirdName = (reader["ThirdName"] != DBNull.Value) ? (string)reader["ThirdName"] : "";
                    Email = (reader["Email"] != DBNull.Value) ? (string)reader["Email"] : "";
                    ImagePath = (reader["ImagePath"] != DBNull.Value) ? (string)reader["ImagePath"] : "";
                }
                else
                {
                    // The record was not found
                    IsFound = false;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                connection.Close();
            }

            return IsFound;
        }


        public static bool GetPersonInfoByNationalNo(string NationalNo, ref int PersonID, ref string FirstName,
            ref string SecondName, ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
            ref short Gender, ref string Address, ref string Phone, ref string Email,
            ref int NationalityCountryID, ref string ImagePath)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select * from People where NationalNo = @NationalNo";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    IsFound = true;

                    PersonID = (int)reader["PersonID"];
                    FirstName = (string)reader["FirstName"];
                    SecondName = (string)reader["SecondName"];
                    LastName = (string)reader["LastName"];
                    DateOfBirth = (DateTime)reader["DateOfBirth"];
                    Gender = (byte)(reader["Gender"]);
                    Address = (string)reader["Address"];
                    Phone = (string)reader["Phone"];
                    NationalityCountryID = (int)reader["NationalityCountryID"];

                    // these columns allow null in database so we should handle null
                    ThirdName = (reader["ThirdName"] != DBNull.Value) ? (string)reader["ThirdName"] : "";
                    Email = (reader["Email"] != DBNull.Value) ? (string)reader["Email"] : "";
                    ImagePath = (reader["ImagePath"] != DBNull.Value) ? (string)reader["ImagePath"] : "";
                }
                else
                {
                    // The record was not found
                    IsFound = false;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                connection.Close();
            }

            return IsFound;
        }


        public static int AddNewPerson(string FirstName, string SecondName,
             string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
             short Gender, string Address, string Phone, string Email,
             int NationalityCountryID, string ImagePath)
        {
            // This function will return the new person id if succeeded and -1 if not
            int PersonID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"insert into People (FirstName, SecondName, ThirdName, LastName, NationalNo, DateOfBirth,
                                 Gender, Address, Phone, Email, NationalityCountryID, ImagePath)
             values (@FirstName, @SecondName, @ThirdName, @LastName, @NationalNo, @DateOfBirth,
                     @Gender, @Address, @Phone, @Email, @NationalityCountryID, @ImagePath)
             select scope_identity()";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gender);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);

            // these columns allow null in database so we should handle null
            if (string.IsNullOrWhiteSpace(ThirdName))
            {
                command.Parameters.AddWithValue("@ThirdName", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@ThirdName", ThirdName);
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                command.Parameters.AddWithValue("@Email", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@Email", Email);
            }

            if (string.IsNullOrWhiteSpace(ImagePath))
            {
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@ImagePath", ImagePath);
            }

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int InsertID))
                {
                    PersonID = InsertID;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return PersonID;
        }


        public static bool UpdatePerson(int PersonID, string FirstName, string SecondName,
             string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
             short Gender, string Address, string Phone, string Email,
             int NationalityCountryID, string ImagePath)
        {
            int RowAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"UPDATE People
             SET    NationalNo = @NationalNo,
                    FirstName = @FirstName,
                    SecondName = @SecondName,
                    ThirdName = @ThirdName,
                    LastName = @LastName,
                    DateOfBirth = @DateOfBirth,
                    Gender = @Gender,
                    Address = @Address,
                    Phone = @Phone,
                    Email = @Email,
                    NationalityCountryID = @NationalityCountryID,
                    ImagePath = @ImagePath
             WHERE  PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gender);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);

            // these columns allow null in database so we should handle null
            if (string.IsNullOrWhiteSpace(ThirdName))
            {
                command.Parameters.AddWithValue("@ThirdName", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@ThirdName", ThirdName);
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                command.Parameters.AddWithValue("@Email", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@Email", Email);
            }

            if (string.IsNullOrWhiteSpace(ImagePath))
            {
                command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@ImagePath", ImagePath);
            }

            try
            {
                connection.Open();

                RowAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return (RowAffected > 0);
        }


        public static bool DeletePerson(int PersonID)
        {
            int RowAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"delete People where PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();

                RowAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return (RowAffected > 0);
        }

        public static bool IsPersonExists(int PersonID)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select found = 1 from People where PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                IsFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                connection.Close();
            }

            return IsFound;
        }

        public static bool IsPersonExists(string NationalNo)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select found = 1 from People where NationalNo = @NationalNo";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                IsFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                connection.Close();
            }

            return IsFound;
        }

        public static DataTable GetAllPeople()
        {
            DataTable dtPeople = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT People.PersonID, People.NationalNo,
              People.FirstName, People.SecondName, People.ThirdName, People.LastName,
			  People.DateOfBirth,  
				  CASE
                  WHEN People.Gender = 0 THEN 'Male'

                  ELSE 'Female'

                  END as GenderCaption ,
			  People.Address, People.Phone, People.Email, 
              People.NationalityCountryID, Countries.CountryName, People.ImagePath
              FROM            People INNER JOIN
                         Countries ON People.NationalityCountryID = Countries.CountryID
                ORDER BY People.FirstName";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dtPeople.Load(reader);
                }

                reader.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return dtPeople;
        }

    }
}
