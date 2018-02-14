using UnityEngine;

namespace PortableConsole
{
    [System.Serializable]
    public class PortableConsoleStyle
    {
        public Color FirstColor = Color.white;
        public Color SecondColor = Color.grey;
        public Sprite InfoSprite;
        public Sprite WarningSprite;
        public Sprite ErrorSprite;
    }
}