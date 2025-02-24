// Decompiled with JetBrains decompiler
// Type: TS_Server.Server.BattleAbstract
// Assembly: TS_Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F406604-6D74-4A33-BF41-C1841FC4C8A8
// Assembly location: C:\Users\anumart.c.BCONNEX\Desktop\TestGraph\Database\ถอดดาต้าTS\TS_Server.exe

using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using TS_Server.Client;
using TS_Server.DataTools;
using TS_Server.Server.BattleClasses;

namespace TS_Server.Server
{
  public abstract class BattleAbstract
  {
    public TSMap map;
    public BattleParticipant[][] position;
    public List<BattleCommand> cmdReg;
    public int cmdNeeded;
    public int nextcmd;
    public int countDisabled = 0;
    public ushort countEnemy = 0;
    public ushort countAlly = 0;
    public byte battle_type;
    public int finish = 0;
    public System.Timers.Timer aTimer = new System.Timers.Timer(21000.0);

    protected BattleAbstract()
    {
    }

    protected BattleAbstract(TSClient c, byte type)
    {
      c.battle = this;
      this.map = c.map;
      this.map.announceBattle(c);
      this.battle_type = type;
      this.position = new BattleParticipant[4][];
      for (byte r = 0; r < (byte) 4; ++r)
      {
        this.position[(int) r] = new BattleParticipant[5];
        for (byte c1 = 0; c1 < (byte) 5; ++c1)
          this.position[(int) r][(int) c1] = new BattleParticipant(this, r, c1);
      }
      this.aTimer.Elapsed += new ElapsedEventHandler(this.timeOut);
      this.cmdReg = new List<BattleCommand>();
    }

    public abstract void start_round();

    public abstract void checkDeath(byte row, byte col, BattleCommand c);

    public abstract void endBattle(bool win);

    public void timeOut(object sender, EventArgs e)
    {
      if (this.countAlly == (ushort) 0)
      {
        this.endBattle(false);
        this.aTimer.Dispose();
      }
      else
        this.execute();
    }

    public void registerCommand(TSClient c, byte[] data, byte type)
    {
      if (!this.aTimer.Enabled || this.position[(int) data[2]][(int) data[3]].alreadyCommand)
        return;
      this.battleBroadcast(new PacketCreator(new byte[4]
      {
        (byte) 53,
        (byte) 5,
        data[2],
        data[3]
      }).send());
      Console.WriteLine("receive cmd " + (object) data[2] + " " + (object) data[3]);
      this.pushCommand(data[2], data[3], data[4], data[5], type, PacketReader.read16(data, 6));
    }

    public void pushCommand(
      byte init_row,
      byte init_col,
      byte dest_row,
      byte dest_col,
      byte type,
      ushort command_id)
    {
      BattleCommand battleCommand = new BattleCommand(init_row, init_col, dest_row, dest_col, type);
      this.position[(int) init_row][(int) init_col].alreadyCommand = true;
      if (type == (byte) 0)
      {
        battleCommand.skill = command_id;
        battleCommand.skill_lvl = this.position[(int) init_row][(int) init_col].getSkillLvl(command_id);
        if (command_id == (ushort) 13015 || command_id == (ushort) 10016 || command_id == (ushort) 11016 || command_id == (ushort) 12016)
          battleCommand.skill += (ushort) (((int) battleCommand.skill_lvl - 1) / 3);
        battleCommand.dmg = this.CalcDmg(init_row, init_col, dest_row, dest_col, battleCommand.skill, battleCommand.skill_lvl);
      }
      if (type == (byte) 1)
      {
        battleCommand.skill = command_id;
        if (ItemData.itemList[command_id].type != (byte) 16)
        {
          battleCommand.dmg = (ushort) ItemData.itemList[command_id].prop1_val;
        }
        else
        {
          battleCommand.type = (byte) 2;
          battleCommand.dmg = this.CalcDmg(init_row, init_col, dest_row, dest_col, ItemData.itemList[command_id].unk9, (byte) 1);
        }
      }
      battleCommand.priority = this.position[(int) init_row][(int) init_col].getAgi();
      if (battleCommand.skill == (ushort) 17001)
      {
        this.position[(int) battleCommand.init_row][(int) battleCommand.init_col].def = true;
        --this.cmdNeeded;
      }
      else
      {
        int index = 0;
        while (index < this.cmdReg.Count && battleCommand.priority <= this.cmdReg[index].priority)
          ++index;
        lock (this.cmdReg)
          this.cmdReg.Insert(index, battleCommand);
        --this.cmdNeeded;
      }
      if (this.cmdNeeded != 0)
        return;
      this.execute();
    }

    public ushort CalcDmg(
      byte initRow,
      byte initCol,
      byte destRow,
      byte destCol,
      ushort skill,
      byte skill_lvl)
    {
      BattleParticipant battleParticipant1 = this.position[(int) initRow][(int) initCol];
      BattleParticipant battleParticipant2 = this.position[(int) destRow][(int) destCol];
      int lvl = battleParticipant1.getLvl();
      battleParticipant1.getElem();
      int atk = battleParticipant1.getAtk();
      int mag = battleParticipant1.getMag();
      byte grade = SkillData.skillList[skill].grade;
      byte unk20 = SkillData.skillList[skill].unk20;
      int num1 = (int) ((double) ((int) grade * 15 * (int) unk20) * (1.0 + 0.2 * (double) skill_lvl));
      int num2;
      if (skill == (ushort) 10000)
      {
        num2 = (int) ((double) atk * 1.6);
        num1 = lvl * 2;
      }
      else
        num2 = SkillData.skillList[skill].unk17 != (ushort) 1 ? mag * 2 : (int) ((double) atk * 1.6 + (double) mag * 0.4);
      int num3 = (int) ((double) (num2 + lvl) * (1.0 + (double) grade * 0.1 * (double) unk20 + (double) skill_lvl * 0.02)) + num1;
      if (num3 > 50000)
        num3 = 50000;
      return (ushort) num3;
    }

