using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Business.Extension
{
	public static class Extensions
	{
		#region Methods
		public static class EnumUtil
		{
			public static IEnumerable<T> GetValues<T>()
			{
				return Enum.GetValues(typeof(T)).Cast<T>();
			}
		}

		public static string ToDateTimeString(this DateTime date)
		{
			return date.Date.ToShortDateString() + " " + date.ToShortTimeString();
		}

		public static bool IsBetween(this DateTime date, DateTime date1, DateTime date2)
		{
			return date.Date >= date1.Date && date.Date <= date2;
		}

		public static string Clean(this string s)
		{
			return s.Replace(" ", "").Replace("'", "");
		}

		public static bool IsCurrentValueNullOrEmpty(this object o)
		{
			if (o == null || o.ToString() == "")
				return true;
			return false;
		}

		public static bool IsCurrentValueNullOrEmptyOrZero(this object o)
		{
			if (o == null || o.ToString() == "" || o.ToString() == "0")
				return true;
			return false;
		}

		public static bool StringEquals(this string s, string compare)
		{
			s = (s == null ? s = "" : s);
			return s.Replace(" ", "").ToUpper() == compare.Replace(" ", "").ToUpper();
		}

		public static Int64 ToInt64(this object o)
		{
			try
			{
				return Convert.ToInt64(o);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public static int ToBit(this bool o)
		{
			return (o ? 1 : 0);
		}

		public static Int32 ToInt32(this object o)
		{
			try
			{
				return Convert.ToInt32(o);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public static Int16 ToInt16(this object o)
		{
			try
			{
				return Convert.ToInt16(o);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public static double ToDouble(this object o)
		{
			try
			{
				return Convert.ToDouble(o);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public static float ToFloat(this object o)
		{
			try
			{
				return Convert.ToSingle(o);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public static bool? ToBoolean(this object o)
		{
			try
			{
				return Convert.ToInt16(o) == 1;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static decimal ToDecimal(this object o)
		{
			try
			{
				return Convert.ToDecimal(o);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public static DateTime ToDateTime(this object o)
		{
			try
			{
				return Convert.ToDateTime(o);
			}
			catch (Exception)
			{
				return DateTime.MinValue;
			}
		}

		public static string ToStringOvr(this object o)
		{
			if (o == null)
				return string.Empty;
			return Convert.ToString(o);
		}
		#endregion
	}
}
