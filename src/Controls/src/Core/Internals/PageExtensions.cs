using System.ComponentModel;

namespace Microsoft.Maui.Controls.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class PageExtensions
	{
		public static Page AncestorToRoot(this Page page)
		{
			Element parent = page;
			while (!Application.IsApplicationOrWindowOrNull(parent.RealParent))
				parent = parent.RealParent;

			return parent as Page;
		}
	}
}
