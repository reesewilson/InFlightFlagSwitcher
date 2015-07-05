using InFlightFlagSwitcher.Main;
using InFlightFlagSwitcher.Module;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InFlightFlagSwitcher.Utilities
{
    public class Utils
    {

        public static FlagBrowser getFlagBrowser(FlagBrowser.FlagSelectedCallback fbCallback = null, Callback fbDismissCallback = null)
        {

            FlagBrowser fb = (UnityEngine.Object.Instantiate((UnityEngine.Object)(new FlagBrowserGUIButton(null, null, null, null)).FlagBrowserPrefab) as GameObject).GetComponent<FlagBrowser>();
            if (fbCallback != null)
                fb.OnFlagSelected += fbCallback;
            if (fbDismissCallback != null)
                fb.OnDismiss += fbDismissCallback;

            return fb;
        
        }

        // Look at all these overloads I can't hold!

        public static int applyFlagToVessel(Vessel v, FlagBrowser.FlagEntry selected)
        {

            return applyFlagToVessel(v, selected.textureInfo.name);

        }

        public static int applyFlagToVessel(Vessel v, string flagURL)
        {

            return applyFlagToParts(v.parts, flagURL);

        }

        public static int applyFlagToParts(List<Part> parts, FlagBrowser.FlagEntry selected)
        {
            return applyFlagToParts(parts, selected.textureInfo.name);
        }

        public static int applyFlagToParts(List<Part> parts, string flagURL)
        {

            int x = 0;
            foreach (Part p in parts)
            {

                applyFlagToPart(p, flagURL);
                x++;

            }

            return x;

        }

        public static void applyFlagToPart(Part p, FlagBrowser.FlagEntry selected)
        {
            applyFlagToPart(p, selected.textureInfo.name);

        }

        public static void applyFlagToPart(Part p, string flagURL)
        {

            IFFSFlagSwitchModule fsm = (IFFSFlagSwitchModule)p.Modules["IFFSFlagSwitchModule"];
            if (fsm != null)
                fsm.recordCurrentFlag();

            p.flagURL = flagURL;

            FlagDecal fd = (FlagDecal)p.Modules["FlagDecal"];
            if (fd != null)
                fd.UpdateFlagTexture();

            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.fetch.activeVessel.rootPart == p)
                Flight.Instance.updateButtonTexture();

        }

        public static Texture getCurrentFlag()
        {
            return GameDatabase.Instance.GetTexture(FlightGlobals.fetch.activeVessel.rootPart.flagURL, false);
        }

        [Conditional("DEBUG")]
        public static void LogDebug(string message, params object[] fillers)
        {
            Log(message, fillers);
        }

        public static void Log(string message, params object[] fillers)
        {
            UnityEngine.Debug.Log("[InFlightFlagSwitcher]: " + String.Format(message, fillers));
        }

    }
}