    public ushort calcDmg(byte row, byte col, ushort skill, byte skill_lvl)
    {
      int num1 = (int) ((double) (int) ((double) (int) ((double) (int) ((double) this.position[(int) row][(int) col].getAtk() * 0.2 + (double) this.position[(int) row][(int) col].getMag() * 0.2) * ((double) SkillData.skillList[skill].grade + 0.1 * (double) skill_lvl)) * Math.Pow((double) this.position[(int) row][(int) col].getLvl(), 0.3)) * ((double) this.position[(int) row][(int) col].getAtk() * 0.2 + (double) this.position[(int) row][(int) col].getMag() * 0.2));
      int num2 = (int) ((SkillData.skillList[skill].unk17 != (ushort) 1 && skill != (ushort) 10000 ? (double) (int) ((double) this.position[(int) row][(int) col].getLvl() + (double) this.position[(int) row][(int) col].getMag() * 0.75) : (double) (this.position[(int) row][(int) col].getLvl() + this.position[(int) row][(int) col].getAtk())) * ((double) SkillData.skillList[skill].grade + 0.1 * (double) skill_lvl));
      if (num2 > 50000)
        num2 = 50000;
      return (ushort) num2;
    }

    public void execute()
    {
      this.nextcmd = 0;
      this.aTimer.Stop();
      Thread.Sleep(500);
      while (this.nextcmd < this.cmdReg.Count)
      {
        if (this.finish == 0)
        {
          try
          {
            BattleCommand c = this.cmdReg[this.nextcmd];
            BattleParticipant battleParticipant1 = this.position[(int) c.init_row][(int) c.init_col];
            BattleParticipant battleParticipant2 = this.position[(int) c.dest_row][(int) c.dest_col];
            if (battleParticipant1.exist && battleParticipant1.disable == 0 && !battleParticipant1.death)
            {
              if (!battleParticipant2.exist || battleParticipant2.death || battleParticipant2.buff_type == (ushort) 13005 || battleParticipant2.buff_type == (ushort) 13025)
              {
                if (this.changeTarget(c))
                  this.execCommand(c);
                else
                  ++this.nextcmd;
              }
              else
                this.execCommand(c);
            }
            else
              ++this.nextcmd;
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.ToString());
            this.finish = 2;
          }
        }
        else
          break;
      }
      this.start_round();
    }

    public bool changeTarget(BattleCommand c)
    {
      if (this.position[(int) c.dest_row][(int) c.dest_col].exist && (this.sameSide(c.init_row, c.dest_row) || c.type == (byte) 1 || SkillData.skillList[c.skill].skill_type > (byte) 2 || c.type == (byte) 2 && SkillData.skillList[ItemData.itemList[c.skill].unk9].skill_type > (byte) 2))
        return true;
      int num = c.dest_row < (byte) 2 ? 0 : 2;
      for (int index = 0; index < 10; ++index)
      {
        if (this.position[index % 2 + num][index / 2].exist && !this.position[index % 2 + num][index / 2].death && this.position[index % 2 + num][index / 2].buff_type != (ushort) 13005 && this.position[index % 2 + num][index / 2].buff_type != (ushort) 13025)
        {
          Console.WriteLine("change target to " + (object) (index % 2 + num) + " " + (object) (index / 2));
          c.dest_row = (byte) (index % 2 + num);
          c.dest_col = (byte) (index / 2);
          return true;
        }
      }
      return false;
    }

    public void execCommand(BattleCommand c)
    {
      if (c.skill == (ushort) 18001)
      {
        if (c.init_col == (byte) 2)
          this.finish = 2;
        else
          this.position[(int) c.init_row][(int) c.init_col].outBattle = true;
        ++this.nextcmd;
      }
      else
      {
        PacketCreator packetCreator = new PacketCreator((byte) 50, (byte) 1);
        if (c.skill >= (ushort) 20001 && c.skill <= (ushort) 20003)
          packetCreator.addBytes(this.makeExecutionPacket(c, false));
        else if (this.cmdReg[this.nextcmd].type == (byte) 0 && this.checkCombo(this.nextcmd + 1))
        {
          c.dmg = (ushort) ((double) c.dmg * 1.2);
          packetCreator.addBytes(this.makeExecutionPacket(c, false));
          for (++this.nextcmd; this.checkCombo(this.nextcmd); ++this.nextcmd)
          {
            c.dmg = (ushort) ((double) c.dmg * 1.2);
            packetCreator.addBytes(this.makeExecutionPacket(this.cmdReg[this.nextcmd], true));
          }
        }
        else
        {
          packetCreator.addBytes(this.makeExecutionPacket(c, false));
          ++this.nextcmd;
        }
        Console.WriteLine("send : " + BitConverter.ToString(packetCreator.getData()));
        this.battleBroadcast(packetCreator.send());
        if (c.skill >= (ushort) 20001 && c.skill <= (ushort) 20003)
          return;
        if (c.type == (byte) 0)
          Thread.Sleep((int) SkillData.skillList[c.skill].delay * 100);
        else if (c.type == (byte) 1)
          Thread.Sleep(2400);
        else
          Thread.Sleep((int) SkillData.skillList[c.skill].delay * 100);
        if (this.finish > 0)
          return;
        for (byte index1 = 0; index1 < (byte) 4; ++index1)
        {
          for (byte index2 = 0; index2 < (byte) 5; ++index2)
          {
            this.position[(int) index1][(int) index2].checkCommandEffect();
            if (this.position[(int) index1][(int) index2].outBattle)
              this.checkOutBattle(this.position[(int) index1][(int) index2]);
            this.position[(int) index1][(int) index2].purge_status();
          }
        }
      }
    }

