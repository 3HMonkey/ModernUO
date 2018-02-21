using System;
using Server;

namespace Server.Items
{
	public class EagleStrikeScroll : SpellScroll
	{
		[Constructible]
		public EagleStrikeScroll()
			: this( 1 )
		{
		}

		[Constructible]
		public EagleStrikeScroll( int amount )
			: base( 682, 0x2DA3, amount )
		{
		}

		public EagleStrikeScroll( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}
}