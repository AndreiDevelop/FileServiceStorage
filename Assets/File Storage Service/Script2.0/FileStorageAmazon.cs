using UnityEngine;
using Amazon;
using System.IO;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon.Runtime;

public class FileStorageAmazon : MonoBehaviour, IFileStorageService 
{
	#region private constants

	private const string S3_BUCKET_NAME = "llvr";
	private const string URL_FIRST_PARTH = "https://s3.amazonaws.com/" + S3_BUCKET_NAME + "/";
	private const string ACCESS_KEY = "AKIAJHPTOQGQXX5USTNA";
	private const string SECRET_KEY = "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";
	private const string EMPTY_URL = "empty_url.com";

	#endregion

	public string S3Region = RegionEndpoint.USEast1.SystemName;
	private RegionEndpoint _S3Region
	{
		get { return RegionEndpoint.GetBySystemName(S3Region); }
	}
		
	#region private members

	private IAmazonS3 _s3Client;
	private AWSCredentials _credentials;

	private AWSCredentials Credentials
	{
		get
		{
			if (_credentials == null)
				_credentials = new BasicAWSCredentials (ACCESS_KEY, SECRET_KEY);
			//_credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
			return _credentials;
		}
	}

	private IAmazonS3 Client
	{
		get
		{
			if (_s3Client == null)
			{
				_s3Client = new AmazonS3Client(Credentials, _S3Region);
			}
			return _s3Client;
		}
	}

	#endregion

	void Start()
	{
		UnityInitializer.AttachToGameObject(this.gameObject);

		AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

		AWSConfigs.LoggingConfig.LogTo = LoggingOptions.UnityLogger;
		AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;
		AWSConfigs.LoggingConfig.LogMetrics = true;
		AWSConfigs.CorrectForClockSkew = true;
	}

	public string UploadFile(FileType type, string pathToFile, string fileName)
	{
		//string urlPathToFile = EMPTY_URL;
		var stream = new FileStream(/*Application.dataPath + */pathToFile + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

		var request = new PostObjectRequest()
		{
			Bucket = S3_BUCKET_NAME,
			Key = fileName,
			InputStream = stream,
			CannedACL = S3CannedACL.PublicRead,
			Region = _S3Region,
			ContentType = GetContentType(type)
		};

		Client.PostObjectAsync(request, (responseObj) =>
		{
			if (responseObj.Exception == null)
			{
				//urlPathToFile = URL_FIRST_PARTH + fileName;
			}
			else
			{
				//Debug.Log(responseObj.Exception);
			}
		});
        print(URL_FIRST_PARTH + fileName);
		return URL_FIRST_PARTH + fileName;
	}

	public string UploadFile(FileType type, Stream stream, string fileName)
	{
		//string urlPathToFile = EMPTY_URL;

		var request = new PostObjectRequest()
		{
			Bucket = S3_BUCKET_NAME,
			Key = fileName,
			InputStream = stream,
			CannedACL = S3CannedACL.PublicRead,
			Region = _S3Region,
			ContentType = GetContentType(type)
		};

		Client.PostObjectAsync(request, (responseObj) =>
			{
				if (responseObj.Exception == null)
				{
					//Debug.Log(URL_FIRST_PARTH + fileName);
				}
				else
				{
					//Debug.Log(responseObj.Exception);
				}
			});
				
		return URL_FIRST_PARTH + fileName;
	}

	private string GetContentType(FileType type)
	{
		string contentType = string.Empty;

		switch (type) 
		{
			case FileType.Text:
				contentType = ContentType.TXT; 
				break;
			case FileType.Image:
				contentType = ContentType.PNG;
				break;
			case FileType.Audio:
				contentType = ContentType.MP3;
				break;
		}

		return contentType;
	}

}
