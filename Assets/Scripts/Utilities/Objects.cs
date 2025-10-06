using UnityEngine;

namespace Utilities.Objects
{
    [System.Serializable]
    public class GameKeys
    {
        public int neededKeys;
        public int currKeys;

        public GameKeys(int curr, int needed)
        {
            currKeys = curr;
            neededKeys = needed;
        }
    }
}
