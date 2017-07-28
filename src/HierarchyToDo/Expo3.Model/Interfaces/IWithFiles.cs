namespace Expo3.Model.Interfaces
{
	public interface IWithFiles
	{
		/// <summary>
		/// Прикрепленные файлы.
		/// mongodb://filename
		/// file://filename
		/// ftp://filename
		/// http://filename
		/// </summary>
		string[] Files { get; set; }
	}
}