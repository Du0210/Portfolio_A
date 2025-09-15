namespace HDU.GameSystem
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using static HDU.Define.CoreDefine;
    using HDU.Interface;

    public class FSM : IDisposable
    {
        private Dictionary<EUnitState, IState> _stateDict = new Dictionary<EUnitState, IState>();

        private IState _currentState;
        private EUnitState _currentStateType;

        private IUnit _unit;

        public FSM(IUnit unit)
        {
            _unit = unit;
        }

        public void AddState(EUnitState type, IState state)
        {
            if (!_stateDict.ContainsKey(type))
            {
                state.RegistChangeStateCallback(ChangeState);
                _stateDict.Add(type, state);
            }
        }

        public void ChangeState(EUnitState newState)
        {
            if (_currentStateType == newState || !_stateDict.ContainsKey(newState))
                return;

            _currentState?.OnExit(_unit);
            _currentState = _stateDict[newState];
            _currentStateType = newState;
            _currentState?.OnEnter(_unit);
        }

        public void Dispose()
        {

            foreach (var state in _stateDict.Values)
            {
                if (state is IDisposable disposableState)
                {
                    disposableState.Dispose(); // 상태 객체가 IDisposable이면 호출
                }
            }

            _stateDict.Clear();
            _currentState = null;
        }

        public void Update(IUnit unit, float deltaTime)
        {
            _currentState?.OnUpdate(unit, deltaTime);
        }
    }
}