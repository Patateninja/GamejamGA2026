using UnityEngine;

public class HoldButton : GameButton
{
    public override void PlayerEnter()
    {
        active = true;
    }

    public override void PlayerExit()
    {
        active = false;
    }
}