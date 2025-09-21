namespace HDU.GameSystem
{
    using HDU.Define;
    using HDU.Interface;
    using System;
    using UnityEngine;

    public abstract class Unit : MonoBehaviour, IUnit, IDisposable
    {
        #region Property
        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public float AttackRange { get => _attackRange; set => _attackRange = value; }
        public float AttackPower { get => _attackPower; set => _attackPower = value; }
        public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }
        public float MaxHP { get => _maxHP; set => _maxHP = value; }
        public float HP { get => _hp; set => _hp = value; }

        public bool IsVaild => this != null && !(this as UnityEngine.Object).Equals(null) &&
            gameObject != null && gameObject.activeInHierarchy && gameObject.scene.IsValid();

        public GameObject GameObject => base.gameObject;

        public Transform Transform => base.transform;
        public Rigidbody Rigidbody => _rigidbody;

        public IUnit Target { get; set; }
        public CoreDefine.EUnitType UnitType { get => _unitType; set => _unitType = value; }

        public Collider Collider => _collder;
        public CoreDefine.EUnitPrefabKey UnitPrefabKey { get => _unitPrefabKey; }

        #endregion
        [SerializeField] private CoreDefine.EUnitPrefabKey _unitPrefabKey;
        [SerializeField] private CoreDefine.EUnitType _unitType; 

        public GFX @GFX;

        protected FSM _fsm;

        private float _moveSpeed;
        private float _attackRange;
        private float _attackPower;
        private float _attackSpeed;
        private float _maxHP;
        private float _hp;
        private Rigidbody _rigidbody;
        private Collider _collder;

        #region Unity Event Function
        protected virtual void Awake()
        {



        }

        protected virtual void FixedUpdate()
        {
            _fsm.Update(this, Time.fixedDeltaTime);
            CheckUnitFallDebug();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }
        #endregion
        public virtual void Initialize(Vector3 pos)
        {
            GFX ??= GetComponentInChildren<GFX>();
            GFX.Initialize(GFX.EGFXType.Animator);
            _rigidbody ??= GetComponent<Rigidbody>();
            _collder ??= GetComponent<Collider>();

            _fsm = new FSM(this);
            RegisterStates();
            _fsm.ChangeState(CoreDefine.EUnitState.Idle);

            transform.position = pos;
        }

        public void Dispose()
        {
            _fsm?.Dispose();
            _fsm = null;
            GFX?.Dispose();
            GFX = null;
            _rigidbody = null;
            _collder = null;
        }

        public abstract void RegisterStates();

        public virtual void PlayAnimation(string animationName)
        {
            GFX.GetAnimator().Play(animationName, 0, 0);
        }

        public void TakeDamage(float dmg)
        {
            HP -= dmg;
            if (HP <= 0)
            {
                HP = 0;
                // Die
                _fsm.ChangeState(CoreDefine.EUnitState.Dead);
            }
        }
        public float GetRadius()
        {
            return Collider.bounds.extents.x;
        }

        private void CheckUnitFallDebug()
        {
            if (Transform.position.y < -100f)
            {
                Debug.LogWarning($"{name} fell below -100 on Y axis. Resetting position.");
                _rigidbody.linearVelocity = Vector3.zero;
                Managers.Managers.Unit.DespawnUnit(this);
            }
        }
    }
}