using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum FileType
{
	Text,
	Image,
	Audio
}

public enum ServiceType
{
	Amazon
}

public class FileStorageService : MonoBehaviour 
{
	private const string EMPTY_URL = "empty_url.com";
		
	[SerializeField]
	private ServiceType _serviceType = ServiceType.Amazon;

	[SerializeField]
	private FileStorageAmazon _amazon = null;

	private IFileStorageService _storageService;

	#region upload parametrs 

	private FileType _currentFileUploadType = FileType.Text;
	private string _pathToUploadFile = string.Empty;			//Sample : "/Arts/"
	private Stream _streamToFile = null;
	private string _fileUploadName = string.Empty;				//Sample : "Kote.png"

	#endregion

	void Start () 
	{
		SetStorageService ();
		SetUploadParametrs (FileType.Image, "/File Storage Service/Arts/", "Kote.png");
	}

	private void SetStorageService()
	{
		switch (_serviceType) 
		{
			case ServiceType.Amazon:
				_storageService = _amazon;
				break;
		}
	}

	public void SetUploadParametrs(FileType type, string pathToFile, string fileName)
	{
		_currentFileUploadType = type;
		_pathToUploadFile = pathToFile;
		_fileUploadName = fileName;
	}

	public void SetUploadParametrs(FileType type, Stream stream, string fileName)
	{
		_currentFileUploadType = type;
		_streamToFile = stream;
		_fileUploadName = fileName;
	}

	public string UploadFileToStorage (FileType type, string pathToFile, string fileName) 
	{
		string urlPathToFile = EMPTY_URL;

		SetUploadParametrs (type, pathToFile, fileName);

		if (_storageService != null &&
		    _pathToUploadFile != string.Empty &&
		    _fileUploadName != string.Empty) 
		{
			urlPathToFile = _storageService.UploadFile (_currentFileUploadType, _pathToUploadFile,	_fileUploadName);
		}

		return urlPathToFile;
	}

	public string UploadFileToStorage (FileType type, Stream stream, string fileName) 
	{
		string urlPathToFile = EMPTY_URL;

		SetUploadParametrs (type, stream, fileName);

		if (_storageService != null &&
			_streamToFile != null &&
			_fileUploadName != string.Empty) 
		{
			urlPathToFile = _storageService.UploadFile (_currentFileUploadType, stream,	_fileUploadName);
		}
	
		return urlPathToFile;
	}
}
