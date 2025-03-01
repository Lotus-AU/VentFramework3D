// using System.Diagnostics.CodeAnalysis;
// using Fusion;
// using HarmonyLib;
// using Il2CppInterop.Runtime.InteropTypes.Arrays;
// using SG.Airlock.Network;
// using SG.Airlock.UI;
// using VentLib.Logging.Default;
// using VentLib.Utilities.Extensions;
// using VentLib.Utilities.Harmony.Attributes;
//
// namespace VentLib.Commands.Patches;
//
// [HarmonyPriority(Priority.First)]
// [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.SendChat))]
// internal static class AddChatPatch
// {
//     [SuppressMessage("ReSharper", "InconsistentNaming")]
//     internal static void HostCommandCheck(ChatManager __instance, Il2CppStructArray<int> chatIndexes)
//     {
//         if (!AirlockNetworkRunner.) return;
//         if (!XRRigExtensions.LocalPlayer().IsHost()) return;
//         CommandRunner.Instance.Execute(new CommandContext(sourcePlayer, chatText[1..]));
//     }
//     
//     [QuickPrefix(typeof(ChatManager), nameof(ChatManager.RPC_Chat), Priority.First)]
//     internal static void CommandCheck(ChatManager __instance, PlayerRef sender, Il2CppStructArray<int> chatIndexes)
//     {
//         if (!AmongUsClient.Instance.AmHost) return;
//         chatText = chatText.Trim(); // Trim spaces from chatText
//         if (!chatText.StartsWith("/")) return;
//         if (sender.IsHost()) return;
//         CommandRunner.Instance.Execute(new CommandContext(sender, chatText[1..]));
//     }
// }