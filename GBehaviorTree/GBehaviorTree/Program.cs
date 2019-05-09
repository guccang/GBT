using GBT;
namespace GBehaviorTreeTest
{
    public class GBT2Action : GBTLeafNode
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
            var root = new GBTSelector();
            var fight = GBTFight.Create();
            var idle  = GBTIdle.Create();
            root.Add(idle.SetPreCondition(new CON_GBTCanIdle()))
                .Add(fight.SetPreCondition(new CON_GBTCanFight()))
                ;

            tree.SetCurrentTree(root);
            for (int i=0;i<300;++i)
            {
                log.Warn($"update..... {i}");
                tree.Update();
            }
            log.Debug("Hello World!");
        }
    }
}
