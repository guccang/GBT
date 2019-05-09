using System;
using System.Collections.Generic;

namespace GBT
{
    // 控制节点
    public class GBTCtrNode : GBTNode
    {
        protected List<GBTNode> _children;
        protected int _maxChild;

        public GBTCtrNode()
        {
            _children = new List<GBTNode>();
            _maxChild = 901124;
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
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
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
            return !(index < 0 || index >= _children.Count);
        }
        protected override void onTransition()
        {
            ForEach(child =>
            {
                child.Transition();
                return true;
            });
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

    public class GBTLoop : GBTCtrNode
    {
        protected int _activityIndex;
        protected int _cnt;
        private int _tmpCnt;
        public GBTLoop()
        {
            _activityIndex = -1;
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
            _activityIndex = -1;
            base.onTransition();
        }
        protected override bool onEvaluate()
        {
            if (_state == ENodeState.success)
                return false;

            if (_state != ENodeState.running && _state != ENodeState.init)
                return false;

            if (_state == ENodeState.running && !isValidateIndex(_activityIndex))
                return false;

            return true;
        }

        protected override ENodeState onUpdate()
        {
            // 继续上一次running
            if (isRunning() && isValidateIndex(_activityIndex))
            {
                var child = Get(_activityIndex);
                if (child.Evaluate())
                    _state = child.Update();

                if (_state == ENodeState.running)
                    return _state;

                if (_state == ENodeState.success)
                {
                    if (_tmpCnt > 0)
                        _state = ENodeState.running;
                }

                if (isValidateIndex(_activityIndex + 1))
                    _activityIndex++;
                else
                {
                    toNextLoop();
                    return _state;
                }
            }
            else
            {
                _activityIndex = 0;
            }

            for (int i = _activityIndex; i < _children.Count; ++i)
            {
                _activityIndex = i;
                var child = _children[i];
                if (child.Evaluate())
                    _state = child.Update();
                if (_state == ENodeState.success)
                {
                    if (_tmpCnt > 0)
                        _state = ENodeState.running;
                }
                else
                {
                    break;
                }
            }

            bool IsToNextLoop = false;
            bool isAllSuccess = true;
            bool isOneFailed = false;
            for (int i = 0; i < _children.Count; ++i)
            {
                if (_children[i].State != ENodeState.success)
                    isAllSuccess = false;
                if (_children[i].State == ENodeState.failed)
                    isOneFailed = true;
            }

            // 所有子节点都成功，或者有一个子节点失败,进入下一个循环
            IsToNextLoop = isOneFailed || isAllSuccess;

            // 次数足够继续循环
            if (IsToNextLoop)
            {
                toNextLoop();
            }
            return _state;
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

    /// <summary>
    /// 序列节点 多个子节点,一个子节点失败，序列失败
    /// 顺序执行,如果一个节点失败，怎返回失败
    /// </summary>
    public class GBTSequence : GBTCtrNode
    {
        protected int _activityIndex;
        public GBTSequence()
        {
            _activityIndex = -1;
        }
        protected override bool onEvaluate()
        {
            if (_state == ENodeState.success)
                return false;

            if (_state != ENodeState.running && _state != ENodeState.init)
                return false;

            if (_state != ENodeState.init && !isValidateIndex(_activityIndex))
                return false;

            return true;
        }

        protected override ENodeState onUpdate()
        {
            if (isRunning() && isValidateIndex(_activityIndex))
            {
                var child = Get(_activityIndex);
                _state = _state = child.Exec();
                if (_state == ENodeState.running)
                    return ENodeState.running;

                if (_state == ENodeState.success)
                {
                    if (isValidateIndex(_activityIndex + 1))
                        _activityIndex++;
                    else
                        return _state;
                }
            }
            else
            {
                _activityIndex = 0;
            }


            for (int i = _activityIndex; i < _children.Count; ++i)
            {
                _activityIndex = i;
                var child = _children[i];
                _state = child.Exec();
                if (_state != ENodeState.success)
                    break;
            }

            if (_state == ENodeState.success)
            {
                if (isValidateIndex(_activityIndex + 1)) // 子节点未完成，父节点一直处于running状态
                    _state = ENodeState.running;
            }

            return _state;
        }

        protected override void onTransition()
        {
            base.onTransition();
            _activityIndex = -1;
        }
    }

    /// <summary>
    /// 选择节点 多个子节点,一个子节点成功，序列成功
    /// 顺序执行，直到找到一个执行成功的节点。
    /// </summary>
    public class GBTSelector : GBTCtrNode
    {
        protected int _activityIndex;

        protected override bool onEvaluate()
        {
            if (_state == ENodeState.success)
                return false;

            if (_state != ENodeState.running && _state != ENodeState.init && _state != ENodeState.failed)
                return false;

            if (_state != ENodeState.init && !isValidateIndex(_activityIndex))
                return false;

            if (_state != ENodeState.failed && !isValidateIndex(_activityIndex))
                return false;

            return true;
        }
        protected override ENodeState onUpdate()
        {
            if (isRunning() && isValidateIndex(_activityIndex))
            {
                var child = Get(_activityIndex);
                if (child.Evaluate())
                    _state = child.Update();

                if (_state == ENodeState.running)
                    return ENodeState.running;

                if (_state == ENodeState.success)
                    return _state;

                if (_state == ENodeState.failed)
                {
                    if (isValidateIndex(_activityIndex + 1))
                        _activityIndex++;
                    else
                        return _state;
                }

            }
            else
            {
                _activityIndex = 0;
            }


            for (int i = _activityIndex; i < _children.Count; ++i)
            {
                _activityIndex = i;
                var child = _children[i];
                _state = child.Exec();
                if (_state != ENodeState.failed)
                    break;
            }

            return _state;
        }
    }

}
