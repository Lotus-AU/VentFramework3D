using VentLib.Options.Interfaces;

namespace VentLib.Options.Events;

public class TabChangeEvent : TabEvent
{
    public readonly IGameOptionTab? Original;
    
    public TabChangeEvent(IGameOptionTab? oldTab, IGameOptionTab? newTab) : base(newTab)
    {
        Original = oldTab;
    }
}