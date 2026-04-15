using UnityEngine;

public class ButtonInteractable : Interactable
{
    public GameObject platform;

    public Transform spawnPoint;


    public override bool OnInteract(InteractionController controller)
    {
        if (!base.OnInteract(controller)) return false;

        Instantiate(platform, spawnPoint.position, spawnPoint.rotation);
        currentInteractor.EndCurrentInteraction();
        return true;

    }

    public override void OnInteractionEnd(InteractionController controller)
    {
        if (currentInteractor == null) return;

        base.OnInteractionEnd(controller);
    }
}
