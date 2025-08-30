namespace HDU.GameSystem
{
    using System;
    using UnityEngine;
    //using Spine.Unity;

    public class GFX : MonoBehaviour, IDisposable
    {
        public enum EGFXType
        {
            Animator,
            Spine,
            Sprite,
        }
        //[SerializeField] SkeletonAnimation SpineGFX;
        public SpriteRenderer SRGFX;
        [SerializeField] private EGFXType _gfxType;
        [SerializeField] public Animator Animator;

        public void Initialize(EGFXType type)
        {
            _gfxType = type;
            switch (type)
            {
                case EGFXType.Animator:
                    Animator = GetComponent<Animator>();
                    break;
                case EGFXType.Spine:
                    //SpineGFX = GetComponent<SkeletonAnimation>();
                    break;
                case EGFXType.Sprite:
                    SRGFX = GetComponent<SpriteRenderer>();
                    break;
                default:
                    Debug.LogError("GFX Is Null");
                    break;
            }
        }
        //public SkeletonAnimation GetSpineGFX()
        //{
        //    if (SpineGFX == null)
        //    {
        //        Debug.LogError("Spine is null");
        //        return null;
        //    }
        //    return SpineGFX;
        //}

        public SpriteRenderer GetSRGFX()
        {
            if (SRGFX == null)
            {
                Debug.LogError("SpriteRenderer is null");
                return null;
            }
            return SRGFX;
        }

        public Animator GetAnimator()
        {
            if (Animator == null)
            {
                Debug.LogError("Animator is null");
                return null;
            }
            return Animator;
        }

        public void Dispose()
        {
            if (SRGFX != null)
            {
                //Destroy(SRGFX);
                SRGFX = null;
            }
            if (Animator != null)
            {
                //Destroy(Animator);
                Animator = null;
            }
            //if (SpineGFX != null)
            //{
            //    Destroy(SpineGFX);
            //    SpineGFX = null;
            //}
            GC.SuppressFinalize(this);
        }
    }
}