using VentLib.Options.Events;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Events;

public class OptionValueIncrementEvent : OptionValueEvent
{
    public OptionValueIncrementEvent(Option source, Optional<object> oldValue, object newValue) : base(source, oldValue, newValue)
    {
    }
}