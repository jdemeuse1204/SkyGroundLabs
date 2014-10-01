using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ion.Business.Services
{
	public static class DisplayServices
	{
		public static string BuildDisplayTime(string startTime, string endTime)
		{
			string startAMPM = (startTime.Contains("AM") ? "a" : "p");
			string endAMPM = (endTime.Contains("AM") ? "a" : "p");
			string finalTime = string.Empty;

			startTime = startTime.Replace(" AM", "").Replace(" PM", "");
			endTime = endTime.Replace(" AM", "").Replace(" PM", "");

			if (startTime.Contains(':'))
				startTime = startTime.Split(':')[1] == "00" ? startTime.Replace(":00", "") : startTime.Replace(":", "");

			if (endTime.Contains(':'))
				endTime = endTime.Split(':')[1] == "00" ? endTime.Replace(":00", "") : endTime.Replace(":", "");

			if (startAMPM != endAMPM)
			{
				if (!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime))
					finalTime = startTime + startAMPM + " - " + endTime + endAMPM;
				else if (!string.IsNullOrWhiteSpace(startTime))
					finalTime = startTime + startAMPM;
				else if (!string.IsNullOrWhiteSpace(endTime))
					finalTime = endTime + endAMPM;
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(startTime) && !string.IsNullOrWhiteSpace(endTime))
					finalTime = startTime + " - " + endTime + endAMPM;
				else if (!string.IsNullOrWhiteSpace(startTime))
					finalTime = startTime + endAMPM;
				else if (!string.IsNullOrWhiteSpace(endTime))
					finalTime = endTime + endAMPM;
			}

			return finalTime;
		}

		public static string GetDisplayDateString(DateTime start, DateTime end)
		{
			string header = string.Empty;

			string sundayMonth = start.ToString("MMMM");
			string sundayDay = start.Day.ToString();
			string sundayYear = start.Year.ToString();

			string saturdayMonth = end.ToString("MMMM");
			string saturdayDay = end.Day.ToString();
			string saturdayYear = end.Year.ToString();

			header = sundayMonth + " " + sundayDay + " - " + saturdayDay + ", " + sundayYear;

			if (start.Month != end.Month)
			{
				if (start.Year != end.Year)
				{
					header = sundayMonth + " " +
						sundayDay + " " +
						sundayYear + " - " +
						saturdayMonth + " " +
						saturdayDay + " " +
						sundayYear;
				}
				else
				{
					header = sundayMonth + " " +
						sundayDay + " - " +
						saturdayMonth + " " +
						saturdayDay + ", " +
						sundayYear;
				}
			}

			return header;
		}
	}
}
