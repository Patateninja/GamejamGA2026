using UnityEngine;

public class TriggerButton : GameButton
{
    public override void Entered()
    {
        active = true;
        OffModel.SetActive(false);
        OnModel.SetActive(true);
    }

    public override void Exited()
    {
        //Nothing
    }
}