    public byte[] makeExecutionPacket(BattleCommand c, bool combo)
    {
      byte b = 0;
      PacketCreator packetCreator1 = new PacketCreator();
      int num1 = 1;
      if (c.type != (byte) 0)
      {
        this.position[(int) c.init_row][(int) c.init_col].useItem(c.skill);
        if (c.type == (byte) 1)
          num1 = 1;
        else if (c.type == (byte) 2)
        {
          c.skill = ItemData.itemList[c.skill].unk9;
          num1 = (int) SkillData.skillList[c.skill].nb_target;
        }
      }
      else
      {
        if (c.skill > (ushort) 20003 || c.skill < (ushort) 20001)
          num1 = (int) SkillData.skillList[c.skill].nb_target;
        if (c.skill == (ushort) 11009 || c.skill == (ushort) 11010)
          num1 = c.skill_lvl < (byte) 4 ? 1 : (c.skill_lvl < (byte) 7 ? 3 : (c.skill_lvl < (byte) 10 ? 6 : 8));
        int num2 = 0;
        if (SkillData.skillList.ContainsKey(c.skill))
          num2 = (int) SkillData.skillList[c.skill].sp_cost;
        this.position[(int) c.init_row][(int) c.init_col].setSp(-num2);
        this.position[(int) c.init_row][(int) c.init_col].refreshSp();
      }
      switch (num1)
      {
        case 2:
          c.dmg /= (ushort) 2;
          ++b;
          packetCreator1.addBytes(this.getSkillEffect(c.dest_row, c.dest_col, c));
          sbyte num3 = c.dest_row == (byte) 0 || c.dest_row == (byte) 2 ? (sbyte) 1 : (sbyte) -1;
          if (this.position[(int) c.dest_row + (int) num3][(int) c.dest_col].exist && (combo || !this.position[(int) c.dest_row + (int) num3][(int) c.dest_col].death))
          {
            ++b;
            packetCreator1.addBytes(this.getSkillEffect((byte) ((uint) c.dest_row + (uint) num3), c.dest_col, c));
            break;
          }
          break;
        case 3:
          c.dmg /= (ushort) 2;
          ++b;
          packetCreator1.addBytes(this.getSkillEffect(c.dest_row, c.dest_col, c));
          for (int index = (int) c.dest_col - 1; index <= (int) c.dest_col + 1; index += 2)
          {
            if (index >= 0 && index < 5 && this.position[(int) c.dest_row][index].exist && (combo || !this.position[(int) c.dest_row][index].death))
            {
              ++b;
              packetCreator1.addBytes(this.getSkillEffect(c.dest_row, (byte) index, c));
            }
          }
          break;
        case 4:
          c.dmg /= (ushort) 2;
          ++b;
          packetCreator1.addBytes(this.getSkillEffect(c.dest_row, c.dest_col, c));
          for (int index = (int) c.dest_col - 1; index <= (int) c.dest_col + 1; index += 2)
          {
            if (index >= 0 && index < 5)
            {
              if (this.position[(int) c.dest_row][index].exist && (combo || !this.position[(int) c.dest_row][index].death))
              {
                ++b;
                packetCreator1.addBytes(this.getSkillEffect(c.dest_row, (byte) index, c));
              }
              else
              {
                ++b;
                packetCreator1.addBytes(this.getSkillEffect(c.dest_row, c.dest_col, c));
              }
            }
            else
            {
              ++b;
              packetCreator1.addBytes(this.getSkillEffect(c.dest_row, c.dest_col, c));
            }
          }
          break;
        case 5:
          c.dmg /= (ushort) 3;
          ++b;
          packetCreator1.addBytes(this.getSkillEffect(c.dest_row, c.dest_col, c));
          for (int index = (int) c.dest_col - 1; index <= (int) c.dest_col + 1; index += 2)
          {
            if (index >= 0 && index < 5 && this.position[(int) c.dest_row][index].exist && (combo || !this.position[(int) c.dest_row][index].death))
            {
              ++b;
              packetCreator1.addBytes(this.getSkillEffect(c.dest_row, (byte) index, c));
            }
          }
          sbyte num4 = c.dest_row == (byte) 0 || c.dest_row == (byte) 2 ? (sbyte) 1 : (sbyte) -1;
          if (this.position[(int) c.dest_row + (int) num4][(int) c.dest_col].exist && (combo || !this.position[(int) c.dest_row + (int) num4][(int) c.dest_col].death))
          {
            ++b;
            packetCreator1.addBytes(this.getSkillEffect((byte) ((uint) c.dest_row + (uint) num4), c.dest_col, c));
            break;
          }
          break;
        case 6:
          c.dmg /= (ushort) 3;
          ++b;
          packetCreator1.addBytes(this.getSkillEffect(c.dest_row, c.dest_col, c));
          byte num5 = c.dest_row == (byte) 0 || c.dest_row == (byte) 1 ? (byte) 0 : (byte) 2;
          for (byte row = num5; (int) row < (int) num5 + 2; ++row)
          {
            for (int index = (int) c.dest_col - 1; index <= (int) c.dest_col + 1; ++index)
            {
              if (index >= 0 && index < 5 && this.position[(int) row][index].exist && !this.position[(int) row][index].death)
              {
                ++b;
                if ((int) row != (int) c.dest_row || index != (int) c.dest_col)
                  packetCreator1.addBytes(this.getSkillEffect(row, (byte) index, c));
              }
            }
          }
          break;
        case 7:
          c.dmg /= (ushort) 3;
          for (byte col = 0; col < (byte) 5; ++col)
          {
            if (this.position[(int) c.dest_row][(int) col].exist && !this.position[(int) c.dest_row][(int) col].death)
            {
              ++b;
              packetCreator1.addBytes(this.getSkillEffect(c.dest_row, col, c));
            }
          }
          break;
        case 8:
          c.dmg /= (ushort) 4;
          byte num6 = c.dest_row >= (byte) 2 ? (byte) 2 : (byte) 0;
          for (byte row = num6; (int) row < (int) num6 + 2; ++row)
          {
            for (byte col = 0; col < (byte) 5; ++col)
            {
              if (this.position[(int) row][(int) col].exist && !this.position[(int) row][(int) col].death)
              {
                ++b;
                packetCreator1.addBytes(this.getSkillEffect(row, col, c));
              }
            }
          }
          break;
        default:
          ++b;
          packetCreator1.addBytes(this.getSkillEffect(c.dest_row, c.dest_col, c));
          break;
      }
      byte[] data = packetCreator1.getData();
      PacketCreator packetCreator2 = new PacketCreator();
      packetCreator2.add16((ushort) (6 + data.Length));
      packetCreator2.add8(c.init_row);
      packetCreator2.add8(c.init_col);
      if (c.type != (byte) 1)
        packetCreator2.add16(c.skill);
      else
        packetCreator2.add16((ushort) 19001);
      packetCreator2.add8((byte) num1);
      packetCreator2.add8(b);
      packetCreator2.addBytes(data);
      return packetCreator2.getData();
    }

