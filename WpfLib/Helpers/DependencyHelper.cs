using System.Windows.Media;
using System.Windows;

namespace WpfLib.Helpers
{
    public static class DependencyHelper
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // Get the parent object
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // Check if we've reached the end of the tree
            if (parentObject == null) return null;

            // Check if the parent is of the specified type
            if (parentObject is T parent)
                return parent;
            else
                // Recursively look up the tree
                return FindParent<T>(parentObject);
        }

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(parent); i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild) return tChild;

                var result = FindChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
