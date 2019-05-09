using GBT;
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
            GBehaviorTree tree = new GBehaviorTree();
            var seq = new GBTSequence();
            seq.Add(new GBTCondition())
                .Add(new ACT_LeafNode())
                .Add(new GBTLoop())
                ;

            var loop = new GBTLoop().SetCnt(10);
            loop.Add(new ACT_LeafNode())
                .Add(new ACT_CalcSkillId())
                ;

            var sel = new GBTSelector();
                sel.Add(new GBTIdle().SetPreCondition(new CON_GBTCanIdle()))
                .Add(new GBTFight().SetPreCondition(new CON_GBTCanFight()))
                ;

            tree.SetCurrentTree(sel);
            for (int i=0;i<300;++i)
            {
                log.Warn($"update..... {i}");
                tree.Update();
            }
            log.Debug("Hello World!");
        }
    }
}
