using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyGroundLabs.Cryptography;
using SkyGroundLabs.Data;
using SkyGroundLabs.Data.Tables;

namespace SkyGroundLabs.Security
{
	public class Authentication
	{
		private DbContext _context { get; set; }

		public Authentication(DbContext context)
		{
			_context = context;
		}

		#region Methods
		public AuthenticationResult Validate(
			string username,
			string unEncryptedPassword,
			string passphraseKey,
			KeySize keySize = KeySize._256,
			int maxAttempts = 3)
		{
			User user = _getUser(username);

			// Make Sure User Exists
			if (user == null)
			{
				return AuthenticationResult.UsernameDoesntExist;
			}

			// Check Attemps
			if (user.LoginAttempts >= maxAttempts)
			{
				return AuthenticationResult.AttemptsExceeded;
			}

			// Check for password match
			if (unEncryptedPassword !=
				CryptographyServices.Decrypt(user.Password, passphraseKey, keySize))
			{
				user.LoginAttempts++;
				user.IsLocked = user.LoginAttempts >= maxAttempts ? true : false;
				_context.SaveChanges(user);
				return AuthenticationResult.PasswordIncorrect;
			}

			// need to reset the failed attempts
			user.LoginAttempts = 0;
			user.IsLocked = false;
			user.LastAuthenticationDate = DateTime.Now;
			_context.SaveChanges(user);

			return AuthenticationResult.Pass;
		}

		/// <summary>
		/// Creates a new user in the database and returns the new users ID
		/// </summary>
		/// <param name="context"></param>
		/// <param name="username"></param>
		/// <param name="unEncryptedUserPassword"></param>
		/// <param name="unEncryptedDatabasePassword"></param>
		/// <param name="passphraseKey"></param>
		/// <param name="authType"></param>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <param name="email"></param>
		/// <returns></returns>
		public AuthenticationStatus RegisterUser(
			User user,
			string username,
			string unEncryptedUserPassword,
			string unEncryptedDatabasePassword,
			string passphraseKey,
			string securityQuestion,
			string securityAnswer)
		{

			if (_context.Users.Select(w => w.Username.ToUpper()).Contains(username.ToUpper()))
			{
				return AuthenticationStatus.UsernameExists;
			}

			user.DatabasePassword = CryptographyServices.Encrypt("aiwa1122", passphraseKey);
			user.Password = CryptographyServices.Encrypt(unEncryptedUserPassword, passphraseKey);
			user.Username = username;
			user.SecurityAnswer = securityAnswer;
			user.SecurityQuestion = securityQuestion;
			user.UserRoleID = 1;
			user.ManagerUserID = 1;
			user.LastAuthenticationDate = DateTime.Now;

			return AuthenticationStatus.Success;
		}

		public bool IsUserInRole(User user, UserRoles role)
		{
			return user == null ? false : user.UserRoleID == role.ID;
		}

		public bool IsUserInRole(long userID, string roleName)
		{
			User user = _context.Users.Find(userID);
			UserRoles role = _context.UserRoles.Where(w => w.Role.ToUpper() == roleName.ToUpper()).FirstOrDefault();
			return IsUserInRole(user, role);
		}

		public bool IsUserInRole(long userID, long userRoleTypeID)
		{
			User user = _context.Users.Find(userID);
			UserRoles role = _context.UserRoles.Find(userRoleTypeID);
			return IsUserInRole(user, role);
		}

		public bool HasPageAccess(long userID, string pageName)
		{
			User user = _context.Users.Find(userID);
			UserRoles role = _context.UserRoles.Where(w => w.ID == user.UserRoleID).FirstOrDefault();
			UserRoleAccessPages page = _context.UserRoleAccessPages.Where(w => w.PageName.ToUpper() == pageName.ToUpper()).FirstOrDefault();

			if (role.Role.ToUpper() == "ADMIN")
			{
				return true;
			}

			if (page != null)
			{
				UserRoleAccess access = _context.UserRoleAccess.Where(w => w.UserRoleID == role.ID && w.UserRoleAccessPageID == page.ID).FirstOrDefault();
				return access != null;
			}
			return false;
		}

		/// <summary>
		/// Resets the users login attemps to zero and unlocks the account
		/// </summary>
		/// <param name="context"></param>
		/// <param name="username"></param>
		public void UnlockUser(string username)
		{
			UnlockUser(_getUser(username));
		}

		/// <summary>
		/// Resets the users login attemps to zero and unlocks the account
		/// </summary>
		/// <param name="context"></param>
		/// <param name="username"></param>
		public void UnlockUser(User user)
		{
			user.LoginAttempts = 0;
			user.IsLocked = false;
			_context.SaveChanges(user);
		}

		/// <summary>
		/// Resets the users password to 'password'
		/// </summary>
		/// <param name="context"></param>
		/// <param name="user"></param>
		/// <param name="passphraseKey"></param>
		public void ResetPassword(string username, string passphraseKey)
		{
			ResetPassword(_getUser(username), passphraseKey);
		}

		/// <summary>
		/// Resets the users password to 'password'
		/// </summary>
		/// <param name="context"></param>
		/// <param name="user"></param>
		/// <param name="passphraseKey"></param>
		public void ResetPassword(User user, string passphraseKey)
		{
			user.Password = CryptographyServices.Encrypt("password", passphraseKey);
			_context.SaveChanges(user);
		}

		private User _getUser(string username)
		{
			return _context.Users.Where(w => w.Username.ToUpper() == username.ToUpper()).FirstOrDefault();
		}
		#endregion
	}
}
