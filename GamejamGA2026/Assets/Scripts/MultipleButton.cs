using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MultipleButton : GameButton
{
    [SerializeField]
    private GameObject WaitModel;

    [SerializeField]
    private List<MultipleButton> otherButtons;

    public void Start()
    {
        WaitModel.SetActive(false);
    }

    public override void Entered()
    {
        active = true;

        OffModel.SetActive(false);
        WaitModel.SetActive(true);
        OnModel.SetActive(false);

        if (otherButtons.Where(b => !b.active).Count() == 0)
        {
            OffModel.SetActive(false);
            WaitModel.SetActive(false);
            OnModel.SetActive(true);

            foreach (MultipleButton button in otherButtons)
            {
                OffModel.SetActive(false);
                WaitModel.SetActive(false);
                OnModel.SetActive(true);
            }
        }
    }

    public override void Exited()
    {
        if (otherButtons.Where(b => !b.active).Count() != 0)
        {
            active = false;

            OffModel.SetActive(true);
            WaitModel.SetActive(false);
            OnModel.SetActive(false);
        }
    }
}
