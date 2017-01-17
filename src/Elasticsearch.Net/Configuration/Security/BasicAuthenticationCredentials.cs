﻿using System;

namespace Elasticsearch252.Net
{
	public class BasicAuthenticationCredentials
	{
		public string Username { get; set; }
		[Obsolete("Removed in 5.0.0. Use Username instead, note the lowercase n")]
		[CLSCompliant(false)]
		public string UserName { get { return Username; } set { Username = value; } }
		public string Password { get; set; }

		public override string ToString() => $"{this.Username}:{this.Password}";
	}
}
