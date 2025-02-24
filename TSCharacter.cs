// Decompiled with JetBrains decompiler
// Type: TS_Server.Client.TSCharacter
// Assembly: TS_Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F406604-6D74-4A33-BF41-C1841FC4C8A8
// Assembly location: C:\Users\anumart.c.BCONNEX\Desktop\TestGraph\Database\ถอดดาต้าTS\TS_Server.exe

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using TS_Server.DataTools;
using TS_Server.Server;

namespace TS_Server.Client
{
  public class TSCharacter
  {
    public byte orient;
    public ushort horseID = 0;
    public TSParty party;
    public int agi;
    public int agi2;
    public int atk;
    public int atk2;
    public TSItemContainer bag;
    public TSItemContainer storage;
    public int charId;
    public TSClient client;
    public uint color1;
    public uint color2;
    public int def;
    public int def2;
    public byte element;
    public TSEquipment[] equipment;
    public byte face;
    public byte ghost;
    public byte god;
    public uint gold;
    public uint gold_bank;
    public byte hair;
    public uint honor;
    public int hp;
    public int hp2;
    public int hp_max;
    public int hpx;
    public TSItemContainer inventory;
    public byte job;
    public byte level;
    public int mag;
    public int mag2;
    public ushort mapID;
    public ushort mapX;
    public ushort mapY;
    public byte[] name;
    public byte nb_equips;
    public int next_item;
    public int next_pet;
    public TSPet[] pet;
    public sbyte pet_battle;
    public TSPet[] pet_car;
    public TSPet[] pet_inn;
    public byte rb;
    public byte sex;
    public Dictionary<ushort, byte> skill;
    public int skill_point;
    public int sp;
    public int sp2;
    public int sp_max;
    public int spx;
    public int stt_point;
    public byte style;
    public int currentxp;
    public uint totalxp;
    public double xp_pow;
    public ushort[] hotkey;
    public byte ball_point;
    public bool[] ballList;
    public ushort[] skill_rb2;
    public ushort outfitId = 0;

    public TSCharacter(TSClient c)
    {
      this.client = c;
      this.pet = new TSPet[4];
      this.next_pet = 0;
      this.pet_battle = (sbyte) -1;
      this.equipment = new TSEquipment[6];
      this.inventory = new TSItemContainer(this, (byte) 25);
      this.bag = new TSItemContainer(this, (byte) 25);
      this.storage = new TSItemContainer(this, (byte) 50);
      this.hotkey = new ushort[10];
      this.ballList = new bool[12];
      this.skill_rb2 = new ushort[8];
      this.next_item = 0;
      this.nb_equips = (byte) 0;
      this.skill = new Dictionary<ushort, byte>();
    }

    public void loadCharDB()
    {
      TSMysqlConnection tsMysqlConnection = new TSMysqlConnection();
      MySqlDataReader mySqlDataReader = tsMysqlConnection.selectQuery("SELECT * FROM chars WHERE accountid = " + (object) this.client.accID);
      mySqlDataReader.Read();
      this.charId = mySqlDataReader.GetInt32("id");
      this.level = mySqlDataReader.GetByte("level");
      this.hp = mySqlDataReader.GetInt32("hp");
      this.hp = Math.Max(1, this.hp);
      this.sp = mySqlDataReader.GetInt32("sp");
      this.mag = mySqlDataReader.GetInt32("mag");
      this.atk = mySqlDataReader.GetInt32("atk");
      this.def = mySqlDataReader.GetInt32("def");
      this.hpx = mySqlDataReader.GetInt32("hpx");
      this.spx = mySqlDataReader.GetInt32("spx");
      this.agi = mySqlDataReader.GetInt32("agi");
      this.hp2 = mySqlDataReader.GetInt32("hp2");
      this.sp2 = mySqlDataReader.GetInt32("sp2");
      this.mag2 = mySqlDataReader.GetInt32("mag2");
      this.atk2 = mySqlDataReader.GetInt32("atk2");
      this.def2 = mySqlDataReader.GetInt32("def2");
      this.agi2 = mySqlDataReader.GetInt32("agi2");
      this.skill_point = mySqlDataReader.GetInt32("sk_point");
      this.stt_point = mySqlDataReader.GetInt32("stt_point");
      this.sex = mySqlDataReader.GetByte("sex");
      this.ghost = mySqlDataReader.GetByte("ghost");
      this.god = mySqlDataReader.GetByte("god");
      this.style = mySqlDataReader.GetByte("style");
      this.hair = mySqlDataReader.GetByte("hair");
      this.face = mySqlDataReader.GetByte("face");
      this.color1 = mySqlDataReader.GetUInt32("color1");
      this.color2 = mySqlDataReader.GetUInt32("color2");
      this.mapID = mySqlDataReader.GetUInt16("map_id");
      this.mapX = mySqlDataReader.GetUInt16("map_x");
      this.mapY = mySqlDataReader.GetUInt16("map_y");
      this.currentxp = mySqlDataReader.GetInt32("exp");
      this.totalxp = mySqlDataReader.GetUInt32("exp_tot");
      this.honor = mySqlDataReader.GetUInt32("honor");
      this.element = mySqlDataReader.GetByte("element");
      this.rb = mySqlDataReader.GetByte("reborn");
      this.job = mySqlDataReader.GetByte("job");
      this.gold = mySqlDataReader.GetUInt32("gold");
      this.gold_bank = mySqlDataReader.GetUInt32("gold_bank");
      this.name = (byte[]) mySqlDataReader["name"];
      this.loadEquipment((byte[]) mySqlDataReader["equip"]);
      this.inventory.loadContainer((byte[]) mySqlDataReader["inventory"]);
      this.storage.loadContainer((byte[]) mySqlDataReader["storage"]);
      this.bag.loadContainer((byte[]) mySqlDataReader["bag"]);
      this.loadSkill((byte[]) mySqlDataReader["skill"]);
      this.loadHotkey((byte[]) mySqlDataReader["hotkey"]);
      this.pet_battle = (sbyte) mySqlDataReader.GetByte("pet_battle");
      mySqlDataReader.Close();
      tsMysqlConnection.connection.Close();
      this.hp_max = this.getHpMax();
      this.sp_max = this.getSpMax();
      this.xp_pow = this.rb == (byte) 0 ? 2.9 : (this.rb == (byte) 1 ? 3.0 : 3.05);
      if (!this.skill.ContainsKey((ushort) 14001))
        this.skill.Add((ushort) 14001, (byte) 1);
      if (!this.skill.ContainsKey((ushort) 14015))
        this.skill.Add((ushort) 14015, (byte) 10);
      if (!this.skill.ContainsKey((ushort) 14021))
        this.skill.Add((ushort) 14021, (byte) 5);
      if (!this.skill.ContainsKey((ushort) 14023))
        this.skill.Add((ushort) 14023, (byte) 1);
      this.skill_point = 0;
      this.loadPet();
    }

