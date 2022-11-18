using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySqlConnector;

namespace WebApplication1
{
    public partial class Default : System.Web.UI.Page
    {
        string connectionString = "server=localhost;user=root;pwd=1234;database=ip2location;";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btCheckCurrent_Click(object sender, EventArgs e)
        {
            CheckCurrentVisitorIP();
        }

        void CheckCurrentVisitorIP()
        {
            IPLite ipLite = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    ipLite = new IPLite(cmd);

                    conn.Close();
                }
            }

            Response.Write("<b>REMOTE_ADDR</b><br />");
            WriteInfo(ipLite.REMOTE_ADDR);

            Response.Write("<b>HTTP_CLIENT_IP</b><br />");
            if (ipLite.HTTP_CLIENT_IP.IpAddress == string.Empty)
            {
                Response.Write("contains no IP.<br /><hr />");
            }
            else
            {
                WriteInfo(ipLite.HTTP_CLIENT_IP);
            }

            if (ipLite.List_HTTP_X_FORWARDED_FOR.Count == 0)
            {
                Response.Write("<b>HTTP_X_FORWARDED_FOR</b><br /><em>- IP chain behind proxy server or load balancing.</em><br />contains no IP.<br /><hr />");
            }
            else
            {
                foreach (IpInfo ipInfo in ipLite.List_HTTP_X_FORWARDED_FOR)
                {
                    Response.Write("<b>HTTP_X_FORWARDED_FOR</b><br /><em>- IP chain behind proxy server or load balancing.</em><br />");
                    WriteInfo(ipInfo);
                }
            }
        }

        protected void btManualCheck_Click(object sender, EventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    con.Open();

                    IPLite ip = new IPLite(cmd, TextBox1.Text.Trim());
                    WriteInfo(ip.REMOTE_ADDR);

                    con.Close();
                }
            }
        }

        void WriteInfo(IpInfo ip)
        {
            Response.Write("IP Address: " + ip.IpAddress + "<br />");
            Response.Write("IP Number: " + ip.IpNumber + "<br />");
            Response.Write("Country Code: " + ip.CountryCode + "<br />");
            Response.Write("Country: " + ip.CountryName + "<br />");
            Response.Write("Region: " + ip.RegionName + "<br />");
            Response.Write("City: " + ip.CityName + "<br />");
            Response.Write("Latitue: " + ip.Latitude + "<br />");
            Response.Write("Longitue: " + ip.Longitude + "<br />");
            Response.Write("Zip Code: " + ip.ZipCode + "<br />");
            Response.Write("Time Zone: " + ip.TimeZone + "<br />");
            Response.Write("<hr />");
        }
    }
}