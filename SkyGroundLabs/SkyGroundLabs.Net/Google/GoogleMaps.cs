using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SkyGroundLabs.Net.Google
{
	public static class GoogleMaps
	{
		public static double GetMileage(Address startAddress, Address destinationAddress)
		{
			string url = "http://maps.googleapis.com/maps/api/directions/xml?origin=" +
				startAddress.GetAddressString() +
				"&destination=" +
				destinationAddress.GetAddressString() +
				"&sensor=false";

			string directions = string.Empty;
			string distance = "0";

			try
			{
				WebClient wc = new WebClient();
				byte[] response = wc.DownloadData(url);
				directions = System.Text.Encoding.ASCII.GetString(response);

				if (!string.IsNullOrWhiteSpace(directions) && !directions.Contains("NOT_FOUND"))
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(directions);
					distance = doc.SelectSingleNode("DirectionsResponse/route/leg/distance/text").InnerText;
				}
			}
			catch (Exception)
			{
				distance = "0";
			}

			distance = distance.Replace(" ", "");
			distance = distance.Replace("mi", "");
			distance = distance.Replace("ft", "");

			return Convert.ToDouble(distance);
		}

		// Get Directions
	}
}
