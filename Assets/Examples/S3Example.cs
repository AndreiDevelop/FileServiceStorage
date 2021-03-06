//
// Copyright 2014-2015 Amazon.com, 
// Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the AWS Mobile SDK For Unity 
// Sample Application License Agreement (the "License"). 
// You may not use this file except in compliance with the 
// License. A copy of the License is located 
// in the "license" file accompanying this file. This file is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, express or implied. See the License 
// for the specific language governing permissions and 
// limitations under the License.
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;

namespace AWSSDK.Examples
{
    public class S3Example : MonoBehaviour
    {
		[SerializeField]
		private string _pathToImages = string.Empty; 

		[SerializeField]
		private string _pathToAudio = string.Empty; 

		private const string S3_BUCKET_NAME = "llvr";
		private const string URL_FIRST_PARTH = "https://s3.amazonaws.com/" + S3_BUCKET_NAME + "/";
		private const string ACCESS_KEY = "AKIAJHPTOQGQXX5USTNA";
		private const string SECRET_KEY = "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";

        public string IdentityPoolId = "";
        public string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;
        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
        }
        public string S3Region = RegionEndpoint.USEast1.SystemName;
        private RegionEndpoint _S3Region
        {
            get { return RegionEndpoint.GetBySystemName(S3Region); }
        }
        
        public string SampleFileName = null;
        public Button GetBucketListButton = null;
        public Button PostBucketButton = null;
        public Button GetObjectsListButton = null;
        public Button DeleteObjectButton = null;
        public Button GetObjectButton = null;
        public Text ResultText = null;

