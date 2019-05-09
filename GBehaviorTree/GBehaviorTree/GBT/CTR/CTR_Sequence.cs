using System;
using System.Collections.Generic;

namespace GBT
{
    /// <summary>
    /// 序列节点 多个子节点,一个子节点失败，序列失败
    /// 顺序执行,如果一个节点失败，怎返回失败
    /// </summary>
    public class GBTSequence : GBTCtrNode
    {
        public GBTSequence()
        {
        }
        protected override ENodeState onUpdate()
        {
            if (false == isValidateIndex(_activityIndex))
                _activityIndex = 0;

            for (int i = _activityIndex; i < _children.Count; ++i)
            {
                _activityIndex = i;
                var child = _children[i];
                _state = child.Exec();
                if (_state == ENodeState.running)
                    return _state;

                if (_state == ENodeState.failed)
                    break;
            }

            if (_children.Count <= 0)
                _state = ENodeState.success;
            return _state;
        }

        protected override void onTransition()
        {
            base.onTransition();
        }
    }
}
