///////////////////////////////////////////////////////////////////////////////
// CVOperationFailedException.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System;

namespace Possible.Vision.Managed.CoreVideo
{
	/// <summary>
	/// This exception is thrown if a native Core Video operation fails to deliver.
	/// </summary>
	public class CVOperationFailedException : Exception
	{
		/// <summary>
		/// Carries the result of the CV operation.
		/// </summary>
		public readonly CVReturn reason;

		public CVOperationFailedException()
		{
			reason = CVReturn.First;
		}

		public CVOperationFailedException(string message) : base(message)
		{
			reason = CVReturn.First;
		}

		public CVOperationFailedException(string message, Exception inner) : base(message, inner)
		{
			reason = CVReturn.First;
		}
		
		public CVOperationFailedException(CVReturn reason) : base("Core Video operation failed. (" + reason + ").")
		{
			this.reason = reason;
		}
	}
}
