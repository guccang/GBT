using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{
    // 优先级的选择节点
    class CTR_PrioritizedSelector : GBTSelector
    {
        // child索引对应的优先级
        private Dictionary<int, int> _indexPriority;
        private HashSet<int> _usedIndex;

        public CTR_PrioritizedSelector()
        {
            _indexPriority = new Dictionary<int, int>();
            _usedIndex = new HashSet<int>();
        }
        // 优先级，越大优先级越高
        public CTR_PrioritizedSelector Add(GBTNode child,int weight)
        {
            int index = _children.Count;
            if(null != Add(child))
                _indexPriority.Add(index, weight);
            return this;
        }
        public new CTR_PrioritizedSelector SetName(string name)
        {
            base.SetName(name);
            return this;
        }
     
        protected override ENodeState onUpdate()
        {
            if(false == isValidateIndex(_activityIndex))
            {
                _activityIndex = calcPriorityIndex();
                if(false == isValidateIndex(_activityIndex))
                {
                    _state = ENodeState.success;
                    return _state;
                }
            }

            for (int i = _usedIndex.Count; i < _children.Count; ++i)
            {
                var child = _children[_activityIndex];
                _state = child.Exec();

                if (_state == ENodeState.running)
                    return _state;

                if (_state == ENodeState.success)
                    break;

                _activityIndex = calcPriorityIndex();
                if(false == isValidateIndex(_activityIndex))
                {
                    _state = ENodeState.success;
                    break;
                }
            }

            if(_state == ENodeState.failed)
            {
                _activityIndex = calcPriorityIndex();
                if(isValidateIndex(_activityIndex))
                {
                    _state = ENodeState.running;
                }
            }

            if (_children.Count <= 0)
                _state = ENodeState.success;

            return _state;
        }

        protected override void onTransition()
        {
            _usedIndex.Clear();
            base.onTransition();
        }

        private int calcPriorityIndex()
        {
            int index = -1;
            int totalWeight = 0;
            foreach (var priority in _indexPriority)
            {
                if(false == _usedIndex.Contains(priority.Key))
                    totalWeight += priority.Value;
            }

            int step = RandomHelp.Next(totalWeight);
            var keys = new List<int>(_indexPriority.Keys);
            int incWeight = 0;
            for(int i=0; i< keys.Count; ++i)
            {
                int key = keys[i];
                if ( _usedIndex.Contains(key))
                    continue;

                incWeight += _indexPriority[key];
                if (step <= incWeight)
                {
                    index = key;
                    _usedIndex.Add(index);
                    break;
                }
            }
            return index;
        }

    }
}
