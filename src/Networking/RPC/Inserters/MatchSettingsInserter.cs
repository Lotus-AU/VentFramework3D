// using VentLib.Networking.RPC.Interfaces;
//
// namespace VentLib.Networking.RPC.Inserters;
//
// public class MatchSettingsInserter: IRpcInserter<MatchCustomizationSettings>
// {
//     public void Insert(MatchCustomizationSettings value, MessageWriter writer)
//     {
//         writer.WriteBytesAndSize(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(value, false));
//     }
// }