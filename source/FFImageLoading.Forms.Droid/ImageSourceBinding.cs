﻿using System;
using Xamarin.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Runtime;

namespace FFImageLoading.Forms.Droid
{
	[Preserve(AllMembers = true)]
	internal class ImageSourceBinding
	{
		public ImageSourceBinding(FFImageLoading.Work.ImageSource imageSource, string path)
		{
			ImageSource = imageSource;
			Path = path;
		}

		public ImageSourceBinding(Func<CancellationToken, Task<Stream>> stream)
		{
			ImageSource = FFImageLoading.Work.ImageSource.Stream;
			Stream = stream;
			Path = "Stream";
		}

		public FFImageLoading.Work.ImageSource ImageSource { get; private set; }

		public string Path { get; private set; }

		public Func<CancellationToken, Task<Stream>> Stream { get; private set; }

		internal static ImageSourceBinding GetImageSourceBinding(ImageSource source)
		{
			if (source == null)
			{
				return null;
			}

			var uriImageSource = source as UriImageSource;
			if (uriImageSource != null)
			{
				var uri = uriImageSource.Uri?.OriginalString;
				if (string.IsNullOrWhiteSpace(uri))
					return null;

				return new ImageSourceBinding(FFImageLoading.Work.ImageSource.Url, uri);
			}

			var fileImageSource = source as FileImageSource;
			if (fileImageSource != null)
			{
				if (string.IsNullOrWhiteSpace(fileImageSource.File))
					return null;

				if (File.Exists(fileImageSource.File))
					return new ImageSourceBinding(FFImageLoading.Work.ImageSource.Filepath, fileImageSource.File);

				return new ImageSourceBinding(FFImageLoading.Work.ImageSource.CompiledResource, fileImageSource.File);
			}

			var streamImageSource = source as StreamImageSource;
			if (streamImageSource != null)
			{
				return new ImageSourceBinding(streamImageSource.Stream);
			}
								
			throw new NotImplementedException("ImageSource type not supported");
		}

		public override bool Equals(object obj)
		{
			var item = obj as ImageSourceBinding;

			if (item == null)
			{
				return false;
			}

			return this.ImageSource == item.ImageSource && this.Path == item.Path && this.Stream == item.Stream;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + this.ImageSource.GetHashCode();
				hash = hash * 23 + Path.GetHashCode();
				hash = hash * 23 + Stream.GetHashCode();
				return  hash;
			}
		}
	}
}

