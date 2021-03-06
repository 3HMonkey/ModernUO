namespace Server.Items
{
  public class PurgeMagicScroll : SpellScroll
  {
    [Constructible]
    public PurgeMagicScroll(int amount = 1)
      : base(679, 0x2DA0, amount)
    {
    }

    public PurgeMagicScroll(Serial serial)
      : base(serial)
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

      /*int version = */
      reader.ReadInt();
    }
  }
}
