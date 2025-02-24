// Decompiled with JetBrains decompiler
// Type: TS_Server.Client.TSPet
// Assembly: TS_Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F406604-6D74-4A33-BF41-C1841FC4C8A8
// Assembly location: C:\Users\anumart.c.BCONNEX\Desktop\TestGraph\Database\ถอดดาต้าTS\TS_Server.exe

using MySql.Data.MySqlClient;
using System;
using TS_Server.DataTools;

namespace TS_Server.Client
{
  public class TSPet
  {
    public int pet_sid;
    public ushort NPCid;
    public byte[] name;
    public int hp;
    public int sp;
    public int mag;
    public int atk;
    public int def;
    public int agi;
    public int hpx;
    public int spx;
    public int hp_max;
    public int sp_max;
    public int hp2;
    public int sp2;
    public int mag2;
    public int atk2;
    public int def2;
    public int agi2;
    public uint totalxp;
    public double xp_pow;
    public int currentxp;
    public int skill_pt;
    public byte level;
    public byte fai;
    public byte reborn;
    public byte skill1_lvl;
    public byte skill2_lvl;
    public byte skill3_lvl;
    public byte skill4_lvl;
    public TSEquipment[] equipment;
    public byte slot;
    public byte location;
    public TSCharacter owner;

    public TSPet(TSCharacter chr, byte sl)
    {
      this.owner = chr;
      this.slot = sl;
      this.location = (byte) 0;
      this.equipment = new TSEquipment[6];
    }

    public TSPet(TSCharacter chr, int sid, byte sl)
    {
      this.owner = chr;
      this.pet_sid = sid;
      this.slot = sl;
      this.location = (byte) 0;
      this.equipment = new TSEquipment[6];
    }

    public void loadPetDB()
    {
      TSMysqlConnection tsMysqlConnection = new TSMysqlConnection();
      MySqlDataReader mySqlDataReader = tsMysqlConnection.selectQuery("SELECT * FROM pet WHERE pet_sid = " + (object) this.pet_sid);
      mySqlDataReader.Read();
      this.NPCid = mySqlDataReader.GetUInt16("npcid");
      this.name = (byte[]) mySqlDataReader["name"];
      this.level = mySqlDataReader.GetByte("level");
      this.currentxp = mySqlDataReader.GetInt32("exp");
      this.totalxp = mySqlDataReader.GetUInt32("exp_tot");
      this.hp = mySqlDataReader.GetInt32("hp");
      this.sp = mySqlDataReader.GetInt32("sp");
      this.mag = mySqlDataReader.GetInt32("mag");
      this.atk = mySqlDataReader.GetInt32("atk");
      this.def = mySqlDataReader.GetInt32("def");
      this.hpx = mySqlDataReader.GetInt32("hpx");
      this.spx = mySqlDataReader.GetInt32("spx");
      this.agi = mySqlDataReader.GetInt32("agi");
      this.hp = Math.Max(1, this.hp);
      this.skill_pt = mySqlDataReader.GetInt32("sk_point");
      this.fai = mySqlDataReader.GetByte("fai");
      this.fai = (byte) 100;
      this.slot = mySqlDataReader.GetByte("slot");
      this.location = mySqlDataReader.GetByte("location");
      this.skill1_lvl = mySqlDataReader.GetByte("sk1_lvl");
      this.skill2_lvl = mySqlDataReader.GetByte("sk2_lvl");
      this.skill3_lvl = mySqlDataReader.GetByte("sk3_lvl");
      this.skill4_lvl = mySqlDataReader.GetByte("sk4_lvl");
      this.loadEquipment((byte[]) mySqlDataReader["equip"]);
      mySqlDataReader.Close();
      tsMysqlConnection.connection.Close();
      this.reborn = NpcData.npcList[this.NPCid].reborn;
      this.hp_max = this.getHpMax();
      this.sp_max = this.getSpMax();
      this.xp_pow = this.reborn == (byte) 0 ? 2.9 : (this.reborn == (byte) 1 ? 2.9 : 3.0);
    }

