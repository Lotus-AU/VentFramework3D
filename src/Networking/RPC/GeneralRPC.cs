using Hazel;

namespace VentLib.Networking.RPC;

public static class GeneralRPC
{
    public static void SendGameData(int clientId = -1)
    {
        // MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
        // writer.StartMessage((byte)(clientId == -1 ? 5 : 6)); //0x05 GameData
        // {
        //     writer.Write(AmongUsClient.Instance.GameId);
        //     if (clientId != -1)
        //         writer.WritePacked(clientId);
        //     writer.StartMessage(1); //0x01 Data
        //     {
        //         writer.WritePacked(GameManager.Instance.NetId);
        //         GameManager.Instance.Serialize(writer, true);
        //     }
        //     writer.EndMessage();
        // }
        // writer.EndMessage();

        // AmongUsClient.Instance.SendOrDisconnect(writer);
        // writer.Recycle();
        MessageWriter writer = MessageWriter.Get(SendOption.None);
        writer.StartMessage((byte)(clientId == -1 ? 5 : 6));
        writer.Write(AmongUsClient.Instance.GameId);

        if (clientId != -1) writer.WritePacked(clientId);
        
        foreach (NetworkedPlayerInfo playerInfo in GameData.Instance.AllPlayers)
        {
            writer.StartMessage(1);
            writer.WritePacked(playerInfo.NetId);
            playerInfo.Serialize(writer, false);
            writer.EndMessage();
        }
        writer.EndMessage();
        AmongUsClient.Instance.SendOrDisconnect(writer);
        writer.Recycle();
    }

    public static void SendMeetingHud(int clientId = -1, bool initialState = true)
    {
        MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
        writer.StartMessage((byte)(clientId == -1 ? 5 : 6)); //0x05 GameData
        {
            writer.Write(AmongUsClient.Instance.GameId);
            if (clientId != -1)
                writer.WritePacked(clientId);
            writer.StartMessage(1); //0x01 Data
            {
                writer.WritePacked(MeetingHud.Instance.NetId);
                MeetingHud.Instance.Serialize(writer, initialState);
            }
            writer.EndMessage();
        }
        writer.EndMessage();

        AmongUsClient.Instance.SendOrDisconnect(writer);
        writer.Recycle();
    }
}