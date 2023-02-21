using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Controls
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(Button))]
    internal class ToggleButton : MonoBehaviour
    {
        public bool Selected
        {
            get
            {
                return highlight.activeSelf;
            }
            set
            {
                highlight.SetActive(value);
            }
        }

        [SerializeField]
        private GameObject highlight;
    }
}
