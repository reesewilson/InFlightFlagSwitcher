using InFlightFlagSwitcher.Module;
using InFlightFlagSwitcher.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InFlightFlagSwitcher.Main
{
    [KSPAddon(KSPAddon.Startup.EditorAny, true)]
    public class Editor : MonoBehaviour
    {

        private bool registeredEvents = false;

        public static Editor Instance { get; protected set; }

        public void Start()
        {

            Utils.Log("Editor Startup");

            if (Instance == null)
                Instance = this;
            if (!registeredEvents)
            {
                EditorLogic.fetch.launchBtn.onClick.AddListener(onEditorLaunchButtonClick);
                registeredEvents = true;
            }
        }

        public void onEditorLaunchButtonClick()
        {
            foreach (Part p in EditorLogic.fetch.ship.parts)
            {
                IFFSFlagSwitchModule fsm = (IFFSFlagSwitchModule)p.Modules["IFFSFlagSwitchModule"];
                if (fsm == null)
                    continue;
                fsm.recordCurrentFlag(true);
            }
        }

    }
}
