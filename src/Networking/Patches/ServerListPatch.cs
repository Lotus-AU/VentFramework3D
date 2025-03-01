// using System.Collections.Generic;
// using System.Linq;
// using SG.Airlock.Network;
// using UnityEngine;
// using VentLib.Utilities.Extensions;
// using VentLib.Utilities.Harmony.Attributes;
//
// namespace VentLib.Networking.Patches;
//
// public class ServerListPatch
// {
//     // """Modified""" from https://github.com/EnhancedNetwork/TownofHost-Enhanced/blob/f21362e2c59b8f2c73580a6dd4a8643decd0f5c5/Patches/RegionMenuPatch.cs
//     public const int MaxColumns = 4;
//
//     public const float ButtonSpacing = 0.6f;
//     public const float ButtonSpacingSide = 2.25f;
//
//     [QuickPostfix(typeof(RegionCollection), nameof(RegionCollection))]
//     public static void AdjustButtonPositions_Postfix(RegionCollection __instance)
//     {
//         List<UiElement> buttons = __instance.controllerSelectable.ToArray().ToList();
//
//         int buttonsPerColumn = 6;
//         int columnCount = (buttons.Count + buttonsPerColumn - 1) / buttonsPerColumn;
//
//         while (columnCount > MaxColumns)
//         {
//             buttonsPerColumn++;
//             columnCount = (buttons.Count + buttonsPerColumn - 1) / buttonsPerColumn;
//         }
//
//         float totalWidth = (columnCount - 1) * ButtonSpacingSide;
//         float totalHeight = (buttonsPerColumn - 1) * ButtonSpacing;
//
//         Vector3 startPosition = new Vector3(-totalWidth / 2, totalHeight / 2, 0f);
//         buttons.ForEach((b, i) => b.transform.localPosition = startPosition + new Vector3(i / buttonsPerColumn * ButtonSpacingSide, -(i % buttonsPerColumn) * ButtonSpacing, 0f));
//     }
// }