        void Start()
        {
            UnityInitializer.AttachToGameObject(this.gameObject);
            GetBucketListButton.onClick.AddListener(() => { GetBucketList(); });
            PostBucketButton.onClick.AddListener(() => { PostObject(); });
            GetObjectsListButton.onClick.AddListener(() => { GetObjects(); });
            DeleteObjectButton.onClick.AddListener(() => { DeleteObject(); });
            GetObjectButton.onClick.AddListener(() => { GetObject(); });

			AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

			AWSConfigs.LoggingConfig.LogTo = LoggingOptions.UnityLogger;
			AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;
			AWSConfigs.LoggingConfig.LogMetrics = true;
			AWSConfigs.CorrectForClockSkew = true;
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
                //test comment
                return _s3Client;
            }
        }

        #endregion

        #region Get Bucket List
        /// <summary>
        /// Example method to Demostrate GetBucketList
        /// </summary>
        public void GetBucketList()
        {
            ResultText.text = "Fetching all the Buckets";
            Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
            {
                ResultText.text += "\n";
                if (responseObject.Exception == null)
                {
                    ResultText.text += "Got Response \nPrinting now \n";
                    responseObject.Response.Buckets.ForEach((s3b) =>
                    {
                        ResultText.text += string.Format("bucket = {0}, created date = {1} \n", s3b.BucketName, s3b.CreationDate);
                    });
                }
                else
                {
					Debug.Log(responseObject.Exception);
                    ResultText.text += "Got Exception \n";
                }
            });
        }

        #endregion

        /// <summary>
        /// Get Object from S3 Bucket
        /// </summary>
        private void GetObject()
        {
			ResultText.text = string.Format("fetching {0} from bucket {1}", SampleFileName, S3_BUCKET_NAME);
			Client.GetObjectAsync(S3_BUCKET_NAME, SampleFileName, (responseObj) =>
            {
                string data = null;
                var response = responseObj.Response;
                if (response.ResponseStream != null)
                {
                    using (StreamReader reader = new StreamReader(response.ResponseStream))
                    {
                        data = reader.ReadToEnd();
                    }

                    ResultText.text += "\n";
                    ResultText.text += data;
                }
            });
        }

        /// <summary>
        /// Post Object to S3 Bucket. 
        /// </summary>
        public void PostObject()
        {
            ResultText.text = "Retrieving the file";

			string fileName = GetFileHelper();
             
			var stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            ResultText.text += "\nCreating request object";
            var request = new PostObjectRequest()
            {
				Bucket = S3_BUCKET_NAME,
                Key = fileName,
                InputStream = stream,
				CannedACL = S3CannedACL.PublicRead,
                Region = _S3Region,
				ContentType = "text/plain"
				
                /*Bucket = S3BucketName,
                Key = fileName,
                InputStream = stream,
                CannedACL = S3CannedACL.Private*/
            };

            ResultText.text += "\nMaking HTTP post call";

            Client.PostObjectAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                {
                    ResultText.text += string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket);
					Debug.Log(URL_FIRST_PARTH + SampleFileName);
                }
                else
                {
					Debug.Log(responseObj.Exception);
                    ResultText.text += "\nException while posting the result object";
                    ResultText.text += string.Format("\n receieved error {0}", responseObj.Response.HttpStatusCode.ToString());
                }
            });
        }

		public void PostImage(string fileName)
		{
			ResultText.text = "Retrieving the file";

			var stream = new FileStream(Application.dataPath + _pathToImages + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

			ResultText.text += "\nCreating request object";
			var request = new PostObjectRequest()
			{
				Bucket = S3_BUCKET_NAME,
				Key = fileName,
				InputStream = stream,
				CannedACL = S3CannedACL.PublicRead,
				Region = _S3Region,
				ContentType = "image/png"
				//ContentType = "audio/wav"
			};

			ResultText.text += "\nMaking HTTP post call";

			Client.PostObjectAsync(request, (responseObj) =>
				{
					if (responseObj.Exception == null)
					{
						ResultText.text += string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket);
						Debug.Log(URL_FIRST_PARTH + fileName);
					}
					else
					{
						Debug.Log(responseObj.Exception);
						ResultText.text += "\nException while posting the result object";
						ResultText.text += string.Format("\n receieved error {0}", responseObj.Response.HttpStatusCode.ToString());
					}
				});
		}

		public void PostAudio(string fileName)
		{
			ResultText.text = "Retrieving the file";

			var stream = new FileStream(Application.dataPath + _pathToAudio + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

			ResultText.text += "\nCreating request object";
			var request = new PostObjectRequest()
			{
				Bucket = S3_BUCKET_NAME,
				Key = fileName,
				InputStream = stream,
				CannedACL = S3CannedACL.PublicRead,
				Region = _S3Region,
				ContentType = "audio/wav"
					//ContentType = "audio/wav"
			};

			ResultText.text += "\nMaking HTTP post call";

			Client.PostObjectAsync(request, (responseObj) =>
				{
					if (responseObj.Exception == null)
					{
						ResultText.text += string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket);
						Debug.Log(URL_FIRST_PARTH + fileName);
					}
					else
					{
						Debug.Log(responseObj.Exception);
						ResultText.text += "\nException while posting the result object";
						ResultText.text += string.Format("\n receieved error {0}", responseObj.Response.HttpStatusCode.ToString());
					}
				});
		}

        /// <summary>
        /// Get Objects from S3 Bucket
        /// </summary>
        public void GetObjects()
        {
			ResultText.text = "Fetching all the Objects from " + S3_BUCKET_NAME;

            var request = new ListObjectsRequest()
            {
				BucketName = S3_BUCKET_NAME
            };

            Client.ListObjectsAsync(request, (responseObject) =>
            {
                ResultText.text += "\n";
                if (responseObject.Exception == null)
                {
                    ResultText.text += "Got Response \nPrinting now \n";
                    responseObject.Response.S3Objects.ForEach((o) =>
                    {
                        ResultText.text += string.Format("{0}\n", o.Key);
                    });
                }
                else
                {
					Debug.Log(responseObject.Exception);
                    ResultText.text += "Got Exception \n";
                }
            });
        }

        /// <summary>
        /// Delete Objects in S3 Bucket
        /// </summary>
        public void DeleteObject()
        {
			ResultText.text = string.Format("deleting {0} from bucket {1}", SampleFileName, S3_BUCKET_NAME);
            List<KeyVersion> objects = new List<KeyVersion>();
            objects.Add(new KeyVersion()
            {
                Key = SampleFileName
            });

            var request = new DeleteObjectsRequest()
            {
				BucketName = S3_BUCKET_NAME,
                Objects = objects
            };

            Client.DeleteObjectsAsync(request, (responseObj) =>
            {
                ResultText.text += "\n";
                if (responseObj.Exception == null)
                {
                    ResultText.text += "Got Response \n \n";

                    ResultText.text += string.Format("deleted objects \n");

                    responseObj.Response.DeletedObjects.ForEach((dObj) =>
                    {
                        ResultText.text += dObj.Key;
                    });
                }
                else
                {
					Debug.Log(responseObj.Exception);
                    ResultText.text += "Got Exception \n";
                }
            });
        }


        #region helper methods

		private string GetFileHelper()
        {
            var fileName = SampleFileName;

            if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName))
            {
                var streamReader = File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);
                streamReader.WriteLine("This is a sample s3 file uploaded from unity s3 sample");
                streamReader.Close();
            }
            return fileName;
        }

        private string GetPostPolicy(string bucketName, string key, string contentType)
        {
            bucketName = bucketName.Trim();

            key = key.Trim();
            // uploadFileName cannot start with /
            if (!string.IsNullOrEmpty(key) && key[0] == '/')
            {
                throw new ArgumentException("uploadFileName cannot start with / ");
            }

            contentType = contentType.Trim();

            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentException("bucketName cannot be null or empty. It's required to build post policy");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("uploadFileName cannot be null or empty. It's required to build post policy");
            }
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("contentType cannot be null or empty. It's required to build post policy");
            }

            string policyString = null;
            int position = key.LastIndexOf('/');
            if (position == -1)
            {
                policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
                    bucketName + "\"},[\"starts-with\", \"$key\", \"" + "\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
            }
            else
            {
                policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
                    bucketName + "\"},[\"starts-with\", \"$key\", \"" + key.Substring(0, position) + "/\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
            }

            return policyString;
        }

    }

        #endregion
}
