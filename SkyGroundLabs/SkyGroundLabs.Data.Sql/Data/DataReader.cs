﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SkyGroundLabs.Data.Sql.Data
{
	public sealed class DataReader<T> : IEnumerable, IDisposable
    {
        #region Properties and Fields
        private readonly SqlDataReader _reader;

        public bool IsEOF {
            get { return !_reader.HasRows; }
        }
        #endregion

        #region Constructor
        public DataReader(SqlDataReader reader)
		{
			_reader = reader;
		}
        #endregion

        #region Methods
        public T Select()
	    {
	        _reader.Read();

            return _reader.ToObject<T>();
	    }

	    public List<T> All()
	    {
	        var result = new List<T>();

	        while (_reader.Read())
	        {
	            result.Add(_reader.ToObject<T>());
	        }

            Dispose();

	        return result;
	    } 

		// IEnumerable Member
		public IEnumerator<T> GetEnumerator()
		{
			while (_reader.Read())
			{
				yield return _reader.ToObject<T>();
			}

            // close when done enumerating
            Dispose();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			// Lets call the generic version here
			return GetEnumerator();
		}

	    public void Dispose()
	    {
	        _reader.Close();
            _reader.Dispose();
        }
        #endregion
    }
}
