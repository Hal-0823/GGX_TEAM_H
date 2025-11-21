using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class startbutton : MonoBehaviour
{
        private Renderer _renderer;

        void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

        public void OnMouseDown()
        {
            transform.DOScale(0.90f,0.24f).SetEase(Ease.OutCubic);
            _renderer.material.DOColor(Color.red,0.2f).SetEase(Ease.OutCubic);
        }
        public void OnMouseUp()
        {
            transform.DOScale(1.0f,0.24f).SetEase(Ease.OutCubic);
            _renderer.material.DOColor(Color.white,0.2f).SetEase(Ease.OutCubic);
        }

};  


