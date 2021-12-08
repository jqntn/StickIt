using UnityEngine;
using UnityEngine.EventSystems;
public class AnimateOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Animation toAnimate;
    public void OnDeselect(BaseEventData eventData)
    {
        toAnimate.Stop();
        toAnimate.gameObject.transform.localScale = Vector3.one;
    }
    public void OnSelect(BaseEventData eventData) => toAnimate.Play();
}