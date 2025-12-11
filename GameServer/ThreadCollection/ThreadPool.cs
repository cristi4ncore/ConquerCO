using System;
using System.Threading;
using System.Threading.Generic;
using DeathWish.Client;
using static DeathWish.Threading.Basic;

namespace DeathWish.ServerSockets
{
    public class ThreadPool
    {
        //public TimerRule<ServerSockets.SecuritySocket> ConnectionReceive;
        public StaticPool ReceivePool, SendPool;
        public static StaticPool GenericThreadPool;
        public TimerRule<GameClient> MainTimer;
        public TimerRule<Bot.AI> BotMainTimer;
        public ThreadPool()
        {
            GenericThreadPool = new StaticPool(32).Run();
            ReceivePool = new StaticPool(64).Run();
            //ConnectionReceive = new TimerRule<ServerSockets.SecuritySocket>(connectionReceive, 1, ThreadPriority.Highest);
            MainTimer = new TimerRule<GameClient>(MainCallBack, 250);
            BotMainTimer = new TimerRule<Bot.AI>(BotMainCallBack, 250);
        }
        public static void BotMainCallBack(Bot.AI bot, int time)
        {
            //if (Time32.Now >= bot.StampJumbCallback.AddMilliseconds(500))
            //{
            //    Bot.Dynamic.Jumb_DoWork(bot);
            //    bot.StampJumbCallback = Time32.Now;
            //}
            if (Time32.Now >= bot.StampHitCallback.AddMilliseconds(500))
            {
                Bot.Dynamic.Hit_DoWork(bot);
                bot.StampHitCallback = Time32.Now;
            }
        }
        public static void MainCallBack(GameClient client, int time)
        {
            try
            {
                if (client.Player.Class >= 41 && client.Player.Class <= 45)
                {
                    if (Time32.Now >= client.StampAutoAttackCallback.AddMilliseconds(300))
                    {
                        ThreadInvoke(new Action(client.AutoAttackCallback));
                        client.StampAutoAttackCallback = Time32.Now;
                    }
                }
                else if (Time32.Now >= client.StampAutoAttackCallback.AddMilliseconds(400))
                {
                    ThreadInvoke(new Action(client.AutoAttackCallback));
                    client.StampAutoAttackCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampPlayer_BuffersCallback.AddMilliseconds(500))
                {
                    ThreadInvoke(new Action(client.BufferCallback));
                    client.StampPlayer_BuffersCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampFloorCallback.AddMilliseconds(100))
                {
                    ThreadInvoke(new Action(client.FloorCallback));
                    client.StampFloorCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampMiningCallBack.AddMilliseconds(2500))
                {
                    ThreadInvoke(new Action(client.MiningCallBack));
                    client.StampMiningCallBack = Time32.Now;
                }
                if (Time32.Now >= client.StampSecondsCallback.AddMilliseconds(1000))
                {
                    ThreadInvoke(new Action(client.SecondsCallback));
                    client.StampSecondsCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampStaminaCallback.AddMilliseconds(400))
                {
                    ThreadInvoke(new Action(client.StaminaCallback));
                    client.StampStaminaCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampAliveMonstersCallback.AddMilliseconds(700))//done
                {
                    ThreadInvoke(new Action(client.AliveMonstersCallback));
                    client.StampAliveMonstersCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampMonster_BuffersCallback.AddMilliseconds(700))
                {
                    ThreadInvoke(new Action(client.BuffersCallback));
                    client.StampMonster_BuffersCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampGuardsCallback.AddMilliseconds(700))
                {
                    ThreadInvoke(new Action(client.GuardsCallback));
                    client.StampGuardsCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampReviversCallback.AddMilliseconds(1000))
                {
                    ThreadInvoke(new Action(client.ReviversCallback));
                    client.StampReviversCallback = Time32.Now;
                }
                if (Time32.Now >= client.StampCheckItemsTime.AddMilliseconds(1000))
                {
                    ThreadInvoke(new Action(client.CheckItemsTimeCallback));
                    client.StampCheckItemsTime = Time32.Now;
                }
                if (Time32.Now >= client.StampAIBotTime.AddMilliseconds(700))
                {
                    ThreadInvoke(new Action(client.CheckAiBotCallback));
                    client.StampAIBotTime = Time32.Now;
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        private static System.Time32 StampWorldTournaments = System.Time32.Now;
        private static System.Time32 StampServer = System.Time32.Now;
        public static void ServerCallBack()
        {
            if (Time32.Now >= StampServer.AddMinutes(1))
            {
                Threading.Server.Handle();
                StampServer = Time32.Now;
            }
            if (Time32.Now >= StampWorldTournaments.AddMilliseconds(900))
            {
                Threading.WorldTournaments.Handle();
                StampWorldTournaments = Time32.Now;
            }
            Threading.Qualifier.ArenaQualifier();
            Threading.Qualifier.TeamArenaQualifier();

        }
        public bool BotRegister(Bot.AI bot)
        {
            if (bot.BEntity.TimerSubscriptions == null)
            {
                bot.BEntity.TimerSubscriptions = new IDisposable[]
                {
                   Subscribe(BotMainTimer, bot)
                };
                return true;
            }
            return false;
        }
        public bool Register(GameClient client)
        {
            if (client.TimerSubscriptions == null)
            {
                client.TimerSubscriptions = new IDisposable[]
                {
                   Subscribe(MainTimer, client)
                };
                return true;
            }
            return false;
        }
        public static void Unregister(GameClient client)
        {
            lock (client.TimerSyncRoot)
            {
                if (client.TimerSubscriptions != null)
                {
                    foreach (var Now in client.TimerSubscriptions)
                        Now.Dispose();
                    client.TimerSubscriptions = null;
                }
            }
        }
        //public static void connectionReceive(ServerSockets.SecuritySocket wrapper, int time)
        //{
        //    if (wrapper.ReceiveBuffer())
        //    {
        //        wrapper.HandlerBuffer();
        //    }
        //}
        #region Funcs
        public static void Execute(Action<int> action, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            GenericThreadPool.Subscribe(new LazyDelegate(action, timeOut, priority));
        }
        public static void Execute<T>(Action<T, int> action, T param, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            GenericThreadPool.Subscribe<T>(new LazyDelegate<T>(action, timeOut, priority), param);
        }
        public static IDisposable Subscribe(Action<int> action, int period = 1, ThreadPriority priority = ThreadPriority.Normal)
        {
            return GenericThreadPool.Subscribe(new TimerRule(action, period, priority));
        }
        public static IDisposable Subscribe<T>(Action<T, int> action, T param, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            return GenericThreadPool.Subscribe<T>(new TimerRule<T>(action, timeOut, priority), param);
        }
        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param, StandalonePool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param, StaticPool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param)
        {
            return GenericThreadPool.Subscribe<T>(rule, param);
        }
        #endregion
    }
}
