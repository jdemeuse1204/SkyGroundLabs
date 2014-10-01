using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ion.Data.Tables;

namespace Ion.Data.Methods
{
	public class ContactMethods
	{
		#region Properties
		private DbContext _context { get; set; }
		#endregion

		#region Constructor
		public ContactMethods(DbContext context)
		{
			_context = context;
		}
		#endregion

		#region Methods
		public ContactArchive GetArchive(Contact contact)
		{
			return new ContactArchive()
			{
				City = contact.City,
				State = contact.State,
				ContactDateEntered = contact.DateEntered,
				ContactID = contact.ID,
				Street = contact.Street,
				Zip = contact.Zip
			};
		}

		public void UpdateAppointmentCount(long contactId)
		{
			var contact = _context.Contacts.Find(contactId);
			contact.AppointmentCount = _context.Appointments.Where(w => w.ContactID == contact.ID).Count();
			_context.SaveChanges(contact);
		}

		public IEnumerable<Contact> GetDuplicatesFromPhone(string phone)
		{
			return _context.Contacts.Where(w => (w.Phone1 == phone && (w.Phone1 != "" && w.Phone1 != null)) ||
				(w.Phone2 == phone && (w.Phone2 != "" && w.Phone2 != null)) ||
				(w.Phone3 == phone && (w.Phone3 != "" && w.Phone3 != null)));
		}

		/// <summary>
		/// Matches on Phone1, Phone2, Phone3 and FirstName1/LastName1/State
		/// </summary>
		/// <param name="contact">Contact</param>
		/// <returns>List of contacts</returns>
		public IEnumerable<Contact> GetDuplicates(Contact contact)
		{
			IEnumerable<Contact> match1 = new List<Contact>();
			IEnumerable<Contact> phone1 = new List<Contact>();
			IEnumerable<Contact> phone2 = new List<Contact>();
			IEnumerable<Contact> phone3 = new List<Contact>();
			var results = new List<Contact>();

			if (!string.IsNullOrWhiteSpace(contact.Phone1))
			{
				phone1 = GetDuplicatesFromPhone(contact.Phone1);
			}

			if (!string.IsNullOrWhiteSpace(contact.Phone2))
			{
				phone2 = GetDuplicatesFromPhone(contact.Phone2);
			}

			if (!string.IsNullOrWhiteSpace(contact.Phone3))
			{
				phone3 = GetDuplicatesFromPhone(contact.Phone3);
			}

			if (!string.IsNullOrWhiteSpace(contact.FirstName1) &&
				!string.IsNullOrWhiteSpace(contact.LastName1) &&
				!string.IsNullOrWhiteSpace(contact.State))
			{
				match1 = _context.Contacts.Where(
							w => w.FirstName1 == contact.FirstName1
							&& w.LastName1 == contact.LastName1
							&& w.State == contact.State).ToList();
			}

			results.AddRange(phone1);
			results.AddRange(phone2);
			results.AddRange(phone3);
			results.AddRange(match1);

			return results.Distinct<Contact>().Take(10);
		}

		#endregion
	}
}
