#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Maui
{
	public abstract class ImageSourceService : IImageSourceService
	{
		public ImageSourceService(ILogger? logger = null)
		{
			Logger = logger;
		}

		public ILogger? Logger { get; }

#if __ANDROID__
		public abstract Task<IImageSourceServiceResult<Android.Graphics.Drawables.Drawable>?> GetDrawableAsync(
			IImageSource imageSource,
			Android.Content.Context context,
			CancellationToken cancellationToken = default);
#elif __IOS__
		public abstract Task<IImageSourceServiceResult<UIKit.UIImage>?> GetImageAsync(
			IImageSource imageSource,
			float scale = 1,
			CancellationToken cancellationToken = default);
#elif TIZEN || __TIZEN__
		public abstract Task<IImageSourceServiceResult<bool>?> LoadImageAsync(
			IImageSource imageSource,
			Tizen.UIExtensions.ElmSharp.Image image,
			CancellationToken cancellationToken = default);
#elif WINDOWS
		public abstract Task<IImageSourceServiceResult<UI.Xaml.Media.ImageSource>?> GetImageSourceAsync(
			IImageSource imageSource,
			float scale = 1,
			CancellationToken cancellationToken = default);
#endif
	}
}