    public byte[] getSkillEffect(byte row, byte col, BattleCommand c)
    {
      BattleParticipant init = this.position[(int) c.init_row][(int) c.init_col];
      BattleParticipant dest = this.position[(int) row][(int) col];
      byte num1 = 0;
      byte num2 = 1;
      byte num3 = 0;
      byte num4 = 1;
      int num5 = 0;
      int num6 = 0;
      if (SkillData.skillList.ContainsKey(c.skill))
        num6 = (int) SkillData.skillList[c.skill].elem;
      if (c.skill == (ushort) 10000)
        num6 = init.getElem();
      int elem = dest.getElem();
      double elem_coef = 1.0;
      switch (num6)
      {
        case 1:
          switch (elem)
          {
            case 2:
              elem_coef = 1.2;
              break;
            case 4:
              elem_coef = 0.8;
              break;
          }
          break;
        case 2:
          switch (elem)
          {
            case 1:
              elem_coef = 0.8;
              break;
            case 3:
              elem_coef = 1.2;
              break;
          }
          break;
        case 3:
          switch (elem)
          {
            case 2:
              elem_coef = 0.8;
              break;
            case 4:
              elem_coef = 1.2;
              break;
          }
          break;
        case 4:
          switch (elem)
          {
            case 1:
              elem_coef = 1.2;
              break;
            case 3:
              elem_coef = 0.8;
              break;
          }
          break;
      }
      byte effect = 1;
      if (c.skill >= (ushort) 13016 && c.skill <= (ushort) 13018)
        effect = (byte) 3;
      else if (c.skill >= (ushort) 10017 && c.skill <= (ushort) 10019)
        effect = (byte) 15;
      else if (c.skill >= (ushort) 11017 && c.skill <= (ushort) 11019)
        effect = (byte) 18;
      else if (c.skill >= (ushort) 12017 && c.skill <= (ushort) 12019)
        c.dmg = Math.Min((ushort) ((uint) c.dmg * ((uint) c.skill_lvl / 3U)), (ushort) 50000);
      else
        effect = c.skill < (ushort) 20001 || c.skill > (ushort) 20003 ? (c.type != (byte) 1 ? SkillData.skillList[c.skill].skill_type : (byte) 13) : (byte) 20;
      byte num7 = 1;
      if (effect != (byte) 20)
        num7 = this.calculateHit(row, col, ref c, effect, elem_coef);
      switch ((int) effect - 1)
      {
        case 2:
          num5 = 0;
          num3 = (byte) 221;
          if (num7 == (byte) 1)
          {
            num1 = (byte) 1;
            dest.disable += (int) (byte) (Math.Ceiling((double) c.skill_lvl / 2.0) + 1.0);
            dest.disable = (int) (byte) (Math.Ceiling((double) c.skill_lvl / 2.0) + 1.0);
            if (c.skill >= (ushort) 13015 && c.skill <= (ushort) 13018)
              dest.disable = 5;
            dest.disable_type = c.skill;
            ++this.countDisabled;
            Console.WriteLine(row.ToString() + " " + (object) col + " get disable " + (object) c.skill);
            goto case 11;
          }
          else
          {
            num1 = (byte) 2;
            goto case 11;
          }
        case 3:
          num5 = 0;
          num3 = (byte) 222;
          if (num7 == (byte) 1)
          {
            num1 = (byte) 1;
            dest.buff = (int) (byte) (Math.Ceiling((double) c.skill_lvl / 2.0) + 1.0);
            dest.buff_type = c.skill;
            if (c.skill == (ushort) 10015)
              dest.reflect_hp = (ushort) 4;
            Console.WriteLine(row.ToString() + " " + (object) col + " get buff " + (object) c.skill);
            goto case 11;
          }
          else
          {
            num1 = (byte) 0;
            goto case 11;
          }
        case 4:
        case 15:
        case 17:
          num5 = 0;
          num1 = (byte) 0;
          num3 = (byte) 0;
          num2 = (byte) 5;
          goto case 11;
        case 5:
          num5 = (int) c.dmg;
          num1 = (byte) 0;
          num3 = (byte) 26;
          num4 = (byte) 0;
          goto case 11;
        case 6:
          num5 = (int) c.dmg;
          num1 = (byte) 0;
          num3 = (byte) 25;
          num4 = (byte) 0;
          goto case 11;
        case 7:
          if (num7 == (byte) 1)
          {
            dest.death = false;
            if (row < (byte) 2)
            {
              ++this.countEnemy;
              goto case 6;
            }
            else
            {
              ++this.countAlly;
              goto case 6;
            }
          }
          else
          {
            num5 = 0;
            num1 = (byte) 1;
            num3 = (byte) 25;
            goto case 11;
          }
        case 8:
          num5 = 0;
          num1 = (byte) 1;
          num3 = (byte) 25;
          goto case 11;
        case 10:
          if (this.CatchPet(init, dest))
          {
            init.chr.addPet(dest.npc.npcid, 0);
            dest.outBattle = true;
            num1 = (byte) 0;
            num3 = (byte) 25;
            goto case 11;
          }
          else
            goto case 11;
        case 11:
          byte[] numArray1 = new byte[5 + (int) num2 * 4];
          numArray1[0] = row;
          numArray1[1] = col;
          numArray1[2] = num7;
          numArray1[3] = num1;
          numArray1[4] = num2;
          numArray1[5] = num3;
          numArray1[6] = (byte) num5;
          numArray1[7] = (byte) (num5 >> 8);
          numArray1[8] = num4;
          if (num2 > (byte) 1)
          {
            byte[] numArray2 = (byte[]) null;
            ushort[] numArray3 = (ushort[]) null;
            byte[] numArray4 = (byte[]) null;
            if (num2 == (byte) 2)
            {
              numArray2 = new byte[1];
              numArray3 = new ushort[1];
              numArray4 = new byte[1];
              if (c.type == (byte) 1)
              {
                ushort prop2 = ItemData.itemList[c.skill].prop2;
                numArray3[0] = (ushort) ItemData.itemList[c.skill].prop2_val;
                switch (prop2)
                {
                  case 25:
                    numArray2[0] = (byte) 25;
                    dest.setHp((int) numArray3[0]);
                    dest.refreshHp();
                    break;
                  case 26:
                    numArray2[0] = (byte) 26;
                    dest.setSp((int) numArray3[0]);
                    dest.refreshSp();
                    break;
                }
                numArray4[0] = (byte) 0;
              }
              else if (effect == (byte) 14)
              {
                numArray2[0] = (byte) 26;
                numArray3[0] = (ushort) ((uint) c.dmg / 5U);
                numArray4[0] = (byte) 0;
              }
            }
            if (num2 == (byte) 5)
            {
              numArray2 = new byte[4]
              {
                (byte) 221,
                (byte) 222,
                (byte) 223,
                (byte) 225
              };
              numArray3 = new ushort[4];
              numArray4 = new byte[4]
              {
                (byte) 1,
                (byte) 1,
                (byte) 1,
                (byte) 1
              };
            }
            for (int index = 1; index < (int) num2; ++index)
            {
              numArray1[5 + index * 4] = numArray2[index - 1];
              numArray1[6 + index * 4] = (byte) numArray3[index - 1];
              numArray1[7 + index * 4] = (byte) ((uint) numArray3[index - 1] >> 8);
              numArray1[8 + index * 4] = numArray4[index - 1];
            }
          }
          return numArray1;
        case 12:
          if (num7 == (byte) 1)
          {
            num5 = ItemData.itemList[c.skill].prop1_val;
            num1 = (byte) 0;
            num4 = (byte) 0;
            switch (ItemData.itemList[c.skill].prop1)
            {
              case 25:
                num3 = (byte) 25;
                dest.setHp((int) c.dmg);
                dest.refreshHp();
                break;
              case 26:
                num3 = (byte) 26;
                dest.setSp((int) c.dmg);
                dest.refreshSp();
                break;
            }
            if (ItemData.itemList[c.skill].prop2 != (ushort) 0)
              num2 = (byte) 2;
            if (ItemData.itemList[c.skill].type == (byte) 50)
            {
              dest.death = false;
              if (c.dest_row < (byte) 2)
                ++this.countEnemy;
              else
                ++this.countAlly;
              goto case 11;
            }
            else
              goto case 11;
          }
          else
          {
            num5 = 0;
            num1 = (byte) 1;
            num3 = (byte) 25;
            num4 = (byte) 0;
            goto case 11;
          }
        case 13:
          num2 = (byte) 2;
          num5 = (int) c.dmg;
          num1 = (byte) 1;
          num3 = (byte) 25;
          num4 = (byte) 0;
          goto case 11;
        case 14:
          num5 = 0;
          num3 = (byte) 223;
          if (num7 == (byte) 1)
          {
            num1 = (byte) 0;
            dest.debuff = (int) (byte) (Math.Ceiling((double) c.skill_lvl / 2.0) + 1.0);
            if (c.skill >= (ushort) 10016 && c.skill <= (ushort) 10019)
              dest.debuff = 5;
            dest.debuff_type = c.skill;
            if (c.skill == (ushort) 14021 || c.skill == (ushort) 20014)
            {
              for (int nextcmd = this.nextcmd; nextcmd < this.cmdReg.Count; ++nextcmd)
              {
                if ((int) this.cmdReg[nextcmd].init_row == (int) row && (int) this.cmdReg[nextcmd].init_col == (int) col)
                  this.cmdReg.RemoveAt(nextcmd);
              }
            }
            Console.WriteLine(row.ToString() + " " + (object) col + " get debuff " + (object) c.skill);
            goto case 11;
          }
          else
          {
            num1 = (byte) 2;
            goto case 11;
          }
        case 18:
          num5 = 0;
          num3 = (byte) 225;
          if (num7 == (byte) 1)
          {
            num1 = (byte) 1;
            dest.aura = (int) (byte) (Math.Ceiling((double) c.skill_lvl / 2.0) + 1.0);
            dest.aura_type = c.skill;
            Console.WriteLine(row.ToString() + " " + (object) col + " get aura " + (object) c.skill);
            goto case 11;
          }
          else
          {
            num1 = (byte) 0;
            goto case 11;
          }
        case 19:
          num5 = (int) c.dmg;
          num3 = (byte) 25;
          Console.WriteLine("special dmg " + (object) num5);
          num1 = (byte) 0;
          dest.setHp((int) -c.dmg);
          dest.refreshHp();
          this.checkDeath(row, col, c);
          goto case 11;
        default:
          num3 = (byte) 25;
          if (num7 == (byte) 1)
          {
            if (dest.buff_type == (ushort) 10015)
            {
              num5 = 0;
              num1 = (byte) 1;
              init.reflect = (ushort) 10015;
              init.reflect_dmg += (ushort) ((uint) c.dmg / 3U);
              --dest.reflect_hp;
              if (dest.reflect_hp == (ushort) 0)
              {
                dest.buff = 0;
                dest.buff_type = (ushort) 0;
                this.battleBroadcast(new PacketCreator(new byte[7]
                {
                  (byte) 53,
                  (byte) 1,
                  row,
                  col,
                  (byte) 2,
                  (byte) 0,
                  (byte) 0
                }).send());
                goto case 11;
              }
              else
                goto case 11;
            }
            else
            {
              int num8 = (int) ((double) c.dmg * elem_coef - (double) this.position[(int) row][(int) col].getDef() * Math.Pow((double) this.position[(int) row][(int) col].getLvl(), 0.3));
              num8 = (int) ((double) ((int) c.dmg / this.position[(int) row][(int) col].getDef() + 2) * elem_coef);
              int num9 = (int) ((double) (dest.getLvl() * 2) + (double) dest.getDef() * 1.75);
              num8 = (int) ((double) ((int) c.dmg - num9) * elem_coef);
              int num10 = init.getRb() - dest.getRb();
              if (num10 < 0)
                num10 = 0;
              num5 = (int) ((double) ((int) c.dmg - dest.getLvl() - dest.getDef()) * elem_coef * (1.0 + 0.1 * (double) num10)) + RandomGen.getInt(-2, 2);
              if (dest.chr != null || dest.pet != null)
                num5 >>= 1;
              if (num5 <= 0)
                num5 = 1;
              if (dest.def)
              {
                num5 /= 2;
                if (elem_coef == 0.8)
                  num5 = 1;
              }
              num1 = dest.def ? (byte) 1 : (byte) 0;
              dest.setHp(-num5);
              dest.refreshHp();
              this.checkDeath(row, col, c);
              goto case 11;
            }
          }
          else
          {
            num1 = dest.buff_type != (ushort) 10010 ? (byte) 2 : (byte) 1;
            num5 = 0;
            goto case 11;
          }
      }
    }

