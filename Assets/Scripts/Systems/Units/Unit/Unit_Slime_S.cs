namespace HDU.GameSystem
{
    using UnityEngine;
    using static HDU.Define.CoreDefine;

    public class Unit_Slime_S : Unit
    {
        public override void Initialize(Vector3 pos)
        {
            base.Initialize(pos);
            UnitType = EUnitType.Spike;
            MaxHP = 10;
            HP = MaxHP;
            AttackPower = 1;
            AttackRange = 0.5f;
            MoveSpeed = 0.5f;
        }

        public override void RegisterStates()
        {
            _fsm.AddState(EUnitState.Idle, new CommonIdleState());
            _fsm.AddState(EUnitState.Move, new CommonMoveState());
            _fsm.AddState(EUnitState.Attack, new CommonAttackState());
            _fsm.AddState(EUnitState.Dead, new CommonDieState());
        }
    }
}