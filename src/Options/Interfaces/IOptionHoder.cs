using VentLib.Options.UI;

namespace VentLib.Options.Interfaces;

public interface IOptionHoder
{
    void AddOption(GameOption option);

    void RemoveOption(GameOption option);
}