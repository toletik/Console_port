public class Jump : ChangeHeightCapacity
{
    protected override Capacity Type => Capacity.JUMP;

    protected override bool TryStartCapacity()
    {
        if (!player.IsUsingCapacity(Capacity.DIG)) 
            return base.TryStartCapacity();

        return false;
    }
}