    public void initPet(NpcInfo n)
    {
      this.level = (byte) 1;
      this.NPCid = n.id;
      this.mag = (int) n.mag;
      this.atk = (int) n.atk;
      this.def = (int) n.def;
      this.agi = (int) n.agi;
      this.hpx = (int) n.hpx;
      this.spx = (int) n.spx;
      this.name = n.name;
      this.hp2 = 0;
      this.sp2 = 0;
      this.mag2 = 0;
      this.atk2 = 0;
      this.def2 = 0;
      this.agi2 = 0;
      this.skill_pt = 0;
      this.fai = (byte) 60;
      this.reborn = n.reborn;
      this.hp_max = this.getHpMax();
      this.sp_max = this.getSpMax();
      this.hp = this.hp_max;
      this.sp = this.sp_max;
      this.totalxp = 6U;
      this.xp_pow = this.reborn == (byte) 0 ? 2.9 : (this.reborn == (byte) 1 ? 2.9 : 3.0);
      this.skill1_lvl = (byte) 1;
      this.skill2_lvl = (byte) 1;
      this.skill3_lvl = (byte) 1;
      this.skill4_lvl = (byte) 0;
      TSMysqlConnection tsMysqlConnection = new TSMysqlConnection();
      tsMysqlConnection.connection.Open();
      this.savePetDB(tsMysqlConnection.connection, true);
      tsMysqlConnection.connection.Close();
    }

    public byte[] sendInfo()
    {
      PacketCreator packetCreator = new PacketCreator();
      packetCreator.addByte(this.slot);
      packetCreator.add16(this.NPCid);
      packetCreator.add32(this.totalxp);
      packetCreator.addByte(this.level);
      packetCreator.add16((ushort) this.hp);
      packetCreator.add16((ushort) this.sp);
      packetCreator.add16((ushort) this.mag);
      packetCreator.add16((ushort) this.atk);
      packetCreator.add16((ushort) this.def);
      packetCreator.add16((ushort) this.agi);
      packetCreator.add16((ushort) this.hpx);
      packetCreator.add16((ushort) this.spx);
      packetCreator.addByte((byte) 0);
      packetCreator.addByte(this.fai);
      packetCreator.addByte((byte) 1);
      packetCreator.add16((ushort) this.skill_pt);
      packetCreator.addByte((byte) this.name.Length);
      packetCreator.addBytes(this.name);
      packetCreator.addByte(this.skill1_lvl);
      packetCreator.addByte(this.skill2_lvl);
      packetCreator.addByte(this.skill3_lvl);
      for (int index = 0; index < 6; ++index)
      {
        if (this.equipment[index] != null)
        {
          packetCreator.add16(this.equipment[index].Itemid);
          packetCreator.addByte(this.equipment[index].duration);
          packetCreator.addZero(7);
        }
        else
          packetCreator.addZero(10);
      }
      packetCreator.addZero(6);
      return packetCreator.getData();
    }

    public void addEquipBonus(ushort prop, int prop_val, int type)
    {
      int num = type == 0 ? prop_val : -prop_val;
      switch (prop)
      {
        case 207:
          this.hp2 += num;
          break;
        case 208:
          this.sp2 += num;
          break;
        case 210:
          this.atk2 += num;
          break;
        case 211:
          this.def2 += num;
          break;
        case 212:
          this.mag2 += num;
          break;
        case 214:
          this.agi2 += num;
          break;
      }
    }

    public void refreshPet()
    {
      this.refreshBonus();
      this.refresh(this.hp, (byte) 25);
      this.refresh(this.sp, (byte) 26);
    }

    public void refreshBonus()
    {
      this.refresh(this.mag2, (byte) 212);
      this.refresh(this.atk2, (byte) 210);
      this.refresh(this.def2, (byte) 211);
      this.refresh(this.hp2, (byte) 207);
      this.refresh(this.sp2, (byte) 208);
      this.refresh(this.agi2, (byte) 214);
    }

