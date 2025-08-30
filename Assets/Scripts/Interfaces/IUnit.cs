using UnityEngine;
namespace HDU.Interface
{
    public interface IUnit
    {
        public float MoveSpeed { get; set; }
        public float AttackRange { get; set; }
        public float AttackPower { get; set; }
        public float AttackSpeed { get; set; }
        public float MaxHP { get; set; }
        public float HP { get; set; }
        public bool IsVaild { get; }
        public GameObject @GameObject { get; }
        // �Ŵ������� �����ϱ� ���� ���� Transform �Ӽ�
        public Transform @Transform { get; }
        public Rigidbody @Rigidbody { get; }
        public Collider @Collider { get; }
        public IUnit Target { get; set; }
        public HDU.Define.CoreDefine.EUnitType UnitType { get; set; }

        public void RegisterStates();
        public void PlayAnimation(string animationName);
        public void TakeDamage(float dmg);
        public void Initialize(Vector3 pos);
        public float GetRadius();
    }
}