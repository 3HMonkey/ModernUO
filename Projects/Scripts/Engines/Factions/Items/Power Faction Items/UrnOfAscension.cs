using Server.Factions;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server
{
  public sealed class UrnOfAscension : PowerFactionItem
  {
    public UrnOfAscension()
      : base(9246)
    {
    }

    public UrnOfAscension(Serial serial)
      : base(serial)
    {
    }

    public override string DefaultName => "urn of ascension";

    public override bool Use(Mobile from)
    {
      Faction ourFaction = Faction.Find(from);

      bool used = false;

      foreach (Mobile mob in from.GetMobilesInRange(8))
        if (mob.Player && !mob.Alive && from.InLOS(mob))
        {
          if (Faction.Find(mob) != ourFaction) continue;

          BaseHouse house = BaseHouse.FindHouseAt(mob);

          if (house?.IsFriend(from) != false || house.IsFriend(mob))
          {
            Faction.ClearSkillLoss(mob);

            mob.SendGump(new ResurrectGump(mob, from));
            used = true;
          }
        }

      if (used)
      {
        from.LocalOverheadMessage(MessageType.Regular, 2219, false, "The urn shatters as you invoke its power.");
        from.PlaySound(64);

        Effects.PlaySound(from.Location, from.Map, 1481);
      }

      return used;
    }

    public override void Serialize(GenericWriter writer)
    {
      base.Serialize(writer);

      writer.WriteEncodedInt(0); // version
    }

    public override void Deserialize(GenericReader reader)
    {
      base.Deserialize(reader);

      int version = reader.ReadEncodedInt();
    }
  }
}
