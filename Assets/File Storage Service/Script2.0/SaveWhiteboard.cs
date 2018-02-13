using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

public class SaveWhiteboard : MonoBehaviour 
{
	[SerializeField]
	private FileStorageService _fileStorageService = null;

	private Material _curMaterial = null;

	void Start()
	{
		_curMaterial = GetComponent<MeshRenderer> ().material;
	}

	//put texture to memory stream in real time
	private MemoryStream PutTextureToMemoryStream (Texture2D texture) 
	{
		MemoryStream ms = new MemoryStream(texture.EncodeToPNG());
		return ms;
	}
		
	///<summary>
	/// string fileName must look like "name.png"
	/// <summary>
	public string PutTextureToFileService (string fileName) 
	{
		MemoryStream streamTexture = PutTextureToMemoryStream (_curMaterial.mainTexture as Texture2D);

		string urlToFile = _fileStorageService.UploadFileToStorage (FileType.Image, streamTexture, fileName);

		return urlToFile;
	}
}