    public byte calculateHit(
      byte row,
      byte col,
      ref BattleCommand c,
      byte effect,
      double elem_coef)
    {
      BattleParticipant battleParticipant1 = this.position[(int) c.init_row][(int) c.init_col];
      BattleParticipant battleParticipant2 = this.position[(int) row][(int) col];
      switch (effect)
      {
        case 1:
          if (battleParticipant2.buff_type == (ushort) 13003)
            return 0;
          if (battleParticipant1.buff_type == (ushort) 13005)
          {
            this.position[(int) c.init_row][(int) c.init_col].buff = 1;
            goto case 2;
          }
          else
            goto case 2;
        case 2:
          if (battleParticipant2.buff_type == (ushort) 10010)
            return 0;
          if (battleParticipant2.buff_type == (ushort) 11002)
            c.dmg = (ushort) ((double) c.dmg * 0.75);
          else if (battleParticipant2.buff_type == (ushort) 10031)
            c.dmg = (ushort) ((double) c.dmg * 0.5);
          if (battleParticipant2.disable_type == (ushort) 20026)
            c.dmg = (ushort) ((double) c.dmg * 0.75);
          if (battleParticipant1.buff_type == (ushort) 13012)
            c.dmg = (ushort) ((double) c.dmg * 1.25);
          else if (battleParticipant1.debuff_type == (ushort) 13011)
            c.dmg = (ushort) ((double) c.dmg * 0.75);
          if (battleParticipant1.aura_type == (ushort) 12025)
            c.dmg = (ushort) ((double) c.dmg * 1.25);
          else if (battleParticipant1.aura_type == (ushort) 14040)
            c.dmg = (ushort) ((double) c.dmg * 1.5);
          if (battleParticipant2.def)
            c.dmg = (ushort) ((double) c.dmg * 0.75);
          return battleParticipant1.debuff_type >= (ushort) 10017 && battleParticipant1.debuff_type <= (ushort) 10019 ? ((double) RandomGen.getByte((byte) 0, (byte) 100) >= (double) (battleParticipant2.getLvl() - battleParticipant1.getLvl() + 5) * 0.2 + 50.0 ? (byte) 1 : (byte) 0) : ((double) RandomGen.getByte((byte) 0, (byte) 100) >= (double) (battleParticipant2.getLvl() - battleParticipant1.getLvl() + 5) * 0.2 ? (byte) 1 : (byte) 0);
        case 3:
          return battleParticipant2.death || battleParticipant2.disable_type != (ushort) 0 ? (byte) 0 : ((double) RandomGen.getByte((byte) 0, (byte) 100) >= Math.Max((double) (battleParticipant2.getLvl() - battleParticipant1.getLvl() + 5) * 0.2, 0.0) + 20.0 ? (byte) 1 : (byte) 0);
        case 4:
          return battleParticipant2.death || battleParticipant2.buff_type != (ushort) 0 ? (byte) 0 : (byte) 1;
        case 5:
          if (c.skill == (ushort) 11012 || c.skill == (ushort) 11025 || c.skill == (ushort) 11031)
          {
            this.position[(int) row][(int) col].purge_type = (byte) 3;
            return 1;
          }
          if (c.skill == (ushort) 11015 && battleParticipant2.disable_type == (ushort) 11014)
          {
            this.position[(int) row][(int) col].purge_type = (byte) 4;
            return 1;
          }
          if (c.skill == (ushort) 14007 && battleParticipant2.disable_type == (ushort) 14008)
          {
            this.position[(int) row][(int) col].purge_type = (byte) 4;
            return 1;
          }
          if (c.skill == (ushort) 14014 && battleParticipant2.debuff_type == (ushort) 14015)
          {
            this.position[(int) row][(int) col].purge_type = (byte) 6;
            return 1;
          }
          if (c.skill == (ushort) 14022 && battleParticipant2.debuff_type == (ushort) 14021)
          {
            this.position[(int) row][(int) col].purge_type = (byte) 6;
            return 1;
          }
          if (c.skill != (ushort) 10009 || battleParticipant2.buff_type != (ushort) 10010)
            return 0;
          this.position[(int) row][(int) col].purge_type = (byte) 5;
          return 1;
        case 6:
          if (battleParticipant2.death)
            return 0;
          c.dmg = (ushort) Math.Min(battleParticipant2.getMaxSp() - battleParticipant2.getSp(), (int) c.dmg);
          battleParticipant2.setSp((int) c.dmg);
          battleParticipant2.refreshSp();
          return 1;
        case 7:
          if (battleParticipant2.death)
            return 0;
          c.dmg = (ushort) Math.Min(battleParticipant2.getMaxHp() - battleParticipant2.getHp(), (int) c.dmg);
          battleParticipant2.setHp((int) c.dmg);
          battleParticipant2.refreshHp();
          return 1;
        case 8:
          if (!battleParticipant2.death)
            return 0;
          c.dmg = (ushort) ((double) (battleParticipant2.getMaxHp() * (int) battleParticipant1.getSkillLvl((ushort) 11013)) * 0.1);
          battleParticipant2.setHp((int) c.dmg);
          battleParticipant2.refreshHp();
          return 1;
        case 11:
          return 1;
        case 13:
          return battleParticipant2.death && ItemData.itemList[c.skill].type != (byte) 50 || !battleParticipant2.death && ItemData.itemList[c.skill].type == (byte) 50 ? (byte) 0 : (byte) 1;
        case 14:
          if (battleParticipant2.death)
            return 0;
          c.dmg = (ushort) Math.Min(battleParticipant2.getMaxHp() - battleParticipant2.getHp(), (int) c.dmg);
          battleParticipant2.setHp((int) c.dmg);
          battleParticipant2.refreshHp();
          battleParticipant2.setSp((int) c.dmg / 5);
          battleParticipant2.refreshSp();
          return 1;
        case 15:
          return battleParticipant2.death || battleParticipant2.debuff_type != (ushort) 0 ? (byte) 0 : ((double) RandomGen.getByte((byte) 0, (byte) 100) >= Math.Max((double) (battleParticipant2.getLvl() - battleParticipant1.getLvl() + 5) * 0.2, 0.0) + 15.0 ? (byte) 1 : (byte) 0);
        case 16:
          if (c.skill == (ushort) 10009 && battleParticipant2.buff_type == (ushort) 10010)
          {
            this.position[(int) row][(int) col].purge_type = (byte) 5;
            return 1;
          }
          if (c.skill != (ushort) 10014 || battleParticipant2.buff_type != (ushort) 10015)
            return 0;
          this.position[(int) row][(int) col].purge_type = (byte) 5;
          return 1;
        case 18:
          battleParticipant2.purge_type = !this.sameSide(c.init_row, row) ? (byte) 1 : (byte) 2;
          return 1;
        case 19:
          return battleParticipant2.death || battleParticipant2.aura_type != (ushort) 0 ? (byte) 0 : (byte) 1;
        default:
          return 0;
      }
    }

