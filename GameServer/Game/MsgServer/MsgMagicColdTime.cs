using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace DeathWish.Game.MsgServer
{
   public static class MsgMagicColdTime
    {
       [ProtoContract]
       public class MagicColdTime
       {
           [ProtoMember(1, IsRequired = true)]
           public Spell[] Spells;

     
           public void WriteSpells(List<MsgSpell> array)
           {
               Spells = new Spell[array.Count];
               for (int x = 0; x < array.Count; x++)
               {
                   Spells[x] = new Spell();
                   Spells[x].SpellID = array[x].ID;
                   Spells[x].Time = array[x].GetColdTime;
               }
           }
           public void ZeroEagleEye()
           {
               Spells = new Spell[1];
               Spells[0] = new Spell();
               Spells[0].SpellID = (ushort)Role.Flags.SpellID.EagleEye;
               Spells[0].Time = 0;
           }

       }
       [ProtoContract]
       public class Spell
       {
           [ProtoMember(1, IsRequired = true)]
           public uint SpellID;
           [ProtoMember(2, IsRequired = true)]
           public int Time;

  
       }

       public static unsafe ServerSockets.Packet MagicColdTimeCreate(this ServerSockets.Packet stream, MagicColdTime obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.MsgMagicColdTime);

           return stream;
       }
       public static unsafe void GetMagicColdTime(this ServerSockets.Packet stream, out MagicColdTime pQuery)
       {
           pQuery = new MagicColdTime();
           pQuery = stream.ProtoBufferDeserialize<MagicColdTime>(pQuery);
       } 
   }
}
