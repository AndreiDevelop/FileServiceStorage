using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	[SerializeField]
	private ServiceType _serviceType = ServiceType.Amazon;

	[SerializeField]
	private FileStorageAmazon _amazon = null;

	private IFileStorageService _storageService;

	#region upload parametrs 

	private FileType _currentFileUploadType = FileType.Text;
	private string _pathToUploadFile = string.Empty;			//Sample : "/Arts/"
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

	public void UploadFileToStorage () 
	{
		if (_storageService != null &&
		    _pathToUploadFile != string.Empty &&
		    _fileUploadName != string.Empty) 
		{
			_storageService.UploadFile (_currentFileUploadType, _pathToUploadFile,	_fileUploadName);
		}
	}
}
