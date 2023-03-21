using Managers;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Controllers.Functional
{
    public class PreviewController : MonoBehaviour
    {
        public string meshName;
        public Interactable interactable;

        [SerializeField] private GameObject visual;

        private Mesh _mesh;
        public Mesh Mesh
        {
            get => _mesh;
            set
            {
                _mesh = value;
                visual.GetComponent<MeshFilter>().mesh = _mesh;
                visual.transform.localScale /= ShapeManger.GetScaleFactor(visual);
            }
        }


    }
}