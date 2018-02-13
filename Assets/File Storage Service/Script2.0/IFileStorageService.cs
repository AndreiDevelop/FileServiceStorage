using System.IO;

public interface IFileStorageService
{
	string UploadFile(FileType type, string pathToFile, string fileName);
	string UploadFile(FileType type, Stream stream, string fileName);
}