    public void loadPet()
    {
      TSMysqlConnection tsMysqlConnection = new TSMysqlConnection();
      MySqlDataReader mySqlDataReader = tsMysqlConnection.selectQuery("SELECT pet_sid, slot, location FROM pet WHERE charid = " + (object) this.charId);
      while (mySqlDataReader.Read())
      {
        int int32_1 = mySqlDataReader.GetInt32("slot");
        int int32_2 = mySqlDataReader.GetInt32("pet_sid");
        this.pet[int32_1 - 1] = new TSPet(this, int32_2, (byte) int32_1);
        this.pet[int32_1 - 1].loadPetDB();
      }
      mySqlDataReader.Close();
      tsMysqlConnection.connection.Close();
      while (this.next_pet < 4 && this.pet[this.next_pet] != null)
        ++this.next_pet;
    }

    public void initChar(byte[] data, byte[] name)
    {
      string str1 = PacketReader.readString(data, 22, (int) data[21]);
      string str2 = PacketReader.readString(data, 22 + str1.Length + 1, (int) data[22 + str1.Length]);
      TSMysqlConnection tsMysqlConnection = new TSMysqlConnection();
      tsMysqlConnection.updateQuery("UPDATE account SET password = '" + str1 + "', password2 = '" + str2 + "' WHERE id = " + (object) this.client.accID);
      tsMysqlConnection.connection.Open();
      MySqlCommand mySqlCommand = new MySqlCommand();
      mySqlCommand.Connection = tsMysqlConnection.connection;
      mySqlCommand.CommandText = "INSERT INTO chars (accountid, name, mag, atk, def, hpx, spx, agi, sex, style, hair, face, color1, color2, element) VALUES (" + (object) this.client.accID + ", @name ," + (object) data[15] + "," + (object) data[16] + "," + (object) data[17] + "," + (object) data[18] + "," + (object) data[19] + "," + (object) data[20] + "," + (object) data[2] + "," + (object) data[3] + "," + (object) data[4] + "," + (object) data[5] + "," + (object) PacketReader.read32(data, 6) + "," + (object) PacketReader.read32(data, 10) + "," + (object) data[14] + ");";
      mySqlCommand.Prepare();
      mySqlCommand.Parameters.AddWithValue("@name", (object) name);
      mySqlCommand.ExecuteNonQuery();
      tsMysqlConnection.connection.Close();
      this.charId = tsMysqlConnection.getLastId("chars");
    }

    public void loginChar()
    {
      this.loadCharDB();
      this.addSummonSkill((byte) 10);
      this.client.online = true;
      this.refreshChr();
      this.reply(new PacketCreator(new byte[2]
      {
        (byte) 20,
        (byte) 8
      }).send());
      this.reply(new PacketCreator(new byte[3]
      {
        (byte) 20,
        (byte) 33,
        (byte) 0
      }).send());
      this.sendLook(false);
      this.sendInfo();
      this.sendPetInfo();
      this.reply(new PacketCreator(new byte[4]
      {
        (byte) 33,
        (byte) 2,
        (byte) 0,
        (byte) 0
      }).send());
      this.inventory.sendItems((byte) 23, (byte) 5);
      this.bag.sendItems((byte) 23, (byte) 47);
      this.storage.sendItems((byte) 30, (byte) 1);
      this.sendEquip();
      this.client.UImportant();
      this.client.AllowMove();
      this.sendGold();
      this.announce("สวัสดีจร้า");
      this.sendHotkey();
      this.sendVoucher();
      this.refreshFull((byte) 111, 1, 1);
      TSServer.getInstance().addPlayer(this.client);
      this.sendUpdateTeam();
    }

    public void addPet(ushort npcid, int bonus)
    {
      Console.WriteLine(this.next_pet.ToString() + " " + (object) npcid);
      for (int index = 0; index < this.next_pet; ++index)
      {
        if ((int) this.pet[index].NPCid == (int) npcid)
          return;
      }
      if (this.next_pet >= 4 || !NpcData.npcList.ContainsKey(npcid))
        return;
      this.pet[this.next_pet] = new TSPet(this, (byte) (this.next_pet + 1));
      this.pet[this.next_pet].initPet(NpcData.npcList[npcid]);
      Console.WriteLine("Pet id " + (object) npcid + ", sid " + (object) this.pet[this.next_pet].pet_sid + " added in slot " + (object) (this.next_pet + 1));
      this.pet[this.next_pet].sendNewPet();
      for (int index = 0; index < bonus; ++index)
        this.pet[this.next_pet].getSttPoint();
      this.nextPet();
    }

    public void changePetName(byte slot, byte[] newName)
    {
      if (this.pet[(int) slot - 1] == null)
        return;
      TSMysqlConnection tsMysqlConnection = new TSMysqlConnection();
      tsMysqlConnection.connection.Open();
      MySqlCommand mySqlCommand = new MySqlCommand();
      mySqlCommand.Connection = tsMysqlConnection.connection;
      mySqlCommand.CommandText = "UPDATE pet SET `name` = @name WHERE pet_sid=" + (object) this.pet[(int) slot - 1].pet_sid;
      mySqlCommand.Prepare();
      mySqlCommand.Parameters.AddWithValue("@name", (object) this.name);
      mySqlCommand.ExecuteNonQuery();
      tsMysqlConnection.connection.Close();
      this.pet[(int) slot - 1].name = newName;
      PacketCreator packetCreator = new PacketCreator((byte) 15, (byte) 9);
      packetCreator.add32(this.client.accID);
      packetCreator.add8(slot);
      packetCreator.addBytes(this.pet[(int) slot - 1].name);
      this.reply(packetCreator.send());
    }

