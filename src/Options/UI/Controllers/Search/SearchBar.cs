using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VentLib.Options.Interfaces;
using VentLib.Utilities;

namespace VentLib.Options.UI.Controllers.Search;

public class SearchBar: ISearchBar
{
    private int charLimit;
    private bool greenOnHover;
    private string placeHolderText;
    private Vector2 iconLocPosition;
    private Func<Sprite> barSupplier;
    private Func<Sprite> iconSupplier;
    public SearchBar(Func<Sprite>? barSupplier = null, Func<Sprite>? iconSupplier = null, Vector2? iconLocPosition = null, int charLimit = 20, bool greenOnHover = true, string placeHolderText = "")
    {
        this.iconSupplier = iconSupplier ?? (() => AssetLoader.LoadSprite("assets.defaultsearchiconblack.png", 100, true));
        this.barSupplier = barSupplier ?? (() => AssetLoader.LoadSprite("assets.defaultsearchbar.png", 300, true));
        this.iconLocPosition = iconLocPosition ?? new Vector2(2.3f, 0);
        this.placeHolderText = placeHolderText;
        this.greenOnHover = greenOnHover;
        this.charLimit = charLimit;
    }

    public int CharLimit() => charLimit;

    public bool HoverColor() => greenOnHover;

    public string PlaceHolderText() => placeHolderText;

    public Sprite SearchBarSprite() => barSupplier();

    public Sprite SearchIconSprite() => iconSupplier();

    public Vector2 IconLocalPosition() => iconLocPosition;
}