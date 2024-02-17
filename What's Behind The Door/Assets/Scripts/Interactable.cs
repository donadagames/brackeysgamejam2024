using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public bool hasInteract;
    public Player player;
    public float rotateionDoor;
    public EnemySpawner[] spawners;

    private Vector3 initialRotation = new Vector3();

    public virtual void Start()
    {
        player = Player.instance;
        initialRotation = transform.localEulerAngles;
    }

    public virtual void OnEnter()
    {
        if (hasInteract) return;

        player.SetInteractionSprite();
    }

    public virtual void OnExit()
    {
        player.SetDefaultInteractionSprite();
    }


    public virtual void Interact()
    {
        if (hasInteract) return;

        hasInteract = true;
        player.isInteracting = true;
        player.SetDefaultInteractionSprite();
        OpenDor();
    }

    public virtual void OpenDor()
    {
        player.OpenDoorSound();

        if (spawners.Length > 0)
        {
            foreach (EnemySpawner spawner in spawners)
            { 
                spawner.shouldSpawn = true;
            }
            
        }

        transform.LeanRotateAround(Vector3.up, rotateionDoor, 1);
    }

    public virtual void ResetDoor()
    { 
        if(hasInteract == true)
        {
            hasInteract = false;
            transform.localEulerAngles = initialRotation;
        }
    }
}

public interface IInteractable
{
    public void Interact();
    public void OnEnter();
    public void OnExit();
}