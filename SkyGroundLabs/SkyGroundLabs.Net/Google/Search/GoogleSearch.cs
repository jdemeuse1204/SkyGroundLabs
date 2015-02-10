using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace SkyGroundLabs.Net.Google.Search
{
	public static class GoogleSearch
	{
        public static GoogleSearchResponse FindPlace(string name, string city, string state)
		{
			var cityState = city + " " + state;
			cityState = cityState.Replace("'", "").Replace(" ", "+");
			name = name.Replace("'", "").Replace(" ", "+");
			var url = string.Format("https://maps.googleapis.com/maps/api/place/textsearch/json?query={0}+in+{1}&sensor=true&key={2}",
                name,
                cityState,
                GoogleAPISettings.Key);
			var wc = new WebClient();
			var response = wc.DownloadData(url);
			var data = Encoding.ASCII.GetString(response);

		    return JsonConvert.DeserializeObject<GoogleSearchResponse>(data);
		}
	}
}
