using VentLib.Options.Interfaces;

namespace VentLib.Options.Events;

public class TabEvent : ITabEvent
{
    private readonly IGameOptionTab? source;
    
    public TabEvent(IGameOptionTab? tab)
    {
        source = tab;
    }

    public IGameOptionTab? Source() => source;
}