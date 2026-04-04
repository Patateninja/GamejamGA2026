using UnityEngine;

public class HoldButton : GameButton
{
    public override void Entered()
    {
        active = true;

        OffModel.SetActive(false);
        OnModel.SetActive(true);
    }

    public override void Exited()
    {
        active = false;

        OffModel.SetActive(true);
        OnModel.SetActive(false);
    }
}