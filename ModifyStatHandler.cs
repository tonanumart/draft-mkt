// Decompiled with JetBrains decompiler
// Type: TS_Server.PacketHandlers.ModifyStatHandler
// Assembly: TS_Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F406604-6D74-4A33-BF41-C1841FC4C8A8
// Assembly location: C:\Users\anumart.c.BCONNEX\Desktop\TestGraph\Database\ถอดดาต้าTS\TS_Server.exe

using System;
using TS_Server.Client;

namespace TS_Server.PacketHandlers
{
  internal class ModifyStatHandler
  {
    public ModifyStatHandler(TSClient client, byte[] data)
    {
      if (data[1] == (byte) 1)
        client.getChar().setStat(data[4], (int) PacketReader.read16(data, 5));
      else
        Console.WriteLine("Modify Stat Handler : unknown subcode" + (object) data[1]);
    }
  }
}
