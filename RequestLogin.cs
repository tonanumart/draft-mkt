// Decompiled with JetBrains decompiler
// Type: TS_Server.PacketHandlers.RequestLogin
// Assembly: TS_Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9F406604-6D74-4A33-BF41-C1841FC4C8A8
// Assembly location: C:\Users\anumart.c.BCONNEX\Desktop\TestGraph\Database\ถอดดาต้าTS\TS_Server.exe

using TS_Server.Client;

namespace TS_Server.PacketHandlers
{
  internal class RequestLogin
  {
    public RequestLogin(TSClient client)
    {
      if (client.isOnline())
        return;
      PacketCreator packetCreator = new PacketCreator();
      packetCreator.addByte((byte) 1);
      packetCreator.addByte((byte) 9);
      packetCreator.addByte((byte) 1);
      client.reply(packetCreator.send());
    }
  }
}
