using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Formats;

namespace BeeFee.ImageApp
{
    public class ImageService
    {
	    private readonly string _folder;
	    public ImageService(string folder)
	    {
		    _folder = folder;
	    }

	    public async Task<AddImageResult> AddImage(Stream stream, string filename, ImageSize[] sizes)
	    {
		    try
		    {
			    var fullpath = Path.Combine(_folder, filename);
			    if (File.Exists(fullpath))
				    return new AddImageResult(EAddImageResut.Exists, filename, null);
				if (!Directory.Exists(Path.Combine(_folder, Path.GetDirectoryName(filename))))
				    Directory.CreateDirectory(Path.Combine(_folder, Path.GetDirectoryName(filename)));
			    using (stream)
			    {
				    await SaveToFile(stream, fullpath);
				    stream.Position = 0;
				    if (sizes != null && sizes.Any())
				    {
					    var image = Image.Load(stream);
					    return await Task.Run(() =>
					    {
						    try
						    {
							    foreach (var size in sizes)
								    ResizeImage(image, size, filename);
						    }
						    catch (Exception e)
						    {
							    image.Dispose();
							    return new AddImageResult(EAddImageResut.Error, filename, e.Message);
						    }
						    image.Dispose();
						    return new AddImageResult(EAddImageResut.Ok, filename, null);
					    }).ConfigureAwait(false);
				    }
			    }
		    }
		    catch (Exception e)
		    {
			    return new AddImageResult(EAddImageResut.Error, filename, e.Message);
		    }
		    return new AddImageResult(EAddImageResut.Ok, filename, null);
	    }

		private string GetMinDirectoryName(ImageSize size)
		    => String.Concat(size.Width, "_", size.Height);

	    private void ResizeImage(Image<Rgba32> image, ImageSize size, string filename)
	    {
		    var folder = Path.Combine(_folder, Path.GetDirectoryName(filename), GetMinDirectoryName(size));
		    var minfullpath = Path.Combine(folder, Path.GetFileName(filename));
		    if (!Directory.Exists(folder))
			    Directory.CreateDirectory(folder);
		    image.Resize(size.Width, size.Height).Save(minfullpath, new JpegEncoder { Quality = 85 });
	    }


		private async Task SaveToFile(Stream stream, string filename)
	    {
		    var buffer = new byte[65000];
		    Task task = Task.CompletedTask;
		    var len = 1;
		    using (var writeStream = File.Create(filename))
		    {
			    while (len > 0)
			    {
				    await task;
				    len = stream.Read(buffer, 0, buffer.Length);
					if (len > 0)
						task = writeStream.WriteAsync(buffer, 0, len);
			    }
			    await task;
		    }
		}
	}
}
