using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ImageApp.Tests
{
    [TestClass]
    public class ImageServiceTest
    {
		private ImageService _service;

	    [TestInitialize]
	    public void Setup()
	    {
		    foreach (var file in new DirectoryInfo("images").GetFiles())
			    file.Delete();
		    _service = new ImageService(@"images");
	    }

        //[TestMethod]
        //public void AddImage()
        //{
	       // var s = Path.GetFullPath("images");
	       // using (var stream = new MemoryStream(ImagesResource.priroda_bwua_02_06_2012_019__1920x12001))
		      //  _service.AddImage(stream, "priroda.jpg", null).Wait();
        //}

	    [TestMethod]
	    public void AddImageTest()
	    {
		    AddImageResult result;
			using (var stream = new MemoryStream(ImagesResource.Rio_de_Janeiro))
			{
				result = _service.AddImage(stream, "priroda.jpg", null).Result;
			}

			Assert.AreEqual(EAddImageResut.Ok, result.Result);
			Assert.AreEqual(null, result.Error);
			Assert.AreEqual("priroda.jpg", result.Path);
	    }

		[TestMethod]
		public void AddExistingImageTest()
		{
			AddImageResult result;
			using (var stream = new MemoryStream(ImagesResource.Rio_de_Janeiro))
			{
				result = _service.AddImage(stream, "priroda.jpg", null).Result;
			}

			Assert.AreEqual(EAddImageResut.Ok, result.Result);
			Assert.AreEqual(null, result.Error);
			Assert.AreEqual("priroda.jpg", result.Path);

			using (var stream = new MemoryStream(ImagesResource.Rio_de_Janeiro))
			{
				result = _service.AddImage(stream, "priroda.jpg", null).Result;
			}

			Assert.AreEqual(EAddImageResut.Exists, result.Result);
			Assert.AreEqual(null, result.Error);
			Assert.AreEqual("priroda.jpg", result.Path);
		}

	    [TestMethod]
	    public void AddImageWithSize()
	    {
			AddImageResult result;
		    using (var stream = new MemoryStream(ImagesResource.Rio_de_Janeiro))
		    {
			    result = _service.AddImage(stream, "priroda.jpg", new[] {new ImageSize(200, 200), new ImageSize(400, 200)})
				    .Result;
		    }

		    Assert.AreEqual(EAddImageResut.Ok, result.Result);
		    Assert.AreEqual(null, result.Error);
		    Assert.AreEqual("priroda.jpg", result.Path);

			Assert.IsTrue(File.Exists("images/200_200/priroda.jpg"));
			Assert.IsTrue(File.Exists("images/400_200/priroda.jpg"));
		}

	    [TestMethod]
		public void GetOriginalImage()
	    {
			AddImageResult result;
		    using (var stream = new MemoryStream(ImagesResource.Rio_de_Janeiro))
		    {
			    result = _service.AddImage(stream, "priroda.jpg", new[] { new ImageSize(200, 200), new ImageSize(400, 200) })
				    .Result;
		    }

		    Assert.AreEqual(EAddImageResut.Ok, result.Result);
		    Assert.AreEqual(null, result.Error);
		    Assert.AreEqual("priroda.jpg", result.Path);

			Assert.IsNotNull(_service.GetImageOrDefault("priroda.jpg"));
		}

	    [TestMethod]
	    public void GetResizedImage()
	    {
		    AddImageResult result;
		    using (var stream = new MemoryStream(ImagesResource.Rio_de_Janeiro))
		    {
			    result = _service.AddImage(stream, "priroda.jpg", new[] { new ImageSize(200, 200), new ImageSize(400, 200) })
				    .Result;
		    }

		    Assert.AreEqual(EAddImageResut.Ok, result.Result);
		    Assert.AreEqual(null, result.Error);
		    Assert.AreEqual("priroda.jpg", result.Path);

		    Assert.IsNotNull(_service.GetImageOrDefault(new ImageSize(200, 200), "priroda.jpg"));
		    Assert.IsNotNull(_service.GetImageOrDefault(new ImageSize(400, 200), "priroda.jpg"));
	    }

	    [TestMethod]
	    public void GetNonexistentImage()
	    {
			AddImageResult result;
		    using (var stream = new MemoryStream(ImagesResource.Rio_de_Janeiro))
		    {
			    result = _service.AddImage(stream, "priroda.jpg", new[] { new ImageSize(200, 200), new ImageSize(400, 200) })
				    .Result;
		    }

		    Assert.AreEqual(EAddImageResut.Ok, result.Result);
		    Assert.AreEqual(null, result.Error);
		    Assert.AreEqual("priroda.jpg", result.Path);

		    Assert.IsNull(_service.GetImageOrDefault(new ImageSize(300, 200), "priroda.jpg"));
		}
	}
}
