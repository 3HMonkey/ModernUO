namespace Server.Factions
{
  public class StrongholdMonolith : BaseMonolith
  {
    public StrongholdMonolith(Town town = null, Faction faction = null) : base(town, faction)
    {

    }

    public StrongholdMonolith(Serial serial) : base(serial)
    {
    }

    public override int DefaultLabelNumber => 1041042; // A Faction Sigil Monolith

    public override void OnTownChanged()
    {
      AssignName(Town?.Definition.StrongholdMonolithName);
    }

    public override void Serialize(GenericWriter writer)
    {
      base.Serialize(writer);

      writer.Write(0); // version
    }

    public override void Deserialize(GenericReader reader)
    {
      base.Deserialize(reader);

      int version = reader.ReadInt();
    }
  }
}
