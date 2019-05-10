using System;
using System.Collections.Generic;

namespace GBT
{
    // 控制节点
    public class GBTCtrNode : GBTNode
    {
        protected List<GBTNode> _children;
        protected int _maxChild;
        protected int _activityIndex;

        public GBTCtrNode()
        {
            _children = new List<GBTNode>();
            _maxChild = 901124;
            _activityIndex = -1;
        }

        public new GBTCtrNode SetDebugName(string name)
        {
            base.SetName(name);
            return this;
        }

        public int GetMaxChildCount()
        {
            return _maxChild;
        }
        public GBTCtrNode Add(GBTNode child)
        {
            if (_maxChild <= _children.Count)
                return null;
            _children.Add(child);
            child.SetParent(this);
            child.SetBB(_bb);
            return this;
        }
        public int GetCurChildCount()
        {
            return _children.Count;
        }
        public void Rm(int index)
        {
            if (!isValidateIndex(index))
                return;

            _children.RemoveAt(index);
        }
        public GBTNode Get(int index)
        {
            if (!isValidateIndex(index))
                return null;

            return _children[index];
        }
        public override void Free()
        {
#if GUCCANG_OBJ_POOL
            ForEach(child =>
            {
                child.Free();
                return true;
            });
            base.Free();
#endif
        }
        public void ForEach(Func<GBTNode, bool> func)
        {
            if (null == func)
                return;
            foreach (var child in _children)
                if (false == func(child))
                    break;
        }
        public override void SetBB(BlackBoard bb)
        {
            base.SetBB(bb);

            if (GetCurChildCount() > 0)
            {
                ForEach(node =>
                {
                    node.SetBB(bb);
                    return true;
                });
            }
        }

        protected bool isValidateIndex(int index)
        {
            if (index < 0)
                return false;
            if (index >= _children.Count)
                return false;

            return true;
        }
        protected override void onTransition()
        {
            ForEach(child =>
            {
                child.Transition();
                return true;
            });
            _activityIndex = -1;
            base.onTransition();
        }
        protected override bool onEvaluate()
        {
            if (_preCondition != null && false == _preCondition.IsTrue())
            {
                _state = ENodeState.failed;
                return false;
            }

            if (_state == ENodeState.failed)
                return false;

            if (_state == ENodeState.success)
                return false;

            if (_state == ENodeState.init)
                return true;

            if (_state == ENodeState.running && isValidateIndex(_activityIndex))
                return true;

            return false;
        }
    }


   

   

  

}
