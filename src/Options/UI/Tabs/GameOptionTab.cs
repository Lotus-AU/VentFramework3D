// using System;
// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
// using UnityEngine;
// using VentLib.Logging;
// using VentLib.Options.Interfaces;
// using VentLib.Utilities.Collections;
// using VentLib.Utilities.Extensions;
// using VentLib.Utilities.Optionals;
// using Object = UnityEngine.Object;
//
// namespace VentLib.Options.UI.Tabs;
//
// public class GameOptionTab : IGameOptionTab
// {
//     private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(GameOptionTab));
//     private List<GameOption> Options { get; } = new();
//     public string name;
//     private Func<Sprite> spriteSupplier;
//     private Optional<Color[]> headerColors = new();
//
//     private UnityOptional<Sprite> sprite = UnityOptional<Sprite>.Null();
//     protected UnityOptional<RoleSettingsTabButton> TabButton = UnityOptional<RoleSettingsTabButton>.Null();
//
//     protected UnityOptional<GameObject> innerMenu = UnityOptional<GameObject>.Null();
//     protected UnityOptional<RolesSettingsMenu> RelatedMenu = UnityOptional<RolesSettingsMenu>.Null();
//
//     private OrderedSet<Action<IGameOptionTab>> callbacks = new();
//
//     public GameOptionTab(string name, Func<Sprite> spriteSupplier, Color[]? c = null)
//     {
//         this.name = name;
//         this.spriteSupplier = spriteSupplier;
//         if (c != null) headerColors = Optional<Color[]>.NonNull(c); 
//     }
//
//     public void AddEventListener(Action<IGameOptionTab> callback) => callbacks.Add(callback);
//
//     public virtual void AddOption(GameOption option)
//     {
//         if (Options.Contains(option)) return;
//         Options.Add(option);
//     }
//
//     public virtual void RemoveOption(GameOption option) => Options.Remove(option);
//
//     public void HandleClick()
//     {
//         RelatedMenu.IfPresent(menu => {
//             menu.RoleChancesSettings.SetActive(false);
//             menu.AllButton.SelectButton(false);
//         });
//         callbacks.ForEach(cb => cb(this));
//     }
//
//     public virtual OptionBehaviour InitializeOption(OptionBehaviour sourceBehavior)
//     {
//         if (!innerMenu.Exists()) throw new ArgumentException("Cannot Initialize Behaviour because menu does not exist");
//         return Object.Instantiate(sourceBehavior, Vector3.zero, Quaternion.identity, innerMenu.Get().transform);
//     }
//
//     public virtual Transform OptionParent() => innerMenu.Get().transform;
//
//     public virtual void Setup(RolesSettingsMenu menu)
//     {
//         TabButton.OrElseSet(() => Object.Instantiate<RoleSettingsTabButton>(menu.roleSettingsTabButtonOrigin, Vector3.zero, Quaternion.identity, menu.tabParent));
//         var roleMenu = RelatedMenu.OrElseSet(() => menu);
//
//         innerMenu.OrElseSet(() => {
//             GameObject copy = Object.Instantiate(menu.transform.Find("Scroller/SliderInner/AdvancedTab").gameObject, menu.transform.Find("Scroller/SliderInner"));
//             copy.name = name;
//             for (int i = 0; i < copy.transform.childCount; i++)
//             {
//                 Transform child = copy.transform.GetChild(i);
//                 if (child.name != "CategoryHeaderMasked") child.gameObject.Destroy();
//             }
//             Transform headerTextTransform = copy.transform.Find("CategoryHeaderMasked/HeaderText");
//             headerTextTransform.GetComponent<TextMeshPro>().enabled = true;
//             headerTextTransform.GetComponent<TextMeshPro>().text = name;
//             headerTextTransform.gameObject.SetActive(true);
//             return copy;
//         });
//         GetTabRenderer().IfPresent(render => render.sprite = sprite.OrElseSet(spriteSupplier));
//
//         GetPassiveButton().IfPresent(button => {
//             button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
//             button.OnClick.AddListener((Action)HandleClick);
//             roleMenu.roleTabs.Add(button);
//         });
//     }
//
//     public virtual bool Ignore() => false;
//
//     public virtual Optional<Color[]> HeaderColors() => headerColors;
//
//     public void SetPosition(Vector2 position)
//     {
//         TabButton.IfPresent(btn => btn.transform.localPosition = new Vector3(position.x, position.y, -2f));
//     }
//
//     public List<GameOption> PreRender(int? targetLevel = null) => targetLevel != null ? Options.SelectMany(opt => opt.GetDisplayedMembers()).Where(opt => opt.Level == targetLevel).ToList() : Options.SelectMany(opt => opt.GetDisplayedMembers()).ToList();
//
//     public Optional<Vector3> GetPosition() => TabButton.Map(btn => btn.transform.localPosition);
//     
//     public List<GameOption> GetOptions() => Options;
//
//     public void Activate()
//     {
//         log.Info($"Activated Tab \"{name}\"", "TabSwitch");
//         GetPassiveButton().IfPresent(button => button.SelectButton(true));
//         innerMenu.IfPresent(menu => menu.SetActive(true));
//     }
//
//     public void Deactivate()
//     {
//         log.Debug($"Deactivated Tab \"{name}\"", "TabSwitch");
//         GetPassiveButton().IfPresent(button => button.SelectButton(false));
//         innerMenu.IfPresent(menu => menu.SetActive(false));
//     }
//
//     public void Show()
//     {
//         TabButton.IfPresent(button => button.gameObject.SetActive(true));
//     }
//
//     public void Hide()
//     {
//         TabButton.IfPresent(button => button.gameObject.SetActive(false));
//     }
//
//
//     private UnityOptional<SpriteRenderer> GetTabRenderer() => TabButton.UnityMap(button => button.icon);
//     
//     private UnityOptional<PassiveButton> GetPassiveButton() => TabButton.UnityMap(button => button.Button);
// }