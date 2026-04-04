using UnityEngine;

public class TriggerButton : GameButton
{
    public override void PlayerEnter()
    {
        active = true;  
    }

    public override void PlayerExit()
    {
        //Nothing
    }
}
