﻿using System;
using System.Linq;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;

namespace Satrabel.OpenContent.Components
{
    public static class DnnUtils
    {
        /// <summary>
        /// Gets the list of the DNN modules by friendlyName.
        /// </summary>
        /// <param name="friendlyName">Friendly name of the module.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static ModuleInfo GetLastModuleByFriendlyName(string friendlyName)
        {
            //DesktopModuleController dmc = new DesktopModuleController();
            //DesktopModuleController.GetDesktopModuleByFriendlyName
            int portalid = PortalSettings.Current.PortalId;
            string culture = PortalSettings.Current.CultureCode;
            TabController tc = new TabController();
            ModuleController mc = new ModuleController();
            var modules = mc.GetModulesByDefinition(portalid, friendlyName).Cast<ModuleInfo>().OrderByDescending(m=> m.ModuleID);
            foreach (var mod in modules)
            {
                var tab = tc.GetTab(mod.TabID, portalid, false);
                if (tab.CultureCode == culture || tab.CultureCode == null)
                {
                    return mod;
                }
            }
            return modules.FirstOrDefault();
        }

        /// <summary>
        /// Gets the DNN tab by URL and culture.
        /// </summary>
        /// <param name="pageUrl">The page URL.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static TabInfo GetDnnTabByUrl(string pageUrl, string culture)
        {
            var alternativeLocale = LocaleController.Instance.GetLocale(culture);
            TabController tc = new TabController();
            var alternativeTab = tc.GetTabByCulture(PortalSettings.Current.ActiveTab.TabID, PortalSettings.Current.PortalId, alternativeLocale);
            throw new NotImplementedException();
        }

        internal static string ToUrl(this IFileInfo file)
        {
            return FileManager.Instance.GetUrl(file);
        }

        public static string GetCurrentCultureCode()
        {
            //strange issues with getting the correct culture.
            if (PortalSettings.Current.ActiveTab != null && PortalSettings.Current.ActiveTab.IsNeutralCulture)
                return PortalSettings.Current.CultureCode;
            if (PortalSettings.Current.ActiveTab != null )
                return PortalSettings.Current.ActiveTab.CultureCode;
                
            return LocaleController.Instance.GetCurrentLocale(PortalSettings.Current.PortalId).Code;
        }
    }
}