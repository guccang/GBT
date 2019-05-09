using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{
    // 执行节点
    public class GBTLeafNode : GBTNode
    {
       
        public override void SetBB(BlackBoard bb)
        {
            base.SetBB(bb);

            if (null != _preCondition)
                _preCondition.SetBB(bb);
        }

        protected override void onTransition()
        {
            _preCondition = null;
            base.onTransition();
        }

        protected override ENodeState onUpdate()
        {
            return base.onUpdate();
        }
    }

    public class GBTMoveTo : GBTLeafNode
    {

    }
}
