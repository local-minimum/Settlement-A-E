using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OptionSelection(string identifier);

public class ConversationOption : MonoBehaviour {

    public event OptionSelection OnOptionSelected;

    [SerializeField]
    KeyCode key = KeyCode.Z;

    [SerializeField]
    Text keyText;

    [SerializeField]
    Text titleText;

    string optionIdentifier;

    public bool IsOption(string identifier)
    {
        return !string.IsNullOrEmpty(identifier) && identifier == optionIdentifier;
    }

    private void Update()
    {
        if (interactable)
        {
            if (Input.GetKeyDown(key))
            {
                SelectOption();
            }
        }
    }

    #region TimeOut

    Material mat;

    [SerializeField]
    Image effectImage;

    [SerializeField]
    string matProperty = "_DetailMask";

    [SerializeField]
    float startOffsetY = -0.05f;

    [SerializeField]
    float endOffsetY = -.99f;

    [SerializeField]
    float animDeltaTime = 0.016f;

    [SerializeField, Range(0.1f, 2f)]
    float animDuration = 0.6f;

    public void AnimateMaterialEffect()
    {
        //TODO: Stop previous?
        StartCoroutine(_AnimateMaterialEffect());
    }

    IEnumerator<WaitForSeconds> _AnimateMaterialEffect()
    {
        float progress = 0;
        float startTme = Time.realtimeSinceStartup;
        while (progress < 1f)
        {
            progress = (Time.realtimeSinceStartup - startTme) / animDuration;
            Vector2 offset = new Vector2(0, Mathf.Lerp(startOffsetY, endOffsetY, progress));
            mat.SetVector(matProperty, offset);
            yield return new WaitForSeconds(animDeltaTime);
        }
    }

    #endregion

    Animator anim;

    bool interactable = true;

    public void DeactivateInteractions()
    {
        interactable = false;
        anim.SetTrigger("Remove");
    }

    public void ShowOption(string identifier, string text)
    {
        optionIdentifier = identifier;
        titleText.text = text;
        keyText.text = key.ToString();
        interactable = true;
        anim.SetTrigger("Introduce");
    }

    public void SelectOption()
    {
        interactable = false;
        if (OnOptionSelected != null)
        {
            OnOptionSelected(optionIdentifier);
        } else
        {
            DeactivateInteractions();
        }
    }

    public void OptionHoverStart()
    {
    }

    public void OptionHoverEnd()
    {

    }

    private void Start()
    {
        mat = effectImage.material;
        anim = GetComponent<Animator>();
        ShowOption("", "Welcome");
    }

}
