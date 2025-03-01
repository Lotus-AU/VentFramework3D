using SG.Airlock.Network;

namespace VentLib.Commands.Interfaces;

public interface ICommandReceiver
{
    void Receive(NetworkedLocomotionPlayer source, CommandContext context);
}