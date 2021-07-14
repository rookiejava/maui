#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Maui.Handlers
{
	public partial class LayoutHandler : ILayoutHandler
	{
		public static PropertyMapper<ILayout, LayoutHandler> LayoutMapper = new PropertyMapper<ILayout, LayoutHandler>(ViewHandler.ViewMapper)
		{
#if TIZEN || __TIZEN__
			[nameof(ILabel.Background)] = MapBackground,
#endif
		};


		public LayoutHandler() : base(LayoutMapper)
		{

		}

		public LayoutHandler(PropertyMapper? mapper = null) : base(mapper ?? LayoutMapper)
		{

		}
	}
}
