using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEngine;

/// Please do not Remove
/// Orignal Authors:
///     � Marcello Morena - UniSa - morma016@mymail.unisa.edu.au - https://github.com/Morma016
///     � Travis Strawbridge - Unisa - strtk001@mymail.unisa.edu.au - https://github.com/STRTK001

/// Additional Authors:
/// 

/// <summary>
/// Class repesenting the Animation menu UI pane used to display an play animations of the 
/// currently selected model.
/// </summary>
public class AnimationPane : MonoBehaviour, ScrollablePane
{

    /// <summary>
    /// the button UI element for returning back to the home pane
    /// </summary>
    [SerializeField] private PressableButton backBtn;
    /// <summary>
    /// The tranform that will parent (hold) the UI elements
    /// </summary>
    [SerializeField] private RectTransform contentHolder;
    /// <summary>
    /// The prefab we instantiate to display and select the model
    /// we want to spawn
    /// </summary>
    [SerializeField] private AnimationUI contentPrefab;
    /// <summary>
    /// reference of the current model animation that is playing
    /// </summary>
    [SerializeField] private Animation currentModelAnimation;
    /// <summary>
    ///  A list of all pooled AnimationUI elements that arent currently being used
    /// </summary>
    [SerializeField] private List<AnimationUI> pooledlUI = new List<AnimationUI>();
    /// <summary>
    /// A list of all current active AnimationUI elements
    /// </summary>
    [SerializeField] private List<AnimationUI> activelUI = new List<AnimationUI>();
    /// <summary>
    /// the prefab of the default element we use if theres no animations for 
    /// the current model
    /// </summary>
    [SerializeField] private RectTransform defaultElement;

    /// <summary>
    /// the Animation Component that holds all the animation clips of the currently
    /// selected interactable.
    /// </summary>
    public Animation CurrentAnimation
    {
        get 
        {
            return currentModelAnimation;
        }
        set
        {
            currentModelAnimation = value;
        }
    }

    private void Awake()
    {
        backBtn.OnClicked.AddListener(back);
    }

    /// <summary>
    /// method for closing the model pane and reopening the Home pane
    /// </summary>
    private void back()
    {
        HandMenuManager.Instance.activateHomePane();
    }

    public void populateScrollablePane(List<GameObject> loadedPrefabs)
    {
        DebugConsole.Instance.LogDebug("clearing anim pane");
        //clear our scroll pane
        clear();
        //check if the current animation exists
        if(!currentModelAnimation)
        {
            DebugConsole.Instance.LogWarning("the is no animation to populate from");
            defaultElement.gameObject.SetActive(true);
            return;
        }
        //get the animation clips from our current animation
        List<AnimationClip> animationClips = AnimationHandler.getAllAnimationClips(currentModelAnimation);
        //check if there are animations
        if(animationClips.Count < 1)
        {
            DebugConsole.Instance.LogDebug("current no of anims is 0");
            defaultElement.gameObject.SetActive(true);
            return;
        }
        DebugConsole.Instance.LogDebug("current no of anims is > 0");
        defaultElement.gameObject.SetActive(false);
        //for each animation in current animations
        foreach (AnimationClip clip in animationClips)
        {
            DebugConsole.Instance.LogDebug($"creating ui for {clip.name}");
            AnimationUI animationUI;
            //check if we have an animationUI free in our pooled list
            if (pooledlUI.Count < 1)
            {
                DebugConsole.Instance.LogDebug("we had to create a prefab");
                //instantiate prefab
                animationUI = Instantiate<AnimationUI>(contentPrefab);
                //set its parent and local transforms
                animationUI.transform.SetParent(contentHolder);
                animationUI.transform.localPosition = Vector3.zero;
                animationUI.transform.localRotation = Quaternion.identity;
                animationUI.transform.localScale = Vector3.one;
            }
            else
            {
                DebugConsole.Instance.LogDebug("the was a prefab in the pool");
                //grab the UI out of the pool
                animationUI = pooledlUI[pooledlUI.Count-1];
                //remove the ref in pool
                pooledlUI.RemoveAt(pooledlUI.Count - 1);
            }
            DebugConsole.Instance.LogDebug("activatign anim UI");
            //activate the animationUI
            animationUI.gameObject.SetActive(true);
            //populate its animationName
            animationUI.populateContent(currentModelAnimation, clip, clip.name);
            //add it to the active UI list
            activelUI.Add(animationUI);
            DebugConsole.Instance.LogDebug("we shouldve been successful");
        }
    }
    /// <summary>
    /// method for clearing the active animationUI elements and placing them back
    /// into the pool.
    /// </summary>
    private void clear()
    {
        //for each activeUI in our active UI list
        for(int i = 0; i < activelUI.Count; i++)
        {
            //pool the object
            pooledlUI.Add(activelUI[i]);
            //deactivate it
            activelUI[i].gameObject.SetActive(false);
        }
        //clear the active ui list
        activelUI.Clear();
    }
}
