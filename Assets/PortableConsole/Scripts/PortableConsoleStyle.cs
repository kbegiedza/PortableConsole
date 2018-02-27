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

        public Sprite ScrollLocked;
        public Sprite ScrollUnlocked;

        public Color WarningColor = new Color32(255, 193, 22, 125);
        public Color ErrorColor = new Color32(234, 67, 53, 125);
        public Color LogColor = new Color32(10, 170, 240, 125);
    }
}