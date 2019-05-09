using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{
    // 执行节点
    public class ACT_LeafNode : GBTNode
    {
       
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

}