    public bool checkCombo(int index) => index != this.cmdReg.Count && this.cmdReg[index].type == (byte) 0 && this.cmdReg[index - 1].type == (byte) 0 && SkillData.skillList[this.cmdReg[index - 1].skill].skill_type == (byte) 1 && SkillData.skillList[this.cmdReg[index].skill].skill_type == (byte) 1 && (int) this.cmdReg[index - 1].dest_col == (int) this.cmdReg[index].dest_col && (int) this.cmdReg[index - 1].dest_row == (int) this.cmdReg[index].dest_row && this.position[(int) this.cmdReg[index].init_row][(int) this.cmdReg[index].init_col].disable <= 0 && !this.position[(int) this.cmdReg[index].init_row][(int) this.cmdReg[index].init_col].death;

    public bool sameSide(byte row1, byte row2) => Math.Abs((int) row1 - (int) row2) == 1 && (int) row1 + (int) row2 != 3;

    public void battleBroadcast(byte[] msg)
    {
      for (int index1 = 0; index1 < 4; index1 += 3)
      {
        for (int index2 = 0; index2 < 5; ++index2)
        {
          if (this.position[index1][index2].exist && this.position[index1][index2].type == (byte) 1)
            this.position[index1][index2].chr.reply(msg);
        }
      }
    }

