using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

[RequireComponent(typeof(Animator))]
public class SelectableAnimator : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool DisableSelect;
    [SerializeField] private string OnNormalTrigger = "Normal";
    [SerializeField] private string OnHighlightedTrigger = "Highlighted";
    [SerializeField] private string OnPressedTrigger = "Pressed";
    [SerializeField, HideIf(nameof(DisableSelect))] private string OnSelectedTrigger = "Selected";
    private bool hasAnimator => animator != null && animator.runtimeAnimatorController != null;

    private enum State
    {
        Normal,
        Press,
        Highlighted,
        Selected
    }
    private State currentState = State.Normal;
    private bool isHovered = false;
    private bool isSelected = false;

    private void Reset()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void SetState(State next)
    {
        switch (next)
        {
            case State.Normal:
                animator.SetTrigger(OnNormalTrigger);
                break;
            case State.Press:
                animator.SetTrigger(OnPressedTrigger);
                break;
            case State.Highlighted:
                animator.SetTrigger(OnHighlightedTrigger);
                break;
            case State.Selected:
                animator.SetTrigger(OnSelectedTrigger);
                break;
        }
        currentState = next;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if(currentState == State.Normal)
        {
            SetState(State.Highlighted);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if(currentState == State.Highlighted)
        {
            SetState(State.Normal);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetState(State.Press);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(currentState == State.Press)
        {
            if (isSelected && !DisableSelect)
            {
                SetState(State.Selected);
            }
            else if (isHovered)
            {
                SetState(State.Highlighted);
            }
            else
            {
                SetState(State.Normal);
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        if (DisableSelect) return;
        SetState(State.Selected);
    }

    public void OnDeselect(BaseEventData eventData)
    {            
        isSelected = false;
        if(currentState == State.Selected)
        {
            if (isHovered)
            {
                SetState(State.Highlighted);
            }
            else
            {
                SetState(State.Normal);
            }
        }
    }

    [Button("Auto Generate Animation"), HideIf(nameof(hasAnimator))]
    public void AudoGenerateAnimation()
    {
#if UNITY_EDITOR
        var controller = GenerateSelectableAnimatorController(gameObject);
        if (controller == null)
        {
            return;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }

        AnimatorController.SetAnimatorController(animator, controller);
        EditorUtility.SetDirty(this);
#else
        Debug.LogWarning("Auto Generate Animation is only available in the Unity Editor.", this);
#endif
    }

#if UNITY_EDITOR
    private AnimatorController GenerateSelectableAnimatorController(GameObject target)
    {
        if (target == null)
        {
            return null;
        }

        var path = GetSaveControllerPath(target);
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        GenerateTriggerableTransition(OnNormalTrigger, controller);
        GenerateTriggerableTransition(OnHighlightedTrigger, controller);
        GenerateTriggerableTransition(OnPressedTrigger, controller);
        GenerateTriggerableTransition(OnSelectedTrigger, controller);

        AssetDatabase.ImportAsset(path);
        return controller;
    }

    private static string GetSaveControllerPath(GameObject target)
    {
        var defaultName = target.gameObject.name;
        var message = string.Format("Create a new animator for the game object '{0}':", defaultName);
        return EditorUtility.SaveFilePanelInProject("New Animation Contoller", defaultName, "controller", message);
    }

    private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller)
    {
        var clip = AnimatorController.AllocateAnimatorClip(name);
        AssetDatabase.AddObjectToAsset(clip, controller);

        var state = controller.AddMotion(clip);

        controller.AddParameter(name, AnimatorControllerParameterType.Trigger);

        var stateMachine = controller.layers[0].stateMachine;
        var transition = stateMachine.AddAnyStateTransition(state);
        transition.AddCondition(AnimatorConditionMode.If, 0, name);
        transition.exitTime = 1f;
        transition.duration = 0.1f;
        return clip;
    }
#endif
}
