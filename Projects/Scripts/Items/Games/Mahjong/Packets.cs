using System.IO;
using Server.Buffers;
using Server.Network;

namespace Server.Engines.Mahjong
{
  public static class MahjongPackets
  {
    public static void SendMahjongJoinGame(NetState ns, MahjongGame game)
    {
      if (ns == null)
        return;

      SpanWriter writer = new SpanWriter(stackalloc byte[9]);
      writer.Write((byte)0xDA); // Packet ID
      writer.Write((ushort)0x09); // Dynamic Length

      writer.Write(game.Serial);
      writer.Write((short)0x19);

      ns.Send(writer.Span);
    }

    public static void SendMahjongPlayersInfo(NetState ns, MahjongGame game, Mobile to)
    {
      if (ns == null)
        return;

      MahjongPlayers players = game.Players;
      SpanWriter writer = new SpanWriter(stackalloc byte[11 + 45 * players.Seats]);
      writer.Write((byte)0xDA); // Packet ID
      writer.Position += 2; // Dynamic Length

      writer.Write(game.Serial);
      writer.Write((short)0x02);
      writer.Write((short)players.Seats);

      short n = 0;
      for (int i = 0; i < players.Seats; i++)
      {
        Mobile mobile = players.GetPlayer(i);

        if (mobile != null)
        {
          writer.Write(mobile.Serial);
          writer.Write(players.DealerPosition == i ? (byte)0x1 : (byte)0x2);
          writer.Write((byte)i);

          if (game.ShowScores || mobile == to)
            writer.Write(players.GetScore(i));
          else
            writer.Position++; // writer.Write(0);

          writer.Position += 3;

          writer.Write(players.IsPublic(i));

          writer.WriteAsciiFixed(mobile.Name, 30);
          writer.Write(!players.IsInGamePlayer(i));

          n++;
        }
        else if (game.ShowScores)
        {
          writer.Position++; // writer.Write(0);
          writer.Write((byte)0x2);
          writer.Write((byte)i);

          writer.Write(players.GetScore(i));

          writer.Position += 3;

          writer.Write(players.IsPublic(i));

          writer.Position += 30; //writer.WriteAsciiFixed("", 30);
          writer.Write((byte)0x1); // true

          n++;
        }
      }

      int position;

      if (n != players.Seats)
      {
        position = writer.Position;
        writer.Position = 9;
        writer.Write(n);
        writer.Position = position;
      }

      position = writer.Position;
      writer.Position = 1;
      writer.Write((short)writer.WrittenCount);
      ns.Send(writer.Span);
    }

    public static void SendMahjongGeneralInfo(NetState ns, MahjongGame game)
    {
      if (ns == null)
        return;

      SpanWriter writer = new SpanWriter(stackalloc byte[25]);
      writer.Write((byte)0xDA); // Packet ID
      writer.Write((ushort)25); // Dynamic Length

      writer.Write(game.Serial);
      writer.Write((short)0x05);

      writer.Position += 3;

      writer.Write((byte)((game.ShowScores ? 0x1 : 0x0) | (game.SpectatorVision ? 0x2 : 0x0)));

      writer.Write((byte)game.Dices.First);
      writer.Write((byte)game.Dices.Second);

      writer.Write((byte)game.DealerIndicator.Wind);
      writer.Write((short)game.DealerIndicator.Position.Y);
      writer.Write((short)game.DealerIndicator.Position.X);
      writer.Write((byte)game.DealerIndicator.Direction);

      writer.Write((short)game.WallBreakIndicator.Position.Y);
      writer.Write((short)game.WallBreakIndicator.Position.X);

      ns.Send(writer.Span);
    }

    public static void SendMahjongTilesInfo(NetState ns, MahjongGame game, Mobile to)
    {
      MahjongTile[] tiles = game.Tiles;
      MahjongPlayers players = game.Players;

      EnsureCapacity(11 + 9 * tiles.Length);

      m_Stream.Write(game.Serial);
      m_Stream.Write((byte)0);
      m_Stream.Write((byte)0x4);

      m_Stream.Write((short)tiles.Length);

      foreach (MahjongTile tile in tiles)
      {
        m_Stream.Write((byte)tile.Number);

        if (tile.Flipped)
        {
          int hand = tile.Dimensions.GetHandArea();

          if (hand < 0 || players.IsPublic(hand) || players.GetPlayer(hand) == to ||
              game.SpectatorVision && players.IsSpectator(to))
            m_Stream.Write((byte)tile.Value);
          else
            m_Stream.Write((byte)0);
        }
        else
        {
          m_Stream.Write((byte)0);
        }

        m_Stream.Write((short)tile.Position.Y);
        m_Stream.Write((short)tile.Position.X);
        m_Stream.Write((byte)tile.StackLevel);
        m_Stream.Write((byte)tile.Direction);

        m_Stream.Write(tile.Flipped ? (byte)0x10 : (byte)0x0);
      }
    }
  }

  public sealed class MahjongTileInfo : Packet
  {
    public MahjongTileInfo(MahjongTile tile, Mobile to) : base(0xDA)
    {
      MahjongGame game = tile.Game;
      MahjongPlayers players = game.Players;

      EnsureCapacity(18);

      m_Stream.Write(tile.Game.Serial);
      m_Stream.Write((byte)0);
      m_Stream.Write((byte)0x3);

      m_Stream.Write((byte)tile.Number);

      if (tile.Flipped)
      {
        int hand = tile.Dimensions.GetHandArea();

        if (hand < 0 || players.IsPublic(hand) || players.GetPlayer(hand) == to ||
            game.SpectatorVision && players.IsSpectator(to))
          m_Stream.Write((byte)tile.Value);
        else
          m_Stream.Write((byte)0);
      }
      else
      {
        m_Stream.Write((byte)0);
      }

      m_Stream.Write((short)tile.Position.Y);
      m_Stream.Write((short)tile.Position.X);
      m_Stream.Write((byte)tile.StackLevel);
      m_Stream.Write((byte)tile.Direction);

      m_Stream.Write(tile.Flipped ? (byte)0x10 : (byte)0x0);
    }
  }

  public sealed class MahjongRelieve : Packet
  {
    public MahjongRelieve(MahjongGame game) : base(0xDA)
    {
      EnsureCapacity(9);

      m_Stream.Write(game.Serial);
      m_Stream.Write((byte)0);
      m_Stream.Write((byte)0x1A);
    }
  }
}
