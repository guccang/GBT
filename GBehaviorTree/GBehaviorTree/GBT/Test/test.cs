using System;
using System.Collections.Generic;
using Pool = GBT.ObjectPoolMgr;
namespace GBT
{
    class test
    {
        public static List<GBTNode> createTest1()
        {
            var seq = Pool.Pop<GBTSequence>().SetDebugName("seq");

            seq.Add(Pool.Pop<GBTCondition>())
                .Add(Pool.Pop<ACT_LeafNode>())
                .Add(Pool.Pop<GBTLoop>().SetCnt(3))
                ;

            var loop = Pool.Pop<GBTLoop>().SetCnt(10).SetDebugName("loop");
            loop.Add(Pool.Pop<ACT_LeafNode>())
                .Add(Pool.Pop<ACT_CalcSkillId>())
                ;

            var sel = Pool.Pop<GBTSelector>().SetDebugName("sel");
            sel.Add(Pool.Pop<GBTIdle>().SetPreCondition(Pool.Pop<CON_CanIdle>()))
            .Add(Pool.Pop<GBTFight>().SetPreCondition(Pool.Pop<CON_False>()))
            .Add(Pool.Pop<GBTLoop>().SetCnt(10))
            ;

            var seqNot = Pool.Pop<GBTSequence>().SetDebugName("seqNot");
            seqNot.Add(Pool.Pop<GBTSequence>())
                 .Add(Pool.Pop<CON_Not>().Add(Pool.Pop<CON_CanIdle>()))
                 .Add(Pool.Pop<ACT_LeafNode>())
                 .Add(Pool.Pop<GBTLoop>().SetCnt(3).Add(Pool.Pop<ACT_LeafNode>()))
                 ;

            var seqOr = Pool.Pop<GBTSequence>().SetDebugName("seqOr");
            seqOr
                 .Add(Pool.Pop<CON_Or>().Add(Pool.Pop<CON_CanIdle>()).Add(Pool.Pop<CON_True>()))
                 .Add(Pool.Pop<ACT_LeafNode>())
                 ;

            var seqAnd = Pool.Pop<GBTSequence>().SetDebugName("seqAnd");
            seqAnd
                 .Add(Pool.Pop<CON_And>().Add(Pool.Pop<CON_CanIdle>()).Add(Pool.Pop<CON_True>()))
                 .Add(Pool.Pop<ACT_LeafNode>())
                 ;

            var selPri = Pool.Pop<CTR_PrioritizedSelector>().SetDebugName("selPri");
            selPri.Add(Pool.Pop<ACT_LeafNode>(), 100)
                .Add(Pool.Pop<CON_True>(), 100)
                .Add(Pool.Pop<GBTLoop>().SetCnt(1), 100)
                ;

            List<GBTNode> trees = Pool.Pop<List<GBTNode>>();
            trees.Add(seq);
            trees.Add(loop);
            trees.Add(sel);
            trees.Add(seqNot);
            trees.Add(seqOr);
            trees.Add(seqAnd);
            trees.Add(selPri);
            return trees;
        }
    }
}
