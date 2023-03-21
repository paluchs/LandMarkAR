using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Controllers.Behavioral
{
    public class ProgressIndicatorController : MonoBehaviour
    {

        private IProgressIndicator _progressIndicator;
        
        public string message = "Progress Message";
        public float progress;
        public bool processRunning;


        public void Start()
        {
            _progressIndicator = GetComponent<IProgressIndicator>();
        }


        // 
        public async void RunProgressBehavior()
        {
            _progressIndicator ??= GetComponent<IProgressIndicator>();
            _progressIndicator.Message = message;
            _progressIndicator.Progress = progress;
            await _progressIndicator.OpenAsync();
            
            await Task.Yield();
            
            // Wait for the process to START running
            while (!processRunning)
            {
                _progressIndicator.Message = message;
                await Task.Yield();
            }
            
            // Wait for process to STOP running
            while (processRunning)
            {
                if (processRunning)
                {
                    _progressIndicator.Message = message;
                    _progressIndicator.Progress = progress;
                }
                await Task.Yield();
            }
            
            _progressIndicator.Progress = 1;
            _progressIndicator.Message = message;

            await _progressIndicator.CloseAsync();
        }
    }
}
