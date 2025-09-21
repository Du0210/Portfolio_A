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
    using static UnityEngine.GraphicsBuffer;

    public class CommonIdleState : IState
    {
        public float _elapsedTime { get; set; } = 0f;
        public Action<CoreDefine.EUnitState> ChangeStateCallback { get; set; }

        public void OnEnter(IUnit unit)
        {
            _elapsedTime = 0f;
            // 애니메이션 세팅
            unit.PlayAnimation(nameof(EAnimationKey.IdleBlend));
        }

        public void OnUpdate(IUnit unit, float deltaTime)
        {
            // 대기 관련 로직
            if(!Managers.IsUseJob)
            {
                IUnit target = Managers.Unit.FindNearstTargetOrNull(unit);

                if (target == null)
                    return;

                var targetNode = Managers.Grid.GetNodeFromWorldPos(target.Transform.position, Managers.Grid.GridSize, Managers.Grid.CellSize);
                if (targetNode == null || !targetNode.IsWalkable)
                    return;

                // 타겟이 존재하면 이동 상태로 전환
                unit.Target = target;
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Move);
            }
            else
            {
                if (unit.Target != null)
                    ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Move);
            }
        }

        public void OnExit(IUnit unit)
        {
            // 대기 상태 종료 로직
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
        private IUnit _target = null;
        public Action<CoreDefine.EUnitState> ChangeStateCallback { get; set; }
        List<Node> _path = null;

        private CancellationTokenSource _moveCts;
        private CancellationToken _destroyToken;

        public async void OnEnter(IUnit unit)
        {
            _elapsedTime = 0f;
            // 애니메이션 세팅
            unit.PlayAnimation(nameof(EAnimationKey.Move));
            // 목적지 세팅
            _targetLastPos = unit.Target.Transform.position;
            _target = unit.Target;
            _destroyToken = unit.GameObject.GetCancellationTokenOnDestroy();

            try
            {
                if (!Managers.IsUseJob)
                    await MovePath(unit);
                else
                    await MovePathJob(unit);
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
                _moveCts?.Cancel();
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            if (_refindTargetTimer >= _refindTargetInterval)
            {
                _refindTargetTimer = 0;

                if(!Managers.IsUseJob)
                {
                    IUnit target = Managers.Unit.FindNearstTargetOrNull(unit);
                    if (unit.Target != target)
                    {
                        unit.Target = target;
                        _target = target;
                        _targetLastPos = unit.Target.Transform.position;
                        _moveCts?.Cancel();
                        try
                        {
                            if (!Managers.IsUseJob)
                                await MovePath(unit);
                            else
                                await MovePathJob(unit);
                        }
                        catch (Exception)
                        {
                        }
                        return;
                    }
                }
                else
                {
                    if(_target != unit.Target)
                    {
                        _target = unit.Target;
                        _targetLastPos = _target.Transform.position;
                        _moveCts?.Cancel();
                        try
                        {
                            if (!Managers.IsUseJob)
                                await MovePath(unit);
                            else
                                await MovePathJob(unit);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            try
            {
                _destroyToken.ThrowIfCancellationRequested();

                // 사거리 체크
                float distanceToTarget = Vector3.Distance(unit.Transform.position, _target.Transform.position);
                if (distanceToTarget < unit.AttackRange)
                {
                    _moveCts?.Cancel();
                    ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Attack);
                    return;
                }

                if (_targetLastPos != _target.Transform.position)
                {
                    if (_repathTimer >= _repathInterval)
                    {
                        _repathTimer = 0f;

                        _targetLastPos = _target.Transform.position;
                        _moveCts?.Cancel();

                        if (!Managers.IsUseJob)
                            await MovePath(unit);
                        else
                            await MovePathJob(unit);
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }


        }

        private async UniTask MovePath(IUnit unit)
        {
            _moveCts = new CancellationTokenSource();

            var path = Managers.AStar.FindPath(unit.Transform.position, _target.Transform.position, unit.GetRadius());

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
                
                // 너무 가까운 노드는 무시
                if ((unit.Transform.position - targetPos).sqrMagnitude < 0.001f)
                    continue;

                // 방향 회전
                Vector3 dir = (targetPos - unit.Transform.position).normalized;
                if(dir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    unit.Transform.rotation = Quaternion.RotateTowards(unit.Transform.rotation, targetRot, 720 * Time.fixedDeltaTime); // 초당 720도 회전
                }

                // 목적지에 도달할 때까지 이동
                while((unit.Transform.position - targetPos).sqrMagnitude > 0.01f)
                {
                    if (!unit.Target.IsVaild)
                    {
                        ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                        return;
                    }

                    if(dir != Vector3.zero)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dir);
                        unit.Transform.rotation = Quaternion.RotateTowards(unit.Transform.rotation, targetRot, 720 * Time.fixedDeltaTime); // 초당 720도 회전
                    }

                    Vector3 nextPos = Vector3.MoveTowards(unit.Transform.position, targetPos, unit.MoveSpeed * Time.fixedDeltaTime);
                    unit.Transform.position = nextPos;

                    await UniTask.WaitForFixedUpdate(); // 프레임마다 대기
                    _destroyToken.ThrowIfCancellationRequested();
                }
            }
        }

        private async UniTask MovePathJob(IUnit unit)
        {
            _moveCts = new CancellationTokenSource();
            var token = unit.GameObject.GetCancellationTokenOnDestroy();

            var path = await Managers.AStar.FindPathAsync(unit.Transform.position, _target.Transform.position, unit.GetRadius());
            _destroyToken.ThrowIfCancellationRequested();

            if (path == null || path.Count == 0)
            {
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            foreach (var targetPos in path)
            {
                // 방향 회전
                Vector3 dir = (targetPos - unit.Transform.position).normalized;
                if (dir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    unit.Transform.rotation = Quaternion.RotateTowards(unit.Transform.rotation, targetRot, 720 * Time.fixedDeltaTime); // 초당 720도 회전
                }

                // 목적지에 도달할 때까지 이동
                while ((unit.Transform.position - targetPos).sqrMagnitude > 0.01f)
                {
                    if (!unit.Target.IsVaild)
                    {
                        ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                        return;
                    }
                    if (dir != Vector3.zero)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dir);
                        unit.Transform.rotation = Quaternion.RotateTowards(unit.Transform.rotation, targetRot, 720 * Time.fixedDeltaTime); // 초당 720도 회전
                    }

                    Vector3 nextPos = Vector3.MoveTowards(unit.Transform.position, targetPos, unit.MoveSpeed * Time.fixedDeltaTime);
                    unit.Transform.position = nextPos;
                    
                    await UniTask.WaitForFixedUpdate(); // 프레임마다 대기
                    token.ThrowIfCancellationRequested();
                    _moveCts?.Token.ThrowIfCancellationRequested();
                }
            }
        }

        public void OnExit(IUnit unit)
        {
            // 이동 종료 로직
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

    public class CommonAttackState : IState, IDisposable
    {
        public float _elapsedTime { get; set; } = 0f;
        public Action<CoreDefine.EUnitState> ChangeStateCallback { get; set; }
        private bool _isAttacking = false;
        private CancellationTokenSource _atkCts;

        public void OnEnter(IUnit unit)
        {
            _elapsedTime = 0f;
            _isAttacking = false;
            // 애니메이션 세팅
            unit.PlayAnimation(nameof(EAnimationKey.IdleBlend));
            // 공격 대상 설정
        }

        public async void OnUpdate(IUnit unit, float deltaTime)
        {
            _elapsedTime += deltaTime;

            if (!unit.Target.IsVaild)
            {
                _atkCts?.Cancel();
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            // 사거리 체크
            float distanceToTarget = Vector3.Distance(unit.Transform.position, unit.Target.Transform.position);
            if (distanceToTarget < unit.AttackRange)
            {
                if (!_isAttacking)
                {
                    _isAttacking = true;
                    try
                    {
                        _atkCts?.Cancel();
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
                _atkCts?.Cancel();
                ChangeStateCallback?.Invoke(EUnitState.Idle);
            }
        }

        public async UniTask StartAttackAnim(float wait, float duration, IUnit unit)
        {
            _atkCts = new CancellationTokenSource();
            if (!unit.Target.IsVaild)
            {
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            var token = unit.GameObject.GetCancellationTokenOnDestroy();
            _atkCts?.Token.ThrowIfCancellationRequested();

            unit.PlayAnimation(nameof(EAnimationKey.AttackBlend));
            await UniTask.Delay(TimeSpan.FromSeconds(wait), cancellationToken: token);

            _atkCts?.Token.ThrowIfCancellationRequested();
            if (!unit.Target.IsVaild)
            {
                ChangeStateCallback?.Invoke(CoreDefine.EUnitState.Idle);
                return;
            }

            unit.Target.Target = unit; // 맞은 대상이 때린 대상을 타겟으로 설정
            unit.Target.TakeDamage(unit.AttackPower);
            Managers.Audio.Play(Define.CoreDefine.ESoundKey.Attack, Define.CoreDefine.ESoundType.FX, volumeRatio: 1f);
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            _atkCts?.Token.ThrowIfCancellationRequested();
        }

        public void OnExit(IUnit unit)
        {
        }
        public void RegistChangeStateCallback(Action<CoreDefine.EUnitState> callback) => ChangeStateCallback = callback;

        public void Dispose()
        {
            _atkCts?.Cancel();
            _atkCts?.Dispose();
            _atkCts = null;
        }
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
            Managers.Audio.Play(Define.CoreDefine.ESoundKey.Die, Define.CoreDefine.ESoundType.FX, volumeRatio: 1f);
            unit.PlayAnimation(nameof(EAnimationKey.Die));
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);

            if (unit is IDisposable disposableState)
                disposableState.Dispose(); // 상태 객체가 IDisposable이면 호출

            Managers.Unit.DespawnUnit(unit);
        }

        public void RegistChangeStateCallback(Action<EUnitState> callback) => ChangeStateCallback = callback;
    }
}