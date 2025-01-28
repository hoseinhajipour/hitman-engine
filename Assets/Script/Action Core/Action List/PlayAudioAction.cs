using UnityEngine;

[System.Serializable]
public class PlayAudioAction : ActionBase
{
    [Tooltip("Audio clip to play.")]
    public AudioClip audioClip;

    [Tooltip("Volume of the audio.")]
    public float volume = 1.0f;

    [Tooltip("The transform position to play the audio at.")]
    public Transform targetTransform;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (audioClip == null)
        {
            Debug.LogError("PlayAudioAction: No audio clip assigned!");
            onComplete?.Invoke();
            return;
        }

        // Create a new GameObject for the audio source
        GameObject audioObject = new GameObject("AudioPlayer");
        audioObject.transform.position = targetTransform != null ? targetTransform.position : hit_target.transform.position;

        // Add AudioSource component and configure it
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Enable 3D audio
        audioSource.Play();

        // Destroy the GameObject after the audio finishes playing
        float destroyTime = audioClip.length;
        Object.Destroy(audioObject, destroyTime);

        onComplete?.Invoke();
    }

}
