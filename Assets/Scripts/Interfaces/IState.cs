using System;
using UnityEngine;
namespace HDU.Interface
{
    public interface IState
    {
        public Action<Define.CoreDefine.EUnitState> ChangeStateCallback { get; set; }
        public float _elapsedTime { get; set; }
        public void OnEnter(IUnit unit);
        public void OnUpdate(IUnit unit, float deltaTime);
        public void OnExit(IUnit unit);
        public void RegistChangeStateCallback(Action<Define.CoreDefine.EUnitState> callback);
    }
}