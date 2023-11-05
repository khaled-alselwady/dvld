using DVLD_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Business
{
    public class clsSettings
    {
        public static byte InternationalLicenseValidity()
        {
            return clsSettingsData.GetInternationalLicenseValidity();
        }
    }
}
