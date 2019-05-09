using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{
    class CON_GBTCanFight : GBTCondition
    {
        public override bool IsTrue()
        {
            int targetId = _bb.GetInt("bbTargetId");
            return base.IsTrue();
        }
    }

    class CON_DistanceUseSkill : GBTCondition
    {
        public override bool IsTrue()
        {
            int selfId = _bb.GetInt("bbSelfId");
            int targetId = _bb.GetInt("bbTargetId");
            int skillId = _bb.GetInt("bbSkillId");
            return base.IsTrue();
        }
    }

    class ACT_CalcSkillId:GBTLeafNode
    {
        protected override ENodeState onUpdate()
        {
            int skillId = calcSkillId();
            _bb.SetInt("bbSkillId", skillId);
            logDebug($"Calc skillId {skillId}");
            return base.onUpdate();
        }
        protected override void onTransition()
        {
            base.onTransition();
        }
        private int calcSkillId()
        {
            return 123;
        }
    }

    class ACT_MovTo : GBTLeafNode
    {
        private int _testMoveCnt = 10;
        protected override bool onEvaluate()
        {
            return base.onEvaluate();
        }

        protected override ENodeState onUpdate()
        {
            int targetId = _bb.GetInt("bbTargetId");
            int selfId = _bb.GetInt("bbSelfId");
            int dis = _bb.GetInt("bbDisToTargetId");
            moveTo(targetId);
            if (isMoveEnd(selfId, targetId))
                _state = ENodeState.success;
            else
                _state = ENodeState.running;
            return _state;
        }

        protected override void onTransition()
        {
            _testMoveCnt = 10;
            base.onTransition();
        }

        private void moveTo(int targetId)
        {
            logDebug($"move to {targetId}");
        }
        private bool isMoveEnd(int selfId,int targetId)
        {
            _testMoveCnt--;
            return _testMoveCnt <= 0;
        }
    }

    class ACT_UseSkill:GBTLeafNode
    {
        private int _testUseSkill = 3;
        protected override bool onEvaluate()
        {
            return base.onEvaluate();
        }

        protected override ENodeState onUpdate()
        {
            logDebug("Using skill");
            _testUseSkill--;
            if (_testUseSkill > 0)
                _state = ENodeState.running;
            else
                _state = ENodeState.success;
            return _state;
        }

        protected override void onTransition()
        {
            _testUseSkill = 3;
            base.onTransition();
        }
    }
    // 战斗
    class GBTFight : GBTSequence
    {
        public static GBTFight Create()
        {
            var gbtFight = new GBTFight();

            gbtFight.Add(new CON_GBTCanFight())
                .Add(new ACT_CalcSkillId())
                .Add(new ACT_MovTo().SetPreCondition(new CON_DistanceUseSkill()))
                .Add(new ACT_UseSkill())
                ;

            return gbtFight;
        }
    }



    class CON_GBTCanIdle: GBTCondition
    {
        private int _testCanIdle = 3;
        public override bool IsTrue()
        {
            if(_testCanIdle-- > 0)
            {
                return true;
            }
            else
            {
                _state = ENodeState.failed;
                return false;
            }
        }

        protected override void onTransition()
        {
            //_testCanIdle = 3;
            base.onTransition();
        }
    }

    class ACT_Idle:GBTLeafNode
    {
        protected override ENodeState onUpdate()
        {
            logDebug("In Idle");
            return base.onUpdate();
        }
    }

    // idle
    class GBTIdle : GBTSequence
    {
        public static GBTIdle Create()
        {
            var gbtIdle = new GBTIdle();
            gbtIdle.Add(new CON_GBTCanIdle())
                .Add(new ACT_Idle())
                ;
            return gbtIdle;
        }
    }
}
