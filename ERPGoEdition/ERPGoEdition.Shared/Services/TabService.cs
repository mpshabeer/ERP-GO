using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace ERPGoEdition.Shared.Services
{
    public class TabView
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public RenderFragment? Content { get; set; }
        public bool IsActive { get; set; }
        public bool IsClosable { get; set; } = true;
    }

    public interface ITabService
    {
        event Action? OnChange;
        List<TabView> Tabs { get; }
        string ActiveTabId { get; }
        void AddTab(string title, RenderFragment content, bool isClosable = true);
        void CloseTab(string id);
        void ActivateTab(string id);
    }

    public class TabService : ITabService
    {
        public event Action? OnChange;
        public List<TabView> Tabs { get; private set; } = new();
        public string ActiveTabId => Tabs.FirstOrDefault(t => t.IsActive)?.Id ?? string.Empty;

        public void AddTab(string title, RenderFragment content, bool isClosable = true)
        {
            // Optional: Check if tab with same title exists and activate it instead?
            // For now, let's allow multiples or just activate if exists.
            
            var existing = Tabs.FirstOrDefault(t => t.Title == title);
            if (existing != null)
            {
                ActivateTab(existing.Id);
                return;
            }

            var newTab = new TabView
            {
                Title = title,
                Content = content,
                IsClosable = isClosable,
                IsActive = true
            };

            // Deactivate others
            foreach (var t in Tabs) t.IsActive = false;

            Tabs.Add(newTab);
            NotifyStateChanged();
        }

        public void CloseTab(string id)
        {
            var tab = Tabs.FirstOrDefault(t => t.Id == id);
            if (tab == null) return;

            var index = Tabs.IndexOf(tab);
            Tabs.Remove(tab);

            if (tab.IsActive && Tabs.Any())
            {
                // Smart activation: try to go to the left (previous tab), otherwise stay at 0
                var newIndex = index > 0 ? index - 1 : 0;
                
                // Ensure index is valid
                if (newIndex >= Tabs.Count) newIndex = Tabs.Count - 1;

                Tabs[newIndex].IsActive = true;
            }
            
            NotifyStateChanged();
        }

        public void ActivateTab(string id)
        {
            var tab = Tabs.FirstOrDefault(t => t.Id == id);
            if (tab != null)
            {
                foreach (var t in Tabs) t.IsActive = false;
                tab.IsActive = true;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
