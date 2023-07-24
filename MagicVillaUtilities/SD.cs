using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVillaUtilities
{
	public static class SD
	{
		public enum ApiType
		{
			GET,
			POST, 
			PUT, 
			DELETE
		}

		public static string SessionToken = "JWTToken";
	}
}
