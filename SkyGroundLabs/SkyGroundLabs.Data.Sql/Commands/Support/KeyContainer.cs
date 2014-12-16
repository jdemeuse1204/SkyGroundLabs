using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyGroundLabs.Data.Sql.Commands.Support
{
	public class KeyContainer
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
