using UnityEngine;

namespace Assets.Scripts.Tool.KeyBoardGrid
{
    public class KeyboardManager: Singleton<KeyboardManager>
    {
        GameObject keyboard = null;
        public GameObject blocker = null;
        public bool IsCanCreateKeyboard()
        {
            if (keyboard == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void SetKeyboard(GameObject m_keyboard)
        {
            this.keyboard = m_keyboard;
        }
        public GameObject GetKeyboard()
        {
            return keyboard;
        }
    }
}