    public BattleParticipant getBpByClient(TSClient client)
    {
      for (int index1 = 0; index1 < 4; index1 += 3)
      {
        for (int index2 = 0; index2 < 5; ++index2)
        {
          if (this.position[index1][index2].exist && this.position[index1][index2].chr == client.getChar())
            return this.position[index1][index2];
        }
      }
      return (BattleParticipant) null;
    }

    public void DoEquip(TSClient client)
    {
      BattleParticipant bpByClient = this.getBpByClient(client);
      if (bpByClient == null)
        return;
      this.battleBroadcast(new PacketCreator(new byte[4]
      {
        (byte) 53,
        (byte) 5,
        bpByClient.row,
        bpByClient.col
      }).send());
      --this.cmdNeeded;
      if (this.cmdNeeded != 0)
        return;
      this.execute();
    }

    public void DoEquipPet(TSClient client)
    {
      BattleParticipant bpByClient = this.getBpByClient(client);
      if (bpByClient == null)
        return;
      this.battleBroadcast(new PacketCreator(new byte[4]
      {
        (byte) 53,
        (byte) 5,
        bpByClient.row != (byte) 0 ? (byte) 2 : (byte) 1,
        bpByClient.col
      }).send());
      --this.cmdNeeded;
      if (this.cmdNeeded != 0)
        return;
      this.execute();
    }

