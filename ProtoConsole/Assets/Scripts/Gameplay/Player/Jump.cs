public class Jump : ChangeHeightCapacity
{
    protected override Capacity Type => Capacity.JUMP;

    protected override void LookToStartAction()
    {
        if (!player.IsUsingCapacity(Capacity.DIG)) base.LookToStartAction(); 
    }
}
