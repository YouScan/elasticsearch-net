﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nest500
{
	public interface IRefreshResponse : IShardsOperationResponse { }

	public class RefreshResponse : ShardsOperationResponseBase, IRefreshResponse { }
}
