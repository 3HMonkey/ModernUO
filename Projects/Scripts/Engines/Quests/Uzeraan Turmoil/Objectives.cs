using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Haven
{
  public class FindUzeraanBeginObjective : QuestObjective
  {
    public override object Message => 1046039;

    public override void OnComplete()
    {
      if (System.From.Profession == 5) // paladin
        System.AddConversation(new UzeraanTitheConversation());
      else
        System.AddConversation(new UzeraanFirstTaskConversation());
    }
  }

  public class TitheGoldObjective : QuestObjective
  {
    private int m_OldTithingPoints;

    public TitheGoldObjective() => m_OldTithingPoints = -1;

    public override object Message => 1060386;

    public override void CheckProgress()
    {
      PlayerMobile pm = System.From;
      int curTithingPoints = pm.TithingPoints;

      if (curTithingPoints >= 500)
        Complete();
      else if (curTithingPoints > m_OldTithingPoints && m_OldTithingPoints >= 0)
        pm.SendLocalizedMessage(1060240, "",
          0x41); // You must have at least 500 tithing points before you can continue in your quest.

      m_OldTithingPoints = curTithingPoints;
    }

    public override void OnComplete()
    {
      System.AddObjective(new FindUzeraanFirstTaskObjective());
    }
  }

  public class FindUzeraanFirstTaskObjective : QuestObjective
  {
    public override object Message => 1060387;

    public override void OnComplete()
    {
      System.AddConversation(new UzeraanFirstTaskConversation());
    }
  }

  public enum KillHordeMinionsStep
  {
    First,
    LearnKarma,
    Others
  }

  public class KillHordeMinionsObjective : QuestObjective
  {
    public KillHordeMinionsObjective()
    {
    }

    public KillHordeMinionsObjective(KillHordeMinionsStep step) => Step = step;

    public KillHordeMinionsStep Step{ get; private set; }

    public override object Message
    {
      get
      {
        return Step switch
        {
          KillHordeMinionsStep.First =>
          /* Find the mountain pass beyond the house which lies at the
             * end of the runic road.<BR><BR>
             *
             * Assist the city Militia by slaying <I>Horde Minions</I>
             */
          1049089,
          KillHordeMinionsStep.LearnKarma =>
          /* You have just gained some <a href="?ForceTopic45">Karma</a>
             * for killing the horde minion. <a href="?ForceTopic134">Learn</a>
             * how this affects your Paladin abilities.
             */
          1060389,
          _ => 1060507
        };
      }
    }

    public override int MaxProgress
    {
      get
      {
        if (System.From.Profession == 5) // paladin
          return Step switch
          {
            KillHordeMinionsStep.First => 1,
            KillHordeMinionsStep.LearnKarma => 2,
            _ => 5
          };

        return 5;
      }
    }

    public override bool Completed
    {
      get
      {
        if (Step == KillHordeMinionsStep.LearnKarma && HasBeenRead)
          return true;
        return base.Completed;
      }
    }

    public override void RenderProgress(BaseQuestGump gump)
    {
      if (!Completed)
      {
        gump.AddHtmlObject(70, 260, 270, 100, 1049090, BaseQuestGump.Blue, false, false); // Horde Minions killed:
        gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
      }
      else
      {
        base.RenderProgress(gump);
      }
    }

    public override void OnRead()
    {
      CheckCompletionStatus();
    }

    public override bool IgnoreYoungProtection(Mobile from) =>
      from is HordeMinion && from.Map == Map.Trammel && TreasureMap.IsInHavenIsland(from);

    public override void OnKill(BaseCreature creature, Container corpse)
    {
      if (creature is HordeMinion && corpse.Map == Map.Trammel && TreasureMap.IsInHavenIsland(corpse))
      {
        if (CurProgress == 0)
          ContentPackets.SendDisplayHelpTopic(System.From.NetState, 29, false); // HEALING

        CurProgress++;
      }
    }

    public override void OnComplete()
    {
      if (System.From.Profession == 5)
        switch (Step)
        {
          case KillHordeMinionsStep.First:
          {
            QuestObjective obj = new KillHordeMinionsObjective(KillHordeMinionsStep.LearnKarma);
            System.AddObjective(obj);
            obj.CurProgress = CurProgress;
            break;
          }
          case KillHordeMinionsStep.LearnKarma:
          {
            QuestObjective obj = new KillHordeMinionsObjective(KillHordeMinionsStep.Others);
            System.AddObjective(obj);
            obj.CurProgress = CurProgress;
            break;
          }
          default:
          {
            System.AddObjective(new FindUzeraanAboutReportObjective());
            break;
          }
        }
      else
        System.AddObjective(new FindUzeraanAboutReportObjective());
    }

    public override void ChildDeserialize(GenericReader reader)
    {
      int version = reader.ReadEncodedInt();

      Step = (KillHordeMinionsStep)reader.ReadEncodedInt();
    }

    public override void ChildSerialize(GenericWriter writer)
    {
      writer.WriteEncodedInt(0); // version

      writer.WriteEncodedInt((int)Step);
    }
  }

  public class FindUzeraanAboutReportObjective : QuestObjective
  {
    public override object Message => 1049091;

    public override void OnComplete()
    {
      System.AddConversation(new UzeraanReportConversation());
    }
  }

  public class FindSchmendrickObjective : QuestObjective
  {
    public override object Message => 1049120;

    public override bool IgnoreYoungProtection(Mobile from)
    {
      // This restriction begins when this objective is completed, and continues until the quest is ended
      if (Completed && from is RestlessSoul && from.Map == Map.Trammel && from.X >= 5199 && from.X <= 5271 &&
          from.Y >= 1812 && from.Y <= 1865) // Schmendrick's cave
        return true;

      return false;
    }

    public override void OnComplete()
    {
      System.AddConversation(new SchmendrickConversation());
    }
  }

  public class FindApprenticeObjective : QuestObjective
  {
    public override object Message => 1049323;

    public override void OnComplete()
    {
      System.AddObjective(new ReturnScrollOfPowerObjective());
    }
  }

  public class ReturnScrollOfPowerObjective : QuestObjective
  {
    public override object Message => 1049324;

    public override void OnComplete()
    {
      System.AddConversation(new UzeraanScrollOfPowerConversation());
    }
  }

  public class FindDryadObjective : QuestObjective
  {
    public override object Message => 1049358;

    public override void OnComplete()
    {
      System.AddConversation(new DryadConversation());
    }
  }

  public class ReturnFertileDirtObjective : QuestObjective
  {
    public override object Message => 1049327;

    public override void OnComplete()
    {
      System.AddConversation(new UzeraanFertileDirtConversation());
    }
  }

  public class GetDaemonBloodObjective : QuestObjective
  {
    private bool m_Ambushed;

    public override object Message => 1049361;

    public override void CheckProgress()
    {
      PlayerMobile player = System.From;

      if (!m_Ambushed && player.Map == Map.Trammel && player.InRange(new Point3D(3456, 2558, 50), 30))
      {
        int x = player.X - 1;
        int y = player.Y - 2;
        int z = Map.Trammel.GetAverageZ(x, y);

        if (Map.Trammel.CanSpawnMobile(x, y, z))
        {
          m_Ambushed = true;

          player.LocalOverheadMessage(MessageType.Regular, 0x3B2,
            1049330); // You have been ambushed! Fight for your honor!!!

          BaseCreature creature = new HordeMinion();
          creature.MoveToWorld(new Point3D(x, y, z), Map.Trammel);
          creature.Combatant = player;
        }
      }
    }

    public override void OnComplete()
    {
      System.AddObjective(new ReturnDaemonBloodObjective());
    }

    public override void ChildDeserialize(GenericReader reader)
    {
      int version = reader.ReadEncodedInt();

      m_Ambushed = reader.ReadBool();
    }

    public override void ChildSerialize(GenericWriter writer)
    {
      writer.WriteEncodedInt(0); // version

      writer.Write(m_Ambushed);
    }
  }

  public class ReturnDaemonBloodObjective : QuestObjective
  {
    public override object Message => 1049332;

    public override void OnComplete()
    {
      System.AddConversation(new UzeraanDaemonBloodConversation());
    }
  }

  public class GetDaemonBoneObjective : QuestObjective
  {
    public Container CorpseWithBone{ get; set; }

    public override object Message => System.From.Profession == 5 ? 1060755 : 1049362;

    public override void OnComplete()
    {
      System.AddObjective(new ReturnDaemonBoneObjective());
    }

    private static bool IsInHavenGraveyard(IPoint2D p) => p.X >= 3391 && p.X <= 3424 &&
                                                          p.Y >= 2639 && p.Y <= 2664;

    public override bool IgnoreYoungProtection(Mobile from) =>
      (from is Zombie || from is Skeleton) && from.Map == Map.Trammel && IsInHavenGraveyard(from);

    public override bool GetKillEvent(BaseCreature creature, Container corpse) =>
      base.GetKillEvent(creature, corpse) || UzeraanTurmoilQuest.HasLostDaemonBone(System.From);

    public override void OnKill(BaseCreature creature, Container corpse)
    {
      if ((creature is Zombie || creature is Skeleton) && corpse.Map == Map.Trammel && IsInHavenGraveyard(corpse) && Utility.RandomDouble() < 0.25)
        CorpseWithBone = corpse;
    }

    public override void ChildDeserialize(GenericReader reader)
    {
      int version = reader.ReadEncodedInt();

      CorpseWithBone = (Container)reader.ReadItem();
    }

    public override void ChildSerialize(GenericWriter writer)
    {
      if (CorpseWithBone?.Deleted == true)
        CorpseWithBone = null;

      writer.WriteEncodedInt(0); // version

      writer.Write(CorpseWithBone);
    }
  }

  public class ReturnDaemonBoneObjective : QuestObjective
  {
    public override object Message => 1049334;

    public override void OnComplete()
    {
      System.AddConversation(new UzeraanDaemonBoneConversation());
    }
  }

  public class CashBankCheckObjective : QuestObjective
  {
    public override object Message => 1049336;

    public override void OnComplete()
    {
      System.AddConversation(new BankerConversation());
    }
  }
}
