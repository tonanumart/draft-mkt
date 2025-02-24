// Decompiled with JetBrains decompiler
// Type: TS_Server.PacketHandlers.ModifySkillHandler
// Assembly: TS_Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F406604-6D74-4A33-BF41-C1841FC4C8A8
// Assembly location: C:\Users\anumart.c.BCONNEX\Desktop\TestGraph\Database\ถอดดาต้าTS\TS_Server.exe

using System;
using TS_Server.Client;

namespace TS_Server.PacketHandlers
{
  internal class ModifySkillHandler
  {
    public ModifySkillHandler(TSClient client, byte[] data)
    {
      int off1 = 2;
      switch (data[1])
      {
        case 1:
          for (; off1 < data.Length; off1 += 3)
            client.getChar().setSkill(PacketReader.read16(data, off1), data[off1 + 2]);
          break;
        case 2:
          for (int off2 = off1 + 1; off2 < data.Length; off2 += 3)
            client.getChar().pet[(int) data[2] - 1].setSkill(PacketReader.read16(data, off2), data[off2 + 2]);
          break;
        case 5:
          client.getChar().setSkillRb2(data);
          break;
        default:
          Console.WriteLine("Modify Stat Handler : unknown subcode" + (object) data[1]);
          break;
      }
    }
  }
}
