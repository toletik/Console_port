public class Jump : ChangeHeightCapacity
{
    protected override Capacity Type => Capacity.JUMP;

    protected override bool LookToStartAction()
    {
        if (!player.IsUsingCapacity(Capacity.DIG)) 
            return base.LookToStartAction();

        return false;
    }
}
