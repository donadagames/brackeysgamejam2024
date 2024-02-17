using UnityEngine;

public class KeyDoor : Interactable
{
    public Transform doorLeft;
    public Transform doorRight;

    private Vector3 rotationLeft = new Vector3();
    private Vector3 rotationRight = new Vector3();


    public override void Start()
    {
        player = Player.instance;
        rotationLeft = doorLeft.localEulerAngles;
        rotationRight = doorRight.localEulerAngles;
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

        if (player.keyQuantity <= 0)
        {
            player.ErrorSound();
            return;
        }

        hasInteract = true;
        player.isInteracting = true;
        player.SetDefaultInteractionSprite();
        player.keyQuantity = 0;
        player.keyQuantityText.text = "0";
        OpenDor();
    }

    public override void OpenDor()
    {
        player.OpenDoorSound();

        if (spawners.Length > 0)
        {
            foreach (EnemySpawner spawner in spawners)
            {
                spawner.shouldSpawn = true;
            }

        }

        doorRight.LeanRotateAround(Vector3.up, rotateionDoor, 1);
        doorLeft.LeanRotateAround(Vector3.up, -rotateionDoor, 1);
    }

    public override void ResetDoor()
    {
        if (hasInteract == true)
        {
            hasInteract = false;
            doorLeft.localEulerAngles = rotationLeft;
            doorRight.localEulerAngles = rotationRight;
        }
    }
}
