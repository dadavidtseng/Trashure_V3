using UnityEngine;
using UnityEngine.Events;

namespace Trashure.Dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("對話詳情")] 
        public Sprite faceImage;
        public bool onLeft;
        public string name;
        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        [HideInInspector] public bool isDone;
        public UnityEvent afterTalkEvent;
    }
}