    public void refresh(int prop, byte prop_code)
    {
      PacketCreator packetCreator = new PacketCreator((byte) 8, (byte) 2);
      packetCreator.addByte((byte) 4);
      packetCreator.addByte(this.slot);
      packetCreator.addByte((byte) 0);
      packetCreator.addByte(prop_code);
      if (prop >= 0)
      {
        packetCreator.addByte((byte) 1);
        packetCreator.add32((uint) prop);
      }
      else
      {
        packetCreator.addByte((byte) 2);
        packetCreator.add32((uint) -prop);
      }
      packetCreator.add32(0U);
      this.owner.reply(packetCreator.send());
    }

    public void refreshFull(byte prop_code, int prop1, int prop2)
    {
      PacketCreator packetCreator = new PacketCreator((byte) 8, (byte) 2);
      packetCreator.addByte((byte) 4);
      packetCreator.addByte(this.slot);
      packetCreator.addByte((byte) 0);
      packetCreator.addByte(prop_code);
      if (prop1 >= 0)
      {
        packetCreator.addByte((byte) 1);
        packetCreator.add32((uint) prop1);
      }
      else
      {
        packetCreator.addByte((byte) 2);
        packetCreator.add32((uint) -prop1);
      }
      packetCreator.add32((uint) prop2);
      this.owner.reply(packetCreator.send());
    }

    public void sendNewPet()
    {
      this.refresh(this.hp, (byte) 25);
      this.refresh(this.sp, (byte) 26);
      PacketCreator packetCreator1 = new PacketCreator((byte) 15, (byte) 1);
      packetCreator1.add32(this.owner.client.accID);
      packetCreator1.addByte(this.slot);
      packetCreator1.add16(this.NPCid);
      packetCreator1.add16((ushort) 0);
      packetCreator1.addByte((byte) 1);
      this.owner.reply(packetCreator1.send());
      PacketCreator packetCreator2 = new PacketCreator((byte) 15, (byte) 7);
      packetCreator2.add32((uint) this.NPCid);
      packetCreator2.addByte(this.slot);
      packetCreator2.add16(this.NPCid);
      packetCreator2.addZero(7);
      packetCreator2.addByte((byte) 1);
      packetCreator2.addByte((byte) this.name.Length);
      packetCreator2.addBytes(this.name);
      this.owner.reply(packetCreator2.send());
    }

    public void savePetDB(MySqlConnection conn, bool newPet)
    {
      MySqlCommand mySqlCommand = new MySqlCommand();
      mySqlCommand.Connection = conn;
      TSMysqlConnection tsMysqlConnection = new TSMysqlConnection();
      if (!newPet)
        mySqlCommand.CommandText = "UPDATE pet SET name = @name , charid = @charid , npcid = @npcid, level = @level , exp = @curr_exp, exp_tot = @exp_tot , hp = @hp , sp = @sp , mag = @mag , atk = @atk , def = @def , hpx = @hpx , spx = @spx , agi = @agi , sk_point = @sk_point , fai = @fai , slot = @slot , location = @location , sk1_lvl = @sk1_lvl , sk2_lvl = @sk2_lvl , sk3_lvl = @sk3_lvl , sk4_lvl = @sk4_lvl, equip = @equip WHERE pet_sid = @pet_sid";
      else
        mySqlCommand.CommandText = "INSERT INTO pet (name , charid , npcid, hp , sp , mag , atk , def , hpx , spx , agi , fai , slot , location)  VALUES (@name , @charid , @npcid, @hp , @sp , @mag , @atk , @def , @hpx , @spx , @agi , @fai , @slot , @location)";
      mySqlCommand.Prepare();
      mySqlCommand.Parameters.AddWithValue("@name", (object) this.name);
      mySqlCommand.Parameters.AddWithValue("@charid", (object) this.owner.charId);
      mySqlCommand.Parameters.AddWithValue("@npcid", (object) this.NPCid);
      mySqlCommand.Parameters.AddWithValue("@hp", (object) this.hp);
      mySqlCommand.Parameters.AddWithValue("@sp", (object) this.sp);
      mySqlCommand.Parameters.AddWithValue("@mag", (object) this.mag);
      mySqlCommand.Parameters.AddWithValue("@atk", (object) this.atk);
      mySqlCommand.Parameters.AddWithValue("@def", (object) this.def);
      mySqlCommand.Parameters.AddWithValue("@hpx", (object) this.hpx);
      mySqlCommand.Parameters.AddWithValue("@spx", (object) this.spx);
      mySqlCommand.Parameters.AddWithValue("@agi", (object) this.agi);
      mySqlCommand.Parameters.AddWithValue("@fai", (object) this.fai);
      mySqlCommand.Parameters.AddWithValue("@slot", (object) this.slot);
      mySqlCommand.Parameters.AddWithValue("@location", (object) this.location);
      if (!newPet)
      {
        mySqlCommand.Parameters.AddWithValue("@level", (object) this.level);
        mySqlCommand.Parameters.AddWithValue("@curr_exp", (object) this.currentxp);
        mySqlCommand.Parameters.AddWithValue("@exp_tot", (object) this.totalxp);
        mySqlCommand.Parameters.AddWithValue("@sk_point", (object) this.skill_pt);
        mySqlCommand.Parameters.AddWithValue("@sk1_lvl", (object) this.skill1_lvl);
        mySqlCommand.Parameters.AddWithValue("@sk2_lvl", (object) this.skill2_lvl);
        mySqlCommand.Parameters.AddWithValue("@sk3_lvl", (object) this.skill3_lvl);
        mySqlCommand.Parameters.AddWithValue("@sk4_lvl", (object) this.skill4_lvl);
        mySqlCommand.Parameters.AddWithValue("@equip", (object) this.saveEquipment());
        mySqlCommand.Parameters.AddWithValue("@pet_sid", (object) this.pet_sid);
      }
      mySqlCommand.ExecuteNonQuery();
      if (!newPet)
        return;
      this.pet_sid = Convert.ToInt32(new MySqlCommand("SELECT LAST_INSERT_ID()", mySqlCommand.Connection).ExecuteScalar());
    }

