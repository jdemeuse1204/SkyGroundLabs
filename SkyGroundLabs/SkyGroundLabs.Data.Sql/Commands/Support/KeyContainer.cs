﻿using System.Collections.Generic;

namespace SkyGroundLabs.Data.Sql.Commands.Support
{
	public sealed class KeyContainer
	{
		public KeyContainer()
		{
			_container = new Dictionary<string, object>();
		}

		private Dictionary<string, object> _container { get; set; }

		public void Add(string columnName, object value)
		{
			_container.Add(columnName, value);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _container.GetEnumerator();
		}
	}
}
