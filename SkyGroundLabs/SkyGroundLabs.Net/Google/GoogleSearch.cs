using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SkyGroundLabs.Net.Google
{
	public static class GoogleSearch
	{
		public static List<Place> FindPlace(string name, string city, string state)
		{
			var cityState = city + " " + state;
			cityState = cityState.Replace("'", "").Replace(" ", "+").Replace(",", "");
			name = name.Replace("'", "").Replace(" ", "+").Replace(",", "");
			var url = "https://maps.googleapis.com/maps/api/place/textsearch/xml?query=" + name + "+in+" + cityState + "&sensor=true&key=AIzaSyDDzJS7KiqM2Ec9n4ayx9mlU-sH_kKLZ2w";
			var data = string.Empty;
			var wc = new WebClient();
			var response = wc.DownloadData(url);
			data = System.Text.Encoding.ASCII.GetString(response);
			var doc = new XmlDocument();
			doc.LoadXml(data);
			var nodes = doc.GetElementsByTagName("result");
			var places = new List<Place>();

			for (int i = 0; i < nodes.Count; i++)
			{
				var place = new Place();
				var node = nodes[i];

				foreach (XmlNode child in node.ChildNodes)
				{
					if (child.Name == "name")
					{
						if (child.InnerText.Contains("-"))
						{
							place.Name = child.InnerText.Split('-')[0].Trim(' ');
						}
						else
						{
							place.Name = child.InnerText;
						}

					}
					else if (child.Name == "formatted_address")
					{
						var items = child.InnerText.Split(',');

						if (items.Count() == 3)
						{
							place.City = items[0].Trim(' ');
							place.State = items[1].Trim(' ');
						}
						else if (items.Count() == 4)
						{
							if (items[2].Trim(' ').Contains(' '))
							{
								place.City = items[1].Trim(' ');
								place.State = items[2].Trim(' ').Split(' ')[0].Trim(' ');
								place.ZipCode = items[2].Trim(' ').Split(' ')[1].Trim(' ');
							}
							else
							{
								place.City = items[1].Trim(' ');
								place.State = items[2].Trim(' ');
								place.Street = items[0].Trim(' ');
							}
						}
						else if (items.Count() == 5)
						{
							place.City = items[2].Trim(' ');
							place.Street = items[1].Trim(' ');

							if (items[3].Trim(' ').Contains(' '))
							{
								place.State = items[3].Trim(' ').Split(' ')[0].Trim(' ');
								place.ZipCode = items[3].Trim(' ').Split(' ')[1].Trim(' ');
							}
						}
						break;
					}
				}

				places.Add(place);
			}

			return places;
		}
	}
}
