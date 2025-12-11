//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using Extensions;
//using Extensions.Threading.Generic;
//using Extensions.Threading;
//using DeathWish.Client;

//namespace DeathWish
//{
//    public static class ThreadPoll
//    {
//        #region Funcs
//        public static void Execute(Action action, uint timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
//        {
//            GenericThreadPool.Subscribe(new Extensions.Threading.LazyDelegate(action, timeOut, priority));
//        }
//        public static void Execute<T>(Action<T> action, T param, uint timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
//        {
//            GenericThreadPool.Subscribe<T>(new Extensions.Threading.Generic.LazyDelegate<T>(action, timeOut, priority), param);
//        }
//        public static IDisposable Subscribe(Action action, uint period = 1, ThreadPriority priority = ThreadPriority.Normal)
//        {
//            return GenericThreadPool.Subscribe(new Extensions.Threading.TimerRule(action, period, priority));
//        }
//        public static IDisposable Subscribe<T>(Action<T> action, T param, uint timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
//        {
//            return GenericThreadPool.Subscribe<T>(new Extensions.Threading.Generic.TimerRule<T>(action, timeOut, priority), param);
//        }
//        public static IDisposable Subscribe<T>(Extensions.Threading.Generic.TimerRule<T> rule, T param, Extensions.Threading.StaticPool pool)
//        {
//            return pool.Subscribe<T>(rule, param);//not complete yet 
//        }
//        public static IDisposable Subscribe<T>(Extensions.Threading.Generic.TimerRule<T> rule, T param)
//        {
//            return GenericThreadPool.Subscribe<T>(rule, param);//here where i stopped . complete work ya mo2mn 
//        }
//        #endregion
//        public static StaticPool ReceivePool;
//        public static StaticPool GenericThreadPool;
//        public static TimerRule<GameClient> MainTimer;
//        public static TimerRule<Bot.AI> BotMainTimer;
//        public static void Create()
//        {
//            GenericThreadPool = new StaticPool(32).Run();
//            ReceivePool = new StaticPool(32).Run();
//            // MainTimer = new TimerRule<GameClient>(MainCallBack, 250);
//            // BotMainTimer = new TimerRule<Bot.AI>(BotMainCallBack, 250);
//        }
//        public static void BotMainCallBack(Bot.AI bot, int time)
//        {
//            if (Time32.Now >= bot.StampJumbCallback.AddMilliseconds(2500))
//            {
//                Bot.Dynamic.Jumb_DoWork(bot);
//                bot.StampJumbCallback = Time32.Now;
//            }
//            if (Time32.Now >= bot.StampHitCallback.AddMilliseconds(1250))
//            {
//                Bot.Dynamic.Hit_DoWork(bot);
//                bot.StampHitCallback = Time32.Now;
//            }
//        }
//        //public static void MainCallBack(GameClient client, int time)
//        //{
//        //    try
//        //    {
//        //        if (client.Player.Class >= 41 && client.Player.Class <= 45)
//        //        {
//        //            if (Time32.Now >= DeathWish.cPoolProcesor.AutoAttackCallback.AddMilliseconds(700))
//        //            {
//        //                ThreadInvoke(new Action(client.AutoAttackCallback));
//        //                client.StampAutoAttackCallback = Time32.Now;
//        //            }
//        //        }
//        //        else if (Time32.Now >= client.StampAutoAttackCallback.AddMilliseconds(1000))
//        //        {
//        //            ThreadInvoke(new Action(client.AutoAttackCallback));
//        //            client.StampAutoAttackCallback = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampPlayer_BuffersCallback.AddMilliseconds(1000))
//        //        {
//        //            ThreadInvoke(new Action(client.BufferCallback));
//        //            client.StampPlayer_BuffersCallback = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampItemsCallBack.AddMilliseconds(1000))
//        //        {
//        //            ThreadInvoke(new Action(client.ItemsCallBack));
//        //            client.StampItemsCallBack = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampMiningCallBack.AddMilliseconds(2500))
//        //        {
//        //            ThreadInvoke(new Action(client.MiningCallBack));
//        //            client.StampMiningCallBack = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampSecondsCallback.AddMilliseconds(1000))
//        //        {
//        //            ThreadInvoke(new Action(client.SecondsCallback));
//        //            client.StampSecondsCallback = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampStaminaCallback.AddMilliseconds(500))
//        //        {
//        //            ThreadInvoke(new Action(client.StaminaCallback));
//        //            client.StampStaminaCallback = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampAliveMonstersCallback.AddMilliseconds(1000))//done
//        //        {
//        //            ThreadInvoke(new Action(client.AliveMonstersCallback));
//        //            client.StampAliveMonstersCallback = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampMonster_BuffersCallback.AddMilliseconds(1000))
//        //        {
//        //            ThreadInvoke(new Action(client.BuffersCallback));
//        //            client.StampMonster_BuffersCallback = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampGuardsCallback.AddMilliseconds(1000))
//        //        {
//        //            ThreadInvoke(new Action(client.GuardsCallback));
//        //            client.StampGuardsCallback = Time32.Now;
//        //        }
//        //        if (Time32.Now >= client.StampReviversCallback.AddMilliseconds(1000))
//        //        {
//        //            ThreadInvoke(new Action(client.ReviversCallback));
//        //            client.StampReviversCallback = Time32.Now;
//        //        }
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        MyConsole.SaveException(e);
//        //    }
//        //}
//        //public bool BotRegister(Bot.AI bot)
//        //{
//        //    if (bot.BEntity.TimerSubscriptions == null)
//        //    {
//        //        bot.BEntity.TimerSubscriptions = new IDisposable[]
//        //        {
//        //           Subscribe(BotMainTimer, bot)
//        //        };
//        //        return true;
//        //    }
//        //    return false;
//        //}
//    }
//}