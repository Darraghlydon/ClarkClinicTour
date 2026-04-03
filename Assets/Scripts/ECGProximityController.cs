using UnityEngine;

public class ECGProximityController : MonoBehaviour
{
    [Header("Optional: only react to this tag")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private GameObject _ecgTextureObject;

    private DrawECGScript[] ecgScripts;
    private DrawSinewaveScript[] sinewaveScripts;
    private HRValueScript[] hrValueScripts;

    private void Awake()
    {
        // Get all matching scripts from this object and its children
        ecgScripts = _ecgTextureObject.GetComponentsInChildren<DrawECGScript>(true);
        sinewaveScripts = _ecgTextureObject.GetComponentsInChildren<DrawSinewaveScript>(true);
        hrValueScripts = _ecgTextureObject.GetComponentsInChildren<HRValueScript>(true);

        SetProcessingState(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter");
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        SetProcessingState(true);
        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit");
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        SetProcessingState(false);
        
    }

    private void SetProcessingState(bool state)
    {
        foreach (DrawECGScript script in ecgScripts)
        {
            if (script != null)
                script.enabled = state;
        }

        foreach (DrawSinewaveScript script in sinewaveScripts)
        {
            if (script != null)
                script.enabled = state;
        }

        foreach (HRValueScript script in hrValueScripts)
        {
            if (script != null)
                script.enabled = state;
        }
    }
}