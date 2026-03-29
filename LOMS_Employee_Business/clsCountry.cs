using LOMS_Employee_DataAccess;
using System;
using System.Data;

namespace LOMS_Employee_Business
{
    public class clsCountry
    {
        public int ID { get; set; }
        public string CountryName { get; set; }

        // Constructeur par défaut
        public clsCountry()
        {
            this.ID = -1;
            this.CountryName = "";
        }

        // Constructeur privé pour l'encapsulation
        private clsCountry(int id, string countryName)
        {
            this.ID = id;
            this.CountryName = countryName;
        }

        #region Méthodes Statiques

        public static clsCountry Find(int id)
        {
            string countryName = "";

            if (clsCountryData.GetCountryInfoByID(id, ref countryName))
                return new clsCountry(id, countryName);
            else
                return null;
        }

        public static clsCountry Find(string countryName)
        {
            int id = -1;

            if (clsCountryData.GetCountryInfoByName(countryName, ref id))
                return new clsCountry(id, countryName);
            else
                return null;
        }

        public static DataTable GetAllCountries()
        {
            return clsCountryData.GetAllCountries();
        }

        #endregion
    }
}