// Decompiled with JetBrains decompiler
// Type: TS_Server.Client.TSClient
// Assembly: TS_Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F406604-6D74-4A33-BF41-C1841FC4C8A8
// Assembly location: C:\Users\anumart.c.BCONNEX\Desktop\TestGraph\Database\ถอดดาต้าTS\TS_Server.exe

using MySql.Data.MySqlClient;
using System;
using System.Net.Sockets;
using TS_Server.Server;

namespace TS_Server.Client
{
  public class TSClient
  {
    private Socket socket;
    private string clientID;
    private TSCharacter chr;
    public bool creating;
    public bool online;
    public BattleAbstract battle;
    public TSMap map;
    public uint accID;
    public byte[] name_temp;
    public TSWorld world;
    public ushort warpPrepare;

    public TSClient(Socket s, string id)
    {
      this.socket = s;
      this.clientID = id;
      this.creating = false;
      this.online = false;
    }

    public void createChar(byte[] data)
    {
      this.chr = new TSCharacter(this);
      this.chr.initChar(data, this.name_temp);
      this.chr.loginChar();
      this.world = TSServer.getInstance().getWorld();
    }

    public int checkLogin(uint acc_id, string password)
    {
      int num = 0;
      TSMysqlConnection tsMysqlConnection1 = new TSMysqlConnection();
      MySqlDataReader mySqlDataReader1 = tsMysqlConnection1.selectQuery("SELECT password, loggedin FROM account WHERE id = " + (object) acc_id);
      if (!mySqlDataReader1.Read())
        num = 1;
      else if (mySqlDataReader1.GetString(0) != password)
        num = 1;
      else if (mySqlDataReader1.GetBoolean(1))
      {
        num = 2;
      }
      else
      {
        TSMysqlConnection tsMysqlConnection2 = new TSMysqlConnection();
        MySqlDataReader mySqlDataReader2 = tsMysqlConnection2.selectQuery("SELECT accountid FROM chars WHERE accountid = " + (object) acc_id);
        if (!mySqlDataReader2.Read())
        {
          this.accID = acc_id;
          num = 3;
        }
        mySqlDataReader2.Close();
        tsMysqlConnection2.connection.Close();
      }
      mySqlDataReader1.Close();
      tsMysqlConnection1.connection.Close();
      if (num == 0)
      {
        this.accID = acc_id;
        this.chr = new TSCharacter(this);
      }
      return num;
    }

    public bool isTeamLeader() => false;

    public bool isJoinedTeam() => false;

    public bool isOnline() => this.online;

    public TSCharacter getChar() => this.chr;

    public Socket getSocket() => this.socket;

    public string getClientID() => this.clientID;

    public void reply(byte[] data)
    {
      try
      {
        this.socket.Send(data);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Socket down, client " + this.clientID + " disconnect");
        this.disconnect();
      }
    }

    public void savetoDB()
    {
      TSMysqlConnection tsMysqlConnection = new TSMysqlConnection();
      tsMysqlConnection.connection.Open();
      this.chr.saveCharDB(tsMysqlConnection.connection);
      for (int index = 0; index < 4; ++index)
      {
        if (this.chr.pet[index] != null)
          this.chr.pet[index].savePetDB(tsMysqlConnection.connection, false);
      }
      tsMysqlConnection.connection.Close();
    }

    public void continueMoving()
    {
      this.RequestComplete();
      this.AllowMove();
    }

    public void RequestComplete() => this.reply(new PacketCreator((byte) 20, (byte) 8).send());

    public void AllowMove()
    {
      this.reply(new PacketCreator((byte) 5, (byte) 4).send());
      this.reply(new PacketCreator((byte) 15, (byte) 10).send());
    }

    public void ClickkNpc(byte[] data, TSClient client)
    {
      PacketCreator packetCreator = new PacketCreator((byte) 20, (byte) 1);
      packetCreator.addByte((byte) 0);
      packetCreator.add16((ushort) 0);
      packetCreator.addByte((byte) 0);
      packetCreator.addByte((byte) 1);
      packetCreator.add16(ushort.Parse(((int) PacketReader.read16(data, 1) + 2).ToString()));
      packetCreator.add16((ushort) 0);
      packetCreator.add16((ushort) 0);
      packetCreator.add16((ushort) 0);
      packetCreator.add16((ushort) 10666);
      client.reply(packetCreator.send());
    }

    public void UImportant()
    {
      this.reply(new PacketCreator(new byte[4]
      {
        (byte) 24,
        (byte) 7,
        (byte) 3,
        (byte) 4
      }).send());
      PacketCreator packetCreator1 = new PacketCreator((byte) 41);
      packetCreator1.add8((byte) 5);
      packetCreator1.add8((byte) 1);
      packetCreator1.add8((byte) 1);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(33554432U);
      packetCreator1.add32(1U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(259U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(66560U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(17104896U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add32(0U);
      packetCreator1.add16((ushort) 0);
      packetCreator1.add8((byte) 0);
      this.reply(packetCreator1.send());
      PacketCreator packetCreator2 = new PacketCreator((byte) 11);
      packetCreator2.add32(4065001988U);
      packetCreator2.add32(1U);
      packetCreator2.add8((byte) 0);
      this.reply(packetCreator2.send());
    }

    public void U0602() => this.reply(new PacketCreator((byte) 6, (byte) 6).send());

    public void U1406() => this.reply(new PacketCreator((byte) 20, (byte) 6).send());

    public void disconnect()
    {
      if (this.battle != null)
        this.battle.outBattle(this);
      if (!this.online)
        return;
      this.savetoDB();
      PacketCreator packetCreator1 = new PacketCreator((byte) 13, (byte) 4);
      packetCreator1.add32(this.accID);
      this.chr.replyToMap(packetCreator1.send(), false);
      PacketCreator packetCreator2 = new PacketCreator((byte) 1, (byte) 1);
      packetCreator2.add32(this.accID);
      this.chr.replyToMap(packetCreator2.send(), false);
      this.map.listPlayers.Remove(this.accID);
      TSServer.getInstance().removePlayer(this.accID);
      this.online = false;
    }
  }
}
