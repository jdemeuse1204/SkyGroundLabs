using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Linq.Mapping
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MaxLengthAttribute : Attribute
	{
		public MaxLengthAttribute() { }
		/// <summary>
		/// Max Length for property
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		/// If true length of property will be trimmed to match the max size
		/// </summary>
		public bool ShouldTruncate { get; set; }
	}
}
