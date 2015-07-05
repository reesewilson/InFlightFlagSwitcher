using InFlightFlagSwitcher.Main;
using InFlightFlagSwitcher.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InFlightFlagSwitcher.Module
{
    [KSPModule("In Flight Flag Switcher")]
    public class IFFSFlagSwitchModule : PartModule
    {
        [KSPField(isPersistant = true)]
        private string originalFlag = String.Empty;

        [KSPField(isPersistant = true)]
        private bool setOriginalFlag = false;

        [KSPField(isPersistant = true)]
        private bool hasFlagChanged = false;

        [KSPField(isPersistant = true)]
        private string selectedFlag = String.Empty;

        public override void OnStart(PartModule.StartState state)
        {
            recordCurrentFlag(true, true);
            if (/*state != PartModule.StartState.Editor && */this.hasFlagChanged && !this.selectedFlag.Equals(String.Empty) && !this.selectedFlag.Equals(this.part.flagURL)) // Fix for flag reverting after launching the vessel
            {
                Utils.applyFlagToPart(this.part, this.selectedFlag);
            }
        }

        public override string GetInfo()
        {
            return "Can have its flag changed in flight.";
        }

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = "Change Flag", name = "setFlagEvent")]
        public void changeFlag()
        {
            FlagBrowser fb = Utils.getFlagBrowser(onFlagSelect);
        }

        private void onFlagSelect(FlagBrowser.FlagEntry selected)
        {

            this.hasFlagChanged = true;
            this.selectedFlag = selected.textureInfo.name;
            Utils.applyFlagToPart(this.part, selected);
            if (HighLogic.LoadedSceneIsFlight)
                Flight.Instance.updateButtonTexture();

            Events["resetFlag"].guiActive = Events["resetFlag"].guiActiveEditor = true;

        }

        [KSPEvent(active = true, guiActive = false, guiActiveEditor = false, guiName = "Reset Flag", name = "resetFlagEvent")]
        public void resetFlag()
        {

            Utils.applyFlagToPart(this.part, this.originalFlag);
            Events["resetFlag"].guiActive = Events["resetFlag"].guiActiveEditor = false;
            if (HighLogic.LoadedSceneIsFlight)
                Flight.Instance.updateButtonTexture();

        }

        [KSPEvent(active = true, guiActive = true, guiName = "Apply Flag To Entire Vessel", name = "globallySetFlagEvent")]
        public void setCurrentFlagToEntireVessel()
        {
            string currentFlag = this.part.flagURL;
            if (HighLogic.LoadedSceneIsEditor)
                Utils.applyFlagToParts(this.part.vessel.parts, currentFlag);
            else
                Utils.applyFlagToVessel(FlightGlobals.fetch.activeVessel, currentFlag);
        }

        public void recordCurrentFlag(bool force = false, bool onlyIfNotSet = false)
        {
            bool set = !this.setOriginalFlag;
            if (force)
            {
                set = true;
                if (onlyIfNotSet && string.IsNullOrEmpty(this.originalFlag))
                    set = true;
                else if (onlyIfNotSet && !string.IsNullOrEmpty(this.originalFlag))
                    set = false;
                
            }
            if (set)
            {
                this.originalFlag = (HighLogic.LoadedSceneIsEditor ? EditorLogic.FlagURL : this.part.flagURL);
                Utils.LogDebug("Set original flag to '{0}'", this.originalFlag);
                this.setOriginalFlag = true;
            }
        }

    }
}
