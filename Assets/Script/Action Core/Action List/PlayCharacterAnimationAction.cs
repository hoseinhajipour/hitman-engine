using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[ActionCategory("Character")]
[System.Serializable]
public class PlayCharacterAnimationAction : ActionBase, ICancelableAction
{
    public GameObject target;
    public AnimationClip animationClip;
    public bool waitForEnd;
    public float fadeDuration = 0.5f; // Duration of the fade in seconds

    private PlayableGraph playableGraph; // Reference to the playable graph for stopping
    private bool isCanceled = false;    // Tracks if the action was canceled

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        isCanceled = false; // Reset the cancel flag

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

        Animator animator = target.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("PlayCharacterAnimationAction: No Animator component found on target!");
            onComplete?.Invoke();
            return;
        }

        if (playableGraph.IsValid())
        {
            Debug.LogWarning("PlayCharacterAnimationAction: PlayableGraph already running. Stopping and recreating.");
            playableGraph.Destroy();
        }

        playableGraph = PlayableGraph.Create("PlayAnimationGraph");
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);

        AnimationClipPlayable currentClipPlayable = CreateCurrentAnimationPlayable(animator, playableGraph);
        AnimationClipPlayable newClipPlayable = AnimationClipPlayable.Create(playableGraph, animationClip);

        AnimationMixerPlayable mixer = AnimationMixerPlayable.Create(playableGraph, 2);
        output.SetSourcePlayable(mixer);

        playableGraph.Connect(currentClipPlayable, 0, mixer, 0);
        playableGraph.Connect(newClipPlayable, 0, mixer, 1);

        mixer.SetInputWeight(0, 1);
        mixer.SetInputWeight(1, 0);

        playableGraph.Play();

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

        while (elapsedTime < fadeDuration)
        {
            if (isCanceled) // Stop if canceled
            {
                CleanupGraph(graph);
                onComplete?.Invoke();
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float easedT = t * t * (3f - 2f * t);

            mixer.SetInputWeight(0, 1 - easedT);
            mixer.SetInputWeight(1, easedT);

            yield return null;
        }

        mixer.SetInputWeight(0, 0);
        mixer.SetInputWeight(1, 1);

        if (waitForEnd)
        {
            while (newClipPlayable.GetTime() < newClipPlayable.GetAnimationClip().length)
            {
                if (isCanceled) // Stop if canceled
                {
                    CleanupGraph(graph);
                    onComplete?.Invoke();
                    yield break;
                }
                yield return null;
            }
        }

        CleanupGraph(graph);
        onComplete?.Invoke();
    }

    public void Cancel()
    {
        isCanceled = true; // Set the canceled flag
        if (playableGraph.IsValid())
        {
            CleanupGraph(playableGraph);
        }
    }

    private void CleanupGraph(PlayableGraph graph)
    {
        if (graph.IsValid())
        {
            graph.Destroy();
        }
    }
}
