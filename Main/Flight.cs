using System;
using UnityEngine;
using InFlightFlagSwitcher.Utilities;
using InFlightFlagSwitcher.Module;

namespace InFlightFlagSwitcher.Main
{
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class Flight : MonoBehaviour
    {

        private ApplicationLauncherButton button;
        private FlagBrowser flagBrowser;

        public static Flight Instance { get; protected set; }

        public void Start()
        {
            //GameEvents.onGUIApplicationLauncherReady.Add(onGUIApplicationLauncherReady);
            onGUIApplicationLauncherReady(); // AppLauncher is always ready by the time the player is in the flight scene
            GameEvents.onCrewOnEva.Add(onCrewOnEVA);
            GameEvents.onCrewBoardVessel.Add(onCrewBoardVessel);
            GameEvents.onVesselChange.Add(onVesselChange);
            //GameEvents.onFlagSelect.Add(onFlagSelect);
            if (Instance == null)
                Instance = this;
        }

        private void onVesselChange(Vessel data)
        {
            if (data.isEVA)
                this.button.Disable();
            else
                this.button.Enable();
            updateButtonTexture();

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

        public void updateButtonTexture()
        {
            this.button.SetTexture(Utils.getCurrentFlag());
        }


        public void onGUIApplicationLauncherReady() 
        {
            this.button = ApplicationLauncher.Instance.AddModApplication(doStuff, dontDoStuff, null, null, null, null, ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW, Utils.getCurrentFlag());
        }

        public void doStuff()
        {

            button.SetTrue(false);

            /* 
             * Credit to Mihara for their code on how to get the FlagBrowser prefab.
             * https://github.com/Mihara/PartUtilities/blob/bcbd90522a1117fbd04a361d1544fc3612d6ffe3/PartUtilities/UtilityFunctions.cs#L28-L41
             */
            this.flagBrowser = Utils.getFlagBrowser(OnFlagSelected, OnFlagBrowserDismiss);
        }

        private void OnFlagBrowserDismiss()
        {
            this.button.SetFalse(false);
        }

        private void OnFlagSelected(FlagBrowser.FlagEntry selected)
        {

            Utils.applyFlagToVessel(FlightGlobals.fetch.activeVessel, selected);
            updateButtonTexture();
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

        public void onFlagSelect(string flag)
        {

        }
    }
}
