using UnityEngine;

namespace PandaCafe.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameState GameState {get; private set;}
        public static GameManager Instance {get; private set;}

        void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy (gameObject);
            } 

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            GameState = GameState.Playing;
        }
    }
}
