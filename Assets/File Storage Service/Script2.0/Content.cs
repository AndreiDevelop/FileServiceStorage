public class ContentType
{
	public const string PNG = "image/png";
	public const string JPEG = "image/jpeg";
	public const string WAV = "audio/vnd.wave";
	public const string MP3 = "audio/mpeg";
	public const string TXT = "text/plain";
}

public class SessionContentType
{
	public const string RECORD_CONVERSATION = "RECORD_CONVERSATION";
	public const string STUDENT_SCREENSHOT = "STUDENT_SCREENSHOT";
}

public class Content
{
	public int llvrIndex;
	public string fileName;

	public string llvrContentType;
	public string fileType;

	public long fileSize;
	public string url;

	public Content()
	{
		llvrIndex = 0;
		fileName = string.Empty;

		llvrContentType = string.Empty;
		fileType = string.Empty;

		fileSize = 0;
		url = string.Empty;
	}

	public Content(int Index, string Name, string sessionType, string type, long Size, string URL)
	{
		llvrIndex = Index;
		fileName = Name;

		llvrContentType = sessionType;
		fileType = type;

		fileSize = Size;
		url = URL;
	}
}
