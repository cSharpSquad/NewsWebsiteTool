using System;
namespace Exceptions
{
	public class ResourceNotFoundException : Exception
	{
		public ResourceNotFoundException(string message) : base(message)
		{
		}
		// You can add additional constructors or properties as needed    }
	}
}