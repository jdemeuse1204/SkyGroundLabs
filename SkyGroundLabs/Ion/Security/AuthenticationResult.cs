using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ion.Security
{
	public enum AuthenticationResult
	{
		AttemptsExceeded,
		UsernameDoesntExist,
		PasswordIncorrect,
		Pass
	}
}
