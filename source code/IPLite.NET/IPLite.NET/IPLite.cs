// http://iplite.codeplex.com
// Note:
// This class uses IP database from: https://lite.ip2location.com/


using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using System.Data;
using System.Net;

namespace System
{
    public class IPLite
    {
        public IpInfo REMOTE_ADDR = new IpInfo();
        public IpInfo HTTP_CLIENT_IP = new IpInfo();
        public IpInfo HTTP_X_FORWARDED_FOR = new IpInfo();
        public List<IpInfo> List_HTTP_X_FORWARDED_FOR = new List<IpInfo>();

        public static long ConvertIPAddressToNumber(string ip)
        {
            try
            {
                string[] ips = ip.Split('.');
                long w = long.Parse(ips[0]) * 16777216;
                long x = long.Parse(ips[1]) * 65536;
                long y = long.Parse(ips[2]) * 256;
                long z = long.Parse(ips[3]);

                long ipnumber = w + x + y + z;
                return ipnumber;
            }
            catch
            {
                return -1;
            }
        }

        public IPLite()
        { }

        public IPLite(MySqlCommand cmd, string ipAddress)
        {
            REMOTE_ADDR = new IpInfo(cmd, ipAddress.Trim());
        }

        public IPLite(MySqlCommand cmd)
        {
            GetData(cmd);
        }

        public void GetData(MySqlCommand cmd)
        {
            // REMOTE_ADDR
            string ipRemoteAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            REMOTE_ADDR = new IpInfo(cmd, ipRemoteAddr.Trim());

            // HTTP_X_FORWARDED_FOR
            string ipList = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (ipList != null)
            {
                string[] ipArray = ipList.Split(',');

                List_HTTP_X_FORWARDED_FOR = new List<IpInfo>();
                foreach (string ip in ipArray)
                {
                    List_HTTP_X_FORWARDED_FOR.Add(new IpInfo(cmd, ip.Trim()));
                }
            }

            foreach (IpInfo ipInfoXForwarded in List_HTTP_X_FORWARDED_FOR)
            {
                if (ipInfoXForwarded.IsValidPublicIP)
                {
                    HTTP_X_FORWARDED_FOR = ipInfoXForwarded;
                }
            }

            // HTTP_CLIENT_IP
            string ipClientIp = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            if (ipClientIp != null)
            {
                HTTP_CLIENT_IP = new IpInfo(cmd, ipClientIp.Trim());
            }
        }
    }

    public class IpInfo
    {
        private bool _isValidPublicIP = false;
        private string _ipAddress = "";
        private long _ipNumber = 0;
        private string _countryCode = "";
        private string _countryName = "";
        private string _regionName = "";
        private string _cityName = "";
        private decimal _latitude = 0;
        private decimal _longitude = 0;
        private string _zipCode = "";
        private string _timeZone = "";
        private int _timeZoneInt = 0;

        public bool IsValidPublicIP { get { return _isValidPublicIP; } }
        public string IpAddress { get { return _ipAddress; } }
        public long IpNumber { get { return _ipNumber; } }
        public string CountryCode { get { return _countryCode; } }
        public string CountryName { get { return _countryName; } }
        public string RegionName { get { return _regionName; } }
        public string CityName { get { return _cityName; } }
        public decimal Latitude { get { return _latitude; } }
        public decimal Longitude { get { return _longitude; } }
        public string ZipCode { get { return _zipCode; } }
        public string TimeZone { get { return _timeZone; } }
        public int TimeZoneInt { get { return _timeZoneInt; } }

        public IpInfo()
        { }

        public IpInfo(MySqlCommand cmd, string ipAddress)
        {
            _ipAddress = ipAddress;

            _ipNumber = IPLite.ConvertIPAddressToNumber(ipAddress);

            if (IpNumber == -1)
            {
                _isValidPublicIP = false;
                return;
            }

            if (!IsPrivateNetwork())
                GetData(cmd);
        }

        bool IsPrivateNetwork()
        {
            bool isPN = false;

            string[] ips = IpAddress.Split('.');
            int w = int.Parse(ips[0]);
            int x = int.Parse(ips[1]);
            int y = int.Parse(ips[2]);
            int z = int.Parse(ips[3]);

            // Private Network
            // http://en.wikipedia.org/wiki/Private_network

            if (w == 127 && x == 0 && y == 0 && z == 1) // 127.0.0.1
            {
                isPN = true;

                _countryName = "Localhost";
                _regionName = "Localhost";
                _cityName = "Localhost";
            }
            else if (w == 10) // 10.0.0.0 - 10.255.255.255
            {
                isPN = true;

                _countryName = "Private Network";
                _regionName = "Private Network";
                _cityName = "Private Network";
            }
            else if (w == 172 && (x >= 16 || x <= 31)) // 172.16.0.0 - 172.31.255.255
            {
                isPN = true;

                _countryName = "Private Network";
                _regionName = "Private Network";
                _cityName = "Private Network";
            }
            else if (w == 192 && x == 168) // 192.168.0.0 - 192.168.255.255
            {
                isPN = true;

                _countryName = "Private Network";
                _regionName = "Private Network";
                _cityName = "Private Network";
            }

            return isPN;
        }



        private void GetData(MySqlCommand cmd)
        {
            cmd.CommandText = string.Format("select * from ip2location_db11 where ip_from <= {0} and ip_to >= {0} limit 0,1;", IpNumber);
            DataTable dt = new DataTable();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                _countryName = "Record not found";
                _regionName = "Record not found";
                _cityName = "Record not found";
                return;
            }

            DataRow dr = dt.Rows[0];

            if (dt.Columns.Contains("country_code"))
                _countryCode = dr["country_code"] + "";
            else
                _countryCode = "<not included in database>";

            if (dt.Columns.Contains("country_name"))
                _countryName = dr["country_name"] + "";
            else
                _countryName = "<not included in database>";

            if (dt.Columns.Contains("region_name"))
                _regionName = dr["region_name"] + "";
            else
                _regionName = "<not included in database>";

            if (dt.Columns.Contains("city_name"))
                _cityName = dr["city_name"] + "";
            else
                _cityName = "<not included in database>";

            if (dt.Columns.Contains("zip_code"))
                _zipCode = dr["zip_code"] + "";
            else
                _zipCode = "<not included in database>";

            if (dt.Columns.Contains("time_zone"))
            {
                _timeZone = dr["time_zone"] + "";
                string[] ta = _timeZone.Split(':');
                int.TryParse(ta[0], out _timeZoneInt);
            }
            else
            {
                _timeZone = "<not included in database>";
            }

            if (dt.Columns.Contains("latitude"))
                decimal.TryParse(dr["latitude"] + "", out _latitude);

            if (dt.Columns.Contains("longitude"))
                decimal.TryParse(dr["longitude"] + "", out _longitude);

            _isValidPublicIP = true;
        }
    }
}