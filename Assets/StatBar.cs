using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Wacki.IsoRPG
{

    /// <summary>
    /// Simple class controlling a status bar UI that is able to match
    /// what ever sized container it is in.
    /// </summary>
    public class StatBar : MonoBehaviour
    {
        public Text statsText;
        
        public void SetStat(int current, int max)
        {
            if(max <= 0)
            {
                Debug.LogError("WARNING, MAX HEALTH 0 OR BELOW!");
                return;
            }

            // get percentage of current stat
            var percentage = (float)current / (float)max;

            // get rect transform 
            var rectTrfrm = GetComponent<RectTransform>();
            var ofssetMax = rectTrfrm.offsetMax;
            
            // get width of parent
            var maxWidth = rectTrfrm.parent.GetComponent<RectTransform>().rect.width;
            
            // set new width based on percentage
            ofssetMax.x = -(1.0f - percentage) * maxWidth;
            rectTrfrm.offsetMax = ofssetMax;

            // oh yea and update the text
            statsText.text = current + " / " + max;
        }

    }

}