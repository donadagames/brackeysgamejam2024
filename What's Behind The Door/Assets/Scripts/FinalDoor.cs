using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    public Transform doorLeft;
    public Transform doorRight;
    public float rotateionDoor;
    private Vector3 rotationLeft = new Vector3();
    private Vector3 rotationRight = new Vector3();

    public bool hasInteract;
    Player player;

    private void Start()
    {
        player= Player.instance;
    }

    public void OpenDor()
    {
        hasInteract = true;
        player.OpenDoorSound();

        doorRight.LeanRotateAround(Vector3.up, rotateionDoor, 1);
        doorLeft.LeanRotateAround(Vector3.up, -rotateionDoor, 1).setOnComplete(WinGame);
    }

    public void ResetDoor()
    {
        if (hasInteract == true)
        {
            hasInteract = false;
            doorLeft.localEulerAngles = rotationLeft;
            doorRight.localEulerAngles = rotationRight;
        }
    }

    private void WinGame()
    {
        player.WinGame();
    }
}
