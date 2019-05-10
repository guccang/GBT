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

            GBehaviorTree tree = Pool.Get<GBehaviorTree>();

            var seq = Pool.Get<GBTSequence>().SetName("seq");

            seq.Add(Pool.Get<GBTCondition>())
                .Add(Pool.Get< ACT_LeafNode>())
                .Add(Pool.Get< GBTLoop>().SetCnt(3))
                ;

            var loop = Pool.Get < GBTLoop>().SetCnt(10).SetName("loop");
            loop.Add(Pool.Get < ACT_LeafNode>())
                .Add(Pool.Get < ACT_CalcSkillId>())
                ;

            var sel = Pool.Get < GBTSelector>().SetName("sel");
                sel.Add(Pool.Get < GBTIdle>().SetPreCondition(Pool.Get < CON_CanIdle>()))
                .Add(Pool.Get < GBTFight>().SetPreCondition(Pool.Get < CON_False>()))
                .Add(Pool.Get < GBTLoop>().SetCnt(10))
                ;

            var seqNot = Pool.Get < GBTSequence>().SetName("seqNot");
            seqNot.Add(Pool.Get < GBTSequence>())
                 .Add(Pool.Get < CON_Not>().Add(Pool.Get < CON_CanIdle>()))
                 .Add(Pool.Get < ACT_LeafNode>())
                 .Add(Pool.Get < GBTLoop>().SetCnt(3).Add(Pool.Get < ACT_LeafNode>()))
                 ;

            var seqOr = Pool.Get < GBTSequence>().SetName("seqOr");
            seqOr
                 .Add(Pool.Get < CON_Or>().Add(Pool.Get < CON_CanIdle>()).Add(Pool.Get < CON_True>()))
                 .Add(Pool.Get < ACT_LeafNode>())
                 ;

            var seqAnd = Pool.Get < GBTSequence>().SetName("seqAnd");
            seqAnd
                 .Add(Pool.Get < CON_And>().Add(Pool.Get < CON_CanIdle>()).Add(Pool.Get < CON_True>()))
                 .Add(Pool.Get < ACT_LeafNode>())
                 ;

            var selPri = Pool.Get < CTR_PrioritizedSelector>().SetName("selPri");
            selPri.Add(Pool.Get < ACT_LeafNode>(), 100)
                .Add(Pool.Get < CON_True>(), 100)
                .Add(Pool.Get < GBTLoop>().SetCnt(1), 100)
                ;

            List<GBTNode> trees = Pool.Get<List<GBTNode>>();
            trees.Add(seq);
            trees.Add(loop);
            trees.Add(sel);
            trees.Add(seqNot);
            trees.Add(seqOr);
            trees.Add(seqAnd);
            trees.Add(selPri);


            int cnt = trees.Count;
            System.Random r = new System.Random();
            tree.SetCurrentTree(selPri);
            for (int i=0;i<30000;++i)
            {
                log.Warn($"update..... {i}");
                //if(tree.IsFinish())
                //    tree.SwitchTo(trees[r.Next(cnt)]);
                tree.Update();
                if (tree.IsFinish())
                    tree.Transition();
            }

            // Object Pool
            foreach (var subTree in trees)
                subTree.Free();
            Pool.Free(tree);
            Pool.Free(trees);
            Pool.Test();
            log.Debug("Hello World!");
        }
    }
}