    public void removePet(byte slot)
    {
      if (this.pet[(int) slot - 1] == null)
        return;
      new TSMysqlConnection().updateQuery("DELETE FROM pet WHERE pet_sid=" + (object) this.pet[(int) slot - 1].pet_sid);
      this.pet[(int) slot - 1] = (TSPet) null;
      if ((int) this.pet_battle == (int) slot - 1)
        this.pet_battle = (sbyte) -1;
      this.nextPet();
      PacketCreator packetCreator = new PacketCreator((byte) 15, (byte) 2);
      packetCreator.add32(this.client.accID);
      packetCreator.add8(slot);
      this.reply(packetCreator.send());
    }

    public void nextPet()
    {
      this.next_pet = 0;
      while (this.next_pet < 4 && this.pet[this.next_pet] != null)
        ++this.next_pet;
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

    public void sendLook(bool forReborn)
    {
      PacketCreator packetCreator = new PacketCreator((byte) 3);
      packetCreator.add32(this.client.accID);
      packetCreator.addByte(this.sex);
      packetCreator.addByte(this.ghost);
      packetCreator.addByte(this.god);
      packetCreator.add16(this.mapID);
      packetCreator.add16(this.mapX);
      packetCreator.add16(this.mapY);
      packetCreator.addByte(this.style);
      packetCreator.addByte(this.hair);
      packetCreator.addByte(this.face);
      packetCreator.add32(this.color1);
      packetCreator.add32(this.color2);
      packetCreator.addByte(this.nb_equips);
      for (int index = 0; index < 6; ++index)
      {
        if (this.equipment[index] != null)
          packetCreator.add16(this.equipment[index].Itemid);
      }
      packetCreator.add32(0U);
      packetCreator.addByte((byte) 5);
      packetCreator.addByte(this.rb);
      packetCreator.addByte(this.job);
      if (!forReborn)
        packetCreator.addBytes(this.name);
      this.reply(packetCreator.send());
    }

    public byte[] sendLookForOther()
    {
      PacketCreator packetCreator = new PacketCreator((byte) 3);
      packetCreator.add32(this.client.accID);
      packetCreator.addByte(this.sex);
      packetCreator.addByte(this.element);
      packetCreator.addByte(this.level);
      packetCreator.addByte(this.ghost);
      packetCreator.addByte(this.god);
      packetCreator.add16(this.mapID);
      packetCreator.add16(this.mapX);
      packetCreator.add16(this.mapY);
      packetCreator.addByte(this.style);
      packetCreator.addByte(this.hair);
      packetCreator.addByte(this.face);
      packetCreator.add32(this.color1);
      packetCreator.add32(this.color2);
      packetCreator.addByte(this.nb_equips);
      for (int index = 0; index < 6; ++index)
      {
        if (this.equipment[index] != null)
          packetCreator.add16(this.equipment[index].Itemid);
      }
      packetCreator.add32(0U);
      packetCreator.add16((ushort) 0);
      packetCreator.addByte(this.rb);
      packetCreator.addByte(this.job);
      packetCreator.addBytes(this.name);
      return packetCreator.send();
    }

    public byte[] setExpress(byte expressType, byte expressCode)
    {
      PacketCreator packetCreator = new PacketCreator((byte) 32);
      packetCreator.add8(expressType);
      packetCreator.add32(this.client.accID);
      packetCreator.add8(expressCode);
      return packetCreator.send();
    }

    public void sendInfo()
    {
      PacketCreator packetCreator = new PacketCreator((byte) 5, (byte) 3);
      packetCreator.addByte(this.element);
      packetCreator.add16((ushort) this.hp);
      packetCreator.add16((ushort) this.sp);
      packetCreator.add16((ushort) this.mag);
      packetCreator.add16((ushort) this.atk);
      packetCreator.add16((ushort) this.def);
      packetCreator.add16((ushort) this.agi);
      packetCreator.add16((ushort) this.hpx);
      packetCreator.add16((ushort) this.spx);
      packetCreator.addByte(this.level);
      packetCreator.add32(this.totalxp);
      packetCreator.add16((ushort) this.skill_point);
      packetCreator.add16((ushort) this.stt_point);
      packetCreator.add32(this.honor);
      packetCreator.add16((ushort) this.hp_max);
      packetCreator.add16((ushort) this.sp_max);
      packetCreator.add32((uint) this.atk2);
      packetCreator.add32((uint) this.def2);
      packetCreator.add32((uint) this.mag2);
      packetCreator.add32((uint) this.agi2);
      packetCreator.add32((uint) this.hp2);
      packetCreator.add32((uint) this.sp2);
      packetCreator.add16((ushort) 500);
      packetCreator.add16((ushort) 500);
      packetCreator.add16((ushort) 500);
      packetCreator.add16((ushort) 500);
      packetCreator.add16((ushort) 500);
      packetCreator.addZero(43);
      foreach (ushort key in this.skill.Keys)
      {
        packetCreator.add16(key);
        packetCreator.addByte(this.skill[key]);
      }
      this.reply(packetCreator.send());
      if (this.rb != (byte) 2)
        return;
      this.sendBallList();
    }

    public void sendBallList()
    {
      PacketCreator packetCreator1 = new PacketCreator((byte) 23, (byte) 77);
      packetCreator1.add8(this.ball_point);
      for (int index = 0; index < 12; ++index)
      {
        if (this.ballList[index])
          packetCreator1.add8((byte) (index + 1));
      }
      this.reply(packetCreator1.send());
      PacketCreator packetCreator2 = new PacketCreator((byte) 23, (byte) 78);
      for (int index = 0; index < 8; ++index)
      {
        if (this.skill_rb2[index] != (ushort) 0)
        {
          packetCreator2.add8((byte) (index + 1));
          packetCreator2.add16(this.skill_rb2[index]);
        }
      }
      this.reply(packetCreator2.send());
    }

    public void sendUpdateTeam()
    {
      if (this.isTeamLeader())
      {
        PacketCreator packetCreator = new PacketCreator((byte) 13);
        packetCreator.add8((byte) 6);
        packetCreator.add32(this.client.accID);
        packetCreator.add8((byte) (this.party.member.Count - 1));
        foreach (TSCharacter tsCharacter in this.party.member)
        {
          tsCharacter.refreshTeam();
          if ((long) tsCharacter.client.accID != (long) this.party.leader_id)
            packetCreator.add32(tsCharacter.client.accID);
        }
        this.replyToMap(packetCreator.send(), true);
      }
      PacketCreator packetCreator1 = new PacketCreator((byte) 15);
      packetCreator1.add8((byte) 7);
      packetCreator1.add32(this.client.accID);
      if (this.pet != null)
      {
        for (int index = 0; index < this.pet.Length; ++index)
        {
          if (this.pet[index] != null)
          {
            packetCreator1.addByte((byte) (index + 1));
            packetCreator1.add16(this.pet[index].NPCid);
            packetCreator1.addZero(7);
            packetCreator1.add8((byte) 1);
            packetCreator1.addByte((byte) this.pet[index].name.Length);
            packetCreator1.addBytes(this.pet[index].name);
          }
        }
      }
      this.replyToMap(packetCreator1.send(), false);
      if (this.horseID > (ushort) 0)
        this.rideHorse(true, this.horseID);
      else
        this.rideHorse(false);
    }

    public void sendPetInfo()
    {
      PacketCreator packetCreator1 = new PacketCreator((byte) 15, (byte) 8);
      for (int index = 0; index < this.pet.Length; ++index)
      {
        if (this.pet[index] != null)
          packetCreator1.addBytes(this.pet[index].sendInfo());
      }
      this.reply(packetCreator1.send());
      this.reply(new PacketCreator(new byte[5]
      {
        (byte) 15,
        (byte) 20,
        (byte) 1,
        (byte) 0,
        (byte) 0
      }).send());
      this.reply(new PacketCreator(new byte[5]
      {
        (byte) 15,
        (byte) 20,
        (byte) 2,
        (byte) 0,
        (byte) 0
      }).send());
      this.reply(new PacketCreator(new byte[5]
      {
        (byte) 15,
        (byte) 20,
        (byte) 3,
        (byte) 0,
        (byte) 0
      }).send());
      this.reply(new PacketCreator(new byte[5]
      {
        (byte) 15,
        (byte) 20,
        (byte) 4,
        (byte) 0,
        (byte) 0
      }).send());
      this.reply(new PacketCreator(new byte[2]
      {
        (byte) 15,
        (byte) 10
      }).send());
      this.reply(new PacketCreator(new byte[14]
      {
        (byte) 15,
        (byte) 18,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 0,
        (byte) 0,
        (byte) 3,
        (byte) 0,
        (byte) 0,
        (byte) 4,
        (byte) 0,
        (byte) 0
      }).send());
      this.reply(new PacketCreator(new byte[4]
      {
        (byte) 15,
        (byte) 19,
        (byte) 1,
        (byte) 0
      }).send());
      if (this.pet_battle != (sbyte) -1)
      {
        PacketCreator packetCreator2 = new PacketCreator((byte) 19);
        packetCreator2.addByte((byte) 1);
        packetCreator2.add16(this.pet[(int) this.pet_battle].NPCid);
        packetCreator2.add16((ushort) 0);
        this.reply(packetCreator2.send());
      }
      if (this.pet == null)
        return;
      for (int index = 0; index < this.pet.Length; ++index)
      {
        if (this.pet[index] != null)
          this.pet[index].refreshPet();
      }
    }

    public void sendEquip()
    {
      PacketCreator packetCreator = new PacketCreator((byte) 23, (byte) 11);
      for (int index = 0; index < 6; ++index)
      {
        if (this.equipment[index] != null)
        {
          packetCreator.add16(this.equipment[index].Itemid);
          packetCreator.addByte(this.equipment[index].duration);
          packetCreator.addZero(7);
        }
      }
      this.reply(packetCreator.send());
    }

    public void sendGold()
    {
      PacketCreator packetCreator = new PacketCreator((byte) 26, (byte) 4);
      packetCreator.add32(this.gold);
      packetCreator.add32(this.gold_bank);
      this.reply(packetCreator.send());
    }

    public void sendHotkey()
    {
      PacketCreator packetCreator = new PacketCreator((byte) 40, (byte) 1);
      for (byte b = 1; b <= (byte) 10; ++b)
      {
        if (this.hotkey[(int) b - 1] != (ushort) 0)
        {
          packetCreator.add8((byte) 2);
          packetCreator.add16(this.hotkey[(int) b - 1]);
          packetCreator.add8(b);
        }
      }
      this.reply(packetCreator.send());
    }

    public void sendVoucher()
    {
      PacketCreator packetCreator = new PacketCreator((byte) 35, (byte) 4);
      packetCreator.add32(999999U);
      packetCreator.addZero(12);
      this.reply(packetCreator.send());
    }

    public void refreshChr()
    {
      this.refresh(this.hpx, (byte) 31);
      this.refresh(this.spx, (byte) 32);
      this.refresh(this.atk, (byte) 28);
      this.refresh(this.def, (byte) 29);
      this.refresh(this.mag, (byte) 27);
      this.refresh(this.agi, (byte) 30);
      this.refresh(this.hp, (byte) 25);
      this.refresh(this.sp, (byte) 26);
      this.refreshBonus();
    }

    public void showOutfit()
    {
      if (!NpcData.npcList.ContainsKey(this.outfitId))
        return;
      PacketCreator packetCreator = new PacketCreator((byte) 5, (byte) 5);
      packetCreator.add32(this.client.accID);
      packetCreator.add16(this.outfitId);
      this.replyToMap(packetCreator.send(), true);
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

    public void refresh(int prop, byte prop_code, bool team = false)
    {
      PacketCreator packetCreator = new PacketCreator((byte) 8);
      if (this.party != null && team)
      {
        packetCreator.addByte((byte) 3);
        packetCreator.add32(this.client.accID);
      }
      else
        packetCreator.addByte((byte) 1);
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
      if (this.party != null && team)
        this.replyToTeam(packetCreator.send());
      else
        this.reply(packetCreator.send());
    }

    public void refreshTeam()
    {
      this.refresh(this.hpx, (byte) 31, true);
      this.refresh(this.spx, (byte) 32, true);
      this.refresh(this.atk, (byte) 28, true);
      this.refresh(this.def, (byte) 29, true);
      this.refresh(this.mag, (byte) 27, true);
      this.refresh(this.agi, (byte) 30, true);
      this.refresh(this.hp, (byte) 25, true);
      this.refresh(this.sp, (byte) 26, true);
      this.refresh(this.mag2, (byte) 212, true);
      this.refresh(this.atk2, (byte) 210, true);
      this.refresh(this.def2, (byte) 211, true);
      this.refresh(this.hp2, (byte) 207, true);
      this.refresh(this.sp2, (byte) 208, true);
      this.refresh(this.agi2, (byte) 214, true);
    }

    public void refreshFull(byte prop_code, int prop1, int prop2)
    {
      PacketCreator packetCreator = new PacketCreator((byte) 8, (byte) 1);
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
      this.reply(packetCreator.send());
    }

    public void announce(string msg)
    {
      PacketCreator packetCreator = new PacketCreator((byte) 2, (byte) 11);
      packetCreator.add32(0U);
      packetCreator.addString(msg);
      this.reply(packetCreator.send());
    }

    public void saveCharDB(MySqlConnection conn)
    {
      MySqlCommand mySqlCommand = new MySqlCommand();
      mySqlCommand.Connection = conn;
      mySqlCommand.CommandText = "UPDATE chars SET level = @level , exp = @curr_exp, exp_tot = @exp_tot , hp = @hp , sp = @sp , mag = @mag , atk = @atk,def = @def , hpx = @hpx , spx = @spx , agi = @agi , sk_point = @sk_point , stt_point = @stt_point,ghost = @ghost , god = @god , map_id = @map_id , map_x = @map_x , map_y = @map_y , gold = @gold , gold_bank = @gold_bank , honor = @honor , pet_battle = @pet_battle, equip = @equip, inventory = @inventory, bag = @bag, storage = @storage, skill = @skill, hotkey = @hotkey, reborn = @rb, job = @job WHERE accountid = @id";
      mySqlCommand.Prepare();
      mySqlCommand.Parameters.AddWithValue("@level", (object) this.level);
      mySqlCommand.Parameters.AddWithValue("@curr_exp", (object) this.currentxp);
      mySqlCommand.Parameters.AddWithValue("@exp_tot", (object) this.totalxp);
      mySqlCommand.Parameters.AddWithValue("@hp", (object) this.hp);
      mySqlCommand.Parameters.AddWithValue("@sp", (object) this.sp);
      mySqlCommand.Parameters.AddWithValue("@mag", (object) this.mag);
      mySqlCommand.Parameters.AddWithValue("@atk", (object) this.atk);
      mySqlCommand.Parameters.AddWithValue("@def", (object) this.def);
      mySqlCommand.Parameters.AddWithValue("@hpx", (object) this.hpx);
      mySqlCommand.Parameters.AddWithValue("@spx", (object) this.spx);
      mySqlCommand.Parameters.AddWithValue("@agi", (object) this.agi);
      mySqlCommand.Parameters.AddWithValue("@sk_point", (object) this.skill_point);
      mySqlCommand.Parameters.AddWithValue("@stt_point", (object) this.stt_point);
      mySqlCommand.Parameters.AddWithValue("@ghost", (object) this.ghost);
      mySqlCommand.Parameters.AddWithValue("@god", (object) this.god);
      mySqlCommand.Parameters.AddWithValue("@map_id", (object) this.mapID);
      mySqlCommand.Parameters.AddWithValue("@map_x", (object) this.mapX);
      mySqlCommand.Parameters.AddWithValue("@map_y", (object) this.mapY);
      mySqlCommand.Parameters.AddWithValue("@gold", (object) this.gold);
      mySqlCommand.Parameters.AddWithValue("@gold_bank", (object) this.gold_bank);
      mySqlCommand.Parameters.AddWithValue("@honor", (object) this.honor);
      mySqlCommand.Parameters.AddWithValue("@pet_battle", (object) this.pet_battle);
      mySqlCommand.Parameters.AddWithValue("@id", (object) this.client.accID);
      mySqlCommand.Parameters.AddWithValue("@equip", (object) this.saveEquipment());
      mySqlCommand.Parameters.AddWithValue("@inventory", (object) this.inventory.saveContainer());
      mySqlCommand.Parameters.AddWithValue("@bag", (object) this.bag.saveContainer());
      mySqlCommand.Parameters.AddWithValue("@storage", (object) this.storage.saveContainer());
      mySqlCommand.Parameters.AddWithValue("@skill", (object) this.saveSkill());
      mySqlCommand.Parameters.AddWithValue("@hotkey", (object) this.saveHotkey());
      mySqlCommand.Parameters.AddWithValue("@rb", (object) this.rb);
      mySqlCommand.Parameters.AddWithValue("@job", (object) this.job);
      mySqlCommand.ExecuteNonQuery();
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
        this.equipment[(int) data[index] - 1].char_owner = this;
        ++this.nb_equips;
        this.addEquipBonus(ItemData.itemList[num].prop1, ItemData.itemList[num].prop1_val, 0);
        this.addEquipBonus(ItemData.itemList[num].prop2, ItemData.itemList[num].prop2_val, 0);
      }
    }

    public byte[] saveSkill()
    {
      byte[] numArray = new byte[600];
      int index1 = 0;
      foreach (ushort key in this.skill.Keys)
      {
        numArray[index1] = (byte) key;
        numArray[index1 + 1] = (byte) ((uint) key >> 8);
        numArray[index1 + 2] = this.skill[key];
        index1 += 3;
      }
      if (this.rb == (byte) 2)
      {
        numArray[index1] = byte.MaxValue;
        numArray[index1 + 1] = byte.MaxValue;
        numArray[index1 + 2] = this.ball_point;
        int index2 = index1 + 3;
        for (int index3 = 0; index3 < 12; ++index3)
        {
          if (this.ballList[index3])
          {
            numArray[index2] = (byte) (index3 + 1);
            ++index2;
          }
        }
        numArray[index2] = byte.MaxValue;
        int index4 = index2 + 1;
        for (int index5 = 0; index5 < 8; ++index5)
        {
          if (this.skill_rb2[index5] != (ushort) 0)
          {
            numArray[index4] = (byte) (index5 + 6);
            numArray[index4 + 1] = (byte) this.skill_rb2[index5];
            numArray[index4 + 2] = (byte) ((uint) this.skill_rb2[index5] >> 8);
            index4 += 3;
          }
        }
      }
      return numArray;
    }

    public void loadSkill(byte[] data)
    {
      int index1 = 0;
      if (data.Length < 3)
        return;
      for (; index1 < data.Length; index1 += 3)
      {
        ushort key = (ushort) ((uint) data[index1] + ((uint) data[index1 + 1] << 8));
        if (key != (ushort) 0 && key != ushort.MaxValue)
        {
          this.skill.Add(key, data[index1 + 2]);
        }
        else
        {
          if (key != ushort.MaxValue)
            break;
          this.ball_point = data[index1 + 2];
          int index2;
          for (index2 = index1 + 3; data[index2] != byte.MaxValue && data[index2] != (byte) 0; ++index2)
            this.ballList[(int) data[index2] - 1] = true;
          for (int index3 = index2 + 1; data[index3] != (byte) 0; index3 += 3)
            this.skill_rb2[(int) data[index3] - 6] = PacketReader.read16(data, index3 + 1);
          break;
        }
      }
    }

    public byte[] saveHotkey()
    {
      byte[] numArray = new byte[30];
      int index1 = 0;
      for (byte index2 = 1; index2 <= (byte) 10; ++index2)
      {
        if (this.hotkey[(int) index2 - 1] != (ushort) 0)
        {
          numArray[index1] = index2;
          numArray[index1 + 1] = (byte) this.hotkey[(int) index2 - 1];
          numArray[index1 + 2] = (byte) ((uint) this.hotkey[(int) index2 - 1] >> 8);
          index1 += 3;
        }
      }
      return numArray;
    }

    public void loadHotkey(byte[] data)
    {
      for (int index = 0; index < data.Length && data[index] != (byte) 0; index += 3)
        this.hotkey[(int) data[index] - 1] = (ushort) ((uint) data[index + 1] + ((uint) data[index + 2] << 8));
    }

    public void reply(byte[] data)
    {
      if (!this.client.online)
        return;
      this.client.reply(data);
    }

    public void replyToMap(byte[] data, bool self) => this.client.map.BroadCast(this.client, data, self);

    public void replyToAll(byte[] data, bool self)
    {
      foreach (TSMap tsMap in TSWorld.getInstance().listMap.Values)
        tsMap.BroadCast(this.client, data, self);
    }

    public void replyToTeam(byte[] data)
    {
      foreach (TSCharacter tsCharacter in this.party.member)
        tsCharacter.reply(data);
    }

    public bool isTeamLeader() => this.party != null && (long) this.party.leader_id == (long) this.client.accID;

    public bool isJoinedTeam() => this.party != null;

    public void setHp(int amount)
    {
      this.hp += amount;
      if (this.hp > this.hp_max)
        this.hp = this.hp_max;
      if (this.hp > 0)
        return;
      if (this.client.battle != null)
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

    public int getHpMax()
    {
      if (this.rb == (byte) 0)
        return (int) Math.Round((Math.Pow((double) this.level, 0.35) + 1.0) * (double) this.hpx * 2.0 + 80.0 + (double) this.level);
      if (this.rb == (byte) 1)
        return (int) Math.Round((Math.Pow((double) this.level, 0.35) + 2.0) * (double) this.hpx * 2.0 + 180.0 + (double) this.level);
      if (this.job == (byte) 1)
        return (int) Math.Round((Math.Pow((double) this.level, 0.35) * 2.0 + 25.0) * (double) this.hpx + 280.0 + (double) this.level);
      if (this.job == (byte) 2)
        return (int) Math.Round((Math.Pow((double) this.level, 0.35) * 3.0 + 30.0) * (double) this.hpx + 380.0 + (double) this.level);
      return this.job == (byte) 3 ? (int) Math.Round((Math.Pow((double) this.level, 0.35) + 11.5) * (double) this.hpx * 2.0 + 180.0 + (double) this.level) : (int) Math.Round((Math.Pow((double) this.level, 0.35) + 10.5) * (double) this.hpx * 2.0 + 180.0 + (double) this.level);
    }

    public int getSpMax()
    {
      if (this.rb == (byte) 0)
        return (int) Math.Round(Math.Pow((double) this.level, 0.25) * (double) this.spx * 2.0 + 60.0 + (double) this.level);
      if (this.rb == (byte) 1)
        return (int) Math.Round(Math.Pow((double) this.level, 0.25) * (double) this.spx * 2.0 + 110.0 + (double) this.level);
      if (this.job == (byte) 1 || this.job == (byte) 2)
        return (int) Math.Round(Math.Pow((double) this.level, 0.25) * (double) this.spx * 2.0 + 160.0 + (double) this.level);
      return this.job == (byte) 3 ? (int) Math.Round(Math.Pow((double) this.level, 0.25) * (double) this.spx * 3.0 + 310.0 + (double) this.level) : (int) Math.Round(Math.Pow((double) this.level, 0.25) * (double) this.spx * 3.5 + 410.0 + (double) this.level);
    }

    public void setExp(int amount)
    {
      if (this.level >= (byte) 200)
        return;
      this.totalxp = (uint) ((ulong) this.totalxp + (ulong) amount);
      this.currentxp += amount;
      if (amount > 0)
      {
        for (int index = (int) (Math.Pow((double) ((int) this.level + 1), this.xp_pow) + 5.0); this.currentxp >= index; index = (int) (Math.Pow((double) ((int) this.level + 1), this.xp_pow) + 5.0))
        {
          this.currentxp -= index;
          if (this.level >= (byte) 200)
            return;
          this.levelUp();
        }
      }
      else if (this.currentxp < 0)
        this.currentxp = 0;
      this.refresh((int) this.totalxp, (byte) 36);
    }

    public void levelUp()
    {
      if (this.level >= (byte) 200)
        return;
      ++this.level;
      this.stt_point += 2;
      ++this.skill_point;
      this.hp_max = this.getHpMax();
      this.sp_max = this.getSpMax();
      this.hp = this.hp_max;
      this.sp = this.sp_max;
      this.refresh((int) this.level, (byte) 35);
      this.refresh(this.skill_point, (byte) 37);
      this.refresh(this.stt_point, (byte) 38);
      this.refresh(this.hp, (byte) 25);
      this.refresh(this.sp, (byte) 26);
    }

    public void setStat(byte prop_code, int val)
    {
      switch (prop_code)
      {
        case 27:
          this.checkSetStat(ref this.mag, prop_code, val);
          break;
        case 28:
          this.checkSetStat(ref this.atk, prop_code, val);
          break;
        case 29:
          this.checkSetStat(ref this.def, prop_code, val);
          break;
        case 30:
          this.checkSetStat(ref this.agi, prop_code, val);
          break;
        case 31:
          this.checkSetStat(ref this.hpx, prop_code, val);
          this.hp_max = this.getHpMax();
          break;
        case 32:
          this.checkSetStat(ref this.spx, prop_code, val);
          this.sp_max = this.getSpMax();
          break;
      }
    }

    public void checkSetStat(ref int prop, byte prop_code, int val)
    {
      if (val > prop + 1 || this.stt_point == 0)
        return;
      ++prop;
      --this.stt_point;
      this.refresh(this.stt_point, (byte) 38);
      this.refresh(prop, prop_code);
    }

    public void setSkill(ushort skillid, byte sk_lvl)
    {
      if (!SkillData.skillList.ContainsKey(skillid) || this.skill_point <= 0)
        return;
      SkillInfo skill = SkillData.skillList[skillid];
      int num;
      bool flag;
      if (this.skill.ContainsKey(skillid))
      {
        num = (int) sk_lvl - (int) this.skill[skillid];
        flag = false;
      }
      else
      {
        if (skill.require_sk != (ushort) 0 && !this.skill.ContainsKey(skill.require_sk) && skill.id != (ushort) 13014)
          return;
        num = (int) SkillData.skillList[skillid].elem == (int) this.element ? (int) skill.sk_point + (int) sk_lvl - 1 : (int) skill.sk_point * 2 + (int) sk_lvl - 1;
        flag = true;
      }
      if (num > 0 && this.skill_point >= num)
      {
        if (flag)
          this.skill.Add(skillid, sk_lvl);
        else
          this.skill[skillid] = sk_lvl;
        this.skill_point -= num;
        this.refresh(this.skill_point, (byte) 37);
        this.refreshFull((byte) 110, (int) sk_lvl, (int) skillid);
      }
    }

    public void setSkillRb2(byte[] data)
    {
      int off1 = 2;
      uint num1 = PacketReader.read32(data, off1);
      if ((uint) this.ball_point < num1)
        return;
      this.ball_point -= (byte) num1;
      int num2 = off1 + 4;
      for (int index = 0; (long) index < (long) num1; ++index)
        this.ballList[(int) data[num2 + index] - 1] = true;
      int off2 = num2 + (int) num1;
      uint num3 = PacketReader.read32(data, off2);
      int off3 = off2 + 4;
      for (int index = 0; (long) index < (long) num3; ++index)
      {
        this.setSkill(PacketReader.read16(data, off3), data[off3 + 2]);
        off3 += 3;
      }
      uint num4 = PacketReader.read32(data, off3) / 3U;
      int index1 = off3 + 4;
      for (int index2 = 0; (long) index2 < (long) num4; ++index2)
      {
        this.skill_rb2[(int) data[index1] - 6] = PacketReader.read16(data, index1 + 1);
        index1 += 3;
      }
      this.sendBallList();
    }

    public bool setBattlePet(ushort npcid)
    {
      for (int index = 0; index < 4; ++index)
      {
        if (this.pet[index] != null && (int) this.pet[index].NPCid == (int) npcid)
        {
          this.pet_battle = (sbyte) index;
          return true;
        }
      }
      return false;
    }

    public bool unsetBattlePet()
    {
      if (this.pet_battle == (sbyte) -1)
        return false;
      this.pet_battle = (sbyte) -1;
      return true;
    }

    public void rebornChar(byte nb_reborn, byte j)
    {
      if (this.level < (byte) 120 || (int) this.rb != (int) nb_reborn - 1 || this.rb == (byte) 2 && (j < (byte) 1 || j > (byte) 4) || this.nb_equips > (byte) 0)
        return;
      ++this.rb;
      if (this.rb == (byte) 2)
        this.job = j;
      this.stt_point = 6 + (int) this.level / (10 / (int) nb_reborn);
      this.skill_point = (int) this.level / (4 / (int) nb_reborn);
      this.atk = 0;
      this.mag = 0;
      this.def = 0;
      this.agi = 0;
      this.hpx = 0;
      this.spx = 0;
      this.level = (byte) 1;
      this.totalxp = 0U;
      this.currentxp = 0;
      this.hp_max = this.getHpMax();
      this.hp = this.hp_max;
      this.sp_max = this.getSpMax();
      this.sp = this.sp_max;
      this.honor = 0U;
      foreach (int key in this.skill.Keys)
        this.refreshFull((byte) 110, 0, key);
      this.skill.Clear();
      this.skill.Add((ushort) 14001, (byte) 1);
      this.skill.Add((ushort) 14015, (byte) 10);
      this.skill.Add((ushort) 14021, (byte) 5);
      this.skill.Add((ushort) 14023, (byte) 1);
      this.skill.Add((ushort) 14035, (byte) 1);
      this.refresh(this.stt_point, (byte) 38);
      this.refresh(this.skill_point, (byte) 37);
      this.refresh((int) this.totalxp, (byte) 36);
      this.refresh((int) this.level, (byte) 35);
      this.refreshChr();
      this.sendLook(true);
      this.sendInfo();
    }

    public bool checkPetReborn(byte nb_reborn)
    {
      int num = nb_reborn == (byte) 1 ? 65 : 67;
      for (int index = 0; index < 4; ++index)
      {
        if (this.pet[index] != null && (int) this.pet[index].reborn == (int) nb_reborn - 1 && (int) this.pet[index].level >= (int) nb_reborn * 30 && (int) this.pet[index].fai >= (int) nb_reborn * 40 + 20)
        {
          ushort itemid = 0;
          foreach (TS_Server.DataTools.ItemInfo itemInfo in ItemData.itemList.Values)
          {
            if ((int) itemInfo.prop1 == num && itemInfo.prop1_val == (int) this.pet[index].NPCid)
            {
              itemid = itemInfo.id;
              break;
            }
          }
          if (itemid != (ushort) 0 && this.inventory.getItemById(itemid) != (byte) 25)
            return true;
        }
      }
      return false;
    }

    public void rebornPet(byte nb_reborn, byte slot)
    {
      int num = nb_reborn == (byte) 1 ? 65 : 67;
      ushort itemid = 0;
      ushort npcid = 0;
      foreach (TS_Server.DataTools.ItemInfo itemInfo in ItemData.itemList.Values)
      {
        if ((int) itemInfo.prop1 == num && itemInfo.prop1_val == (int) this.pet[(int) slot - 1].NPCid)
        {
          itemid = itemInfo.id;
          npcid = (ushort) itemInfo.prop2_val;
          break;
        }
      }
      if (itemid == (ushort) 0)
        return;
      byte itemById = this.inventory.getItemById(itemid);
      if (itemById == (byte) 25)
        return;
      this.inventory.dropItem((byte) ((uint) itemById + 1U), (byte) 1);
      int bonus = (int) this.pet[(int) this.pet_battle].level / ((int) nb_reborn * 2);
      this.removePet(slot);
      this.addPet(npcid, bonus);
    }

    public void rideHorse(bool ride, ushort horseid = 0)
    {
      if (ride)
      {
        for (int index = 0; index < 4; ++index)
        {
          if (this.pet[index] != null && (int) this.pet[index].NPCid == (int) horseid && NpcData.npcList[horseid].type == (byte) 9)
          {
            PacketCreator packetCreator = new PacketCreator((byte) 15, (byte) 5);
            packetCreator.add32(this.client.accID);
            packetCreator.add16(horseid);
            packetCreator.addZero(6);
            this.replyToMap(packetCreator.send(), true);
            break;
          }
        }
        this.horseID = horseid;
      }
      else
      {
        PacketCreator packetCreator = new PacketCreator((byte) 15, (byte) 6);
        packetCreator.add32(this.client.accID);
        packetCreator.addZero(2);
        this.replyToMap(packetCreator.send(), true);
        this.horseID = (ushort) 0;
      }
    }

    public void setCharElement(byte element)
    {
      new TSMysqlConnection().updateQuery("UPDATE chars SET `element` = " + (object) element + " WHERE id=" + (object) this.charId);
      this.element = element;
    }

    public void sleep()
    {
      this.reply(new PacketCreator((byte) 31, (byte) 10).send());
      this.setHp(1000000);
      this.refresh(this.hp, (byte) 25);
      this.setSp(1000000);
      this.refresh(this.sp, (byte) 26);
      for (int index = 0; index < 4; ++index)
      {
        if (this.pet[index] != null)
        {
          this.pet[index].setHp(100000);
          this.pet[index].refresh(this.pet[index].hp, (byte) 25);
          this.pet[index].setSp(100000);
          this.pet[index].refresh(this.pet[index].sp, (byte) 26);
        }
      }
      this.client.reply(new PacketCreator(new byte[3]
      {
        (byte) 31,
        (byte) 1,
        (byte) 0
      }).send());
    }

    public void addSummonSkill(byte level)
    {
      if (this.skill.ContainsKey((ushort) 14026))
        return;
      this.skill.Add((ushort) 14026, level);
    }

    public void addSummonSkill()
    {
      if (!this.skill.ContainsKey((ushort) 14026))
        return;
      switch (this.element)
      {
        case 1:
          this.skill.Add((ushort) 10016, this.skill[(ushort) 14026]);
          this.refreshFull((byte) 110, (int) this.skill[(ushort) 14026], 10016);
          break;
        case 2:
          this.skill.Add((ushort) 11016, this.skill[(ushort) 14026]);
          this.refreshFull((byte) 110, (int) this.skill[(ushort) 14026], 11016);
          break;
        case 3:
          this.skill.Add((ushort) 12016, this.skill[(ushort) 14026]);
          this.refreshFull((byte) 110, (int) this.skill[(ushort) 14026], 12016);
          break;
        case 4:
          this.skill.Add((ushort) 13015, this.skill[(ushort) 14026]);
          this.refreshFull((byte) 110, (int) this.skill[(ushort) 14026], 13015);
          break;
      }
    }

    public void removeSummonSkill()
    {
      if (!this.skill.ContainsKey((ushort) 14026))
        return;
      switch (this.element)
      {
        case 1:
          this.skill.Remove((ushort) 10016);
          this.refreshFull((byte) 110, 0, 10016);
          break;
        case 2:
          this.skill.Remove((ushort) 11016);
          this.refreshFull((byte) 110, 0, 11016);
          break;
        case 3:
          this.skill.Remove((ushort) 12016);
          this.refreshFull((byte) 110, 0, 12016);
          break;
        case 4:
          this.skill.Remove((ushort) 13015);
          this.refreshFull((byte) 110, 0, 13015);
          break;
      }
    }
  }
}
