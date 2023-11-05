using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DVLD_DataAccess.clsCountryData;
using System.Net;
using System.Security.Policy;

namespace DVLD_DataAccess
{
    public class clsDetainedLicenseData
    {
        public static bool GetDetainedLicenseInfoByDetainID(int DetainID, ref int LicenseID,
            ref DateTime DetainDate, ref decimal FineFees, ref int CreatedByUserID,
            ref bool IsReleased, ref DateTime ReleaseDate, ref int ReleasedByUserID,
            ref int ReleaseApplicationID)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select * from DetainedLicenses where DetainID = @DetainID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DetainID", DetainID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    IsFound = true;

                    LicenseID = (int)reader["LicenseID"];
                    DetainDate = (DateTime)reader["DetainDate"];
                    FineFees = (decimal)reader["FineFees"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                    IsReleased = (bool)reader["IsReleased"];
                    ReleaseDate = (reader["ReleaseDate"] != DBNull.Value) ? (DateTime)reader["ReleaseDate"] : DateTime.MaxValue;
                    ReleasedByUserID = (reader["ReleasedByUserID"] != DBNull.Value) ? (int)reader["ReleasedByUserID"] : -1;
                    ReleaseApplicationID = (reader["ReleaseApplicationID"] != DBNull.Value) ? (int)reader["ReleaseApplicationID"] : -1;
                }
                else
                {
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

        public static bool GetDetainedLicenseInfoByLicenseID(ref int DetainID, int LicenseID,
           ref DateTime DetainDate, ref decimal FineFees, ref int CreatedByUserID,
           ref bool IsReleased, ref DateTime ReleaseDate, ref int ReleasedByUserID,
           ref int ReleaseApplicationID)
        {
            bool IsFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select top 1  * from DetainedLicenses where LicenseID = @LicenseID and IsReleased = 0 order by DetainID DESC";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    IsFound = true;

                    DetainID = (int)reader["DetainID"];
                    DetainDate = (DateTime)reader["DetainDate"];
                    FineFees = (decimal)reader["FineFees"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                    IsReleased = (bool)reader["IsReleased"];
                    ReleaseDate = (reader["ReleaseDate"] != DBNull.Value) ? (DateTime)reader["ReleaseDate"] : DateTime.MaxValue;
                    ReleasedByUserID = (reader["ReleasedByUserID"] != DBNull.Value) ? (int)reader["ReleasedByUserID"] : -1;
                    ReleaseApplicationID = (reader["ReleaseApplicationID"] != DBNull.Value) ? (int)reader["ReleaseApplicationID"] : -1;
                }
                else
                {
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

        public static int AddNewDetainedLicense(int LicenseID, DateTime DetainDate,
                 decimal FineFees, int CreatedByUserID)
        {
            int DetainID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"INSERT INTO dbo.DetainedLicenses
                               (LicenseID,
                               DetainDate,
                               FineFees,
                               CreatedByUserID,
                               IsReleased
                               )
                            VALUES
                               (@LicenseID,
                               @DetainDate, 
                               @FineFees, 
                               @CreatedByUserID,
                               0
                             );
                            
                            SELECT SCOPE_IDENTITY()";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            command.Parameters.AddWithValue("@DetainDate", DetainDate);
            command.Parameters.AddWithValue("@FineFees", FineFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int InsertID))
                {
                    DetainID = InsertID;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return DetainID;
        }

        public static bool UpdateDetainedLicense(int DetainID, int LicenseID, DateTime DetainDate,
                 decimal FineFees, int CreatedByUserID)
        {
            int AffectedRows = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"UPDATE dbo.DetainedLicenses
                              SET LicenseID = @LicenseID, 
                              DetainDate = @DetainDate, 
                              FineFees = @FineFees,
                              CreatedByUserID = @CreatedByUserID
                              WHERE DetainID=@DetainID;";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DetainedLicenseID", DetainID);
            command.Parameters.AddWithValue("@LicenseID", LicenseID);
            command.Parameters.AddWithValue("@DetainDate", DetainDate);
            command.Parameters.AddWithValue("@FineFees", FineFees);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();

                AffectedRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return (AffectedRows > 0);
        }

        public static bool ReleaseDetainedLicense(int DetainID, int ReleasedByUserID,
            int ReleaseApplicationID)
        {
            int AffectedRows = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"UPDATE dbo.DetainedLicenses
                              SET IsReleased = 1, 
                              ReleaseDate = @ReleaseDate, 
                              ReleasedByUserID = @ReleasedByUserID,
                              ReleaseApplicationID = @ReleaseApplicationID   
                              WHERE DetainID=@DetainID;";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ReleaseDate", DateTime.Now);
            command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
            command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
            command.Parameters.AddWithValue("@DetainID", DetainID);

            try
            {
                connection.Open();

                AffectedRows = command.ExecuteNonQuery();
            }
            catch (Exception ex) { }

            finally { connection.Close(); }

            return (AffectedRows > 0);
        }

        public static DataTable GetAllDetainedLicenses()
        {
            DataTable dtDetainedLicenses = new DataTable();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select * from DetainedLicenses_View order by DetainID desc";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dtDetainedLicenses.Load(reader);
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

            return dtDetainedLicenses;
        }

        public static bool IsLicenseDetained(int LicenseID)
        {
            bool IsDetained = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select IsDetained = 1 from DetainedLicenses where LicenseID = @LicenseID and IsReleased = 0";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                IsDetained = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                IsDetained = false;
            }
            finally
            {
                connection.Close();
            }

            return IsDetained;
        }
    }
}