    public void SetBattlePet(TSClient client, byte[] data)
    {
      ushort num = PacketReader.read16(data, 2);
      TSCharacter tsCharacter = client.getChar();
      if (client.getChar().setBattlePet(PacketReader.read16(data, 2)) && (int) num != (int) tsCharacter.pet_battle)
      {
        BattleParticipant bpByClient = this.getBpByClient(client);
        if (bpByClient == null)
          return;
        this.battleBroadcast(new PacketCreator(new byte[4]
        {
          (byte) 53,
          (byte) 5,
          bpByClient.row,
          bpByClient.col
        }).send());
        bpByClient.alreadyCommand = true;
        --this.cmdNeeded;
        byte r = bpByClient.row != (byte) 0 ? (byte) 2 : (byte) 1;
        byte col = bpByClient.col;
        if (this.position[(int) r][(int) col].exist)
          this.checkOutBattle(this.position[(int) r][(int) col]);
        this.position[(int) r][(int) col] = new BattleParticipant(this, r, col);
        this.position[(int) r][(int) col].petIn(tsCharacter.pet[(int) tsCharacter.pet_battle]);
        ++this.countAlly;
        TSPet tsPet = tsCharacter.pet[(int) tsCharacter.pet_battle];
        PacketCreator packetCreator = new PacketCreator((byte) 11, (byte) 5);
        packetCreator.addBytes(this.position[(int) r][(int) col].announce((byte) 5, this.countAlly).getData());
        this.battleBroadcast(packetCreator.send());
        client.reply(new PacketCreator(data).send());
      }
      if (this.cmdNeeded != 0)
        return;
      this.execute();
    }

    public void UnBattlePet(TSClient client, byte[] data)
    {
      BattleParticipant bpByClient = this.getBpByClient(client);
      if (bpByClient == null)
        return;
      this.battleBroadcast(new PacketCreator(new byte[4]
      {
        (byte) 53,
        (byte) 5,
        bpByClient.row,
        bpByClient.col
      }).send());
      bpByClient.alreadyCommand = true;
      --this.cmdNeeded;
      this.checkOutBattle(this.position[bpByClient.row == (byte) 0 ? 1 : 2][(int) bpByClient.col]);
      if (client.getChar().unsetBattlePet())
        client.reply(new PacketCreator(data).send());
      if (this.cmdNeeded != 0)
        return;
      this.execute();
    }

    public void outBattle(TSClient c)
    {
      for (int index = 0; index < 5; ++index)
      {
        if (this.position[3][index].exist && this.position[3][index].chr.client == c)
        {
          Console.WriteLine("3 " + (object) index + " out of battle");
          BattleParticipant bp1 = this.position[3][index];
          bp1.outBattle = true;
          if (this.aTimer.Enabled)
            this.checkOutBattle(bp1);
          if (this.position[2][index].exist)
          {
            BattleParticipant bp2 = this.position[2][index];
            bp2.outBattle = true;
            if (this.aTimer.Enabled)
              this.checkOutBattle(bp2);
          }
        }
      }
      if (!this.aTimer.Enabled || this.cmdNeeded != 0)
        return;
      this.execute();
    }

    public void checkOutBattle(BattleParticipant bp)
    {
      bp.exist = false;
      bp.outBattle = false;
      if (!bp.death)
      {
        if (bp.row >= (byte) 2)
          --this.countAlly;
        else
          --this.countEnemy;
        if (this.countEnemy == (ushort) 0)
          this.finish = 1;
        else if (this.countAlly == (ushort) 0)
          this.finish = 2;
        if (bp.disable > 0)
          --this.countDisabled;
      }
      this.battleBroadcast(new PacketCreator(new byte[4]
      {
        (byte) 11,
        (byte) 1,
        bp.row,
        bp.col
      }).send());
      if (!this.aTimer.Enabled || bp.alreadyCommand || bp.death || bp.disable != 0)
        return;
      --this.cmdNeeded;
    }

    public bool CatchPet(BattleParticipant init, BattleParticipant dest)
    {
      if (dest.npc != null && init.chr != null && init.chr.next_pet < 4 && (int) init.chr.level + 5 >= (int) dest.npc.level && NpcData.npcList[dest.npc.npcid].notPet == (byte) 0)
      {
        double num = (1.0 - (double) dest.getHp() / (double) dest.getMaxHp()) * 100.0;
        if ((double) RandomGen.getInt(0, 100) < num)
          return true;
      }
      return false;
    }
  }
}
