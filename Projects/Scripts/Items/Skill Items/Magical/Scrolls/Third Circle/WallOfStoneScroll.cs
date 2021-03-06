namespace Server.Items
{
  public class WallOfStoneScroll : SpellScroll
  {
    [Constructible]
    public WallOfStoneScroll(int amount = 1) : base(23, 0x1F44, amount)
    {
    }

    public WallOfStoneScroll(Serial serial) : base(serial)
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
