using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{
    public class CON_True : GBTCondition
    {
        public override bool IsTrue()
        {
            return true;
        }
    }

    public class CON_False : GBTCondition
    {
        public override bool IsTrue()
        {
            return false;
        }
    }

    public class CON_Not : GBTCondition
    {
        public override bool IsTrue()
        {
            if (false == onEvaluate())
                return false;

            var c1 = _children[0] as GBTCondition;
            if (c1.IsTrue())
                return false;
            else
                return true;
        }

        protected override bool onEvaluate()
        {
            if(_children.Count != 1)
            {
                _state = ENodeState.failed;
                return false;
            }

            var c1 = _children[0] as GBTCondition;
            if(null == c1)
            {
                _state = ENodeState.failed;
                return false;
            }

            return true;
        }
    }

    public class CON_And : GBTCondition
    {
        public override bool IsTrue()
        {
            if (false == onEvaluate())
                return false;

            var c1 = _children[0] as GBTCondition;
            var c2 = _children[1] as GBTCondition;
            if (c1.IsTrue() && c2.IsTrue())
                return true;
            else
                return false;
        }
        protected override bool onEvaluate()
        {
            if (_children.Count != 2)
            {
                _state = ENodeState.failed;
                return false;
            }
            var c1 = _children[0] as GBTCondition;
            var c2 = _children[1] as GBTCondition;
            if (null == c1 || null == c2)
            {
                _state = ENodeState.failed;
                return false;
            }

            return true;
        }

        protected override ENodeState onUpdate()
        {
            return base.onUpdate();
        }

        protected override void onTransition()
        {
            base.onTransition();
        }
    }

   public class CON_Or : GBTCondition
    {
        public override bool IsTrue()
        {
            if (false == onEvaluate())
                return false;

            var c1 = _children[0] as GBTCondition;
            var c2 = _children[1] as GBTCondition;
            if (c1.IsTrue() || c2.IsTrue())
                return true;
            else
                return false;
        }
        protected override bool onEvaluate()
        {
            if(_children.Count != 2)
            {
                _state = ENodeState.failed;
                return false;
            }
            var c1 = _children[0] as GBTCondition;
            var c2 = _children[1] as GBTCondition;
            if (null == c1 || null == c2)
            {
                _state = ENodeState.failed;
                return false;
            }

            return true;
        }

        protected override ENodeState onUpdate()
        {
            return base.onUpdate();
        }

        protected override void onTransition()
        {
            base.onTransition();
        }
    }

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
