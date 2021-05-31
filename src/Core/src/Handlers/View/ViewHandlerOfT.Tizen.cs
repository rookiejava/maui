using System;
using Tizen.UIExtensions.Common;
using ElmSharp;

using Rectangle = Microsoft.Maui.Graphics.Rectangle;
using Size = Microsoft.Maui.Graphics.Size;
using Point = Microsoft.Maui.Graphics.Point;
using ESize = ElmSharp.Size;
using ERect = ElmSharp.Rect;

namespace Microsoft.Maui.Handlers
{
	public abstract partial class ViewHandler<TVirtualView, TNativeView> : INativeViewHandler
	{
		bool _disposedValue;

		EvasObject? INativeViewHandler.NativeView => WrappedNativeView;
		EvasObject? INativeViewHandler.ContainerView => ContainerView;

		protected new EvasObject? WrappedNativeView => (EvasObject?)base.WrappedNativeView;

		public new WrapperView? ContainerView
		{
			get => (WrapperView?)base.ContainerView;
			protected set => base.ContainerView = value;
		}

		public void SetParent(INativeViewHandler parent) => Parent = parent;

		public CoreUIAppContext? Context => MauiContext?.Context;

		public INativeViewHandler? Parent { get; private set; }

		public EvasObject? NativeParent => Context?.BaseLayout;

		~ViewHandler()
		{
			Dispose(disposing: false);
		}

		public override void NativeArrange(Rectangle frame)
		{
			var nativeView = WrappedNativeView;

			if (nativeView == null)
				return;

			if (frame.Width < 0 || frame.Height < 0)
			{
				// This is just some initial Forms value nonsense, nothing is actually laying out yet
				return;
			}

			if (NativeParent == null)
				return;

			var updatedGeometry = new Rectangle(ComputeAbsolutePoint(frame), new Size(frame.Width, frame.Height)).ToEFLPixel();

			if (nativeView.Geometry != updatedGeometry)
			{
				nativeView.Geometry = updatedGeometry;
			}
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var nativeView = WrappedNativeView;

			if (nativeView == null || VirtualView == null)
			{
				return Size.Zero;
			}

			if (NativeParent == null)
			{
				return new Size(widthConstraint, heightConstraint);
			}

			int availableWidth = widthConstraint.ToScaledPixel();
			int availableHeight = heightConstraint.ToScaledPixel();

			if (availableWidth < 0)
				availableWidth = int.MaxValue;
			if (availableHeight < 0)
				availableHeight = int.MaxValue;

			Size measured;
			var nativeViewMeasurable = nativeView as IMeasurable;
			if (nativeViewMeasurable != null)
			{
				measured = nativeViewMeasurable.Measure(availableWidth, availableHeight).ToDP();
			}
			else
			{
				measured = Measure(availableWidth, availableHeight);
			}

			return new SizeRequest(measured, MinimumSize());
		}

		protected virtual Size MinimumSize()
		{
			return new ESize(NativeView!.MinimumWidth, NativeView!.MinimumHeight).ToDP();
		}

		public virtual ERect GetNativeContentGeometry()
		{
			var nativeView = WrappedNativeView;

			if (nativeView == null)
			{
				return new ERect();
			}
			return nativeView.Geometry;
		}

		protected virtual Size Measure(double availableWidth, double availableHeight)
		{
			var nativeView = WrappedNativeView;

			if (nativeView == null)
			{
				return new Size(0, 0);
			}
			return new ESize(nativeView.MinimumWidth, nativeView.MinimumHeight).ToDP();
		}

		protected virtual double ComputeAbsoluteX(Rectangle frame)
		{
			if (Parent != null)
			{
				return frame.X + Parent.GetNativeContentGeometry().X.ToScaledDP();
			}
			else
			{
				return frame.X;
			}
		}

		protected virtual double ComputeAbsoluteY(Rectangle frame)
		{
			if (Parent != null)
			{
				return frame.Y + Parent.GetNativeContentGeometry().Y.ToScaledDP();
			}
			else
			{
				return frame.Y;
			}
		}

		protected virtual Point ComputeAbsolutePoint(Rectangle frame)
		{
			return new Point(ComputeAbsoluteX(frame), ComputeAbsoluteY(frame));
		}

		protected override void SetupContainer()
		{

		}

		protected override void RemoveContainer()
		{

		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// Dispose managed state (managed objects)
					if (WrappedNativeView != null)
					{
						DisconnectHandler(WrappedNativeView);
						WrappedNativeView.Unrealize();
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

	public abstract class EViewHandler<TVirtualView, TNativeView> : ViewHandler<TVirtualView, TNativeView>
		where TVirtualView : class, IView
		where TNativeView : EvasObject
	{
		protected EViewHandler(PropertyMapper mapper) : base(mapper)
		{
		}

		protected virtual void OnNativeViewDeleted(object? sender, EventArgs e)
		{
			Dispose();
		}

		protected virtual void OnMoved(object? sender, EventArgs e)
		{
		}

		protected virtual void OnFocused(object? sender, EventArgs e)
		{
		}

		protected virtual void OnUnfocused(object? sender, EventArgs e)
		{
		}

		protected override void ConnectHandler(TNativeView nativeView)
		{
			base.ConnectHandler(nativeView);
			
			nativeView.Deleted += OnNativeViewDeleted;
			nativeView.Moved += OnMoved;

			if (nativeView is Widget widget)
			{
				widget.Focused += OnFocused;
				widget.Unfocused += OnUnfocused;
			}
		}

		protected override void DisconnectHandler(TNativeView nativeView)
		{
			base.DisconnectHandler(nativeView);

			nativeView.Moved -= OnMoved;
			nativeView.Deleted -= OnNativeViewDeleted;

			if (nativeView is Widget widget)
			{
				widget.Focused -= OnFocused;
				widget.Unfocused -= OnUnfocused;
			}
		}

		protected override Size MinimumSize()
		{
			if (WrappedNativeView == null)
			{
				return base.MinimumSize();
			}

			if (WrappedNativeView is IMeasurable im)
			{
				return im.Measure(WrappedNativeView.MinimumWidth, WrappedNativeView.MinimumHeight).ToDP();
			}
			else
			{
				return base.MinimumSize();
			}
		}
	}
}