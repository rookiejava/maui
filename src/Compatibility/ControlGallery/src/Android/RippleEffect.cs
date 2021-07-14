﻿using Android.Util;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using AView = Android.Views.View;
using Microsoft.Maui.Controls.Platform;

[assembly: ExportEffect(typeof(Microsoft.Maui.Controls.Compatibility.ControlGallery.Android.RippleEffect), nameof(Microsoft.Maui.Controls.Compatibility.ControlGallery.Android.RippleEffect))]
namespace Microsoft.Maui.Controls.Compatibility.ControlGallery.Android
{
    [Preserve(AllMembers = true)]
    public class RippleEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Container is AView view)
                {
                    view.Clickable = true;
                    view.Focusable = true;

                    using (var outValue = new TypedValue())
                    {
                        view.Context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, outValue, true);
                        view.SetBackgroundResource(outValue.ResourceId);
                    }
                }
            }
            catch
            {
              
            }
        }

        protected override void OnDetached()
        {

        }
    }
}
