﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Data.Entity.Mapping;

namespace SkyGroundLabs.Data.Tables
{
	[Table("UserClockingSpecialCodes")]
	public class UserClockingSpecialCode : DbTableEquatable<IDbTableEquatable<long>>
	{
		public long ID { get; set; }
		[Column]
		[MaxLength(3)]
		public string Code { get; set; }
		[Column]
		public string Description { get; set; }
		[Column]
		public bool IsAutoGenerated { get; set; }
	}
}
