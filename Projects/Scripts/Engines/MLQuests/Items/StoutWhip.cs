namespace Server.Items
{
  public class StoutWhip : Item
  {
    [Constructible]
    public StoutWhip() : base(0x166F) => LootType = LootType.Blessed;

    public StoutWhip(Serial serial) : base(serial)
    {
    }

    public override int LabelNumber => 1074812; // Stout Whip

    public override void Serialize(GenericWriter writer)
    {
      base.Serialize(writer);

      writer.Write(0); // Version
    }

    public override void Deserialize(GenericReader reader)
    {
      base.Deserialize(reader);

      int version = reader.ReadInt();
    }
  }
}