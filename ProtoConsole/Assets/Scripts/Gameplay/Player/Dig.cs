public class Dig : ChangeHeightCapacity
{
    protected override Capacity Type => Capacity.DIG;

    protected override void LookToStartAction()
    {
        if (player.IsUsingCapacity(Capacity.NONE)) base.LookToStartAction(); 
    }
}
