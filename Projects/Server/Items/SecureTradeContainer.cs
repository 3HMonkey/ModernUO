/***************************************************************************
 *                          SecureTradeContainer.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Server.Accounting;
using Server.Network;

namespace Server.Items
{
  public class SecureTradeContainer : Container
  {
    public SecureTradeContainer(SecureTrade trade) : base(0x1E5E)
    {
      Trade = trade;

      Movable = false;
    }

    public SecureTradeContainer(Serial serial) : base(serial)
    {
    }

    public SecureTrade Trade{ get; }

    public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
    {
      if (item == Trade.From.VirtualCheck || item == Trade.To.VirtualCheck) return true;

      Mobile to = Trade.From.Container != this ? Trade.From.Mobile : Trade.To.Mobile;

      return m.CheckTrade(to, item, this, message, checkItems, plusItems, plusWeight);
    }

    public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
    {
      reject = LRReason.CannotLift;
      return false;
    }

    public override bool IsAccessibleTo(Mobile check) => IsChildOf(check) && Trade?.Valid == true && base.IsAccessibleTo(check);

    public override void OnItemAdded(Item item)
    {
      if (!(item is VirtualCheck))
        ClearChecks();
    }

    public override void OnItemRemoved(Item item)
    {
      if (!(item is VirtualCheck))
        ClearChecks();
    }

    public override void OnSubItemAdded(Item item)
    {
      if (!(item is VirtualCheck))
        ClearChecks();
    }

    public override void OnSubItemRemoved(Item item)
    {
      if (!(item is VirtualCheck))
        ClearChecks();
    }

    public void ClearChecks()
    {
      if (Trade == null)
        return;

      if (Trade.From?.IsDisposed == false)
        Trade.From.Accepted = false;

      if (Trade.To?.IsDisposed == false)
        Trade.To.Accepted = false;

      Trade.Update();
    }

    public override bool IsChildVisibleTo(Mobile m, Item child) =>
      child is VirtualCheck
        ? AccountGold.Enabled && m.NetState?.NewSecureTrading != true
        : base.IsChildVisibleTo(m, child);

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