    public byte[] saveEquipment()
    {
      byte[] data = new byte[100];
      int pos = 0;
      for (int index = 0; index < 6; ++index)
      {
        if (this.equipment[index] != null)
          this.equipment[index].generateEquipBinary(ref data, ref pos);
      }
      return data;
    }

    public void loadEquipment(byte[] data)
    {
      for (int index = 0; index < data.Length && data[index] != (byte) 0; index += 7)
      {
        ushort num = (ushort) ((uint) data[index + 1] + ((uint) data[index + 2] << 8));
        this.equipment[(int) data[index] - 1] = new TSEquipment((TSItemContainer) null, num, data[index], (byte) 1);
        this.equipment[(int) data[index] - 1].equip.duration = data[index + 3];
        this.equipment[(int) data[index] - 1].equip.elem_type = data[index + 4];
        this.equipment[(int) data[index] - 1].equip.elem_val = (int) data[index + 5] + ((int) data[index + 6] << 8);
        this.equipment[(int) data[index] - 1].pet_owner = this;
        this.addEquipBonus(ItemData.itemList[num].prop1, ItemData.itemList[num].prop1_val, 0);
        this.addEquipBonus(ItemData.itemList[num].prop2, ItemData.itemList[num].prop2_val, 0);
      }
    }

    public void setHp(int amount)
    {
      this.hp += amount;
      if (this.hp > this.hp_max)
        this.hp = this.hp_max;
      if (this.hp > 0)
        return;
      if (this.owner.client.battle != null)
        this.hp = 0;
      else
        this.hp = 1;
    }

    public void setSp(int amount)
    {
      this.sp += amount;
      if (this.sp > this.sp_max)
        this.sp = this.sp_max;
      if (this.sp >= 0)
        return;
      this.sp = 0;
    }

    public void setFai(int amount)
    {
      if (this.fai == (byte) 100)
        return;
      this.fai += (byte) amount;
    }

    public int getHpMax() => this.reborn <= (byte) 1 ? (int) Math.Round((Math.Pow((double) this.level, 0.35) + 1.0) * (double) this.hpx * 2.0 + 80.0 + (double) this.level) : (int) Math.Round((Math.Pow((double) this.level, 0.35) + 2.0) * (double) this.hpx * 2.0 + 180.0 + (double) this.level);

