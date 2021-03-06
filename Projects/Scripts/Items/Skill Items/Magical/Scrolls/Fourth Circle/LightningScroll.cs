namespace Server.Items
{
  public class LightningScroll : SpellScroll
  {
    [Constructible]
    public LightningScroll(int amount = 1) : base(29, 0x1F4A, amount)
    {
    }

    public LightningScroll(Serial serial) : base(serial)
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
