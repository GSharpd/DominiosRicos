using PaymentContext.Shared.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentContext.Shared.Handlers
{
    public interface IHandler<T> where T : ICommands
    {
        ICommandResult Handle(T command);
    }
}
