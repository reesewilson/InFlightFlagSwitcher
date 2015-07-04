using System;
using UnityEngine;

namespace InFlightFlagSwitcher
{
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class Main : MonoBehaviour
    {

        private ApplicationLauncherButton button;
        private FlagBrowser flagBrowser;

        public void Start()
        {
            //GameEvents.onGUIApplicationLauncherReady.Add(onGUIApplicationLauncherReady);
            onGUIApplicationLauncherReady(); // AppLauncher is always ready by the time the player is in the flight scene
            GameEvents.onCrewOnEva.Add(onCrewOnEVA);
            GameEvents.onCrewBoardVessel.Add(onCrewBoardVessel);
            GameEvents.onVesselChange.Add(onVesselChange);
        }

        private void onVesselChange(Vessel data)
        {
            if (data.isEVA)
                this.button.Disable();
            else
                this.button.Enable();

        }

        private void onCrewBoardVessel(GameEvents.FromToAction<Part, Part> data)
        {
            if (!this.button.enabled)
                this.button.Enable();
        }

        private void onCrewOnEVA(GameEvents.FromToAction<Part, Part> data)
        {
            if (this.button.enabled)
                this.button.Disable();

        }

        public Texture getCurrentFlag()
        {
            return GameDatabase.Instance.GetTexture(FlightGlobals.fetch.activeVessel.rootPart.flagURL, false);
        }


        public void onGUIApplicationLauncherReady() 
        {
            this.button = ApplicationLauncher.Instance.AddModApplication(doStuff, dontDoStuff, null, null, null, null, ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW, getCurrentFlag());
        }

        public void doStuff()
        {

            button.SetTrue(false);

            /* 
             * Can't lie, I didn't find this myself. Google is a great and powerful tool.
             * https://github.com/Mihara/PartUtilities/blob/bcbd90522a1117fbd04a361d1544fc3612d6ffe3/PartUtilities/UtilityFunctions.cs#L28-L41
             * 
             */
            this.flagBrowser = (UnityEngine.Object.Instantiate((UnityEngine.Object)(new FlagBrowserGUIButton(null, null, null, null)).FlagBrowserPrefab) as GameObject).GetComponent<FlagBrowser>();
            this.flagBrowser.OnFlagSelected += OnFlagSelected;
            this.flagBrowser.OnDismiss += OnFlagBrowserDismiss;
        }

        private void OnFlagBrowserDismiss()
        {
            this.button.SetFalse(false);
        }

        private void OnFlagSelected(FlagBrowser.FlagEntry selected)
        {
            Vessel v = FlightGlobals.fetch.activeVessel;
            foreach (Part p in v.parts)
            {

                p.flagURL = selected.textureInfo.name;

                FlagDecal fd = (FlagDecal)p.Modules["FlagDecal"];
                if (fd != null)
                {
                    fd.UpdateFlagTexture();
                }

            }

            this.button.SetTexture((Texture)selected.textureInfo.texture);
            this.button.SetFalse(false);

        }

        public void dontDoStuff()
        {
            this.flagBrowser.Dismiss();
            this.button.SetFalse(false);
        }

        private void Log(string text, params object[] args)
        {
            Debug.Log(String.Format(text, args));
        }
    }
}
