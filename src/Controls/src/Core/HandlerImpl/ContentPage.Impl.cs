﻿using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.HotReload;
using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	public partial class ContentPage : IPage, HotReload.IHotReloadableView
	{
		// TODO ezhart That there's a layout alignment here tells us this hierarchy needs work :) 
		public Primitives.LayoutAlignment HorizontalLayoutAlignment => Primitives.LayoutAlignment.Fill;

		IView IPage.Content => Content;

		protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
		{
			if (Content is IFrameworkElement frameworkElement)
			{
				frameworkElement.Measure(widthConstraint, heightConstraint);
			}

			return new Size(widthConstraint, heightConstraint);
		}

		protected override Size ArrangeOverride(Rectangle bounds)
		{
			// Update the Bounds (Frame) for this page
			Layout(bounds);

			if (Content is IFrameworkElement element and VisualElement visualElement)
			{
				// The size checks here are a guard against legacy layouts which try to lay things out before the
				// native side is ready. We just ignore those invalid values.
				if (bounds.Size.Width >= 0 && bounds.Size.Height >= 0)
				{
					visualElement.Frame = element.ComputeFrame(bounds);
					element.Handler?.NativeArrange(visualElement.Frame);
				}
			}

			return Frame.Size;
		}

		protected override void InvalidateMeasureOverride()
		{
			base.InvalidateMeasureOverride();
			if (Content is IFrameworkElement frameworkElement)
			{
				frameworkElement.InvalidateMeasure();
			}
		}

		#region HotReload

		IView IReplaceableView.ReplacedView => HotReload.MauiHotReloadHelper.GetReplacedView(this) ?? this;

		HotReload.IReloadHandler HotReload.IHotReloadableView.ReloadHandler { get; set; }

		void HotReload.IHotReloadableView.TransferState(IView newView)
		{
			//TODO: Let you hot reload the the ViewModel
			//TODO: Lets do a real state transfer
			if (newView is View v)
				v.BindingContext = BindingContext;
		}

		void HotReload.IHotReloadableView.Reload()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				this.CheckHandlers();
				var reloadHandler = ((IHotReloadableView)this).ReloadHandler;
				reloadHandler?.Reload();
				//TODO: if reload handler is null, Do a manual reload?
			});
		}
		#endregion
	}
}
