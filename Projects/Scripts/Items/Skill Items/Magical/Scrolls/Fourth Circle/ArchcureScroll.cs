namespace Server.Items
{
  public class ArchCureScroll : SpellScroll
  {
    [Constructible]
    public ArchCureScroll(int amount = 1) : base(24, 0x1F45, amount)
    {
    }

    public ArchCureScroll(Serial serial) : base(serial)
    {
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
