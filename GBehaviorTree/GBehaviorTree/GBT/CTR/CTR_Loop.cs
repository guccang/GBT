using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{
    public class GBTLoop : GBTCtrNode
    {
        protected int _cnt;
        private int _tmpCnt;
        public GBTLoop()
        {
        }
        public GBTLoop SetCnt(int cnt)
        {
            _cnt = cnt;
            _tmpCnt = cnt;
            return this;
        }

        protected override void onTransition()
        {
            _tmpCnt = _cnt;
            base.onTransition();
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
            }
            toNextLoop();
            return _state;
        }
        protected override bool onEvaluate()
        {
            if (_state == ENodeState.failed)
                return false;

            if (_state == ENodeState.success)
                return false;

            if (_state == ENodeState.init)
                return true;

            if (_state == ENodeState.running && (isValidateIndex(_activityIndex) || _children.Count<=0))
                return true;

            return false;
        }
        private void toNextLoop()
        {
            if (_tmpCnt - 1 > 0)
            {
                _tmpCnt--;
                _state = ENodeState.running;
                _activityIndex = 0;
                ForEach(child =>
                {
                    child.Transition();
                    return true;
                });
            }
            else
            {
                _state = ENodeState.success;
            }
        }
    }
}
