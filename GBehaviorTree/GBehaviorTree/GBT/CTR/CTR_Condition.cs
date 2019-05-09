using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{

    public class GBTCondition : GBTCtrNode
    {
        public virtual bool IsTrue()
        {
            return true;
        }

        protected override bool onEvaluate()
        {
            return true;
        }

        protected override ENodeState onUpdate()
        {
            if (IsTrue())
                _state = ENodeState.success;
            else
                _state = ENodeState.failed;
            return _state;
        }

    }
}
