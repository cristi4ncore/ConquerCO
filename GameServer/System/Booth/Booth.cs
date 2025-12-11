using Extensions;
using DeathWish.Game.MsgServer;
using DeathWish.Role;
using System;
using System.Collections.Generic;

namespace DeathWish.Game
{
    public struct BoothItem
    {
        public MsgGameItem Item;
        public uint Cost;
        public MsgItemView.ActionMode Cost_Type;
    }
    public class Booth
    {
        public static Extensions.Counter BoothCounter = new Extensions.Counter(19312181);
        private static Dictionary<uint, Booth> Booths = new Dictionary<uint, Booth>();
        public static object SyncRoot = new Object();
        public System.Collections.Generic.SafeDictionary<uint, BoothItem> ItemList;
        public Client.GameClient Owner;
        public Game.PlayerbotBooth MainOwner;
        public SobNpc Base;
        public MsgMessage HawkMessage;
        public Booths.BoothType Type { get; internal set; }
        public Booth()
        {
            ItemList = new System.Collections.Generic.SafeDictionary<uint, BoothItem>();
        }
        public static implicit operator SobNpc(Booth booth)
        {
            return booth.Base;
        }
    }
}
