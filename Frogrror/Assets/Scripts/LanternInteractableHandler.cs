public class LanternInteractableHandler : InteractableHandler
{
    public override bool CanInteract()
    {
        return GameManager.Instance.HasLampActive;
    }
}