using UnityEngine;

public class Dig : ChangeHeightCapacity
{
    [SerializeField] private bool ejectionImmunityWhileInGround = true;

    protected override Capacity Type => Capacity.DIG;

    protected override bool TryStartCapacity()
    {
        if (player.IsUsingCapacity(Capacity.NONE))
        {
            if (base.TryStartCapacity())
            {
                player.CanBeEjected = !ejectionImmunityWhileInGround;
                return true;
            }
        }
            
        return false;
    }

    protected override void EndCapacity()
    {
        base.EndCapacity();
        player.CanBeEjected = true;
    }
}
