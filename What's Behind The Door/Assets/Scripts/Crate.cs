using UnityEngine;

public class Crate : Interactable
{

    public Transform tampa;

    public override void Start()
    {
        player = Player.instance;
    }

    public override void OnEnter()
    {
        if (hasInteract) return;

        player.SetInteractionSprite();
    }

    public override void OnExit()
    {
        player.SetDefaultInteractionSprite();
    }



    public override void Interact()
    {
        if (hasInteract) return;

        hasInteract = true;
        player.isInteracting = true;
        player.SetDefaultInteractionSprite();
        OpenLid();
    }

    public void OpenLid()
    {
        player.OpenCrateSound();
        tampa.LeanRotateAroundLocal(Vector3.right, -45, .5f).setOnComplete(GiveItem);
    }

    private void GiveItem()
    {
        player.AddPotion();
    }

    public void ResetCrate()
    {
        if (hasInteract == true)
        {
            hasInteract = false;
            tampa.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}
