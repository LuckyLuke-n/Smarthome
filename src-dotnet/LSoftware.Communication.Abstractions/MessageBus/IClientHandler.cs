using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSoftware.Communication.Abstractions.MessageBus
{
	public interface IClientHandler
	{
		Task ConnectAsync( CancellationToken cancellationToken = default );
	}
}