    public int getSpMax() => this.reborn <= (byte) 1 ? (int) Math.Round(Math.Pow((double) this.level, 0.25) * (double) this.spx * 2.0 + 60.0 + (double) this.level) : (int) Math.Round(Math.Pow((double) this.level, 0.25) * (double) this.spx * 2.0 + 110.0 + (double) this.level);

    public void setExp(int amount)
    {
      if (this.level >= (byte) 250)
        return;
      this.totalxp = (uint) ((ulong) this.totalxp + (ulong) amount);
      this.currentxp += amount;
      if (amount > 0)
      {
        for (int index = (int) (Math.Pow((double) ((int) this.level + 1), this.xp_pow) + 5.0); this.currentxp >= index; index = (int) (Math.Pow((double) ((int) this.level + 1), this.xp_pow) + 5.0))
        {
          this.currentxp -= index;
          this.levelUp();
        }
      }
      else if (this.currentxp < 0)
        this.currentxp = 0;
      this.refresh((int) this.totalxp, (byte) 36);
    }

    public void levelUp()
    {
      ++this.level;
      ++this.skill_pt;
      this.setFai(1);
      this.hp_max = this.getHpMax();
      this.sp_max = this.getSpMax();
      this.hp = this.hp_max;
      this.sp = this.sp_max;
      this.refresh(this.hp, (byte) 25);
      this.refresh(this.sp, (byte) 26);
      this.refresh((int) this.level, (byte) 35);
      this.refresh(this.skill_pt, (byte) 37);
      this.getSttPoint();
    }

    public void getSttPoint()
    {
      int num = RandomGen.getInt(0, this.mag + this.atk + this.def + this.hpx + this.spx + this.agi);
      if (num < this.mag)
      {
        ++this.mag;
        this.refresh(this.mag, (byte) 27);
      }
      else if (num < this.mag + this.atk)
      {
        ++this.atk;
        this.refresh(this.atk, (byte) 28);
      }
      else if (num < this.mag + this.atk + this.def)
      {
        ++this.def;
        this.refresh(this.def, (byte) 29);
      }
      else if (num < this.mag + this.atk + this.def + this.hpx)
      {
        ++this.hpx;
        this.refresh(this.hpx, (byte) 31);
        this.hp_max = this.getHpMax();
      }
      else if (num < this.mag + this.atk + this.def + this.hpx + this.spx)
      {
        ++this.spx;
        this.refresh(this.spx, (byte) 32);
        this.sp_max = this.getSpMax();
      }
      else
      {
        ++this.agi;
        this.refresh(this.agi, (byte) 30);
      }
    }

    public void setSkill(ushort skillid, byte sk_lvl)
    {
      if ((int) skillid == (int) NpcData.npcList[this.NPCid].skill1)
      {
        if ((int) sk_lvl - (int) this.skill1_lvl > this.skill_pt)
          return;
        this.skill_pt -= (int) sk_lvl - (int) this.skill1_lvl;
        this.skill1_lvl = sk_lvl;
      }
      else if ((int) skillid == (int) NpcData.npcList[this.NPCid].skill2)
      {
        if ((int) sk_lvl - (int) this.skill2_lvl > this.skill_pt)
          return;
        this.skill_pt -= (int) sk_lvl - (int) this.skill2_lvl;
        this.skill2_lvl = sk_lvl;
      }
      else if ((int) skillid == (int) NpcData.npcList[this.NPCid].skill3)
      {
        if ((int) sk_lvl - (int) this.skill3_lvl > this.skill_pt)
          return;
        this.skill_pt -= (int) sk_lvl - (int) this.skill3_lvl;
        this.skill3_lvl = sk_lvl;
      }
      else
      {
        if ((int) skillid != (int) NpcData.npcList[this.NPCid].skill4 || (int) sk_lvl - (int) this.skill4_lvl > this.skill_pt)
          return;
        this.skill_pt -= (int) sk_lvl - (int) this.skill4_lvl;
        this.skill4_lvl = sk_lvl;
      }
      this.refresh(this.skill_pt, (byte) 37);
      this.refreshFull((byte) 110, (int) sk_lvl, (int) skillid);
    }
  }
}
