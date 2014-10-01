using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineIon.Business
{
	public class JsonAppointmentDisplay
	{
		private string _dateEdited;
		public string DateEdited 
		{
			get { return _dateEdited; }
			set { _dateEdited = (value == null ? "" : value); } 
		}

		private string _startAndEnd;
		public string StartAndEnd
		{
			get { return _startAndEnd; }
			set { _startAndEnd = (value == null ? "" : value); }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = (value == null ? "" : value); }
		}

		private string _moveDateTime;
		public string MoveDateTime
		{
			get { return _moveDateTime; }
			set { _moveDateTime = (value == null ? "" : value); }
		}

		private string _phone1;
		public string Phone1
		{
			get { return _phone1; }
			set { _phone1 = (value == null ? "" : value); }
		}

		private string _phone2;
		public string Phone2
		{
			get { return _phone2; }
			set { _phone2 = (value == null ? "" : value); }
		}

		private string _phone3;
		public string Phone3
		{
			get { return _phone3; }
			set { _phone3 = (value == null ? "" : value); }
		}

		private string _pickupAddress;
		public string PickupAddress
		{
			get { return _pickupAddress; }
			set { _pickupAddress = (value == null ? "" : value); }
		}

		private string _dropoffAddress;
		public string DropoffAddress
		{
			get { return _dropoffAddress; }
			set { _dropoffAddress = (value == null ? "" : value); }
		}

		private string _store;
		public string Store
		{
			get { return _store; }
			set { _store = (value == null ? "" : value); }
		}

		private string _location;
		public string Location
		{
			get { return _location; }
			set { _location = (value == null ? "" : value); }
		}

		private string _quote;
		public string Quote
		{
			get { return _quote; }
			set { _quote = (value == null ? "" : value); }
		}

		private string _weight;
		public string Weight
		{
			get { return _weight; }
			set { _weight = (value == null ? "" : value); }
		}

		private string _notes;
		public string Notes
		{
			get { return _notes; }
			set { _notes = (value == null ? "" : value); }
		}

		private string _crewName;
		public string CrewName
		{
			get { return _crewName; }
			set { _crewName = (value == null ? "" : value); }
		}

		private string _contactID;
		public string ContactID
		{
			get { return _contactID; }
			set { _contactID = (value == null ? "" : value); }
		}
	}
}