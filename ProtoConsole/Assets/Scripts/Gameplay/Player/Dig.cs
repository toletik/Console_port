using UnityEngine;

public class Dig : ChangeHeightCapacity
{
    [SerializeField] private bool ejectionImmunityWhileInGround = true;

    protected override Capacity Type => Capacity.DIG;

    protected override bool LookToStartAction()
    {
        if (player.IsUsingCapacity(Capacity.NONE))
        {
            if (base.LookToStartAction())
            {
                player.CanBeEjected = !ejectionImmunityWhileInGround;
                return true;
            }
        }
            
        return false;
    }

    protected override void ClearCapacityEffects()
    {
        base.ClearCapacityEffects();
        player.CanBeEjected = true;
    }
}
