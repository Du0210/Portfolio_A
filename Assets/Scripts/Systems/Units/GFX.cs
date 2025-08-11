using UnityEngine;
//using Spine.Unity;

public class GFX : MonoBehaviour
{
    //[SerializeField] SkeletonAnimation SpineGFX;
    [SerializeField] SpriteRenderer SRGFX;

    private void Awake()
    {
        //if (SpineGFX == null)
        //    SpineGFX = GetComponent<SkeletonAnimation>();
        //else if (SRGFX == null)
            SRGFX = GetComponent<SpriteRenderer>();
        //else
        //    Debug.LogError("GFX Init Fail");
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
}
