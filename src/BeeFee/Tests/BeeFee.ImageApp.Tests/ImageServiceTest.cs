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
		    _service = new ImageService(@"images");
	    }

        [TestMethod]
        public void AddImage()
        {
	        var s = Path.GetFullPath("images");
	        using (var stream = new MemoryStream(ImagesResource.priroda_bwua_02_06_2012_019__1920x12001))
		        _service.AddImage(stream, "priroda.jpg", null).Wait();
        }
    }
}
