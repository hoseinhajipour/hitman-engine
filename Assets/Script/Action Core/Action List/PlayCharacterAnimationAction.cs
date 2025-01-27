using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[ActionCategory("Character")]
[System.Serializable]
public class PlayCharacterAnimationAction : ActionBase
{
    public GameObject target;
    public AnimationClip animationClip;
    public bool waitForEnd;
    public float fadeDuration = 0.5f; // Duration of the fade in seconds

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("PlayCharacterAnimationAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        if (animationClip == null)
        {
            Debug.LogError("PlayCharacterAnimationAction: Animation Clip is null!");
            onComplete?.Invoke();
            return;
        }

        // Get Animator component
        Animator animator = target.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("PlayCharacterAnimationAction: No Animator component found on target!");
            onComplete?.Invoke();
            return;
        }

        // Create a PlayableGraph
        PlayableGraph playableGraph = PlayableGraph.Create("PlayAnimationGraph");
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);

        // Create two playables for the current and new animation
        AnimationClipPlayable currentClipPlayable = CreateCurrentAnimationPlayable(animator, playableGraph);
        AnimationClipPlayable newClipPlayable = AnimationClipPlayable.Create(playableGraph, animationClip);

        // Create a mixer to blend animations
        AnimationMixerPlayable mixer = AnimationMixerPlayable.Create(playableGraph, 2);
        output.SetSourcePlayable(mixer);

        // Connect the playables to the mixer
        playableGraph.Connect(currentClipPlayable, 0, mixer, 0);
        playableGraph.Connect(newClipPlayable, 0, mixer, 1);

        // Set initial weights
        mixer.SetInputWeight(0, 1); // Current clip starts fully active
        mixer.SetInputWeight(1, 0); // New clip starts inactive

        playableGraph.Play();

        // Start fading
        target.GetComponent<MonoBehaviour>()?.StartCoroutine(SmoothFadeAnimation(mixer, currentClipPlayable, newClipPlayable, playableGraph, onComplete));
    }

    private AnimationClipPlayable CreateCurrentAnimationPlayable(Animator animator, PlayableGraph graph)
    {
        AnimationClip currentClip = animator.GetCurrentAnimatorClipInfo(0).Length > 0
            ? animator.GetCurrentAnimatorClipInfo(0)[0].clip
            : null;

        if (currentClip != null)
        {
            AnimationClipPlayable currentClipPlayable = AnimationClipPlayable.Create(graph, currentClip);
            currentClipPlayable.SetTime(animator.GetCurrentAnimatorStateInfo(0).normalizedTime * currentClip.length);
            return currentClipPlayable;
        }

        // Return an empty clip if no current animation is active
        return AnimationClipPlayable.Create(graph, new AnimationClip());
    }

    private System.Collections.IEnumerator SmoothFadeAnimation(
        AnimationMixerPlayable mixer,
        AnimationClipPlayable currentClipPlayable,
        AnimationClipPlayable newClipPlayable,
        PlayableGraph graph,
        System.Action onComplete)
    {
        float elapsedTime = 0;

        // Gradually fade from input 0 (current) to input 1 (new)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // Smoothstep easing for smoother fade transitions
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float easedT = t * t * (3f - 2f * t); // Smoothstep function

            mixer.SetInputWeight(0, 1 - easedT); // Decrease weight of the current clip
            mixer.SetInputWeight(1, easedT);     // Increase weight of the new clip

            yield return null;
        }

        // Ensure the new clip is fully active
        mixer.SetInputWeight(0, 0);
        mixer.SetInputWeight(1, 1);

        // Wait for the new animation to complete if required
        if (waitForEnd)
        {
            while (newClipPlayable.GetTime() < newClipPlayable.GetAnimationClip().length)
            {
                yield return null;
            }
        }

        // Clean up the graph
        graph.Destroy();
        onComplete?.Invoke();
    }
}
