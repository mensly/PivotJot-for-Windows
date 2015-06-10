using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace PivotJot
{
    public static class Extensions
    {
        public static T FindParent<T>(this FrameworkElement obj) where T:FrameworkElement
        {
            do
            {
                obj = obj.Parent as FrameworkElement;
                if (obj is T)
                {
                    return (T)obj;
                }
            } while (obj != null);
            return null;
        }
    }
}
