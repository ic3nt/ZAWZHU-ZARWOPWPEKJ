using UnityEngine;

namespace EasyTransition
{

    public class DemoLoadScene : MonoBehaviour
    {
        public TransitionSettings transition;
        public float startDelay;

        // загрузочка

        public void LoadScene(string _sceneName)
        {
            TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
        }   
    }

}


