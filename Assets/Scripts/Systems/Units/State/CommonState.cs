namespace HDU.GameSystem
{
    using Cysharp.Threading.Tasks;
    using HDU.Define;
    using HDU.Interface;
    using HDU.Managers;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine;
    using static HDU.Define.CoreDefine;

    public class CommonIdleState : IState
    {
        public float _elapsedTime { get; set; } = 0f;
        public Action<CoreDefine.EUnitState> ChangeStateCallback { get; set; }

        public void OnEnter(IUnit unit)
        {
            _elapsedTime = 0f;
            // �ִϸ��̼� ����
            unit.PlayAnimation(nameof(EAnimationKey.IdleBlend));
        }

        public void OnUpdate(IUnit unit, float deltaTime)
        {
            // ��� ���� ����
            IUnit target = Managers.Unit.FindNearstTargetOrNull(unit);

            if (target == null)
                return;

            var targetNode = Managers.Grid.GetNodeFromWorldPos(target.Transform.position, Managers.Grid.GridSize, Managers.Grid.CellSize);
            if (targetNode == null || !targetNode.IsWalkable)
                return;

            // Ÿ���� �����ϸ� �̵� ���·� ��ȯ
            unit.Target = target;

            ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Move);
        }

        public void OnExit(IUnit unit)
        {
            // ��� ���� ���� ����
        }

        public void RegistChangeStateCallback(Action<CoreDefine.EUnitState> callback) => ChangeStateCallback = callback;
    }

    public class CommonMoveState : IState, IDisposable
    {
        public float _elapsedTime { get; set; } = 0f;
        private float _repathTimer = 0f;
        private float _repathInterval = 0.5f;
        private float _refindTargetTimer = 0f;
        private float _refindTargetInterval = 1f;
        private Vector3 _targetLastPos = Vector3.zero;
        public Action<CoreDefine.EUnitState> ChangeStateCallback { get; set; }
        List<Node> _path = null;

        private CancellationTokenSource _moveCts;

        public async void OnEnter(IUnit unit)
        {
            _elapsedTime = 0f;
            // �ִϸ��̼� ����
            unit.PlayAnimation(nameof(EAnimationKey.Move));
            // ������ ����
            _targetLastPos = unit.Target.Transform.position;

            try
            {
                await MovePath(unit);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async void OnUpdate(IUnit unit, float deltaTime)
        {
            _elapsedTime += deltaTime;
            _repathTimer += deltaTime;
            _refindTargetTimer += deltaTime;

            if (!unit.Target.IsVaild)
            {
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            if (_refindTargetTimer >= _refindTargetInterval)
            {
                _refindTargetTimer = 0;
                IUnit target = Managers.Unit.FindNearstTargetOrNull(unit);
                if(unit.Target != target)
                {
                    unit.Target = target;
                    _targetLastPos = unit.Target.Transform.position;
                    _moveCts?.Cancel();
                    try
                    {
                        await MovePath(unit);
                    }
                    catch (Exception)
                    {
                    }
                    return;
                }
            }

            // ��Ÿ� üũ
            float distanceToTarget = Vector3.Distance(unit.Transform.position, unit.Target.Transform.position);
            if(distanceToTarget < unit.AttackRange)
            {
                _moveCts?.Cancel();
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Attack);
                return;
            }

            if(_targetLastPos != unit.Target.Transform.position)
            {
                if (_repathTimer >= _repathInterval)
                {
                    _repathTimer = 0f;

                    _targetLastPos = unit.Target.Transform.position;
                    _moveCts?.Cancel();
                    try
                    {
                        await MovePath(unit);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
        }

        private async UniTask MovePath(IUnit unit)
        {
            _moveCts = new CancellationTokenSource();

             var path = Managers.AStar.FindPath(unit.Transform.position, unit.Target.Transform.position, unit.GetRadius());

            if (path == null || path.Count == 0)
            {
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            foreach (var node in path)
            {
                if (!unit.Target.IsVaild)
                {
                    ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                    return;
                }

                Vector3 targetPos = new Vector3(node.WorldPos.x, unit.Transform.position.y, node.WorldPos.z);
                
                // �ʹ� ����� ���� ����
                if ((unit.Transform.position - targetPos).sqrMagnitude < 0.001f)
                    continue;

                // ���� ȸ��
                Vector3 dir = (targetPos - unit.Transform.position).normalized;
                if(dir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    unit.Transform.rotation = Quaternion.RotateTowards(unit.Transform.rotation, targetRot, 720 * Time.fixedDeltaTime); // �ʴ� 720�� ȸ��
                }

                // �������� ������ ������ �̵�
                while((unit.Transform.position - targetPos).sqrMagnitude > 0.01f)
                {
                    if (!unit.Target.IsVaild)
                    {
                        ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                        return;
                    }

                    _moveCts.Token.ThrowIfCancellationRequested();

                    if(dir != Vector3.zero)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dir);
                        unit.Transform.rotation = Quaternion.RotateTowards(unit.Transform.rotation, targetRot, 720 * Time.fixedDeltaTime); // �ʴ� 720�� ȸ��
                    }

                    Vector3 nextPos = Vector3.MoveTowards(unit.Transform.position, targetPos, unit.MoveSpeed * Time.fixedDeltaTime);
                    unit.Transform.position = nextPos;

                    await UniTask.WaitForFixedUpdate(); // �����Ӹ��� ���
                }
            }
        }

        public void OnExit(IUnit unit)
        {
            // �̵� ���� ����
            _moveCts?.Cancel();
        }

        public void RegistChangeStateCallback(Action<CoreDefine.EUnitState> callback) => ChangeStateCallback = callback;

        public void Dispose()
        {
            _moveCts?.Cancel();
            _moveCts?.Dispose();
            _moveCts = null;
        }
    }

    public class CommonAttackState : IState
    {
        public float _elapsedTime { get; set; } = 0f;
        public Action<CoreDefine.EUnitState> ChangeStateCallback { get; set; }
        private bool _isAttacking = false;

        public void OnEnter(IUnit unit)
        {
            _elapsedTime = 0f;
            _isAttacking = false;
            // �ִϸ��̼� ����
            unit.PlayAnimation(nameof(EAnimationKey.IdleBlend));
            // ���� ��� ����
        }

        public async void OnUpdate(IUnit unit, float deltaTime)
        {
            _elapsedTime += deltaTime;

            if (!unit.Target.IsVaild)
            {
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            // ��Ÿ� üũ
            float distanceToTarget = Vector3.Distance(unit.Transform.position, unit.Target.Transform.position);
            if (distanceToTarget < unit.AttackRange)
            {
                if (!_isAttacking)
                {
                    _isAttacking = true;
                    try
                    {
                        await StartAttackAnim(0.5f, 1f, unit);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    _isAttacking = false;
                }
            }
            else
            {
                ChangeStateCallback?.Invoke(EUnitState.Idle);
            }
        }

        public async UniTask StartAttackAnim(float wait, float duration, IUnit unit)
        {
            if (!unit.Target.IsVaild)
            {
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            var token = unit.GameObject.GetCancellationTokenOnDestroy();

            unit.PlayAnimation(nameof(EAnimationKey.AttackBlend));
            await UniTask.Delay(TimeSpan.FromSeconds(wait), cancellationToken: token);

            if (!unit.Target.IsVaild)
            {
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            unit.Target.Target = unit; // ���� ����� ���� ����� Ÿ������ ����
            unit.Target.TakeDamage(unit.AttackPower);
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);

        }

        public void OnExit(IUnit unit)
        {
        }
        public void RegistChangeStateCallback(Action<CoreDefine.EUnitState> callback) => ChangeStateCallback = callback;
    }

    public class CommonDieState : IState
    {
        public Action<EUnitState> ChangeStateCallback { get; set; }
        public float _elapsedTime { get; set; }

        public async void OnEnter(IUnit unit)
        {
            _elapsedTime = 0f;
            try
            {
                await StartDieAnim(0.1f, 2f, unit);
            }
            catch (OperationCanceledException)
            {
            }
        }
        public void OnUpdate(IUnit unit, float deltaTime)
        {
        }

        public void OnExit(IUnit unit)
        {
        }

        public async UniTask StartDieAnim(float wait, float duration, IUnit unit)
        {
            var token = unit.GameObject.GetCancellationTokenOnDestroy();

            await UniTask.Delay(TimeSpan.FromSeconds(wait), cancellationToken: token);
            unit.PlayAnimation(nameof(EAnimationKey.Die));
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);

            if (unit is IDisposable disposableState)
                disposableState.Dispose(); // ���� ��ü�� IDisposable�̸� ȣ��

            Managers.Unit.DespawnUnit(unit);
        }

        public void RegistChangeStateCallback(Action<EUnitState> callback) => ChangeStateCallback = callback;
    }
}