using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{
    /// <summary>
    /// 选择节点 多个子节点,一个子节点成功，序列成功
    /// 顺序执行，直到找到一个执行成功的节点。
    /// </summary>
    public class GBTSelector : GBTCtrNode
    {
      

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

                if (_state == ENodeState.success)
                    break;

            }

            // this is impossiable
            //if(isValidateIndex(_activityIndex+1) && _state == ENodeState.failed)
            //{
            //    _activityIndex++;
            //    _state = ENodeState.running;
            //}

            if (_children.Count <= 0)
                _state = ENodeState.success;

            return _state;
        }
    }
}
