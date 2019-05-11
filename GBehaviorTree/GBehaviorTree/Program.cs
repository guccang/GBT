using GBT;
using System.Collections.Generic;
using Pool =  GBT.ObjectPoolMgr;




namespace GBehaviorTreeTest
{
    public class GBT2Action : ACT_LeafNode
    {
        private int _cnt;
        public GBT2Action SetCnt (int cnt)
        {
            _cnt = cnt;
            return this;
        }
        protected override ENodeState onUpdate()
        {
            _cnt--;
            if (_cnt > 0)
                return ENodeState.running;
            return base.onUpdate();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            LogConfig.InitLog();
            var log = LogConfig.GetLog(typeof(Program));

            GBehaviorTree tree = Pool.Pop<GBehaviorTree>();

            var trees = test.createTest1();
            int cnt = trees.Count;
            System.Random r = new System.Random();
            for (int i=0;i<300;++i)
            {
                log.Warn($"update..... {i}");
                if(tree.IsFinish())
                    tree.SwitchTo(trees[r.Next(cnt)]);
                tree.Update();
                if (tree.IsFinish())
                    tree.Transition();
            }

            System.Random ran = new System.Random();
            int poolCnt = 100;
            while(poolCnt-->0)
            {
                ran = Pool.Pop<System.Random>();
                Pool.Push(ran);
            }
            // Object Pool
            foreach (var subTree in trees)
                subTree.Free();
            Pool.Push(tree);
            Pool.Push(trees);
            Pool.Test();
            log.Debug("Hello World!");
        }
    }
}
