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

	#endregion

	#region content type region

	private const string CONTENT_TYPE_TEXT = "text/plain";
	private const string CONTENT_TYPE_IMAGE = "image/png";
	private const string CONTENT_TYPE_AUDIO = "audio/wav";

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

	public void UploadFile(FileType type, string pathToFile, string fileName)
	{
		var stream = new FileStream(Application.dataPath + pathToFile + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

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
				Debug.Log(URL_FIRST_PARTH + fileName);
			}
			else
			{
				Debug.Log(responseObj.Exception);
			}
		});
	}

	private string GetContentType(FileType type)
	{
		string contentType = string.Empty;

		switch (type) 
		{
			case FileType.Text:
				contentType = CONTENT_TYPE_TEXT; 
				break;
			case FileType.Image:
				contentType = CONTENT_TYPE_IMAGE;
				break;
			case FileType.Audio:
				contentType = CONTENT_TYPE_AUDIO;
				break;
		}

		return contentType;
	}

}
