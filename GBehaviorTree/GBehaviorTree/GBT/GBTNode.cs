using log4net;
using System;
using System.Collections.Generic;
using System.IO;

class LogConfig
{
    private static string _logRepositoryName = "GBTLog";
    public static ILog GetLog(Type type)
    {
        return LogManager.GetLogger(_logRepositoryName, type);
    }
    public static void InitLog()
    {
        GlobalContext.Properties["FolderName"] = AppDomain.CurrentDomain.BaseDirectory;
        GlobalContext.Properties["FileName"] = "GBT.log";
        //string fileName = AppDomain.CurrentDomain.BaseDirectory + "config.log4net";
        string fileName = Environment.CurrentDirectory + "/config.log4net";
        var rep = log4net.LogManager.CreateRepository(_logRepositoryName);
        var ret = log4net.Config.XmlConfigurator.ConfigureAndWatch(rep, new FileInfo(fileName));
    }
}

namespace GBT
{
    public class BlackBoard
    {
        private Dictionary<string, int> _bbInt;
        public BlackBoard()
        {
            _bbInt = new Dictionary<string, int>();
        }
        public int GetInt(string key)
        {
            if (!_bbInt.ContainsKey(key))
                return  -1;

            int ret = _bbInt[key];
            return ret;
        }
        public void SetInt(string key,int value)
        {
            if (_bbInt.ContainsKey(key))
                _bbInt[key] = value;
            else
                _bbInt.Add(key, value);
        }
        public void Clear()
        {
            if (null != _bbInt)
                _bbInt.Clear();
            Data = null;
        }
        public GBTNodeData Data { get; set; }
    }

    public class GBTNodeData
    {

    }

    // 行为树节点
    public class GBTNode
    {
        protected static ILog log = LogConfig.GetLog(typeof(GBTNode));
        public enum ENodeState
        {
            invalid = 0, // 无效节点
            init = 1,    // 初始化完成可以update了，设置节点所需数据
            failed = 2,  // 运行失败, 如条件节点返回false
            running = 3, // 正在运行
            success = 4, // 运行成功，如条件节点返回true
        }

        protected GBTNode _parent;
        protected BlackBoard _bb;
        protected ENodeState _state;
        protected string _debugKey;
        protected GBTCondition _preCondition;


        public ENodeState State { get { return _state; } }
        public virtual void SetKey (string key,int index=1)
        {
            if(null != _parent)
                _debugKey = $"{key}({this})-{index}";
            else
                _debugKey = key;

            if (this is GBTCtrNode)
            {
                var ctr = this as GBTCtrNode;
                if (ctr.GetCurChildCount() > 0)
                {
                    int num = 1;
                    ctr.ForEach(node =>
                    {
                        node.SetKey(_debugKey, num);
                        num++;
                        return true;
                    });
                }
            }

        }
        public string GetKey() { return _debugKey; }
        public GBTNode()
        {
            _state = ENodeState.init;
            _parent = null;
        }

        public GBTNode SetPreCondition(GBTCondition pre)
        {
            _preCondition = pre;
            return this;
        }
        public virtual void SetBB(BlackBoard bb)
        {
            _bb = bb;
            if (null != _preCondition)
                _preCondition.SetBB(bb);
        }
        public BlackBoard GetBB()
        {
            return _bb;
        }

       // 评估节点是否可以执行
        // 节点执行条件和所需数据是否正确
        public bool Evaluate()
        {
            //logDebug("Evaluate");
            return onEvaluate();
        }
        public ENodeState Update()
        {
            logDebug("Update");
            _state = onUpdate();
            return _state;
        }
        public void Transition()
        {
            //logDebug("Transition");
            onTransition();
            _state = ENodeState.init;
        }
        public void SetParent(GBTNode parent)
        {
            _parent = parent;
            SetKey(parent.GetKey());
        }
        public ENodeState Exec()
        {
            if (Evaluate())
                Update();
            return _state;
        }
        protected bool isRunning() { return _state == ENodeState.running; }
        protected void logDebug(string str)
        {
            log.Debug($"{_debugKey}-{str}");
            //Console.WriteLine($"{_debugKey}-{str}");
        }
        protected virtual bool onEvaluate()
        {
            if (_preCondition != null && false == _preCondition.IsTrue())
            {
                _state = ENodeState.failed;
                return false;
            }
            else
            {
                _state = ENodeState.success;
                return true;
            }
        }

        // 更新节点
        protected virtual ENodeState onUpdate()
        {
            _state = ENodeState.success;
            return _state;
        }

        // 节点退出清理
        protected virtual void onTransition()
        {
            _state = ENodeState.init;
        }
    }

  


    // 行为树
    public class GBehaviorTree
    {
        private GBTNode _root;
        private BlackBoard _bb;

        public GBehaviorTree()
        {
            _bb = new BlackBoard();
            _root = null;
        }
        public void SetCurrentTree(GBTNode tree)
        {
            _root = tree;
            _root.SetBB(_bb);
            _root.SetKey("root");
        }
        public void Update()
        {
            if(null != _root)
            {
                if (_root.Evaluate())
                {
                    _root.Update();
                }
                else
                {
                    _root.Transition();
                    _root.Update();
                }
            }
        }
    }

    
}
