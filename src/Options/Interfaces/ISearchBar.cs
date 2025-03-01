using UnityEngine;

namespace VentLib.Options.Interfaces;

public interface ISearchBar
{
    int CharLimit();

    bool HoverColor();

    string PlaceHolderText();

    Sprite SearchBarSprite();

    Sprite SearchIconSprite();

    Vector2 IconLocalPosition();
}