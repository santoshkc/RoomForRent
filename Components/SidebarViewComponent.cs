using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Components
{
    public class SideBarViewData
    {
        public Dictionary<string, ActionAndDisplayName> Data
        { get; internal set; } = new Dictionary<string, ActionAndDisplayName>();

        public void AddViewData(string controllerName, string actionName, string displayName)
        {
            this.Data.Add(controllerName, new SideBarViewData.ActionAndDisplayName
            {
                ActionName = actionName,
                DisplayName = displayName,
            });
        }

        internal void SetActiveController(string controllerName)
        {
            if (this.Data.ContainsKey(controllerName) == false)
                this.Data.Add(controllerName, new ActionAndDisplayName
                {
                    ActionName = "Index",
                    DisplayName = controllerName
                });
            this.Data[controllerName].IsSelected = true;
        }

        public IEnumerable<(string controllerName,string actioName ,string displayName,bool isSelected)> GetItems()
        {
            foreach(var key in this.Data.Keys)
            {
                yield return (key, this.Data[key].ActionName, this.Data[key].DisplayName, this.Data[key].IsSelected);
            }
        }
        
        public class ActionAndDisplayName
        {
            public string ActionName { get; set; }
            public string DisplayName { get; set; }
            public bool IsSelected { get; set; }
        }
    }

    public class SidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var currentController = this.RouteData?.Values["controller"]?.ToString() ?? "Home";

            var viewData = BuildViewData();
            viewData.SetActiveController(currentController);
            return View(viewData);
        }

        private SideBarViewData BuildViewData()
        {
            var sideBarData = new SideBarViewData();

            sideBarData.AddViewData("Home", "Index", "Transactions");
            sideBarData.AddViewData("Renter", "Index", "Renters");
            sideBarData.AddViewData("Leaser", "Index", "Leasers");
            
            return sideBarData;
        }
    }
}
