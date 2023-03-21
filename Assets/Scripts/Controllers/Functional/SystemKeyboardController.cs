using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UI.Interactions;
using UnityEngine;

namespace Controllers.Functional
{
    // Code from SystemKeyboard example of the MRTK! Should you ever need the keyboard of another device, check there
    // https://github.com/microsoft/MixedRealityToolkit-Unity/tree/main/Assets/MRTK/Examples/Experimental/MixedRealityKeyboard
    public class SystemKeyboardController : MonoBehaviour
    {
        [SerializeField] private MixedRealityKeyboard wmrKeyboard;

        private KeyboardTextChange _keyboardTextChange;
        public KeyboardTextChange KeyboardTextChange
        {
            get => _keyboardTextChange;
            set
            {
                _keyboardTextChange = value;
                wmrKeyboard.OnHideKeyboard.RemoveAllListeners();
                wmrKeyboard.OnShowKeyboard.RemoveAllListeners();
                wmrKeyboard.OnShowKeyboard.AddListener(value.ShowCaret);
                wmrKeyboard.OnShowKeyboard.AddListener(value.StartCaret);
                wmrKeyboard.OnHideKeyboard.AddListener(value.StopCaret);
                wmrKeyboard.OnHideKeyboard.AddListener(value.HideCaret);
                
            }
        }

        public void OpenSystemKeyboard()
        {
            wmrKeyboard.ShowKeyboard(wmrKeyboard.Text);
        }
        
        
        private void Start()
        {
            // Initially make sure that the caret is hidden
            
            // // Initially hide the preview.
            // if (keyboardTextChange != null)
            // {
            //     keyboardTextChange.gameObject.SetActive(false);
            // }
            //
            // // Windows mixed reality keyboard initialization goes here
        
            // if (wmrKeyboard.OnShowKeyboard != null)
            // {
            //     wmrKeyboard.OnShowKeyboard.AddListener(() =>
            //     {
            //         if (keyboardTextChange != null)
            //         {
            //             keyboardTextChange.gameObject.SetActive(true);
            //         }
            //     });
            // }
            //
            // if (wmrKeyboard.OnHideKeyboard != null)
            // {
            //     wmrKeyboard.OnHideKeyboard.AddListener(() =>
            //     {
            //         if (keyboardTextChange != null)
            //         {
            //             keyboardTextChange.gameObject.SetActive(false);
            //         }
            //     });
            // }
        }

        private void Update()
        {
            if (KeyboardTextChange is null) return;
                
            KeyboardTextChange.Text = wmrKeyboard.Text;
            KeyboardTextChange.CaretIndex = wmrKeyboard.CaretIndex;
            
            
            // // Windows mixed reality keyboard update goes here
            // if (wmrKeyboard.Visible)
            // {
            //
            // }
            // else
            // {
            //     if (keyboardTextChange is null) return;
            //     
            //     keyboardTextChange.Text = string.Empty;
            //     keyboardTextChange.CaretIndex = 0;
            // }
        }
    }
}
