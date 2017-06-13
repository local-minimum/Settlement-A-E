using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationOption : MonoBehaviour {

    bool interactable = true;

    Material mat;

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

    private void Start()
    {
        mat = GetComponent<Image>().material;
    }

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

    public void DeactivateInteractions()
    {
        interactable = false;
    }

    public void EnableInteractions()
    {
        interactable = true;
    }
}
